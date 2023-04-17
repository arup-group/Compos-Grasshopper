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
  public class SlabStressResults : GH_OasysDropDownComponent {
    internal enum Load {
      AdditionalDead,
      LiveLoad,
      Shrinkage,
      Final
    }

    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6a4a30b1-41b3-4fbb-bcd1-64c7834b6306");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.SlabStressResults;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    private Load SelectedLoad = Load.Final;

    private StrainUnit StrainUnit = DefaultUnits.StrainUnitResult;

    private PressureUnit StressUnit = DefaultUnits.StressUnitResult;

    public SlabStressResults()
                                  : base("Slab Stress Results",
      "SlabStress",
      "Get slab stress and strain results for a " + MemberGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat7()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)
        SelectedLoad = (Load)Enum.Parse(typeof(Load), _selectedItems[i]);
      else if (i == 1)
        StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
      else if (i == 2)
        StrainUnit = (StrainUnit)UnitsHelper.Parse(typeof(StrainUnit), _selectedItems[i]);
      else if (i == 3)
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Load", "Stress Unit", "Strain Unit", "Length Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // load
      _dropDownItems.Add(Enum.GetNames(typeof(Load)).ToList());
      _selectedItems.Add(SelectedLoad.ToString());

      // stress
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(Pressure.GetAbbreviation(StressUnit));

      // strain
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Strain));
      _selectedItems.Add(Strain.GetAbbreviation(StrainUnit));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Stress", "σ", "Maximum stress in concrete slab for selected load case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Strain", "ε", "Maximum strain in concrete slab for to selected load case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(LengthUnit))).ToList();
      ISlabStressResult result = res.SlabStresses;

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;

      switch (SelectedLoad) {
        case Load.AdditionalDead:
          outputs0 = result.ConcreteStressAdditionalDeadLoad.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.ConcreteStrainAdditionalDeadLoad.Select(x => new GH_UnitNumber(x.ToUnit(StrainUnit))).ToList();
          break;

        case Load.LiveLoad:
          outputs0 = result.ConcreteStressFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.ConcreteStrainFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(StrainUnit))).ToList();
          break;

        case Load.Shrinkage:
          outputs0 = result.ConcreteStressFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.ConcreteStrainFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(StrainUnit))).ToList();
          break;

        case Load.Final:
          outputs0 = result.ConcreteStressFinal.Select(x => new GH_UnitNumber(x.ToUnit(StressUnit))).ToList();
          outputs1 = result.ConcreteStrainFinal.Select(x => new GH_UnitNumber(x.ToUnit(StrainUnit))).ToList();
          break;
      }

      int i = 0;
      Output.SetList(this, DA, i++, outputs0);
      Output.SetList(this, DA, i++, outputs1);

      Output.SetList(this, DA, i, positions);
    }

    protected override void UpdateUIFromSelectedItems() {
      SelectedLoad = (Load)Enum.Parse(typeof(Load), _selectedItems[0]);
      StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      StrainUnit = (StrainUnit)UnitsHelper.Parse(typeof(StrainUnit), _selectedItems[2]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[3]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
