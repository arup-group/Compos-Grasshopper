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
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components {
  public class CreateRebarMaterial : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("E91D37A1-81D4-427D-9910-E8A514466F3C");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.RebarMaterial;
    private RebarGrade Grade = RebarGrade.EN_500B;

    private PressureUnit StressUnit = DefaultUnits.MaterialStrengthUnit;

    public CreateRebarMaterial() : base("Create" + ReinforcementMaterialGoo.Name.Replace(" ", string.Empty),
      ReinforcementMaterialGoo.Name.Replace(" ", string.Empty),
      "Create a Standard " + ReinforcementMaterialGoo.Description + " for a " + TransverseReinforcementGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        // change is made to grade
        Grade = (RebarGrade)Enum.Parse(typeof(RebarGrade), _selectedItems[i]);
      }
      if (i == 1) {
        // change is made to unit
        StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
      }
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(StressUnit);
      Params.Input[0].Name = "Strength [" + stressUnitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Grade", "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // grade
      _dropDownItems.Add(Enum.GetValues(typeof(RebarGrade)).Cast<RebarGrade>().Select(x => x.ToString()).ToList());
      _selectedItems.Add(Grade.ToString());

      // strength
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(Pressure.GetAbbreviation(StressUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(StressUnit);
      pManager.AddGenericParameter("Strength [" + stressUnitAbbreviation + "]", "fu", "(Optional) Custom Characteristic Steel Strength", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ReinforcementMaterialParam());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      if (Params.Input[0].Sources.Count > 0) {
        _selectedItems[0] = "Custom";
        Output.SetItem(this, DA, 0, new ReinforcementMaterialGoo(new ReinforcementMaterial((Pressure)Input.UnitNumber(this, DA, 0, StressUnit))));
      } else {
        Output.SetItem(this, DA, 0, new ReinforcementMaterialGoo(new ReinforcementMaterial(Grade)));
      }
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] != "Custom") {
        Grade = (RebarGrade)Enum.Parse(typeof(RebarGrade), _selectedItems[0]);
      }

      StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
