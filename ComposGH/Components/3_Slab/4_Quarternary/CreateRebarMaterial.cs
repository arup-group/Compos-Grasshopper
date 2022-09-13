using System;
using System.Linq;
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
  public class CreateRebarMaterial : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("E91D37A1-81D4-427D-9910-E8A514466F3C");
    public CreateRebarMaterial()
      : base("Create" + ReinforcementMaterialGoo.Name.Replace(" ", string.Empty),
          ReinforcementMaterialGoo.Name.Replace(" ", string.Empty),
          "Create a Standard " + ReinforcementMaterialGoo.Description + " for a " + TransverseReinforcementGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Resources.RebarMaterial;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);
      pManager.AddGenericParameter("Strength [" + stressUnitAbbreviation + "]", "fu", "(Optional) Custom Characteristic Steel Strength", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ReinforcementMaterialParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (this.Params.Input[0].Sources.Count > 0)
      {
        SelectedItems[0] = "Custom";
        Output.SetItem(this, DA, 0, new ReinforcementMaterialGoo(new ReinforcementMaterial((Pressure)Input.UnitNumber(this, DA, 0, StressUnit))));
      }
      else
        Output.SetItem(this, DA, 0, new ReinforcementMaterialGoo(new ReinforcementMaterial(Grade)));
    }

    #region Custom UI
    private PressureUnit StressUnit = Units.StressUnit;
    private RebarGrade Grade = RebarGrade.EN_500B;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Grade", "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // grade
      this.DropDownItems.Add(Enum.GetValues(typeof(RebarGrade)).Cast<RebarGrade>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(Grade.ToString());

      // strength
      this.DropDownItems.Add(Units.FilteredStressUnits);
      this.SelectedItems.Add(StressUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to grade
        this.Grade = (RebarGrade)Enum.Parse(typeof(RebarGrade), this.SelectedItems[i]);
      if (i == 1) // change is made to unit
        this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      if (SelectedItems[0] != "Custom")
        this.Grade = (RebarGrade)Enum.Parse(typeof(RebarGrade), this.SelectedItems[0]);

      this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);
      Params.Input[0].Name = "Strength [" + stressUnitAbbreviation + "]";
    }
    #endregion
  }
}
