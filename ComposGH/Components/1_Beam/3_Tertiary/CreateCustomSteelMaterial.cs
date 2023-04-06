using System;
using System.Linq;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits;
using OasysUnits.Units;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateCustomSteelMaterial : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2C3C07F4-C395-4747-A111-D5A67B250104");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateCustomSteelMaterial;
    public CreateCustomSteelMaterial()
      : base("Custom" + SteelMaterialGoo.Name.Replace(" ", string.Empty),
          SteelMaterialGoo.Name.Replace(" ", string.Empty), 
          "Create a Custom " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string stressunitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);
      string densityunitAbbreviation = Density.GetAbbreviation(this.DensityUnit);

      pManager.AddGenericParameter("Strength [" + stressunitAbbreviation + "]", "fy", "Steel Yield Strength", GH_ParamAccess.item);
      pManager.AddGenericParameter("Young's Modulus [" + stressunitAbbreviation + "]", "E", "Steel Young's Modulus", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + densityunitAbbreviation + "]", "ρ", "Steel Density", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Reduction Factor", "RF", "Apply reduction factor for plastic moment capacity, EC4 (6.2.1.2 (2))", GH_ParamAccess.item, false);
      pManager.AddGenericParameter("Grade", "G", "(Optional) Weld Grade", GH_ParamAccess.item);

      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new SteelMaterialParam(), "Custom " + SteelMaterialGoo.Name, SteelMaterialGoo.NickName, "Custom " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
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
          this._dropDownItems[0] = new List<string>();
          this._selectedItems[0] = "-";
          this.Override_dropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse steel grade. Valid steel grades are ";
          foreach (string g in Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList())
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this._dropDownItems[0] = Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
        }
      }
      else if (this.Override_dropDownItems[0])
      {
        this._dropDownItems[0] = Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList();
        this.Override_dropDownItems[0] = false;
      }

      bool redFact = new bool();

      if (DA.GetData(3, ref redFact))
        if (redFact)
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Note that reduction factor only applies for EC4 DesignCode");

      Output.SetItem(this, DA, 0, new SteelMaterialGoo(new SteelMaterial(
        (Pressure)Input.UnitNumber(this, DA, 0, this.StressUnit),
        (Pressure)Input.UnitNumber(this, DA, 1, this.StressUnit),
        (Density)Input.UnitNumber(this, DA, 2, this.DensityUnit), 
        this.Grade, true, redFact)));
    }

    #region Custom UI
    List<bool> Override_dropDownItems;
    private PressureUnit StressUnit = DefaultUnits.MaterialStrengthUnit;
    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;
    private WeldMaterialGrade Grade = WeldMaterialGrade.Grade_35;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[]
        {
          "Weld Material Grade",
          "StressUnit",
          "DensityUnit"
        });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // WeldMaterial
      this._dropDownItems.Add(Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList());
      this._dropDownItems[0].RemoveAt(0);
      this._selectedItems.Add(Grade.ToString());

      // Stress
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      this._selectedItems.Add(Pressure.GetAbbreviation(this.StressUnit));

      // Density
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      this._selectedItems.Add(Density.GetAbbreviation(this.DensityUnit));

      this.Override_dropDownItems = new List<bool>() { false, false, false };
      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (this.Grade.ToString() == _selectedItems[i])
          return; // return if selected value is same as before

        this.Grade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), _selectedItems[i]);
      }
      if (i == 1)
        this.StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
      if (i == 2)
        this.DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      if (this._selectedItems[0] != "-")
        this.Grade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), _selectedItems[0]);
      this.StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      this.DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      string stressunitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);
      string densityunitAbbreviation = Density.GetAbbreviation(this.DensityUnit);

      Params.Input[0].Name = "Strength [" + stressunitAbbreviation + "]";
      Params.Input[1].Name = "Young's Modulus [" + stressunitAbbreviation + "]";
      Params.Input[2].Name = "Density [" + densityunitAbbreviation + "]";
    }
    #endregion
  }
}
