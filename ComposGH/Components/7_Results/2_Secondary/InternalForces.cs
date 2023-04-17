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
  public class InternalForces : GH_OasysDropDownComponent {
    internal enum Case {
      ConstructionDead,
      ConstructionLive,
      AdditionalDead,
      LiveLoad,
      ShrinkageMoment,
      ConstructionULS,
      FinalUltimate
    }

    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ceece06d-48e7-4dd4-9d1e-895872080c12");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.InternalForceResults;
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;

    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    private MomentUnit MomentUnit = DefaultUnits.MomentUnit;

    private Case SelectedCase = Case.FinalUltimate;

    public InternalForces()
                                  : base("Internal Force Results",
      "Internal Forces",
      "Get the axial, shear and moment internal force results for a " + MemberGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat7()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 0)
        SelectedCase = (Case)Enum.Parse(typeof(Case), _selectedItems[i]);
      else if (i == 1)
        MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[i]);
      else if (i == 2)
        ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
      else if (i == 3)
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

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
      pManager.AddGenericParameter("Axial force", "NEd", "Axial force for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Shear force", "VEd", "Shear force for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Moment", "MEd", "Moment for selected case. Values given at each position", GH_ParamAccess.list);

      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
      IInternalForceResult result = res.InternalForces;

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;
      List<GH_UnitNumber> outputs2 = null;

      switch (SelectedCase) {
        case Case.ConstructionDead:
          outputs0 = result.AxialConstructionDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs1 = result.ShearConstructionDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs2 = result.MomentConstructionDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          break;

        case Case.ConstructionLive:
          outputs0 = result.AxialConstructionLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs1 = result.ShearConstructionLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs2 = result.MomentConstructionLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          break;

        case Case.AdditionalDead:
          outputs0 = result.AxialFinalAdditionalDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs1 = result.ShearFinalAdditionalDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs2 = result.MomentFinalAdditionalDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          break;

        case Case.LiveLoad:
          outputs0 = result.AxialFinalLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs1 = result.ShearFinalLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs2 = result.MomentFinalLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          break;

        case Case.ShrinkageMoment:
          outputs0 = null;
          outputs1 = null;
          outputs2 = result.MomentFinalShrinkage
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          break;

        case Case.ConstructionULS:
          outputs0 = result.AxialULSConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs1 = result.ShearULSConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs2 = result.MomentULSConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
          break;

        case Case.FinalUltimate:
          outputs0 = result.AxialULS
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs1 = result.ShearULS
            .Select(x => new GH_UnitNumber(x.ToUnit(ForceUnit))).ToList();
          outputs2 = result.MomentULS
            .Select(x => new GH_UnitNumber(x.ToUnit(MomentUnit))).ToList();
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
      MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[1]);
      ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[2]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[3]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
