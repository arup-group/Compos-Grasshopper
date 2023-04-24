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
  public class CreateConcreteMaterialEN : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("fd361dc8-98bc-4cad-ba15-4da5da3c52bb");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateConcreteMaterialEN;
    private ConcreteMaterial.DensityClass DensityClass = ConcreteMaterial.DensityClass.DC801_1000;

    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;

    private ConcreteGradeEN Grade = ConcreteGradeEN.C20_25;

    private bool isLightWeight = false;

    private List<bool> Override_dropDownItems;

    private StrainUnit StrainUnit = StrainUnit.MilliStrain;

    public CreateConcreteMaterialEN() : base("EN" + ConcreteMaterialGoo.Name.Replace(" ", string.Empty),
      "EN" + ConcreteMaterialGoo.NickName.Replace(" ", string.Empty),
      "Look up a Standard EN " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      isLightWeight = reader.GetBoolean("isLightWeight");
      return base.Read(reader);
    }

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) // change is made to grade
      {
        Grade = (ConcreteGradeEN)Enum.Parse(typeof(ConcreteGradeEN), _selectedItems[i]);
        if (Grade.ToString().StartsWith("LC")) {
          isLightWeight = true;
          if (_dropDownItems.Count < 4) {
            // density class
            var densityClasses = Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList();
            densityClasses.RemoveAt(0);
            _dropDownItems.Insert(1, densityClasses);
            _selectedItems.Insert(1, DensityClass.ToString());
            _spacerDescriptions.Insert(1, "Density Class");
          }
        } else {
          isLightWeight = false;
          if (_dropDownItems.Count > 3) {
            _dropDownItems.RemoveAt(1);
            _selectedItems.RemoveAt(1);
            _spacerDescriptions.RemoveAt(1);
          }
        }
      } else if (isLightWeight & i == 1) {
        // change is made to density class
        DensityClass = (ConcreteMaterial.DensityClass)Enum.Parse(typeof(ConcreteMaterial.DensityClass), _selectedItems[i]);
      }
      if (!isLightWeight) {
        i++; //
      } else if (i == 2) {
        // change is made to density unit
        DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[i]);
      } else if (i == 3) {
        // change is made to strain unit
        StrainUnit = (StrainUnit)UnitsHelper.Parse(typeof(StrainUnit), _selectedItems[i]);
      }
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string densityUnitAbbreviation = Density.GetAbbreviation(DensityUnit);
      string strainUnitAbbreviation = Strain.GetAbbreviation(StrainUnit);
      Params.Input[0].Name = "Density [" + densityUnitAbbreviation + "]";
      Params.Input[3].Name = "Strain [" + strainUnitAbbreviation + "]";
    }

    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetBoolean("isLightWeight", isLightWeight);
      return base.Write(writer);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Grade",
        "Density Unit",
        "Strain Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // grade
      var concreteGrades = Enum.GetValues(typeof(ConcreteGradeEN)).Cast<ConcreteGradeEN>().Select(x => x.ToString()).ToList();
      _dropDownItems.Add(concreteGrades);
      _selectedItems.Add(Grade.ToString());

      // density unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      _selectedItems.Add(Density.GetAbbreviation(DensityUnit));

      // strain unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Strain));
      _selectedItems.Add(Strain.GetAbbreviation(StrainUnit));

      Override_dropDownItems = new List<bool>() { false, false, false, false };

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string densityUnitAbbreviation = Density.GetAbbreviation(DensityUnit);
      string strainUnitAbbreviation = Strain.GetAbbreviation(StrainUnit);

      // optional
      pManager.AddNumberParameter("Dry Density [" + densityUnitAbbreviation + "]", "DD", "(Optional) Dry density", GH_ParamAccess.item);
      pManager.AddGenericParameter(ERatioGoo.Name, ERatioGoo.NickName, "(Optional)" + ERatioGoo.Description, GH_ParamAccess.item);
      pManager.AddNumberParameter("Imposed Load Percentage [-]", "ILP", "(Optional) Percentage of imposed load acting long term as decimal fraction", GH_ParamAccess.item, 0.33);
      pManager.AddNumberParameter("Shrinkage Strain [" + strainUnitAbbreviation + "]", "SS", "(Optional) Shrinkage strain", GH_ParamAccess.item, -0.5);
      pManager.AddGenericParameter("Concrete Grade", "CG", "(Optional) Concrete grade", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density Class", "DC", "(Optional) Density class", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ConcreteMaterialParam(), ConcreteMaterialGoo.Name, ConcreteMaterialGoo.NickName, "EN " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      // override concrete grade?
      if (Params.Input[4].Sources.Count > 0) {
        string grade = "";
        DA.GetData(4, ref grade);
        try {
          grade = grade.Replace(" ", "_").Replace("/", "_"); // C30/37 -> C30_37
          Grade = (ConcreteGradeEN)Enum.Parse(typeof(ConcreteGradeEN), grade);
          _dropDownItems[0] = new List<string>();
          _selectedItems[0] = "-";
          Override_dropDownItems[0] = true;
        } catch (ArgumentException) {
          string text = "Could not parse concrete grade. Valid EC4 concrete grades are ";
          foreach (string g in Enum.GetValues(typeof(ConcreteGradeEN)).Cast<ConcreteGradeEN>().Select(x => x.ToString()).ToList()) {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          _dropDownItems[0] = Enum.GetValues(typeof(ConcreteGradeEN)).Cast<ConcreteGradeEN>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      } else if (Override_dropDownItems[0]) {
        _dropDownItems[0] = Enum.GetValues(typeof(ConcreteGradeEN)).Cast<ConcreteGradeEN>().Select(x => x.ToString()).ToList();
        Override_dropDownItems[0] = false;
      }
      // override density class?
      if (Params.Input[5].Sources.Count > 0) {
        string densityClass = "";
        DA.GetData(5, ref densityClass);
        try {
          DensityClass = (ConcreteMaterial.DensityClass)Enum.Parse(typeof(ConcreteMaterial.DensityClass), densityClass);
          _dropDownItems[1] = new List<string>();
          _selectedItems[1] = "-";
          Override_dropDownItems[1] = true;
        } catch (ArgumentException) {
          string text = "Could not parse density class. Valid density classes are ";
          foreach (string dc in Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList()) {
            text += dc + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          _dropDownItems[1] = Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
        }
      } else if (Override_dropDownItems[1]) {
        _dropDownItems[1] = Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList();
        Override_dropDownItems[1] = false;
      }

      var dryDensity = new Density(2400, DensityUnit.KilogramPerCubicMeter);
      bool userDensity = false;
      if (Params.Input[0].Sources.Count > 0) {
        dryDensity = (Density)Input.UnitNumber(this, DA, 0, DensityUnit);
        userDensity = true;
        if (isLightWeight) {
          _selectedItems[1] = "NOT_APPLY";
        }
      } else if (Grade.ToString().StartsWith("L")) {
        if (DensityClass == ConcreteMaterial.DensityClass.NOT_APPLY) {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please select a densitiy class.");
          return;
        }
        dryDensity = new Density((double)DensityClass, DensityUnit.KilogramPerCubicMeter);
      }

      var eRatio = (ERatioGoo)Input.GenericGoo<ERatioGoo>(this, DA, 1);

      var imposedLoadPercentage = (Ratio)Input.UnitNumber(this, DA, 2, RatioUnit.DecimalFraction);

      var shrinkageStrain = new Strain(-0.5, StrainUnit.MilliStrain);
      bool userStrain = false;
      if (Params.Input[3].Sources.Count > 0) {
        shrinkageStrain = (Strain)Input.UnitNumber(this, DA, 3, StrainUnit, true);
        userStrain = true;
      }

      ConcreteMaterial.DensityClass selectedDensityClass = DensityClass;
      if (!isLightWeight) {
        selectedDensityClass = ConcreteMaterial.DensityClass.NOT_APPLY;
      }

      var concreteMaterial = new ConcreteMaterial(Grade, selectedDensityClass, dryDensity, userDensity, (eRatio == null) ? new ERatio() { ShortTerm = 6, LongTerm = 18, Vibration = 5.39 } : eRatio.Value, imposedLoadPercentage, shrinkageStrain, userStrain);

      Output.SetItem(this, DA, 0, new ConcreteMaterialGoo(concreteMaterial));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] != "-") {
        Grade = (ConcreteGradeEN)Enum.Parse(typeof(ConcreteGradeEN), _selectedItems[0]);
      }
      int i = 1;
      if (isLightWeight) {
        DensityClass = (ConcreteMaterial.DensityClass)Enum.Parse(typeof(ConcreteMaterial.DensityClass), _selectedItems[i++]);
      }

      DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), _selectedItems[i++]);
      StrainUnit = (StrainUnit)UnitsHelper.Parse(typeof(StrainUnit), _selectedItems[i++]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
