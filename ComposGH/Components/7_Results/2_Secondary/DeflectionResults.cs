using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;
using UnitsNet.Units;
using UnitsNet.GH;

namespace ComposGH.Components
{
  public class DeflectionResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("88d3ea49-2f7b-4ce6-bf47-3d9a1be57651");
    public DeflectionResults()
      : base("Deflection Results",
          "Deflections",
          "Get deflection results for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Resources.DeflectionResults;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new ComposMemberParameter());
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Construction Dead Load", "DL", "Construction stage dead load deflection results. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Additional Dead Load", "+DL", "Additional dead load deflection results. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Live Load", "LL", "Live load deflection results. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Shrinkage", "Shk", "Shrinkage load deflection results. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Post construction", "PS", "Post construction deflection results. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Total", "Tot", "Total deflection results. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
      IDeflectionResult result = res.Deflections;


      int i = 0;
      Output.SetList(this, DA, i++, result.ConstructionDeadLoad
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.AdditionalDeadLoad
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.LiveLoad
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.Shrinkage
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.PostConstruction
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());
      Output.SetList(this, DA, i++, result.Total
        .Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList());

      Output.SetList(this, DA, i, positions);
    }

    #region Custom UI
    private LengthUnit LengthUnit = Units.LengthUnitResult;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
