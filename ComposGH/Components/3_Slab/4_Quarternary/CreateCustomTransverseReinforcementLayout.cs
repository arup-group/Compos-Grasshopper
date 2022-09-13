using System;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreateCustomTransverseReinforcementLayout : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public CreateCustomTransverseReinforcementLayout()
      : base("Create" + CustomTransverseReinforcementLayoutGoo.Name.Replace(" ", string.Empty),
          CustomTransverseReinforcementLayoutGoo.Name.Replace(" ", string.Empty),
          "Create a " + CustomTransverseReinforcementLayoutGoo.Description + " for a " + TransverseReinforcementGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override Guid ComponentGuid => new Guid("19322156-8b1a-4849-9772-813411af965c");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Resources.CustomRebarLayout;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Start Pos x [" + unitAbbreviation + "]", "PxS", "Start Position where this Rebar Spacing Groups begins on Beam (beam local x-axis)" 
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set positions as percentage (-0.5 => 50%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("End Pos x [" + unitAbbreviation + "]", "PxE", "End Position where this Rebar Spacing Groups begins on Beam (beam local x-axis)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set positions as percentage (-0.5 => 50 %)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Transverse rebar diameter", GH_ParamAccess.item);
      pManager.AddGenericParameter("Spacing [" + unitAbbreviation + "]", "S", "The centre/centre distance between rebars in this group (along beam local x-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new CustomTransverseReinforcementParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IQuantity start = Input.LengthOrRatio(this, DA, 0, LengthUnit);
      IQuantity end = Input.LengthOrRatio(this, DA, 1, LengthUnit);
      Length dia = (Length)Input.UnitNumber(this, DA, 2, LengthUnit);
      Length spacing = (Length)Input.UnitNumber(this, DA, 3, LengthUnit);
      Length cov = (Length)Input.UnitNumber(this, DA, 4, LengthUnit);

      Output.SetItem(this, DA, 0, new CustomTransverseReinforcementLayoutGoo(new CustomTransverseReinforcementLayout(start, end, dia, spacing, cov)));
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
      int i = 0;
      Params.Input[i++].Name = "Start Pos x [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "End Pos x [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Diameter [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Spacing [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Cover [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
