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
  public class CreateConcreteMaterialHKSUOS : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6781ee34-494e-414c-9542-6be29f1a5696");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateConcreteMaterialHK;
    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;

    private ConcreteGrade Grade = ConcreteGrade.C25;

    private List<bool> Override_dropDownItems;

    public CreateConcreteMaterialHKSUOS() : base("HK" + ConcreteMaterialGoo.Name.Replace(" ", string.Empty),
      "HK" + ConcreteMaterialGoo.NickName.Replace(" ", string.Empty),
      "Look up a Standard HK " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) {
        // change is made to grade
        Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), _selectedItems[i]);
      } else if (i == 1) {
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
      _spacerDescriptions = new List<string>(new string[] { "Grade", "Density Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // grade
      var concreteGrades = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
      concreteGrades.RemoveAt(0); // C20
      concreteGrades.RemoveAt(2); // C32
      concreteGrades.RemoveRange(8, 4); // C70...C100
      _dropDownItems.Add(concreteGrades);
      _selectedItems.Add(Grade.ToString());

      // density unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      _selectedItems.Add(Density.GetAbbreviation(DensityUnit));

      Override_dropDownItems = new List<bool>() { false, false };

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
      pManager.AddParameter(new ConcreteMaterialParam(), ConcreteMaterialGoo.Name, ConcreteMaterialGoo.NickName, "HK " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
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
          string text = "Could not parse concrete grade. Valid HKSUOS concrete grades are ";
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

      var dryDensity = new Density(2450, DensityUnit.KilogramPerCubicMeter);
      bool userDensity = false;
      if (Params.Input[0].Sources.Count > 0) {
        dryDensity = (Density)Input.UnitNumber(this, DA, 0, DensityUnit);
        userDensity = true;
      }

      var imposedLoadPercentage = (Ratio)Input.UnitNumber(this, DA, 2, RatioUnit.DecimalFraction);

      var eRatio = (ERatioGoo)Input.GenericGoo<ERatioGoo>(this, DA, 1);

      var concreteMaterial = new ConcreteMaterial(Grade, dryDensity, userDensity, (eRatio == null) ? new ERatio() : eRatio.Value, imposedLoadPercentage);

      Output.SetItem(this, DA, 0, new ConcreteMaterialGoo(concreteMaterial));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] != "-") {
        Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), _selectedItems[0]);
      }
      DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
