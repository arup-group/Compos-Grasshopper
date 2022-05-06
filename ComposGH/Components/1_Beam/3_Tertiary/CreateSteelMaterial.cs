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
  public class CreateSteelMaterial : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2C3C07F4-C395-4747-A111-D5A67B250104");
    public CreateSteelMaterial()
      : base("Steel Material", "SteelMat", "Create Standard Steel Material for a Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateSteelMaterial;
    #endregion

    #region Custom UI

    // list of lists with all dropdown lists content
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
            "Steel Type",
            "Weld Material",
            "StressUnit",
            "DensityUnit"
    });

    private bool First = true;
    private PressureUnit StressUnit = Units.StressUnit;
    private DensityUnit DensityUnit = Units.DensityUnit;
    private SteelMaterialGrade SteelGrade = SteelMaterialGrade.S235;
    private WeldMaterialGrade WeldingGrade = WeldMaterialGrade.Grade_35;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // SteelType
        DropdownItems.Add(Enum.GetValues(typeof(SteelMaterialGrade)).Cast<SteelMaterialGrade>().Select(x => x.ToString()).ToList());
        SelectedItems.Add(SteelGrade.ToString());

        // WeldMaterial
        DropdownItems.Add(Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList());
        SelectedItems.Add(WeldingGrade.ToString());

        // Stress
        DropdownItems.Add(Units.FilteredStressUnits);
        SelectedItems.Add(StressUnit.ToString());

        // Density
        DropdownItems.Add(Units.FilteredDensityUnits);
        SelectedItems.Add(DensityUnit.ToString());

        First = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (SteelGrade.ToString() == SelectedItems[i])
          return; // return if selected value is same as before

        SteelGrade = (SteelMaterialGrade)Enum.Parse(typeof(SteelMaterialGrade), SelectedItems[i]);

      }
      if (i == 1)  // change is made to code 
      {
        if (WeldingGrade.ToString() == SelectedItems[i])
          return; // return if selected value is same as before

        WeldingGrade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), SelectedItems[i]);

      }
      if (i == 2)
      {
        StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[i]);
      }
      if (i == 3)
      {
        DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), SelectedItems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      SteelGrade = (SteelMaterialGrade)Enum.Parse(typeof(SteelMaterialGrade), SelectedItems[0]);
      WeldingGrade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), SelectedItems[1]);
      StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[2]);
      DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), SelectedItems[3]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity stress = new Pressure(0, StressUnit);
      IQuantity density = new Density(0, DensityUnit);

      string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
      string densityunitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Strength [" + stressunitAbbreviation + "]", "fy", "Steel Yield Strength", GH_ParamAccess.item);
      pManager.AddGenericParameter("Young's Modulus [" + stressunitAbbreviation + "]", "E", "Steel Young's Modulus", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + densityunitAbbreviation + "]", "ρ", "Steel Density", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Reduction Factor", "RF", "Apply reduction factor for plastic moment capacity", GH_ParamAccess.item, false);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("StandardSteelMaterial", "SSM", "Standard Steel Material for a Compos Beam", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool redFact = new bool();

      DA.GetData(3, ref redFact);

      if (this.Params.Input[0].Sources.Count > 0)
      {
        SelectedItems[0] = "Custom";
        DA.SetData(0, new SteelMaterialGoo(new SteelMaterial(
          GetInput.Stress(this, DA, 0, StressUnit), 
          GetInput.Stress(this, DA, 1, StressUnit), 
          GetInput.Density(this, DA, 2, DensityUnit), 
          WeldingGrade, true, redFact)));
      }
      else
        DA.SetData(0, new SteelMaterialGoo(new SteelMaterial(SteelGrade)));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropdownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropdownItems, ref SelectedItems, ref SpacerDescriptions);

      UpdateUIFromSelectedItems();

      First = false;

      return base.Read(reader);
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity stress = new Pressure(0, StressUnit);
      IQuantity density = new Density(0, DensityUnit);

      string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
      string densityunitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));

      Params.Input[0].Name = "Strength [" + stressunitAbbreviation + "]";
      Params.Input[1].Name = "Young's Modulus [" + stressunitAbbreviation + "]";
      Params.Input[2].Name = "Density [" + densityunitAbbreviation + "]";
    }
    #endregion

  }
}
