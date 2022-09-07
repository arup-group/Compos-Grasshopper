using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using UnitsNet.Units;
using UnitsNet.GH;

namespace ComposGH.Components
{
  public class TransverseRebarResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6d88f397-b1da-421f-a8ba-97de100feab1");
    public TransverseRebarResults()
      : base("Transverse Rebar Results",
          "RebarResults",
          "Get transverse rebar results for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Resources.TransverseRebarResults;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description, GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Position", "Pos", "Location for each critical transverse shear position. Values are measured from beam start.", GH_ParamAccess.list);
      pManager.AddTextParameter("Control Surface", "Ctr", "Failure surface for each critical shear position, being either a-a section, b-b section, or e-e section", GH_ParamAccess.list);
      pManager.AddGenericParameter("Eff. Perimeter", "Per", "Effective perimeter at each critical shear position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Transverse Shear", "VEd", "Actual transverse shear force at each critical shear position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Shear Resistance", "VRd", "Total combined shear resistance at each critical shear position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Concrete Resistance", "Cvr", "Concrete shear resistance at each critical shear position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Decking Resistance", "Dvr", "Decking shear resistance at each critical shear position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Mesh Bar Resistance", "Mvr", "Mesh bar shear resistance at each critical shear position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Rebar Resistance", "Rvr", "Rebar shear resistance at each critical shear position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Max Allowed", "VRdc", "Maximum allowed shear resistance at each critical shear position", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)GetInput.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      ITransverseRebarResult result = res.TransverseRebarResults;

      int i = 0;
      SetOutput.List(this, DA, i++, 
        result.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      SetOutput.List(this, DA, i++,
        result.ControlSurface);
      SetOutput.List(this, DA, i++,
        result.EffectiveShearPerimeter.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      SetOutput.List(this, DA, i++,
        result.TransverseShearForce.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      SetOutput.List(this, DA, i++,
        result.TotalShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      SetOutput.List(this, DA, i++,
        result.ConcreteShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      SetOutput.List(this, DA, i++,
        result.DeckingShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      SetOutput.List(this, DA, i++,
        result.MeshBarShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      SetOutput.List(this, DA, i++,
        result.RebarShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      SetOutput.List(this, DA, i++,
        result.MaxAllowedShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
    }

    #region Custom UI
    private ForceUnit ForceUnit = Units.ForceUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Force Unit", "Length Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

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
        this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      else if (i == 1)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
