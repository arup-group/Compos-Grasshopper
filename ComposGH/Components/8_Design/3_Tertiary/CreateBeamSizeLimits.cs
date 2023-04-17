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
  public class CreateBeamSizeLimits : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("a1c37716-886d-4816-afa3-ef0b9ab42f79");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.BeamSizeLimits;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public CreateBeamSizeLimits()
          : base("Create" + BeamSizeLimitsGoo.Name.Replace(" ", string.Empty),
      BeamSizeLimitsGoo.Name.Replace(" ", string.Empty),
      "Create a " + BeamSizeLimitsGoo.Description + " for a " + DesignCriteriaGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat8()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];
      if (LengthUnit.ToString() == _selectedItems[i])
        return;

      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbb = Length.GetAbbreviation(LengthUnit);
      int i = 0;
      Params.Input[i++].Name = "Min Depth [" + unitAbb + "]";
      Params.Input[i++].Name = "Max Depth [" + unitAbb + "]";
      Params.Input[i++].Name = "Min Width [" + unitAbb + "]";
      Params.Input[i++].Name = "Max Width [" + unitAbb + "]";
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
      pManager.AddGenericParameter("Min Depth [" + unitAbb + "]", "Dmin", "(Optional) Minimum Depth (default ≥ 20 cm)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Max Depth [" + unitAbb + "]", "Dmax", "(Optional) Maximum Depth  (default ≤ 100 cm)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Min Width [" + unitAbb + "]", "Wmin", "(Optional) Minimum Width  (default ≥ 10 cm)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Max Width [" + unitAbb + "]", "Wmax", "(Optional) Maximum Width  (default ≤ 50 cm)", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new BeamSizeLimitsParam());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      Length minDepth = new Length(20, LengthUnit.Centimeter);
      if (Params.Input[0].Sources.Count > 0)
        minDepth = (Length)Input.UnitNumber(this, DA, 0, LengthUnit);

      Length maxDepth = new Length(100, LengthUnit.Centimeter);
      if (Params.Input[1].Sources.Count > 0)
        maxDepth = (Length)Input.UnitNumber(this, DA, 1, LengthUnit);

      Length minWidth = new Length(10, LengthUnit.Centimeter);
      if (Params.Input[2].Sources.Count > 0)
        minWidth = (Length)Input.UnitNumber(this, DA, 2, LengthUnit);

      Length maxWidth = new Length(50, LengthUnit.Centimeter);
      if (Params.Input[3].Sources.Count > 0)
        maxWidth = (Length)Input.UnitNumber(this, DA, 3, LengthUnit);

      BeamSizeLimits beamSizeLimits = new BeamSizeLimits() {
        MinDepth = minDepth,
        MaxDepth = maxDepth,
        MinWidth = minWidth,
        MaxWidth = maxWidth
      };

      Output.SetItem(this, DA, 0, new BeamSizeLimitsGoo(beamSizeLimits));
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
