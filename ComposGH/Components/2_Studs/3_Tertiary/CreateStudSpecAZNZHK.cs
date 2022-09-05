using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using OasysGH.Components;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreateStudSpecAZNZHK : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("1ef0e7f8-bd0a-4a10-b6ed-009745062628");
    public CreateStudSpecAZNZHK()
      : base("Create" + StudSpecificationGoo.Name.Replace(" ", string.Empty),
          StudSpecificationGoo.Name.Replace(" ", string.Empty),
          "Create a " + StudSpecificationGoo.Description + " applicable for AS/NZ or HK codes, for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardStudSpecs;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("No Stud Zone Start [" + unitAbbreviation + "]",
          "NSZS", "Length of zone without shear studs at the start of the beam (default = 0)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter("No Stud Zone End [" + unitAbbreviation + "]",
          "NSZE", "Length of zone without shear studs at the end of the beam (default = 0)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Welded", "Wld", "Welded through profiled steel sheeting", GH_ParamAccess.item, true);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(StudSpecificationGoo.Name, StudSpecificationGoo.NickName, StudSpecificationGoo.Description + " applicable for AS/NZ or HK codes, for a " + StudGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // get default length inputs used for all cases
      IQuantity noStudZoneStart = Length.Zero;
      if (this.Params.Input[0].Sources.Count > 0)
        noStudZoneStart = GetInput.LengthOrRatio(this, DA, 0, LengthUnit, true);
      IQuantity noStudZoneEnd = Length.Zero;
      if (this.Params.Input[1].Sources.Count > 0)
        noStudZoneEnd = GetInput.LengthOrRatio(this, DA, 1, LengthUnit, true);

      bool welded = true;
      DA.GetData(2, ref welded);

      StudSpecification specOther = new StudSpecification(
          noStudZoneStart, noStudZoneEnd, welded);
      DA.SetData(0, new StudSpecificationGoo(specOther));
    }

    #region Custom UI
    private LengthUnit LengthUnit = Units.LengthUnitSection;

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
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (this.LengthUnit.ToString() == this.SelectedItems[i])
        return;

      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[0].Name = "No Stud Zone Start [" + unitAbbreviation + "]";
      Params.Input[1].Name = "No Stud Zone End [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
