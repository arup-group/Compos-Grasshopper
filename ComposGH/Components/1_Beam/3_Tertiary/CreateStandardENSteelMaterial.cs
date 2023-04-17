﻿using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposGH.Components {
  public class CreateStandardENSteelMaterial : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("e671a346-5989-47e0-aacc-920c77fdfb1f");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardENSteelMaterial;
    private List<bool> Override_dropDownItems;

    private StandardSteelGrade SteelGrade = StandardSteelGrade.S235;

    public CreateStandardENSteelMaterial()
                  : base("StandardEN" + SteelMaterialGoo.Name.Replace(" ", string.Empty),
      "EN" + SteelMaterialGoo.NickName.Replace(" ", string.Empty),
      "Look up a Standard EN " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat1()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)  // change is made to code
      {
        if (SteelGrade.ToString() == _selectedItems[i])
          return; // return if selected value is same as before

        SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), _selectedItems[i]);
      }

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Grade" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // SteelType
      _dropDownItems.Add(Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList());
      _dropDownItems[0].RemoveAt(3); // remove S450
      _selectedItems.Add(SteelGrade.ToString());

      Override_dropDownItems = new List<bool>() { false };

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Grade", "G", "(Optional) Grade", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new SteelMaterialParam(), "Standard " + SteelMaterialGoo.Name, SteelMaterialGoo.NickName, "Standard EN1993-1-1 " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      // override steel grade?
      if (Params.Input[0].Sources.Count > 0) {
        string grade = "";
        DA.GetData(0, ref grade);
        try {
          if (Char.IsDigit(grade[0]))
            grade = "S" + grade;
          SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), grade);
          _dropDownItems[0] = new List<string>();
          _selectedItems[0] = "-";
          Override_dropDownItems[0] = true;
        }
        catch (ArgumentException) {
          string text = "Could not parse steel grade. Valid EC4 steel grades are ";
          List<string> gradesEN = Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList();
          gradesEN.RemoveAt(3);
          foreach (string g in gradesEN) {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          _dropDownItems[0] = gradesEN;
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      }
      else if (Override_dropDownItems[0]) {
        List<string> gradesEN = Enum.GetValues(typeof(StandardSteelGrade)).Cast<StandardSteelGrade>().Select(x => x.ToString()).ToList();
        gradesEN.RemoveAt(3);
        _dropDownItems[0] = gradesEN;
        Override_dropDownItems[0] = false;
      }

      Output.SetItem(this, DA, 0, new SteelMaterialGoo(new SteelMaterial(SteelGrade, Code.EN1994_1_1_2004)));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] != "-")
        SteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), _selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
