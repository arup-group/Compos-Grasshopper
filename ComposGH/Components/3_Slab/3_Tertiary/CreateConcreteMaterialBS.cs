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
  public class CreateConcreteMaterialBS : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2ca1b0d6-44a2-441f-bf4b-8367d98d90a8");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateConcreteMaterialBS;
    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;

    private ConcreteGrade Grade = ConcreteGrade.C25;

    private List<bool> Override_dropDownItems;

    private ConcreteMaterial.WeightType Type = ConcreteMaterial.WeightType.Normal;

    public CreateConcreteMaterialBS() : base("BS" + ConcreteMaterialGoo.Name.Replace(" ", string.Empty),
      "BS" + ConcreteMaterialGoo.NickName.Replace(" ", string.Empty),
      "Look up a Standard BS " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {// change is made to grade
        Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), _selectedItems[i]);
      } else if (i == 1) {
        // change is made to weight type
        Type = (ConcreteMaterial.WeightType)Enum.Parse(typeof(ConcreteMaterial.WeightType), _selectedItems[i]);
      } else if (i == 2) {
        // change is made to density unit
        DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[i]);
      }
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string densityUnitAbbreviation = Density.GetAbbreviation(DensityUnit);
      Params.Input[0].Name = "Density [" + densityUnitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Grade",
        "Weight Type",
        "Density Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // grade
      var concreteGrades = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
      concreteGrades.RemoveAt(0); // C20
      concreteGrades.RemoveAt(2); // C32
      _dropDownItems.Add(concreteGrades);
      _selectedItems.Add(Grade.ToString());

      // weight type
      _dropDownItems.Add(Enum.GetValues(typeof(ConcreteMaterial.WeightType)).Cast<ConcreteMaterial.WeightType>().Select(x => x.ToString()).ToList());
      _selectedItems.Add(Type.ToString());

      // density unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      _selectedItems.Add(Density.GetAbbreviation(DensityUnit));

      Override_dropDownItems = new List<bool>() { false, false, false };

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string densityUnitAbbreviation = Density.GetAbbreviation(DensityUnit);

      // optional
      pManager.AddNumberParameter("Dry Density [" + densityUnitAbbreviation + "]", "DD", "(Optional) Dry density", GH_ParamAccess.item);
      pManager.AddGenericParameter(ERatioGoo.Name, ERatioGoo.NickName, "(Optional)" + ERatioGoo.Description, GH_ParamAccess.item);
      pManager.AddNumberParameter("Imposed Load Percentage [-]", "ILP", "(Optional) Percentage of imposed load acting long term as decimal fraction", GH_ParamAccess.item, 0.33);
      pManager.AddGenericParameter("Concrete Grade", "CG", "(Optional) Concrete grade", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ConcreteMaterialParam(), ConcreteMaterialGoo.Name, ConcreteMaterialGoo.NickName, "BS " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      // override concrete grade?
      if (Params.Input[3].Sources.Count > 0) {
        string grade = "";
        DA.GetData(3, ref grade);
        try {
          Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), grade);
          _dropDownItems[0] = new List<string>();
          _selectedItems[0] = "-";
          Override_dropDownItems[0] = true;
        } catch (ArgumentException) {
          string text = "Could not parse concrete grade. Valid BS concrete grades are ";
          foreach (string g in Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList()) {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          _dropDownItems[0] = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      } else if (Override_dropDownItems[0]) {
        _dropDownItems[0] = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
        Override_dropDownItems[0] = false;
      }

      var dryDensity = new Density(2400, DensityUnit.KilogramPerCubicMeter);
      bool userDensity = false;
      if (Params.Input[0].Sources.Count > 0) {
        dryDensity = (Density)Input.UnitNumber(this, DA, 0, DensityUnit);
        userDensity = true;
      } else {
        if (Type == ConcreteMaterial.WeightType.LightWeight) {
          dryDensity = new Density(1800, DensityUnit.KilogramPerCubicMeter);
        }
      }

      var imposedLoadPercentage = (Ratio)Input.UnitNumber(this, DA, 2, RatioUnit.DecimalFraction);

      var eRatio = (ERatioGoo)Input.GenericGoo<ERatioGoo>(this, DA, 1);

      IConcreteMaterial concreteMaterial = new ConcreteMaterial(Grade, Type, dryDensity, userDensity, (eRatio == null) ? new ERatio() : eRatio.Value, imposedLoadPercentage);

      Output.SetItem(this, DA, 0, new ConcreteMaterialGoo(concreteMaterial));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] != "-") {
        Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), _selectedItems[0]);
      }
      Type = (ConcreteMaterial.WeightType)Enum.Parse(typeof(ConcreteMaterial.WeightType), _selectedItems[1]);
      DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
