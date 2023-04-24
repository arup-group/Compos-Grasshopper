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
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components {
  public class CapacityResults : GH_OasysDropDownComponent {
    internal enum Case {
      HoggingConstruction,
      SaggingConstruction,
      Hogging,
      Sagging,
      PlasticSteelHogging,
      PlasticSteel,
      FullInteractHogging,
      FullInteraction
    }

    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("456bccbd-32cc-408f-9001-168834a323a3");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CapacityResults;
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;

    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    private MomentUnit MomentUnit = DefaultUnits.MomentUnit;

    private Case SelectedCase = Case.Sagging;

    public CapacityResults() : base("Capacity Results",
      "Capacities",
      "Get the moment and shear resistance/capacity results for a " + MemberGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat7()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0) {
        SelectedCase = (Case)Enum.Parse(typeof(Case), _selectedItems[i]);
      } else if (i == 1) {
        MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[i]);
      } else if (i == 2) {
        ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
      } else if (i == 3) {
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Case", "Moment Unit", "Force Unit", "Length Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // case
      _dropDownItems.Add(Enum.GetNames(typeof(Case)).ToList());
      _selectedItems.Add(SelectedCase.ToString());

      // moment
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment));
      _selectedItems.Add(Moment.GetAbbreviation(MomentUnit));

      // force
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(ForceUnit));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Moment", "Mrd", "Moment capacity for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Neutral line position", "x", "Neutral line (measured from bottom of steel beam) for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Shear capacity", "Vrd", "Shear capacity. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Shear buckling capacity", "Vbrd", "Shear capacity with web buckling. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Used shear capacity", "Vr", "Used shear capacity. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      var positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
      ICapacityResult result = res.Capacities;

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;

      switch (SelectedCase) {
        case Case.HoggingConstruction:
          outputs0 = result.MomentHoggingConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          outputs1 = result.NeutralAxisHoggingConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          break;

        case Case.SaggingConstruction:
          outputs0 = result.MomentConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          outputs1 = result.NeutralAxisConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          break;

        case Case.Hogging:
          outputs0 = result.MomentHoggingFinal
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          outputs1 = result.NeutralAxisHoggingFinal
            .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          break;

        case Case.Sagging:
          outputs0 = result.Moment
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          outputs1 = result.NeutralAxis
            .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          break;

        case Case.PlasticSteelHogging:
          outputs0 = result.AssumedBeamPlasticMomentHogging
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          outputs1 = result.AssumedPlasticNeutralAxisHogging
            .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          break;

        case Case.PlasticSteel:
          outputs0 = result.AssumedBeamPlasticMoment
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          outputs1 = result.AssumedPlasticNeutralAxis
            .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          break;

        case Case.FullInteractHogging:
          outputs0 = result.AssumedMomentFullShearInteractionHogging
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          outputs1 = result.AssumedNeutralAxisFullShearInteractionHogging
            .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          break;

        case Case.FullInteraction:
          outputs0 = result.AssumedMomentFullShearInteraction
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          outputs1 = result.AssumedNeutralAxisFullShearInteraction
            .Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
          break;
      }

      int i = 0;
      Output.SetList(this, DA, i++, outputs0);
      Output.SetList(this, DA, i++, outputs1);

      Output.SetList(this, DA, i++, result.Shear
        .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList());
      Output.SetList(this, DA, i++, result.ShearBuckling
        .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList());
      Output.SetList(this, DA, i++, result.ShearRequired
        .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList());

      Output.SetList(this, DA, i, positions);
    }

    protected override void UpdateUIFromSelectedItems() {
      SelectedCase = (Case)Enum.Parse(typeof(Case), _selectedItems[0]);
      MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[1]);
      ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[2]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[3]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
