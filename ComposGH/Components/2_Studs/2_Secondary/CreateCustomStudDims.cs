using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateCustomStudDimensions : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("e70db6bb-b4bf-4033-a3d0-3ad131fe09b1");
    public CreateCustomStudDimensions()
      : base("Custom" + StudDimensionsGoo.Name.Replace(" ", string.Empty),
          "StudDimsCustom",
          "Create a Custom " + StudDimensionsGoo.Description + " for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CustomStudDims;
    #endregion
    
    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string forceunitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Diameter of stud head", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Height of stud", GH_ParamAccess.item);
      pManager.AddGenericParameter("Strength [" + forceunitAbbreviation + "]", "fu", "Stud Character strength", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(StudDimensionsGoo.Name, StudDimensionsGoo.NickName, StudDimensionsGoo.Description + " for a " + StudGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length dia = GetInput.Length(this, DA, 0, LengthUnit, true);
      Length h = GetInput.Length(this, DA, 1, LengthUnit, true);
      Force strengthF = GetInput.Force(this, DA, 2, ForceUnit);
      DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthF)));
    }

    #region Custom UI
    private LengthUnit LengthUnit = Units.LengthUnitSection;
    private ForceUnit ForceUnit = Units.ForceUnit;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Length Unit", "Strength Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      // strength
      this.DropDownItems.Add(Units.FilteredForceUnits);
      this.SelectedItems.Add(this.ForceUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to length unit
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      if (i == 1)
        this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[0].Name = "Diameter [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Height [" + unitAbbreviation + "]";

      string forceunitAbbreviation = Force.GetAbbreviation(ForceUnit);
      Params.Input[2].Name = "Strength [" + forceunitAbbreviation + "]";
    }
    #endregion
  }
}
