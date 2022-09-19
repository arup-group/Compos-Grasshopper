using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnitsNet.Units;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CompositeProperties : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("afdb280d-8af4-4aba-b4c0-e9247b5b16d3");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.SectionProperties;
    public CompositeProperties()
      : base("Composite Section Properties",
          "CompositeProps",
          "Get calculated, case dependent, composite section properties for a " + MemberGoo.Description,
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
      pManager.AddGenericParameter("Moment of Inertia", "I", "Moment of intertia for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Neutral line position", "x", "Neutral line (measured from bottom of steel beam) for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Area", "A", "Area for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
      ICompositeSectionProperties result = res.SectionProperties;

      AreaUnit areaUnit = UnitsHelper.GetAreaUnit(this.LengthUnit);
      AreaMomentOfInertiaUnit inertiaUnit = UnitsHelper.GetAreaMomentOfInertiaUnit(this.LengthUnit);

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;
      List<GH_UnitNumber> outputs2 = null;

      switch (this.SelectedCase)
      {
        case Case.BeamOnly:
          outputs0 = result.BeamMomentOfInertia.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.BeamNeutralAxisPosition.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
          outputs2 = result.BeamArea.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.LongTerm:
          outputs0 = result.MomentOfInertiaLongTerm.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionLongTerm.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
          outputs2 = result.AreaLongTerm.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.ShortTerm:
          outputs0 = result.MomentOfInertiaShortTerm.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionShortTerm.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
          outputs2 = result.AreaShortTerm.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.Shrinkage:
          outputs0 = result.MomentOfInertiaShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
          outputs2 = result.AreaShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.Effective:
          outputs0 = result.MomentOfInertiaEffective.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionEffective.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
          outputs2 = result.AreaEffective.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

        case Case.Vibration:
          outputs0 = result.MomentOfInertiaVibration.Select(x => new GH_UnitNumber(x.ToUnit(inertiaUnit))).ToList();
          outputs1 = result.NeutralAxisPositionVibration.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
          outputs2 = result.AreaVibration.Select(x => new GH_UnitNumber(x.ToUnit(areaUnit))).ToList();
          break;

      }

      int i = 0;
      Output.SetList(this, DA, i++, outputs0);
      Output.SetList(this, DA, i++, outputs1);
      Output.SetList(this, DA, i++, outputs2);

      Output.SetList(this, DA, i, positions);
    }

    #region Custom UI
    internal enum Case
    {
      BeamOnly,
      LongTerm,
      ShortTerm,
      Shrinkage,
      Effective,
      Vibration
    }
    private Case SelectedCase = Case.LongTerm;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Case", "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // case
      this.DropDownItems.Add(Enum.GetNames(typeof(Case)).ToList());
      this.SelectedItems.Add(this.SelectedCase.ToString());

      // length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)
        this.SelectedCase = (Case)Enum.Parse(typeof(Case), this.SelectedItems[i]);
      else if (i == 1)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.SelectedCase = (Case)Enum.Parse(typeof(Case), this.SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
