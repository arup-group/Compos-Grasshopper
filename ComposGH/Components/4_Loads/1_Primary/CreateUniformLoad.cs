using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits;
using OasysUnits.Units;
using OasysGH.Units.Helpers;
using OasysGH.Units;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateUniformLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.UniformLoad;
    public CreateUniformLoad()
      : base("CreateUniformLoad", "UniformLoad", "Create a uniformly distributed Compos Load.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Pressure.GetAbbreviation(ForcePerAreaUnit);
      pManager.AddGenericParameter("Const. Dead [" + unitAbbreviation + "]", "dl", "Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live [" + unitAbbreviation + "]", "ll", "Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead [" + unitAbbreviation + "]", "DL", "Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live [" + unitAbbreviation + "]", "LL", "Final Live Load", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposLoadParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      switch (DistributionType)
      {
        case LoadDistribution.Line:
          ForcePerLength constDeadL = (ForcePerLength)Input.UnitNumber(this, DA, 0, ForcePerLengthUnit);
          ForcePerLength constLiveL = (ForcePerLength)Input.UnitNumber(this, DA, 1, ForcePerLengthUnit);
          ForcePerLength finalDeadL = (ForcePerLength)Input.UnitNumber(this, DA, 2, ForcePerLengthUnit);
          ForcePerLength finalLiveL = (ForcePerLength)Input.UnitNumber(this, DA, 3, ForcePerLengthUnit);
          Load loadL = new UniformLoad(constDeadL, constLiveL, finalDeadL, finalLiveL);
          Output.SetItem(this, DA, 0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          Pressure constDeadA = (Pressure)Input.UnitNumber(this, DA, 0, ForcePerAreaUnit);
          Pressure constLiveA = (Pressure)Input.UnitNumber(this, DA, 1, ForcePerAreaUnit);
          Pressure finalDeadA = (Pressure)Input.UnitNumber(this, DA, 2, ForcePerAreaUnit);
          Pressure finalLiveA = (Pressure)Input.UnitNumber(this, DA, 3, ForcePerAreaUnit);
          Load loadA = new UniformLoad(constDeadA, constLiveA, finalDeadA, finalLiveA);
          Output.SetItem(this, DA, 0, new LoadGoo(loadA));
          break;
      }
    }

    #region Custom UI
    private ForcePerLengthUnit ForcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;
    private PressureUnit ForcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;
    private LoadDistribution DistributionType = LoadDistribution.Area;

    protected override void InitialiseDropdowns()
    {
      _spacerDescriptions = new List<string>(new string[] { "Distribution", "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // type
      _dropDownItems.Add(Enum.GetValues(typeof(LoadDistribution)).Cast<LoadDistribution>().Select(x => x.ToString()).ToList());
      _selectedItems.Add(LoadDistribution.Area.ToString());

      // force unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea));
      _selectedItems.Add(Pressure.GetAbbreviation(ForcePerAreaUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)
      {
        DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), _selectedItems[i]);
        if (DistributionType == LoadDistribution.Line)
        {
          _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength);
          _selectedItems[1] = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);
        }
        else
        {
          _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea);
          _selectedItems[1] = Pressure.GetAbbreviation(ForcePerAreaUnit);
        }
      }
      if (i == 1)
      {
        if (DistributionType == LoadDistribution.Line)
          ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[i]);
        else
          ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), _selectedItems[0]);
      if (DistributionType == LoadDistribution.Line)
        ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
      else
        ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation;
      if (DistributionType == LoadDistribution.Line)
        unitAbbreviation = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);
      else
        unitAbbreviation = Pressure.GetAbbreviation(ForcePerAreaUnit);
      int i = 0;
      Params.Input[i++].Name = "Const. Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
