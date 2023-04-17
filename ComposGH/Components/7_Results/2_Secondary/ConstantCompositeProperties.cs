using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposGH.Components {
  public class ConstantCompositeProperties : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ea4b6e90-b769-45e2-bfa6-18b139bd4888");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.SectionPropertiesConstant;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public ConstantCompositeProperties()
          : base("Composite Static Properties",
    "StaticProps",
      "Get case indifferent calculated composite section properties for a " + MemberGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat7()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
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
      pManager.AddParameter(new ComposMemberParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Eff. Slab Width L", "Bel", "Effective slab width on left side. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Eff. Slab Width R", "Ber", "Effective slab width on right side. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Girder Weld THK Top", "Tta", "Welding thickness at top - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Girder Weld THK Top", "Bta", "Welding thickness at bottom - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Nat. Frequency", "f", "Natural frequency for composite section.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
      ICompositeSectionProperties result = res.SectionProperties;

      int i = 0;
      Output.SetList(this, DA, i++, result.EffectiveSlabWidthLeft
        .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.EffectiveSlabWidthRight
        .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.GirderWeldThicknessTop
        .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.GirderWeldThicknessBottom
        .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList());
      Output.SetItem(this, DA, i++, new GH_UnitNumber(result.NaturalFrequency));
      Output.SetList(this, DA, i, positions);
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
