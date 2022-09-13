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
  public class StudResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("75fbd8a9-eb55-443e-88c8-353307c96097");
    public StudResults()
      : base("Stud Results",
          "StudResults",
          "Get stud results for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Resources.StudResults;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description, GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Capacity", "Rd", "Actual stud capacity, as [number of studs] x [single stud capacity]. Values given at each position", GH_ParamAccess.list);
      pManager.AddIntegerParameter("StartCount", "Sp", "Actual number of studs provided from start. Values given at each position", GH_ParamAccess.list);
      pManager.AddIntegerParameter("EndCount", "Ep", "Actual number of studs provided from end. Values given at each position", GH_ParamAccess.list);
      pManager.AddIntegerParameter("StartRequired", "Sr", "Required number of studs provided from start. Values given at each position", GH_ParamAccess.list);
      pManager.AddIntegerParameter("EndRequired", "Er", "Required number of studs provided from end. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Req.Full IA", "RFI", "Required stud capacity for 100% shear interaction. Values given at each position", GH_ParamAccess.list);
      pManager.AddNumberParameter("Req. Interaction", "RIA", "Required shear interaction for given moment. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Single Capacity", "1Rd", "Capacity of a single stud", GH_ParamAccess.item);
      pManager.AddNumberParameter("Interaction", "IA", "Actual shear interaction for given moment. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Stud Capacity Start", "SRd", "Actual shear capacity from start. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Stud Capacity Start", "ERd", "Actual shear capacity from end. Values given at each position", GH_ParamAccess.list);
      pManager.AddGenericParameter("Positions", "Pos", "Positions for each critical section location. Values are measured from beam start.", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IResult res = ((MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0)).Value.Result;
      List<GH_UnitNumber> positions = res.Positions.Select(x => new GH_UnitNumber(x.ToUnit(this.LengthUnit))).ToList();
      IStudResult result = res.StudResults;

      int i = 0;
      Output.SetList(this, DA, i++, 
        result.StudCapacity.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());
      
      Output.SetList(this, DA, i++,
        result.NumberOfStudsStart);
      
      Output.SetList(this, DA, i++,
        result.NumberOfStudsEnd);

      Output.SetList(this, DA, i++,
        result.NumberOfStudsRequiredStart);

      Output.SetList(this, DA, i++,
        result.NumberOfStudsRequiredEnd);

      Output.SetList(this, DA, i++,
        result.StudCapacityRequiredForFullShearInteraction.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());

      Output.SetList(this, DA, i++,
        result.ShearInteractionRequired.Select(x => new GH_UnitNumber(x.ToUnit(RatioUnit.DecimalFraction))).ToList());

      Output.SetItem(this, DA, i++, new GH_UnitNumber(
        result.SingleStudCapacity.ToUnit(this.ForceUnit)));

      Output.SetList(this, DA, i++,
        result.ShearInteraction.Select(x => new GH_UnitNumber(x.ToUnit(RatioUnit.DecimalFraction))).ToList());

      Output.SetList(this, DA, i++,
        result.StudCapacityStart.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());

      Output.SetList(this, DA, i++,
        result.StudCapacityEnd.Select(x => new GH_UnitNumber(x.ToUnit(this.ForceUnit))).ToList());

      Output.SetList(this, DA, i, positions);
    }

    #region Custom UI
    private ForceUnit ForceUnit = Units.ForceUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void InitialiseDropdowns()
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
