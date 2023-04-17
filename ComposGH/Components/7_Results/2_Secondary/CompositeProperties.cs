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
  public class CompositeProperties : GH_OasysDropDownComponent {
    internal enum Case {
      BeamOnly,
      LongTerm,
      ShortTerm,
      Shrinkage,
      Effective,
      Vibration
    }

    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("afdb280d-8af4-4aba-b4c0-e9247b5b16d3");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.SectionProperties;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    private Case SelectedCase = Case.LongTerm;

    public CompositeProperties()
                  : base("Composite Section Properties",
      "CompositeProps",
      "Get calculated, case dependent, composite section properties for a " + MemberGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat7()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0)
        SelectedCase = (Case)Enum.Parse(typeof(Case), _selectedItems[i]);
      else if (i == 1)
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Case", "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // case
      _dropDownItems.Add(Enum.GetNames(typeof(Case)).ToList());
      _selectedItems.Add(SelectedCase.ToString());

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Moment of Inertia", "I", "Moment of intertia for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Neutral line position", "x", "Neutral line (measured from bottom of steel beam) for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Area", "A", "Area for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
      ICompositeSectionProperties result = res.SectionProperties;

      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(LengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(LengthUnit);

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;
      List<GH_UnitNumber> outputs2 = null;

      switch (SelectedCase) {
        case Case.BeamOnly:
          outputs0 = result.BeamMomentOfInertia.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.BeamNeutralAxisPosition.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          outputs2 = result.BeamArea.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.LongTerm:
          outputs0 = result.MomentOfInertiaLongTerm.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionLongTerm.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          outputs2 = result.AreaLongTerm.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.ShortTerm:
          outputs0 = result.MomentOfInertiaShortTerm.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionShortTerm.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          outputs2 = result.AreaShortTerm.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.Shrinkage:
          outputs0 = result.MomentOfInertiaShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          outputs2 = result.AreaShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.Effective:
          outputs0 = result.MomentOfInertiaEffective.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionEffective.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          outputs2 = result.AreaEffective.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.Vibration:
          outputs0 = result.MomentOfInertiaVibration.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionVibration.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          outputs2 = result.AreaVibration.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;
      }

      int i = 0;
      Output.SetList(this, DA, i++, outputs0);
      Output.SetList(this, DA, i++, outputs1);
      Output.SetList(this, DA, i++, outputs2);

      Output.SetList(this, DA, i, positions);
    }

    protected override void UpdateUIFromSelectedItems() {
      SelectedCase = (Case)Enum.Parse(typeof(Case), _selectedItems[0]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
