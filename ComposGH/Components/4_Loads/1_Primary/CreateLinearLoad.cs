using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ComposGH.Components {
  public class CreateLinearLoad : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.LinearLoad;
    private LoadDistribution DistributionType = LoadDistribution.Area;

    private PressureUnit ForcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;

    private ForcePerLengthUnit ForcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;

    public CreateLinearLoad() : base("CreateLinearLoad", "LinearLoad", "Create a linearly varying distributed Compos Load.",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat4()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), _selectedItems[i]);
        if (DistributionType == LoadDistribution.Line) {
          _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength);
          _selectedItems[1] = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);
        } else {
          _dropDownItems[1] = UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerArea);
          _selectedItems[1] = Pressure.GetAbbreviation(ForcePerAreaUnit);
        }
      }
      if (i == 1) {
        if (DistributionType == LoadDistribution.Line) {
          ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[i]);
        } else {
          ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
        }
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation;
      if (DistributionType == LoadDistribution.Line) {
        unitAbbreviation = ForcePerLength.GetAbbreviation(ForcePerLengthUnit);
      } else {
        unitAbbreviation = Pressure.GetAbbreviation(ForcePerAreaUnit);
      }

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

    protected override void InitialiseDropdowns() {
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

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(ForcePerAreaUnit);

      pManager.AddGenericParameter("Const. Dead 1 [" + unitAbbreviation + "]", "dl1", "Start Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 1 [" + unitAbbreviation + "]", "ll1", "Start Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 1 [" + unitAbbreviation + "]", "DL1", "Start Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 1 [" + unitAbbreviation + "]", "LL1", "Start Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Dead 2 [" + unitAbbreviation + "]", "dl2", "End Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 2 [" + unitAbbreviation + "]", "ll2", "End Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 2 [" + unitAbbreviation + "]", "DL2", "End Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 2 [" + unitAbbreviation + "]", "LL2", "End Final Live Load", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ComposLoadParameter());
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      switch (DistributionType) {
        case LoadDistribution.Line:
          var constDeadL1 = (ForcePerLength)Input.UnitNumber(this, DA, 0, ForcePerLengthUnit);
          var constLiveL1 = (ForcePerLength)Input.UnitNumber(this, DA, 1, ForcePerLengthUnit);
          var finalDeadL1 = (ForcePerLength)Input.UnitNumber(this, DA, 2, ForcePerLengthUnit);
          var finalLiveL1 = (ForcePerLength)Input.UnitNumber(this, DA, 3, ForcePerLengthUnit);
          var constDeadL2 = (ForcePerLength)Input.UnitNumber(this, DA, 4, ForcePerLengthUnit);
          var constLiveL2 = (ForcePerLength)Input.UnitNumber(this, DA, 5, ForcePerLengthUnit);
          var finalDeadL2 = (ForcePerLength)Input.UnitNumber(this, DA, 6, ForcePerLengthUnit);
          var finalLiveL2 = (ForcePerLength)Input.UnitNumber(this, DA, 7, ForcePerLengthUnit);
          Load loadL = new LinearLoad(
            constDeadL1, constLiveL1, finalDeadL1, finalLiveL1, constDeadL2, constLiveL2, finalDeadL2, finalLiveL2);
          DA.SetData(0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          var constDeadA1 = (Pressure)Input.UnitNumber(this, DA, 0, ForcePerAreaUnit);
          var constLiveA1 = (Pressure)Input.UnitNumber(this, DA, 1, ForcePerAreaUnit);
          var finalDeadA1 = (Pressure)Input.UnitNumber(this, DA, 2, ForcePerAreaUnit);
          var finalLiveA1 = (Pressure)Input.UnitNumber(this, DA, 3, ForcePerAreaUnit);
          var constDeadA2 = (Pressure)Input.UnitNumber(this, DA, 4, ForcePerAreaUnit);
          var constLiveA2 = (Pressure)Input.UnitNumber(this, DA, 5, ForcePerAreaUnit);
          var finalDeadA2 = (Pressure)Input.UnitNumber(this, DA, 6, ForcePerAreaUnit);
          var finalLiveA2 = (Pressure)Input.UnitNumber(this, DA, 7, ForcePerAreaUnit);
          Load loadA = new LinearLoad(
            constDeadA1, constLiveA1, finalDeadA1, finalLiveA1, constDeadA2, constLiveA2, finalDeadA2, finalLiveA2);
          DA.SetData(0, new LoadGoo(loadA));
          break;
      }
    }

    protected override void UpdateUIFromSelectedItems() {
      DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), _selectedItems[0]);
      if (DistributionType == LoadDistribution.Line) {
        ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
      } else {
        ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      }

      base.UpdateUIFromSelectedItems();
    }
  }
}
