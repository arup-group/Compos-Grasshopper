﻿using ComposGH.Parameters;
using Grasshopper.Kernel;
using Oasys.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreateConcreteMaterialEN : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("fd361dc8-98bc-4cad-ba15-4da5da3c52bb");
    public CreateConcreteMaterialEN()
      : base("Concrete Material EN", "ConcMatEN", "Create concrete material to European Standard (EN) for concrete slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateConcreteMaterial;
    #endregion

    #region Custom UI
    // This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
            "Grade",
            "Density Class",
            "Density Unit",
            "Strain Unit"
    });
    private bool First = true;
    private ConcreteMaterial.ConcreteGradeEN Grade = ConcreteMaterial.ConcreteGradeEN.C20_25;
    private ConcreteMaterial.DensityClass DensityClass = ConcreteMaterial.DensityClass.DC1801_2000;
    private DensityUnit DensityUnit = Units.DensityUnit;
    private StrainUnit StrainUnit = StrainUnit.MilliStrain;

    public override void CreateAttributes()
    {
      if (this.First)
      {
        this.DropDownItems = new List<List<string>>();
        this.SelectedItems = new List<string>();

        // grade
        List<string> concreteGrades = Enum.GetValues(typeof(ConcreteMaterial.ConcreteGradeEN)).Cast<ConcreteMaterial.ConcreteGradeEN>().Select(x => x.ToString()).ToList();
        //foreach (string concreteGrade in concreteGrades)
        //{
        //  concreteGrade.Replace("_", "/");
        //}
        this.DropDownItems.Add(concreteGrades);
        this.SelectedItems.Add(this.Grade.ToString());

        // density class
        List<string> densityClasses = Enum.GetValues(typeof(ConcreteMaterial.DensityClass)).Cast<ConcreteMaterial.DensityClass>().Select(x => x.ToString()).ToList();
        densityClasses.RemoveAt(0);
        this.DropDownItems.Add(densityClasses);
        this.SelectedItems.Add(this.DensityClass.ToString());

        // density unit
        this.DropDownItems.Add(Units.FilteredDensityUnits);
        this.SelectedItems.Add(this.DensityUnit.ToString());

        // strain unit
        this.DropDownItems.Add(Units.FilteredStrainUnits);
        this.SelectedItems.Add(this.StrainUnit.ToString());

        this.First = false;
      }
      this.m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, this.DropDownItems, this.SelectedItems, this.SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to grade
        this.Grade = (ConcreteMaterial.ConcreteGradeEN)Enum.Parse(typeof(ConcreteMaterial.ConcreteGradeEN), this.SelectedItems[i]);

      else if (i == 1) // change is made to density class
        this.DensityClass = (ConcreteMaterial.DensityClass)Enum.Parse(typeof(ConcreteMaterial.DensityClass), this.SelectedItems[i]);

      else if (i == 2) // change is made to density unit
        this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[i]);

      else if (i == 3) // change is made to strain unit
        this.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), this.SelectedItems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      this.Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      this.Grade = (ConcreteMaterial.ConcreteGradeEN)Enum.Parse(typeof(ConcreteMaterial.ConcreteGradeEN), this.SelectedItems[0]);
      this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[2]);
      this.StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), this.SelectedItems[3]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      this.Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity density = new Density(0, this.DensityUnit);
      string densityUnitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));

      string strainUnitAbbreviation = Oasys.Units.Strain.GetAbbreviation(this.StrainUnit);

      // optional
      pManager.AddNumberParameter("Dry Density [" + densityUnitAbbreviation + "]", "DD", "(Optional) Dry density", GH_ParamAccess.item);
      pManager.AddGenericParameter("E Ratios", "ER", "(Optional) Steel/concrete Young´s modulus ratios", GH_ParamAccess.item);
      pManager.AddNumberParameter("Imposed Load Percentage [%]", "ILP", "(Optional) Percentage of imposed load acting long term", GH_ParamAccess.item, 33);
      pManager.AddNumberParameter("Shrinkage Strain [" + strainUnitAbbreviation + "]", "SS", "(Optional) Shrinkage strain", GH_ParamAccess.item, -0.0005);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Concrete Material", "CMt", "Concrete material for concrete slab", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Density dryDensity = new Density(2400, DensityUnit.KilogramPerCubicMeter);
      bool userDensity = false;
      if (this.Params.Input[0].Sources.Count > 0)
      {
        dryDensity = GetInput.Density(this, DA, 0, this.DensityUnit);
        userDensity = true;
      }
      else
        if (this.Grade.ToString().StartsWith("L"))
        dryDensity = new Density((double)this.DensityClass, DensityUnit.KilogramPerCubicMeter);

      ERatio eRatio = new ERatio();
      if (!DA.GetData(1, ref eRatio))
      {
        eRatio.ShortTerm = 6;
        eRatio.LongTerm = 18;
        eRatio.Vibration = 5.39;
      }

      double imposedLoadPercentage = 33;
      DA.GetData(2, ref imposedLoadPercentage);

      Strain shrinkageStrain = new Strain(-0.0005, StrainUnit.MilliStrain);
      bool userStrain = false;
      if (this.Params.Input[3].Sources.Count > 0)
      {
        shrinkageStrain = GetInput.Strain(this, DA, 3, StrainUnit, true);
        userStrain = true;
      }

      ConcreteMaterial concreteMaterial = new ConcreteMaterial(this.Grade, this.DensityClass, dryDensity, userDensity, eRatio, imposedLoadPercentage, shrinkageStrain, userStrain);

      DA.SetData(0, concreteMaterial);
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, this.DropDownItems, this.SelectedItems, this.SpacerDescriptions);
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref this.DropDownItems, ref this.SelectedItems, ref this.SpacerDescriptions);

      UpdateUIFromSelectedItems();

      this.First = false;

      return base.Read(reader);
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity density = new Density(2, this.DensityUnit);
      string densityUnitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));
      this.Params.Input[0].Name = "Density [" + densityUnitAbbreviation + "]";

      IQuantity strain = new Strain(3, this.StrainUnit);
      string strainUnitAbbreviation = string.Concat(strain.ToString().Where(char.IsLetter));
      this.Params.Input[3].Name = "Strain [" + strainUnitAbbreviation + "]";
    }
    #endregion
  }
}
