using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Properties;
using ComposGH.Parameters;
using OasysGH.Components;
using OasysGH.Helpers;
using UnitsNet.Units;
using UnitsNet.GH;
using static ComposGH.Components.CompositeProperties;

namespace ComposGH.Components
{
  public class BeamStressResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5ea441b0-03aa-4aa9-a63f-356c1fa05427");
    public BeamStressResults()
      : base("Beam Stress Results",
          "BeamStress",
          "Get beam stress results for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Resources.BeamStressResults;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description, GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("TopFlange", "Tfl", "Maximum stress in steel beam top flange for selected load case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Web", "Web", "Maximum stress in steel beam web due to selected load case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("BottomFlange", "Bfl", "Maximum stress in steel beam bottom flange for selected load case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)GetInput.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
      IBeamStressResult result = res.BeamStresses;

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;
      List<GH_UnitNumber> outputs2 = null;

      switch (this.SelectedCase)
      {
        case Case.Construction:
          outputs0 = result.TopFlangeConstruction.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.WebConstruction.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs2 = result.BottomFlangeConstruction.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          break;

        case Case.AdditionalDead:
          outputs0 = result.TopFlangeFinalAdditionalDeadLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.WebFinalAdditionalDeadLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs2 = result.BottomFlangeConstruction.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          break;

        case Case.LiveLoad:
          outputs0 = result.TopFlangeFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.WebFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs2 = result.BottomFlangeFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          break;

        case Case.Shrinkage:
          outputs0 = result.TopFlangeFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.WebFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs2 = result.BottomFlangeFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          break;

        case Case.Final:
          outputs0 = result.TopFlangeFinal.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.WebFinal.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs2 = result.BottomFlangeFinal.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
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
      Construction,
      AdditionalDead,
      LiveLoad,
      Shrinkage,
      Final
    }
    private Case SelectedCase = Case.Final;
    private PressureUnit StressUnit = Units.StressUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Case", "Stress Unit", "Length Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Case
      this.DropDownItems.Add(Enum.GetNames(typeof(Case)).ToList());
      this.SelectedItems.Add(this.SelectedCase.ToString());

      // stress
      this.DropDownItems.Add(Units.FilteredStressUnits);
      this.SelectedItems.Add(this.StressUnit.ToString());

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.SelectedCase = (Case)Enum.Parse(typeof(Case), this.SelectedItems[i]);
      else if (i == 1)
        this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[i]);
      else if (i == 2)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.SelectedCase = (Case)Enum.Parse(typeof(Case), this.SelectedItems[0]);
      this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[1]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
