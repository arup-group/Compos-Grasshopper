using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Oasys.Units;
using OasysGH.Components;
using OasysGH.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreateConcreteMaterialEN : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("fd361dc8-98bc-4cad-ba15-4da5da3c52bb");
    public CreateConcreteMaterialEN()
      : base("EN" + ConcreteMaterialGoo.Name.Replace(" ", string.Empty),
          "EN" + ConcreteMaterialGoo.NickName.Replace(" ", string.Empty),
          "Look up a Standard EN " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateConcreteMaterialEN;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string densityUnitAbbreviation = Density.GetAbbreviation(this.DensityUnit);
      string strainUnitAbbreviation = Strain.GetAbbreviation(this.StrainUnit);

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

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(ConcreteMaterialGoo.Name, ConcreteMaterialGoo.NickName, "EN " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // override concrete grade?
      if (this.Params.Input[4].Sources.Count > 0)
      {
        string grade = "";
        DA.GetData(4, ref grade);
        try
        {
          grade = grade.Replace(" ", "_").Replace("/", "_"); // C30/37 -> C30_37
          this.Grade = (ConcreteGradeEN)Enum.Parse(typeof(ConcreteGradeEN), grade);
          this.DropDownItems[0] = new List<string>();
          this.SelectedItems[0] = "-";
          this.OverrideDropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse concrete grade. Valid EC4 concrete grades are ";
          foreach (string g in Enum.GetValues(typeof(ConcreteGradeEN)).Cast<ConcreteGradeEN>().Select(x => x.ToString()).ToList())
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this.DropDownItems[0] = Enum.GetValues(typeof(ConcreteGradeEN)).Cast<ConcreteGradeEN>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      }
      else if (this.OverrideDropDownItems[0])
      {
        this.DropDownItems[0] = Enum.GetValues(typeof(ConcreteGradeEN)).Cast<ConcreteGradeEN>().Select(x => x.ToString()).ToList();
        this.OverrideDropDownItems[0] = false;
      }
      // override density class?
      if (this.Params.Input[5].Sources.Count > 0)
      {
        string densityClass = "";
        DA.GetData(5, ref densityClass);
        try
        {
          this.DensityClass = (ConcreteMaterial.DensityClass)Enum.Parse(typeof(ConcreteMaterial.DensityClass), densityClass);
          this.DropDownItems[1] = new List<string>();
          this.SelectedItems[1] = "-";
          this.OverrideDropDownItems[1] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse density class. Valid density classes are ";
          foreach (string dc in Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList())
          {
            text += dc + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this.DropDownItems[1] = Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
        }
      }
      else if (this.OverrideDropDownItems[1])
      {
        this.DropDownItems[1] = Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList();
        this.OverrideDropDownItems[1] = false;
      }

      Density dryDensity = new Density(2400, DensityUnit.KilogramPerCubicMeter);
      bool userDensity = false;
      if (this.Params.Input[0].Sources.Count > 0)
      {
        dryDensity = GetInput.Density(this, DA, 0, this.DensityUnit);
        userDensity = true;
        if (this.isLightWeight)
          SelectedItems[1] = "NOT_APPLY";
      }
      else if (this.Grade.ToString().StartsWith("L"))
      {
        if (this.DensityClass == ConcreteMaterial.DensityClass.NOT_APPLY)
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please select a densitiy class.");
          return;
        }
        dryDensity = new Density((double)this.DensityClass, DensityUnit.KilogramPerCubicMeter);
      }

      ERatioGoo eRatio = (ERatioGoo)GetInput.GenericGoo<ERatioGoo>(this, DA, 1);

      Ratio imposedLoadPercentage = GetInput.Ratio(this, DA, 2, RatioUnit.DecimalFraction);

      Strain shrinkageStrain = new Strain(-0.5, StrainUnit.MilliStrain);
      bool userStrain = false;
      if (this.Params.Input[3].Sources.Count > 0)
      {
        shrinkageStrain = GetInput.Strain(this, DA, 3, StrainUnit, true);
        userStrain = true;
      }

      ConcreteMaterial.DensityClass selectedDensityClass = this.DensityClass;
      if (!isLightWeight)
        selectedDensityClass = ConcreteMaterial.DensityClass.NOT_APPLY;

      ConcreteMaterial concreteMaterial = new ConcreteMaterial(this.Grade, selectedDensityClass, dryDensity, userDensity, (eRatio == null) ? new ERatio() { ShortTerm = 6, LongTerm = 18, Vibration = 5.39 } : eRatio.Value, imposedLoadPercentage, shrinkageStrain, userStrain);

      Output.SetItem(this, DA, 0, new ConcreteMaterialGoo(concreteMaterial));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetBoolean("isLightWeight", this.isLightWeight);
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.isLightWeight = reader.GetBoolean("isLightWeight");
      return base.Read(reader);
    }
    #endregion


    #region Custom UI
    List<bool> OverrideDropDownItems;
    private ConcreteGradeEN Grade = ConcreteGradeEN.C20_25;
    private ConcreteMaterial.DensityClass DensityClass = ConcreteMaterial.DensityClass.DC801_1000;
    private DensityUnit DensityUnit = Units.DensityUnit;
    private StrainUnit StrainUnit = StrainUnit.MilliStrain;
    private bool isLightWeight = false;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] {
        "Grade",
        "Density Unit",
        "Strain Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // grade
      List<string> concreteGrades = Enum.GetValues(typeof(ConcreteGradeEN)).Cast<ConcreteGradeEN>().Select(x => x.ToString()).ToList();
      this.DropDownItems.Add(concreteGrades);
      this.SelectedItems.Add(this.Grade.ToString());

      // density unit
      this.DropDownItems.Add(Units.FilteredDensityUnits);
      this.SelectedItems.Add(this.DensityUnit.ToString());

      // strain unit
      this.DropDownItems.Add(Units.FilteredStrainUnits);
      this.SelectedItems.Add(this.StrainUnit.ToString());

      this.OverrideDropDownItems = new List<bool>() { false, false, false, false };

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to grade
      {
        this.Grade = (ConcreteGradeEN)Enum.Parse(typeof(ConcreteGradeEN), this.SelectedItems[i]);
        if (this.Grade.ToString().StartsWith("LC"))
        {
          this.isLightWeight = true;
          if (this.DropDownItems.Count < 4)
          {
            // density class
            List<string> densityClasses = Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList();
            densityClasses.RemoveAt(0);
            this.DropDownItems.Insert(1, densityClasses);
            this.SelectedItems.Insert(1, this.DensityClass.ToString());
            this.SpacerDescriptions.Insert(1, "Density Class");
          }
        }
        else
        {
          this.isLightWeight = false;
          if (this.DropDownItems.Count > 3)
          {
            this.DropDownItems.RemoveAt(1);
            this.SelectedItems.RemoveAt(1);
            this.SpacerDescriptions.RemoveAt(1);
          }
        }
      }

      else if (this.isLightWeight & i == 1) // change is made to density class
        this.DensityClass = (ConcreteMaterial.DensityClass)Enum.Parse(typeof(ConcreteMaterial.DensityClass), this.SelectedItems[i]);

      if (!this.isLightWeight)
        i++; // 

      else if (i == 2) // change is made to density unit
        this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[i]);

      else if (i == 3) // change is made to strain unit
        this.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      if (this.SelectedItems[0] != "-")
        this.Grade = (ConcreteGradeEN)Enum.Parse(typeof(ConcreteGradeEN), this.SelectedItems[0]);
      int i = 1;
      if (this.isLightWeight)
        this.DensityClass = (ConcreteMaterial.DensityClass)Enum.Parse(typeof(ConcreteMaterial.DensityClass), this.SelectedItems[i++]);

      this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[i++]);
      this.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), this.SelectedItems[i++]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string densityUnitAbbreviation = Density.GetAbbreviation(this.DensityUnit);
      string strainUnitAbbreviation = Strain.GetAbbreviation(this.StrainUnit);
      this.Params.Input[0].Name = "Density [" + densityUnitAbbreviation + "]";
      this.Params.Input[3].Name = "Strain [" + strainUnitAbbreviation + "]";
    }
    #endregion
  }
}
