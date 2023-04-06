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
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateLinearLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.LinearLoad;
    public CreateLinearLoad()
      : base("CreateLinearLoad", "LinearLoad", "Create a linearly varying distributed Compos Load.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Pressure.GetAbbreviation(this.ForcePerAreaUnit);

      pManager.AddGenericParameter("Const. Dead 1 [" + unitAbbreviation + "]", "dl1", "Start Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 1 [" + unitAbbreviation + "]", "ll1", "Start Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 1 [" + unitAbbreviation + "]", "DL1", "Start Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 1 [" + unitAbbreviation + "]", "LL1", "Start Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Dead 2 [" + unitAbbreviation + "]", "dl2", "End Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 2 [" + unitAbbreviation + "]", "ll2", "End Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 2 [" + unitAbbreviation + "]", "DL2", "End Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 2 [" + unitAbbreviation + "]", "LL2", "End Final Live Load", GH_ParamAccess.item);
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
          ForcePerLength constDeadL1 = (ForcePerLength)Input.UnitNumber(this, DA, 0, this.ForcePerLengthUnit);
          ForcePerLength constLiveL1 = (ForcePerLength)Input.UnitNumber(this, DA, 1, this.ForcePerLengthUnit);
          ForcePerLength finalDeadL1 = (ForcePerLength)Input.UnitNumber(this, DA, 2, this.ForcePerLengthUnit);
          ForcePerLength finalLiveL1 = (ForcePerLength)Input.UnitNumber(this, DA, 3, this.ForcePerLengthUnit);
          ForcePerLength constDeadL2 = (ForcePerLength)Input.UnitNumber(this, DA, 4, this.ForcePerLengthUnit);
          ForcePerLength constLiveL2 = (ForcePerLength)Input.UnitNumber(this, DA, 5, this.ForcePerLengthUnit);
          ForcePerLength finalDeadL2 = (ForcePerLength)Input.UnitNumber(this, DA, 6, this.ForcePerLengthUnit);
          ForcePerLength finalLiveL2 = (ForcePerLength)Input.UnitNumber(this, DA, 7, this.ForcePerLengthUnit);
          Load loadL = new LinearLoad(
            constDeadL1, constLiveL1, finalDeadL1, finalLiveL1, constDeadL2, constLiveL2, finalDeadL2, finalLiveL2);
          Output.SetItem(this, DA, 0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          Pressure constDeadA1 = (Pressure)Input.UnitNumber(this, DA, 0, this.ForcePerAreaUnit);
          Pressure constLiveA1 = (Pressure)Input.UnitNumber(this, DA, 1, this.ForcePerAreaUnit);
          Pressure finalDeadA1 = (Pressure)Input.UnitNumber(this, DA, 2, this.ForcePerAreaUnit);
          Pressure finalLiveA1 = (Pressure)Input.UnitNumber(this, DA, 3, this.ForcePerAreaUnit);
          Pressure constDeadA2 = (Pressure)Input.UnitNumber(this, DA, 4, this.ForcePerAreaUnit);
          Pressure constLiveA2 = (Pressure)Input.UnitNumber(this, DA, 5, this.ForcePerAreaUnit);
          Pressure finalDeadA2 = (Pressure)Input.UnitNumber(this, DA, 6, this.ForcePerAreaUnit);
          Pressure finalLiveA2 = (Pressure)Input.UnitNumber(this, DA, 7, this.ForcePerAreaUnit);
          Load loadA = new LinearLoad(
            constDeadA1, constLiveA1, finalDeadA1, finalLiveA1, constDeadA2, constLiveA2, finalDeadA2, finalLiveA2);
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
      this._spacerDescriptions = new List<string>(new string[] { "Distribution", "Unit" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // type
      this._dropDownItems.Add(Enum.GetValues(typeof(LoadDistribution)).Cast<LoadDistribution>().Select(x => x.ToString()).ToList());
      this._selectedItems.Add(LoadDistribution.Area.ToString());

      // force unit
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea));
      this._selectedItems.Add(Pressure.GetAbbreviation(this.ForcePerAreaUnit));

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0)
      {
        this.DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), this._selectedItems[i]);
        if (this.DistributionType == LoadDistribution.Line)
        {
          this._dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength);
          this._selectedItems[1] = ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);
        }
        else
        {
          this._dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea);
          this._selectedItems[1] = Pressure.GetAbbreviation(this.ForcePerAreaUnit);
        }
      }
      if (i == 1)
      {
        if (this.DistributionType == LoadDistribution.Line)
          this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), this._selectedItems[i]);
        else
          this.ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this._selectedItems[i]);
      }

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      this.DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), this._selectedItems[0]);
      if (this.DistributionType == LoadDistribution.Line)
        this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), this._selectedItems[1]);
      else
        this.ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this._selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation;
      if (DistributionType == LoadDistribution.Line)
        unitAbbreviation = ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);
      else
        unitAbbreviation = Pressure.GetAbbreviation(this.ForcePerAreaUnit);
      
      int i = 0;
      
      Params.Input[i++].Name = "Const. Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 2 [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
