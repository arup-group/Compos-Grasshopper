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

namespace ComposGH.Components
{
  public class BeamStressResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5ea441b0-03aa-4aa9-a63f-356c1fa05427");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.BeamStressResults;
    public BeamStressResults()
      : base("Beam Stress Results",
          "BeamStress",
          "Get beam stress results for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new ComposMemberParameter());
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
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
      IBeamStressResult result = res.BeamStresses;

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;
      List<GH_UnitNumber> outputs2 = null;

      switch (SelectedCase)
      {
        case Case.Construction:
          outputs0 = result.TopFlangeConstruction.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.WebConstruction.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs2 = result.BottomFlangeConstruction.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          break;

        case Case.AdditionalDead:
          outputs0 = result.TopFlangeFinalAdditionalDeadLoad.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.WebFinalAdditionalDeadLoad.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs2 = result.BottomFlangeConstruction.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          break;

        case Case.LiveLoad:
          outputs0 = result.TopFlangeFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.WebFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs2 = result.BottomFlangeFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          break;

        case Case.Shrinkage:
          outputs0 = result.TopFlangeFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.WebFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs2 = result.BottomFlangeFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          break;

        case Case.Final:
          outputs0 = result.TopFlangeFinal.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.WebFinal.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs2 = result.BottomFlangeFinal.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
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
    private PressureUnit StressUnit = DefaultUnits.StressUnitResult;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    protected override void InitialiseDropdowns()
    {
      _spacerDescriptions = new List<string>(new string[] { "Case", "Stress Unit", "Length Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // Case
      _dropDownItems.Add(Enum.GetNames(typeof(Case)).ToList());
      _selectedItems.Add(SelectedCase.ToString());

      // stress
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(Pressure.GetAbbreviation(StressUnit));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)
        SelectedCase = (Case)Enum.Parse(typeof(Case), _selectedItems[i]);
      else if (i == 1)
        StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
      else if (i == 2)
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      SelectedCase = (Case)Enum.Parse(typeof(Case), _selectedItems[0]);
      StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
