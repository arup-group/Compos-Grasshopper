using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components
{
  public class CreateTriLinearLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.TriLinearLoad;
    public CreateTriLinearLoad()
      : base("CreateTriLinearLoad", "TriLinearLoad", "Create a tri-linearly varying distributed Compos Load starting from left end of the beam and ending at the right." +
          Environment.NewLine + "The two peak load points can be defined at any positions along the span.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Pressure.GetAbbreviation(ForcePerAreaUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(LengthUnit);
      pManager.AddGenericParameter("Const. Dead 1 [" + unitAbbreviation + "]", "dl1", "Start Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 1 [" + unitAbbreviation + "]", "ll1", "Start Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 1 [" + unitAbbreviation + "]", "DL1", "Start Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 1 [" + unitAbbreviation + "]", "LL1", "Start Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x1 [" + lengthunitAbbreviation + "]", "Px1", "Start Position on beam where line load begins (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Dead 2 [" + unitAbbreviation + "]", "dl2", "End Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 2 [" + unitAbbreviation + "]", "ll2", "End Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 2 [" + unitAbbreviation + "]", "DL2", "End Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 2 [" + unitAbbreviation + "]", "LL2", "End Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x2 [" + lengthunitAbbreviation + "]", "Px2", "End Position on beam where line load ends (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposLoadParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IQuantity pos1 = Input.LengthOrRatio(this, DA, 4, LengthUnit);
      IQuantity pos2 = Input.LengthOrRatio(this, DA, 9, LengthUnit);

      switch (DistributionType)
      {
        case LoadDistribution.Line:
          ForcePerLength constDeadL1 = (ForcePerLength)Input.UnitNumber(this, DA, 0, ForcePerLengthUnit);
          ForcePerLength constLiveL1 = (ForcePerLength)Input.UnitNumber(this, DA, 1, ForcePerLengthUnit);
          ForcePerLength finalDeadL1 = (ForcePerLength)Input.UnitNumber(this, DA, 2, ForcePerLengthUnit);
          ForcePerLength finalLiveL1 = (ForcePerLength)Input.UnitNumber(this, DA, 3, ForcePerLengthUnit);
          ForcePerLength constDeadL2 = (ForcePerLength)Input.UnitNumber(this, DA, 5, ForcePerLengthUnit);
          ForcePerLength constLiveL2 = (ForcePerLength)Input.UnitNumber(this, DA, 6, ForcePerLengthUnit);
          ForcePerLength finalDeadL2 = (ForcePerLength)Input.UnitNumber(this, DA, 7, ForcePerLengthUnit);
          ForcePerLength finalLiveL2 = (ForcePerLength)Input.UnitNumber(this, DA, 8, ForcePerLengthUnit);
          Load loadL = new TriLinearLoad(
            constDeadL1, constLiveL1, finalDeadL1, finalLiveL1, pos1, constDeadL2, constLiveL2, finalDeadL2, finalLiveL2, pos2);
          Output.SetItem(this, DA, 0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          Pressure constDeadA1 = (Pressure)Input.UnitNumber(this, DA, 0, ForcePerAreaUnit);
          Pressure constLiveA1 = (Pressure)Input.UnitNumber(this, DA, 1, ForcePerAreaUnit);
          Pressure finalDeadA1 = (Pressure)Input.UnitNumber(this, DA, 2, ForcePerAreaUnit);
          Pressure finalLiveA1 = (Pressure)Input.UnitNumber(this, DA, 3, ForcePerAreaUnit);
          Pressure constDeadA2 = (Pressure)Input.UnitNumber(this, DA, 5, ForcePerAreaUnit);
          Pressure constLiveA2 = (Pressure)Input.UnitNumber(this, DA, 6, ForcePerAreaUnit);
          Pressure finalDeadA2 = (Pressure)Input.UnitNumber(this, DA, 7, ForcePerAreaUnit);
          Pressure finalLiveA2 = (Pressure)Input.UnitNumber(this, DA, 8, ForcePerAreaUnit);
          Load loadA = new TriLinearLoad(
            constDeadA1, constLiveA1, finalDeadA1, finalLiveA1, pos1, constDeadA2, constLiveA2, finalDeadA2, finalLiveA2, pos2);
          Output.SetItem(this, DA, 0, new LoadGoo(loadA));
          break;
      }
    }

    #region Custom UI
    private ForcePerLengthUnit ForcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;
    private PressureUnit ForcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    private LoadDistribution DistributionType = LoadDistribution.Area;

    protected override void InitialiseDropdowns()
    {
      _spacerDescriptions = new List<string>(new string[] {
        "Distribution",
        "Force Unit",
        "Length Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // type
      _dropDownItems.Add(Enum.GetValues(typeof(LoadDistribution)).Cast<LoadDistribution>().Select(x => x.ToString()).ToList());
      _selectedItems.Add(LoadDistribution.Area.ToString());

      // force unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea));
      _selectedItems.Add(Pressure.GetAbbreviation(ForcePerAreaUnit));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

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
      if (i == 2)
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), _selectedItems[0]);
      if (DistributionType == LoadDistribution.Line)
        ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
      else
        ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
    
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation;
      if (DistributionType == LoadDistribution.Line)
        unitAbbreviation = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);
      else
        unitAbbreviation = Pressure.GetAbbreviation(ForcePerAreaUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(LengthUnit);

      int i = 0;
      Params.Input[i++].Name = "Const. Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Pos x1 [" + lengthunitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Pos x2 [" + lengthunitAbbreviation + "]";
    }
    #endregion
  }
}
