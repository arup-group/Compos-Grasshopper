using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits.Units;

namespace ComposGH.Components
{
  public class TransverseRebarResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6d88f397-b1da-421f-a8ba-97de100feab1");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.TransverseRebarResults;
    public TransverseRebarResults()
      : base("Transverse Rebar Results",
          "RebarResults",
          "Get transverse rebar results for a " + MemberGoo.Description,
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
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      ITransverseRebarResult result = res.TransverseRebarResults;

      int i = 0;
      Output.SetList(this, DA, i++, 
        result.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.ControlSurface.Select(x => new GH_String(x)).ToList());
      Output.SetList(this, DA, i++,
        result.EffectiveShearPerimeter.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.TransverseShearForce.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.TotalShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.ConcreteShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.DeckingShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.MeshBarShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.RebarShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      Output.SetList(this, DA, i++,
        result.MaxAllowedShearResistance.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
    }

    #region Custom UI
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Force Unit", "Length Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // force
      this.DropDownItems.Add(FilteredUnits.FilteredForceUnits);
      this.SelectedItems.Add(this.ForceUnit.ToString());

      // length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      else if (i == 1)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
