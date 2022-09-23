using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysGH;
using OasysUnits.Units;

namespace ComposGH.Components
{
  public class SlabStressResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6a4a30b1-41b3-4fbb-bcd1-64c7834b6306");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.SlabStressResults;
    public SlabStressResults()
      : base("Slab Stress Results",
          "SlabStress",
          "Get slab stress and strain results for a " + MemberGoo.Description,
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
      pManager.AddGenericParameter("Stress", "σ", "Maximum stress in concrete slab for selected load case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Strain", "ε", "Maximum strain in concrete slab for to selected load case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
      ISlabStressResult result = res.SlabStresses;

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;

      switch (this.SelectedLoad)
      {
        case Load.AdditionalDead:
          outputs0 = result.ConcreteStressAdditionalDeadLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.ConcreteStrainAdditionalDeadLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StrainUnit))).ToList();
          break;

        case Load.LiveLoad:
          outputs0 = result.ConcreteStressFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.ConcreteStrainFinalLiveLoad.Select(x => new GH_UnitNumber(x.ToUnit(this.StrainUnit))).ToList();
          break;

        case Load.Shrinkage:
          outputs0 = result.ConcreteStressFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.ConcreteStrainFinalShrinkage.Select(x => new GH_UnitNumber(x.ToUnit(this.StrainUnit))).ToList();
          break;

        case Load.Final:
          outputs0 = result.ConcreteStressFinal.Select(x => new GH_UnitNumber(x.ToUnit(this.StressUnit))).ToList();
          outputs1 = result.ConcreteStrainFinal.Select(x => new GH_UnitNumber(x.ToUnit(this.StrainUnit))).ToList();
          break;
      }

      int i = 0;
      Output.SetList(this, DA, i++, outputs0);
      Output.SetList(this, DA, i++, outputs1);

      Output.SetList(this, DA, i, positions);
    }

    #region Custom UI
    internal enum Load
    {
      AdditionalDead,
      LiveLoad,
      Shrinkage,
      Final
    }
    private Load SelectedLoad = Load.Final;
    private PressureUnit StressUnit = DefaultUnits.StressUnitResult;
    private StrainUnit StrainUnit = DefaultUnits.StrainUnitResult;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Load", "Stress Unit", "Strain Unit", "Length Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // load
      this.DropDownItems.Add(Enum.GetNames(typeof(Load)).ToList());
      this.SelectedItems.Add(this.SelectedLoad.ToString());

      // stress
      this.DropDownItems.Add(FilteredUnits.FilteredStressUnits);
      this.SelectedItems.Add(this.StressUnit.ToString());

      // strain
      this.DropDownItems.Add(FilteredUnits.FilteredStrainUnits);
      this.SelectedItems.Add(this.StrainUnit.ToString());

      // length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.SelectedLoad = (Load)Enum.Parse(typeof(Load), this.SelectedItems[i]);
      else if (i == 1)
        this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[i]);
      else if (i == 2)
        this.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), this.SelectedItems[i]);
      else if (i == 3)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.SelectedLoad = (Load)Enum.Parse(typeof(Load), this.SelectedItems[0]);
      this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[1]);
      this.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), this.SelectedItems[2]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[3]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
