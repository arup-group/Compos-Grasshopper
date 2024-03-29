﻿using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components {
  public class CreateCustomSteelMaterial : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2C3C07F4-C395-4747-A111-D5A67B250104");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateCustomSteelMaterial;
    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;

    private WeldMaterialGrade Grade = WeldMaterialGrade.Grade_35;

    private List<bool> Override_dropDownItems;

    private PressureUnit StressUnit = DefaultUnits.MaterialStrengthUnit;

    public CreateCustomSteelMaterial() : base("Custom" + SteelMaterialGoo.Name.Replace(" ", string.Empty),
      SteelMaterialGoo.Name.Replace(" ", string.Empty),
      "Create a Custom " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)  // change is made to code
      {
        if (Grade.ToString() == _selectedItems[i]) {
          return; // return if selected value is same as before
        }

        Grade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), _selectedItems[i]);
      }
      if (i == 1) {
        StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
      }
      if (i == 2) {
        DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string stressunitAbbreviation = Pressure.GetAbbreviation(StressUnit);
      string densityunitAbbreviation = Density.GetAbbreviation(DensityUnit);

      Params.Input[0].Name = "Strength [" + stressunitAbbreviation + "]";
      Params.Input[1].Name = "Young's Modulus [" + stressunitAbbreviation + "]";
      Params.Input[2].Name = "Density [" + densityunitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[]
        {
          "Weld Material Grade",
          "StressUnit",
          "DensityUnit"
        });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // WeldMaterial
      _dropDownItems.Add(Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList());
      _dropDownItems[0].RemoveAt(0);
      _selectedItems.Add(Grade.ToString());

      // Stress
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(Pressure.GetAbbreviation(StressUnit));

      // Density
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      _selectedItems.Add(Density.GetAbbreviation(DensityUnit));

      Override_dropDownItems = new List<bool>() { false, false, false };
      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string stressunitAbbreviation = Pressure.GetAbbreviation(StressUnit);
      string densityunitAbbreviation = Density.GetAbbreviation(DensityUnit);

      pManager.AddGenericParameter("Strength [" + stressunitAbbreviation + "]", "fy", "Steel Yield Strength", GH_ParamAccess.item);
      pManager.AddGenericParameter("Young's Modulus [" + stressunitAbbreviation + "]", "E", "Steel Young's Modulus", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + densityunitAbbreviation + "]", "ρ", "Steel Density", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Reduction Factor", "RF", "Apply reduction factor for plastic moment capacity, EC4 (6.2.1.2 (2))", GH_ParamAccess.item, false);
      pManager.AddGenericParameter("Grade", "G", "(Optional) Weld Grade", GH_ParamAccess.item);

      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new SteelMaterialParam(), "Custom " + SteelMaterialGoo.Name, SteelMaterialGoo.NickName, "Custom " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      // override steel grade?
      if (Params.Input[4].Sources.Count > 0) {
        string grade = "";
        DA.GetData(4, ref grade);
        try {
          Grade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), grade);
          _dropDownItems[0] = new List<string>();
          _selectedItems[0] = "-";
          Override_dropDownItems[0] = true;
        } catch (ArgumentException) {
          string text = "Could not parse steel grade. Valid steel grades are ";
          foreach (string g in Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList()) {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          _dropDownItems[0] = Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
        }
      } else if (Override_dropDownItems[0]) {
        _dropDownItems[0] = Enum.GetValues(typeof(WeldMaterialGrade)).Cast<WeldMaterialGrade>().Select(x => x.ToString()).ToList();
        Override_dropDownItems[0] = false;
      }

      bool redFact = new bool();

      if (DA.GetData(3, ref redFact)) {
        if (redFact) {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Note that reduction factor only applies for EC4 DesignCode");
        }
      }

      DA.SetData(0, new SteelMaterialGoo(new SteelMaterial(
        (Pressure)Input.UnitNumber(this, DA, 0, StressUnit),
        (Pressure)Input.UnitNumber(this, DA, 1, StressUnit),
        (Density)Input.UnitNumber(this, DA, 2, DensityUnit),
        Grade, true, redFact)));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] != "-") {
        Grade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), _selectedItems[0]);
      }
      StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
