using System;
using System.Collections.Generic;
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
  public class CreateDeflectionLimit : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("02b54c18-a142-4e9c-a2ad-715a71c962f7");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.DeflectionLimit;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitResult;

    public CreateDeflectionLimit() : base("Create" + DeflectionLimitGoo.Name.Replace(" ", string.Empty),
      DeflectionLimitGoo.Name.Replace(" ", string.Empty),
      "Create a " + DeflectionLimitGoo.Description + " for a " + DesignCriteriaGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat8()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];
      if (LengthUnit.ToString() == _selectedItems[i]) {
        return;
      }

      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbb = Length.GetAbbreviation(LengthUnit);
      Params.Input[0].Name = "Absolute Deflection [" + unitAbb + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbb = Length.GetAbbreviation(LengthUnit);
      pManager.AddGenericParameter("Absolute Deflection [" + unitAbb + "]", "Abs", "Absolute Deflection", GH_ParamAccess.item);
      pManager.AddGenericParameter("Span/Deflection Ratio [L/x]", "L/δ", "Span over Deflection ratio, for instance input '500' for 'L/500'", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new DeflectionLimitParam());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      if (Params.Input[0].Sources.Count == 0 &
        Params.Input[1].Sources.Count == 0) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Inputs failed to collect data.");
        return;
      }

      var deflectionLimit = new DeflectionLimit();

      if (Params.Input[0].Sources.Count > 0) {
        deflectionLimit.AbsoluteDeflection = (Length)Input.UnitNumber(this, DA, 0, LengthUnit, true);
      }

      if (Params.Input[1].Sources.Count > 0) {
        deflectionLimit.SpanOverDeflectionRatio = (Ratio)Input.UnitNumber(this, DA, 1, RatioUnit.DecimalFraction);
      }

      Output.SetItem(this, DA, 0, new DeflectionLimitGoo(deflectionLimit));
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
