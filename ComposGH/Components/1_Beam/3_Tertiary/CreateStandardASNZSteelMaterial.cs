using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components {
  public class CreateStandardASNZSteelMaterial : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8656c967-817c-49fe-9297-d863664b714a");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardASNZSteelMaterial;
    private List<bool> Override_dropDownItems;

    private StandardASNZSteelMaterialGrade SteelGrade = StandardASNZSteelMaterialGrade.C450_AS1163;

    public CreateStandardASNZSteelMaterial() : base("StandardASNZ" + SteelMaterialGoo.Name.Replace(" ", string.Empty),
      "ASNZ" + SteelMaterialGoo.NickName.Replace(" ", string.Empty),
      "Look up a Standard ASNZ " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)  // change is made to code
      {
        if (SteelGrade.ToString().Replace("_", " ") == _selectedItems[i]) {
          return; // return if selected value is same as before
        }
        var grade = (StandardASNZSteelMaterialGrade)Enum.Parse(typeof(StandardASNZSteelMaterialGrade), _selectedItems[i].Replace(" ", "_"));
        SteelGrade = grade;
      }

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Grade" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // SteelType
      var grades = Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().ToList();

      _dropDownItems.Add(grades.Select(x => x.ToString().Replace("_", " ")).ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      Override_dropDownItems = new List<bool>() { false };

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Grade", "G", "(Optional) Grade", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new SteelMaterialParam(), "Standard " + SteelMaterialGoo.Name, SteelMaterialGoo.NickName, "Standard ASNZ " + SteelMaterialGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      // override steel grade?
      if (Params.Input[0].Sources.Count > 0) {
        string grade = "";
        DA.GetData(0, ref grade);
        try {
          SteelGrade = (StandardASNZSteelMaterialGrade)Enum.Parse(typeof(StandardASNZSteelMaterialGrade), grade.Replace(" ", "_"));
          _dropDownItems[0] = new List<string>();
          _selectedItems[0] = "-";
          Override_dropDownItems[0] = true;
        } catch (ArgumentException) {
          string text = "Could not parse steel grade. Valid AS/NZS steel grades are ";
          foreach (string g in Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().Select(x => x.ToString()).ToList()) {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          _dropDownItems[0] = Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      } else if (Override_dropDownItems[0]) {
        _dropDownItems[0] = Enum.GetValues(typeof(StandardASNZSteelMaterialGrade)).Cast<StandardASNZSteelMaterialGrade>().Select(x => x.ToString()).ToList();
        Override_dropDownItems[0] = false;
      }

      DA.SetData(0, new SteelMaterialGoo(new ASNZSteelMaterial(SteelGrade)));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] != "-") {
        var grade = (StandardASNZSteelMaterialGrade)Enum.Parse(typeof(StandardASNZSteelMaterialGrade), _selectedItems[0].Replace(" ", "_"));
        SteelGrade = grade;
      }

      base.UpdateUIFromSelectedItems();
    }
  }
}
