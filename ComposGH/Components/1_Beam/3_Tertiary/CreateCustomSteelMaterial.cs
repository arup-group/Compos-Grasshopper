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
  public class CreateCustomSteelMaterial : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2C3C07F4-C395-4747-A111-D5A67B250104");
    public CreateCustomSteelMaterial()
      : base("Custom Steel Material", "CustomSteelMat", "Create Custom Steel Material for a Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateCustomSteelMaterial;
    #endregion

    #region Custom UI

    // list of lists with all dropdown lists content
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
            "Weld Material Grade",
            "StressUnit",
            "DensityUnit"
    });
    List<bool> OverrideDropDownItems;

    private bool First = true;
    private PressureUnit StressUnit = Units.StressUnit;
    private DensityUnit DensityUnit = Units.DensityUnit;
    private WeldMaterialGrade Grade = WeldMaterialGrade.Grade_35;

    public override void CreateAttributes()
    {
      if (First)
      {
        this.DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // WeldMaterial
        this.DropDownItems.Add(Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList());
        SelectedItems.Add(Grade.ToString());

        // Stress
        this.DropDownItems.Add(Units.FilteredStressUnits);
        SelectedItems.Add(StressUnit.ToString());

        // Density
        this.DropDownItems.Add(Units.FilteredDensityUnits);
        SelectedItems.Add(DensityUnit.ToString());

        this.OverrideDropDownItems = new List<bool>() { false, false, false };
        First = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, this.DropDownItems, SelectedItems, SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (Grade.ToString() == SelectedItems[i])
          return; // return if selected value is same as before

        Grade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), SelectedItems[i]);

      }
      if (i == 1)
      {
        StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[i]);
      }
      if (i == 2)
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
      if (this.SelectedItems[0] != "-")
        Grade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), SelectedItems[0]);
      StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[1]);
      DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), SelectedItems[2]);

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
      pManager.AddBooleanParameter("Reduction Factor", "RF", "Apply reduction factor for plastic moment capacity, EC4 (6.2.1.2 (2))", GH_ParamAccess.item, false);
      pManager.AddGenericParameter("Grade", "G", "(Optional) Grade", GH_ParamAccess.item);

      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("StandardSteelMaterial", "SSM", "Standard Steel Material for a Compos Beam", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // override steel grade?
      if (this.Params.Input[4].Sources.Count > 0)
      {
        string grade = "";
        DA.GetData(4, ref grade);
        try
        {
          this.Grade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), grade);
          this.DropDownItems[0] = new List<string>();
          this.SelectedItems[0] = "-";
          this.OverrideDropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse steel grade. Valid steel grades are ";
          foreach (string g in Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList())
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this.DropDownItems[0] = Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
        }
      }
      else if (this.OverrideDropDownItems[0])
      {
        this.DropDownItems[0] = Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList();
        this.OverrideDropDownItems[0] = false;
      }

      bool redFact = new bool();

      if (DA.GetData(3, ref redFact))
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Note that reduction factor only applies for EC4 DesignCode");

      DA.SetData(0, new SteelMaterialGoo(new SteelMaterial(GetInput.Stress(this, DA, 0, StressUnit), GetInput.Stress(this, DA, 1, StressUnit), GetInput.Density(this, DA, 2, DensityUnit), Grade, true, redFact)));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, this.DropDownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref this.DropDownItems, ref SelectedItems, ref SpacerDescriptions);
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref this.DropDownItems, ref SelectedItems, ref SpacerDescriptions);

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
