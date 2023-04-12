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
  public class CreateConcreteMaterialBS : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2ca1b0d6-44a2-441f-bf4b-8367d98d90a8");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateConcreteMaterialBS;
    public CreateConcreteMaterialBS()
      : base("BS" + ConcreteMaterialGoo.Name.Replace(" ", string.Empty),
          "BS" + ConcreteMaterialGoo.NickName.Replace(" ", string.Empty),
          "Look up a Standard BS " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string densityUnitAbbreviation = Density.GetAbbreviation(this.DensityUnit);

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

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ConcreteMaterialParam(), ConcreteMaterialGoo.Name, ConcreteMaterialGoo.NickName, "BS " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // override concrete grade?
      if (this.Params.Input[3].Sources.Count > 0)
      {
        string grade = "";
        DA.GetData(3, ref grade);
        try
        {
          this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), grade);
          this._dropDownItems[0] = new List<string>();
          this._selectedItems[0] = "-";
          this.Override_dropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse concrete grade. Valid BS concrete grades are ";
          foreach (string g in Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList())
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this._dropDownItems[0] = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, text);
          return;
        }
      }
      else if (this.Override_dropDownItems[0])
      {
        this._dropDownItems[0] = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
        this.Override_dropDownItems[0] = false;
      }

      Density dryDensity = new Density(2400, DensityUnit.KilogramPerCubicMeter);
      bool userDensity = false;
      if (this.Params.Input[0].Sources.Count > 0)
      {
        dryDensity = (Density)Input.UnitNumber(this, DA, 0, this.DensityUnit);
        userDensity = true;
      }
      else
      {
        if (this.Type == ConcreteMaterial.WeightType.LightWeight)
          dryDensity = new Density(1800, DensityUnit.KilogramPerCubicMeter);
      }

      Ratio imposedLoadPercentage = (Ratio)Input.UnitNumber(this, DA, 2, RatioUnit.DecimalFraction);

      ERatioGoo eRatio = (ERatioGoo)Input.GenericGoo<ERatioGoo>(this, DA, 1);

      IConcreteMaterial concreteMaterial = new ConcreteMaterial(this.Grade, this.Type, dryDensity, userDensity, (eRatio == null) ? new ERatio() : eRatio.Value, imposedLoadPercentage);

      Output.SetItem(this, DA, 0, new ConcreteMaterialGoo(concreteMaterial));
    }


    #region Custom UI
    List<bool> Override_dropDownItems;
    private ConcreteGrade Grade = ConcreteGrade.C25;
    private ConcreteMaterial.WeightType Type = ConcreteMaterial.WeightType.Normal;
    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] {
        "Grade",
        "Weight Type",
        "Density Unit" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // grade
      List<string> concreteGrades = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
      concreteGrades.RemoveAt(0); // C20
      concreteGrades.RemoveAt(2); // C32
      this._dropDownItems.Add(concreteGrades);
      this._selectedItems.Add(this.Grade.ToString());

      // weight type
      this._dropDownItems.Add(Enum.GetValues(typeof(ConcreteMaterial.WeightType)).Cast<ConcreteMaterial.WeightType>().Select(x => x.ToString()).ToList());
      this._selectedItems.Add(this.Type.ToString());

      // density unit
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      this._selectedItems.Add(Density.GetAbbreviation(this.DensityUnit));

      this.Override_dropDownItems = new List<bool>() { false, false, false };

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0) // change is made to grade
        this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), this._selectedItems[i]);

      else if (i == 1) // change is made to weight type
        this.Type = (ConcreteMaterial.WeightType)Enum.Parse(typeof(ConcreteMaterial.WeightType), this._selectedItems[i]);

      else if (i == 2) // change is made to density unit
        this.DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), this._selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      if (this._selectedItems[0] != "-")
        this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), this._selectedItems[0]);
      this.Type = (ConcreteMaterial.WeightType)Enum.Parse(typeof(ConcreteMaterial.WeightType), this._selectedItems[1]);
      this.DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), this._selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string densityUnitAbbreviation = Density.GetAbbreviation(this.DensityUnit);
      this.Params.Input[0].Name = "Density [" + densityUnitAbbreviation + "]";
    }
    #endregion
  }
}
