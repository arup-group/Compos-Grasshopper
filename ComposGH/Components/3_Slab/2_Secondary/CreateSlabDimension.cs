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
  public class CreateSlabDimension : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("3da0ace2-b5a0-4a6a-8bf0-d669800c1f08");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.SlabDimensions;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public CreateSlabDimension() : base("Create" + SlabDimensionGoo.Name.Replace(" ", string.Empty),
      SlabDimensionGoo.Name.Replace(" ", string.Empty),
      "Create a " + SlabDimensionGoo.Description + " for a " + SlabGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[0].Name = "Start [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Overall depth [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Available width Left [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Available width Right [" + unitAbbreviation + "]";
      Params.Input[4].Name = "Effective width Left [" + unitAbbreviation + "]";
      Params.Input[5].Name = "Effective width Right [" + unitAbbreviation + "]";
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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Start [" + unitAbbreviation + "]", "Px", "(Optional) Start Position of this profile (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddNumberParameter("Depth [" + unitAbbreviation + "]", "D", "Overall depth", GH_ParamAccess.item);
      pManager.AddNumberParameter("Width Left [" + unitAbbreviation + "]", "WL", "Available width left", GH_ParamAccess.item);
      pManager.AddNumberParameter("Width Right [" + unitAbbreviation + "]", "WR", "Available width right", GH_ParamAccess.item);
      pManager.AddNumberParameter("Effective width Left [" + unitAbbreviation + "]", "EWL", "(Optional) Effective width left", GH_ParamAccess.item);
      pManager.AddNumberParameter("Effective width Right [" + unitAbbreviation + "]", "EWR", "(Optional) Effective width right", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Tapered", "Tp", "Taper to next (default = false)", GH_ParamAccess.item, false);
      pManager[0].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
      pManager[6].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new SlabDimensionParam());
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      var start = (Length)Input.UnitNumber(this, DA, 0, LengthUnit, true);
      var overallDepth = (Length)Input.UnitNumber(this, DA, 1, LengthUnit, true);
      var availableWidthLeft = (Length)Input.UnitNumber(this, DA, 2, LengthUnit, true);
      var availableWidthRight = (Length)Input.UnitNumber(this, DA, 3, LengthUnit, true);

      bool customEffectiveWidth = false;
      Length effectiveWidthLeft = Length.Zero;
      Length effectiveWidthRight = Length.Zero;

      if (Params.Input[4].Sources.Count > 0 && Params.Input[5].Sources.Count > 0) {
        customEffectiveWidth = true;
        effectiveWidthLeft = (Length)Input.UnitNumber(this, DA, 4, LengthUnit, true);
        effectiveWidthRight = (Length)Input.UnitNumber(this, DA, 5, LengthUnit, true);
      }
      bool taperedToNext = false;
      DA.GetData(6, ref taperedToNext);

      SlabDimension slabDimension;
      if (customEffectiveWidth) {
        slabDimension = new SlabDimension(start, overallDepth, availableWidthLeft, availableWidthRight, effectiveWidthLeft, effectiveWidthRight, taperedToNext);
      } else {
        slabDimension = new SlabDimension(start, overallDepth, availableWidthLeft, availableWidthRight, taperedToNext);
      }

      DA.SetData(0, new SlabDimensionGoo(slabDimension));
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
