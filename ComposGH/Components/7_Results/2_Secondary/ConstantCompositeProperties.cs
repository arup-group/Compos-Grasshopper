using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits.Units;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class ConstantCompositeProperties : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ea4b6e90-b769-45e2-bfa6-18b139bd4888");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.SectionPropertiesConstant;
    public ConstantCompositeProperties()
      : base("Composite Static Properties",
        "StaticProps",
          "Get case indifferent calculated composite section properties for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new ComposMemberParameter());
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Eff. Slab Width L", "Bel", "Effective slab width on left side. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Eff. Slab Width R", "Ber", "Effective slab width on right side. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Girder Weld THK Top", "Tta", "Welding thickness at top - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Girder Weld THK Top", "Bta", "Welding thickness at bottom - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Nat. Frequency", "f", "Natural frequency for composite section.", GH_ParamAccess.item);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
      ICompositeSectionProperties result = res.SectionProperties;

      int i = 0;
      Output.SetList(this, DA, i++, result.EffectiveSlabWidthLeft
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.EffectiveSlabWidthRight
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.GirderWeldThicknessTop
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.GirderWeldThicknessBottom
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetItem(this, DA, i++, new GH_UnitNumber(result.NaturalFrequency));
      Output.SetList(this, DA, i, positions);
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
