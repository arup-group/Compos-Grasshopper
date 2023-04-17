using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;

namespace ComposGH.Components {
  public class CreateAxialLoad : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("9dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.AxialLoad;
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;

    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public CreateAxialLoad()
                  : base("CreateAxialLoad", "AxialLoad", "Create an Axial Compos Load applied at both end positions.",
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat4()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)
        ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
      if (i == 1)
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(LengthUnit);
      int i = 0;
      Params.Input[i++].Name = "Const. Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Depth 1 [" + lengthunitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Depth 2 [" + lengthunitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Force Unit", "Length Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // force unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(ForceUnit));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(LengthUnit);
      pManager.AddGenericParameter("Const. Dead 1 [" + unitAbbreviation + "]", "dl1", "Start Constant dead load; construction stage dead load which are used for construction stage analysis."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 1 [" + unitAbbreviation + "]", "ll1", "Start Constant live load; construction stage live load which are used for construction stage analysis."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 1 [" + unitAbbreviation + "]", "DL1", "Start Final Dead Load."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 1 [" + unitAbbreviation + "]", "LL1", "Start Final Live Load."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth 1 [" + lengthunitAbbreviation + "]", "dz1", "Start Depth below top of steel where axial load is applied (beam local z-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Dead 2 [" + unitAbbreviation + "]", "dl2", "End Constant dead load; construction stage dead load which are used for construction stage analysis."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 2 [" + unitAbbreviation + "]", "ll2", "End Constant live load; construction stage live load which are used for construction stage analysis."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 2 [" + unitAbbreviation + "]", "DL2", "End Final Dead Load."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 2 [" + unitAbbreviation + "]", "LL2", "End Final Live Load."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth 2 [" + lengthunitAbbreviation + "]", "dz2", "End Depth below top of steel where axial load is applied (beam local z-axis)", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ComposLoadParameter());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      Force constDead1 = (Force)Input.UnitNumber(this, DA, 0, ForceUnit);
      Force constLive1 = (Force)Input.UnitNumber(this, DA, 1, ForceUnit);
      Force finalDead1 = (Force)Input.UnitNumber(this, DA, 2, ForceUnit);
      Force finalLive1 = (Force)Input.UnitNumber(this, DA, 3, ForceUnit);
      Length pos1 = (Length)Input.UnitNumber(this, DA, 4, LengthUnit);
      Force constDead2 = (Force)Input.UnitNumber(this, DA, 5, ForceUnit);
      Force constLive2 = (Force)Input.UnitNumber(this, DA, 6, ForceUnit);
      Force finalDead2 = (Force)Input.UnitNumber(this, DA, 7, ForceUnit);
      Force finalLive2 = (Force)Input.UnitNumber(this, DA, 8, ForceUnit);
      Length pos2 = (Length)Input.UnitNumber(this, DA, 9, LengthUnit);

      Load load = new AxialLoad(
        constDead1, constLive1, finalDead1, finalLive1, pos1, constDead2, constLive2, finalDead2, finalLive2, pos2);
      Output.SetItem(this, DA, 0, new LoadGoo(load));
    }

    protected override void UpdateUIFromSelectedItems() {
      ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
