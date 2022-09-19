using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnitsNet;
using OasysUnitsNet.Units;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateConcreteMaterialASNZ : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("0f51c6bb-e8cc-4b8a-add2-03b91ed2ca9b");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateConcreteMaterialAZ;
    public CreateConcreteMaterialASNZ()
      : base("ASNZ" + ConcreteMaterialGoo.Name.Replace(" ", string.Empty),
          "ASNZ" + ConcreteMaterialGoo.NickName.Replace(" ", string.Empty),
          "Look up a Standard ASNZ " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
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
      pManager.AddNumberParameter("Shrinkage Strain [" + strainUnitAbbreviation + "]", "SS", "(Optional) Shrinkage strain", GH_ParamAccess.item, -0.85);
      pManager.AddGenericParameter("Concrete Grade", "CG", "(Optional) Concrete grade", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ConcreteMaterialParam(), ConcreteMaterialGoo.Name, ConcreteMaterialGoo.NickName, "ASNZ " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
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
          this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), grade);
          this.DropDownItems[0] = new List<string>();
          this.SelectedItems[0] = "-";
          this.OverrideDropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse concrete grade. Valid AS/NZS concrete grades are ";
          foreach (string g in Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList())
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this.DropDownItems[0] = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      }
      else if (this.OverrideDropDownItems[0])
      {
        this.DropDownItems[0] = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
        this.OverrideDropDownItems[0] = false;
      }

      Density dryDensity = new Density(2450, DensityUnit.KilogramPerCubicMeter);
      bool userDensity = false;
      if (this.Params.Input[0].Sources.Count > 0)
      {
        dryDensity = (Density)Input.UnitNumber(this, DA, 0, this.DensityUnit);
        userDensity = true;
      }

      Ratio imposedLoadPercentage = (Ratio)Input.UnitNumber(this, DA, 2, RatioUnit.DecimalFraction);

      ERatioGoo eRatio = (ERatioGoo)Input.GenericGoo<ERatioGoo>(this, DA, 1);

      Strain shrinkageStrain = new Strain(-0.85, StrainUnit.MilliStrain);
      bool userStrain = false;
      if (this.Params.Input[3].Sources.Count > 0)
      {
        shrinkageStrain = (Strain)Input.UnitNumber(this, DA, 3, StrainUnit, true);
        userStrain = true;
      }

      ConcreteMaterial concreteMaterial = new ConcreteMaterial(this.Grade, dryDensity, userDensity, (eRatio == null) ? new ERatio() : eRatio.Value, imposedLoadPercentage, shrinkageStrain, userStrain);

      Output.SetItem(this, DA, 0, new ConcreteMaterialGoo(concreteMaterial));
    }

    #region Custom UI
    List<bool> OverrideDropDownItems;
    private ConcreteGrade Grade = ConcreteGrade.C20;
    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;
    private StrainUnit StrainUnit = StrainUnit.MilliStrain;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] {
        "Grade",
        "Density Unit",
        "Strain Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // grade
      List<string> concreteGrades = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
      this.DropDownItems.Add(concreteGrades);
      this.SelectedItems.Add(this.Grade.ToString());

      // density unit
      this.DropDownItems.Add(FilteredUnits.FilteredDensityUnits);
      this.SelectedItems.Add(this.DensityUnit.ToString());

      // strain unit
      this.DropDownItems.Add(FilteredUnits.FilteredStrainUnits);
      this.SelectedItems.Add(this.StrainUnit.ToString());

      this.OverrideDropDownItems = new List<bool>() { false, false, false };

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to grade
        this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), this.SelectedItems[i]);

      else if (i == 1) // change is made to density unit
        this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[i]);

      else if (i == 2) // change is made to strain unit
        this.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      if (this.SelectedItems[0] != "-")
        this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), this.SelectedItems[0]);
      this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[1]);
      this.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), this.SelectedItems[2]);

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
