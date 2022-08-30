﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;
using Oasys.Units;
using UnitsNet.GH;

namespace ComposGH.Components
{
  public class SlabStressResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6a4a30b1-41b3-4fbb-bcd1-64c7834b6306");
    public SlabStressResults()
      : base("Slab Stress Results",
          "SlabStress",
          "Get slab stress and strain results for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.SlabStressResults;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description, GH_ParamAccess.item);
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
      IResult res = ((MemberGoo)GetInput.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
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
      SetOutput.List(this, DA, i++, outputs0);
      SetOutput.List(this, DA, i++, outputs1);

      SetOutput.List(this, DA, i, positions);
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
    private PressureUnit StressUnit = Units.StressUnit;
    private StrainUnit StrainUnit = Units.StrainUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Load", "Stress Unit", "Strain Unit", "Length Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // load
      this.DropDownItems.Add(Enum.GetNames(typeof(Load)).ToList());
      this.SelectedItems.Add(this.SelectedLoad.ToString());

      // stress
      this.DropDownItems.Add(Units.FilteredStressUnits);
      this.SelectedItems.Add(this.StressUnit.ToString());

      // strain
      this.DropDownItems.Add(Units.FilteredStrainUnits);
      this.SelectedItems.Add(this.StrainUnit.ToString());

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
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

    internal override void UpdateUIFromSelectedItems()
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
