﻿using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Oasys.Units;
using UnitsNet.Units;
using UnitsNet.GH;

namespace ComposGH.Components
{
  public class InternalForces : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ceece06d-48e7-4dd4-9d1e-895872080c12");
    public InternalForces()
      : base("Internal Force Results",
          "Internal Forces",
          "Get the axial, shear and moment internal force results for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Resources.InternalForceResults;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description, GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Axial force", "NEd", "Axial force for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Shear force", "VEd", "Shear force for selected case. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Moment", "MEd", "Moment for selected case. Values given at each position", GH_ParamAccess.list);

      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)GetInput.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
      IInternalForceResult result = res.InternalForces;

      List<GH_UnitNumber> outputs0 = null;
      List<GH_UnitNumber> outputs1 = null;
      List<GH_UnitNumber> outputs2 = null;

      switch (this.SelectedCase)
      {
        case Case.ConstructionDead:
          outputs0 = result.AxialConstructionDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs1 = result.ShearConstructionDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs2 = result.MomentConstructionDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.MomentUnit))).ToList();
          break;

        case Case.ConstructionLive:
          outputs0 = result.AxialConstructionLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs1 = result.ShearConstructionLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs2 = result.MomentConstructionLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.MomentUnit))).ToList();
          break;

        case Case.AdditionalDead:
          outputs0 = result.AxialFinalAdditionalDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs1 = result.ShearFinalAdditionalDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs2 = result.MomentFinalAdditionalDeadLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.MomentUnit))).ToList();
          break;

        case Case.LiveLoad:
          outputs0 = result.AxialFinalLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs1 = result.ShearFinalLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs2 = result.MomentFinalLiveLoad
            .Select(x => new GH_UnitNumber(x.ToUnit(this.MomentUnit))).ToList();
          break;

        case Case.ShrinkageMoment:
          outputs0 = null;
          outputs1 = null;
          outputs2 = result.MomentFinalShrinkage
            .Select(x => new GH_UnitNumber(x.ToUnit(this.MomentUnit))).ToList();
          break;

        case Case.ConstructionULS:
          outputs0 = result.AxialULSConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs1 = result.ShearULSConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs2 = result.MomentULSConstruction
            .Select(x => new GH_UnitNumber(x.ToUnit(this.MomentUnit))).ToList();
          break;

        case Case.FinalUltimate:
          outputs0 = result.AxialULS
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs1 = result.ShearULS
            .Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList();
          outputs2 = result.MomentULS
            .Select(x => new GH_UnitNumber(x.ToUnit(this.MomentUnit))).ToList();
          break;
      }

      int i = 0;
      SetOutput.List(this, DA, i++, outputs0);
      SetOutput.List(this, DA, i++, outputs1);
      SetOutput.List(this, DA, i++, outputs2);

      SetOutput.List(this, DA, i, positions);
    }

    #region Custom UI
    internal enum Case
    {
      ConstructionDead,
      ConstructionLive,
      AdditionalDead,
      LiveLoad,
      ShrinkageMoment,
      ConstructionULS,
      FinalUltimate
    }
    private Case SelectedCase = Case.FinalUltimate;
    private MomentUnit MomentUnit = Units.MomentUnit;
    private ForceUnit ForceUnit = Units.ForceUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Case", "Moment Unit", "Force Unit", "Length Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // case
      this.DropDownItems.Add(Enum.GetNames(typeof(Case)).ToList());
      this.SelectedItems.Add(this.SelectedCase.ToString());

      // moment
      this.DropDownItems.Add(Units.FilteredMomentUnits);
      this.SelectedItems.Add(this.MomentUnit.ToString());

      // force
      this.DropDownItems.Add(Units.FilteredForceUnits);
      this.SelectedItems.Add(this.ForceUnit.ToString());

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)
        this.SelectedCase = (Case)Enum.Parse(typeof(Case), this.SelectedItems[i]);
      else if (i == 1)
        this.MomentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), this.SelectedItems[i]);
      else if (i == 2)
        this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      else if (i == 3)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.SelectedCase = (Case)Enum.Parse(typeof(Case), this.SelectedItems[0]);
      this.MomentUnit = (MomentUnit)Enum.Parse(typeof(MomentUnit), this.SelectedItems[1]);
      this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[2]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[3]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}