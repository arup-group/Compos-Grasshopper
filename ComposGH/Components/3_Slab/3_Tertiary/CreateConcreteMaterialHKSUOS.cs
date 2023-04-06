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
  public class CreateConcreteMaterialHKSUOS : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6781ee34-494e-414c-9542-6be29f1a5696");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateConcreteMaterialHK;
    public CreateConcreteMaterialHKSUOS()
      : base("HK" + ConcreteMaterialGoo.Name.Replace(" ", string.Empty),
          "HK" + ConcreteMaterialGoo.NickName.Replace(" ", string.Empty),
          "Look up a Standard HK " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description,
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
      pManager.AddParameter(new ConcreteMaterialParam(), ConcreteMaterialGoo.Name, ConcreteMaterialGoo.NickName, "HK " + ConcreteMaterialGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
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
          string text = "Could not parse concrete grade. Valid HKSUOS concrete grades are ";
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

      Density dryDensity = new Density(2450, DensityUnit.KilogramPerCubicMeter);
      bool userDensity = false;
      if (this.Params.Input[0].Sources.Count > 0)
      {
        dryDensity = (Density)Input.UnitNumber(this, DA, 0, this.DensityUnit);
        userDensity = true;
      }

      Ratio imposedLoadPercentage = (Ratio)Input.UnitNumber(this, DA, 2, RatioUnit.DecimalFraction);

      ERatioGoo eRatio = (ERatioGoo)Input.GenericGoo<ERatioGoo>(this, DA, 1);

      ConcreteMaterial concreteMaterial = new ConcreteMaterial(this.Grade, dryDensity, userDensity, (eRatio == null) ? new ERatio() : eRatio.Value, imposedLoadPercentage);

      Output.SetItem(this, DA, 0, new ConcreteMaterialGoo(concreteMaterial));
    }

    #region Custom UI
    List<bool> Override_dropDownItems;
    private ConcreteGrade Grade = ConcreteGrade.C25;
    private DensityUnit DensityUnit = DefaultUnits.DensityUnit;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] { "Grade", "Density Unit" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // grade
      List<string> concreteGrades = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
      concreteGrades.RemoveAt(0); // C20
      concreteGrades.RemoveAt(2); // C32
      concreteGrades.RemoveRange(8, 4); // C70...C100
      this._dropDownItems.Add(concreteGrades);
      this._selectedItems.Add(this.Grade.ToString());

      // density unit
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Density));
      this._selectedItems.Add(Density.GetAbbreviation(this.DensityUnit));

      this.Override_dropDownItems = new List<bool>() { false, false };

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0) // change is made to grade
        this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), this._selectedItems[i]);

      else if (i == 1) // change is made to density unit
        this.DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), this._selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      if (this._selectedItems[0] != "-")
        this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), this._selectedItems[0]);
      this.DensityUnit = (DensityUnit)UnitsHelper.Parse(typeof(DensityUnit), this._selectedItems[1]);

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
