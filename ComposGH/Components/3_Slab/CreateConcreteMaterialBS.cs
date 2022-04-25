using ComposGH.Parameters;
using Grasshopper.Kernel;
using Oasys.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreateConcreteMaterialBS : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2ca1b0d6-44a2-441f-bf4b-8367d98d90a8");
    public CreateConcreteMaterialBS()
      : base("Concrete Material BS", "ConcMatBS", "Create Concrete Material to British Standard (BS) for Concrete Slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.RebarMaterial;
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
            "Weight Type",
            "Density Unit",
            "Strain Unit"
    });
    private bool first = true;
    private ConcreteMaterial.ConcreteGrade Grade = ConcreteMaterial.ConcreteGrade.C25;
    private ConcreteMaterial.WeightType Type = ConcreteMaterial.WeightType.Normal;
    private DensityUnit DensityUnit = Units.DensityUnit;
    private StrainUnit StrainUnit = StrainUnit.MilliStrain;

    public override void CreateAttributes()
    {
      if (first)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // grade
        List<string> concreteGrades = Enum.GetValues(typeof(ConcreteMaterial.ConcreteGrade)).Cast<ConcreteMaterial.ConcreteGrade>().Select(x => x.ToString()).ToList();
        concreteGrades.RemoveAt(0); // C20
        DropDownItems.Add(concreteGrades);
        SelectedItems.Add(Grade.ToString());

        // weight type
        DropDownItems.Add(Enum.GetValues(typeof(ConcreteMaterial.WeightType)).Cast<ConcreteMaterial.WeightType>().Select(x => x.ToString()).ToList());
        SelectedItems.Add(Type.ToString());

        // density
        DropDownItems.Add(Units.FilteredDensityUnits);
        SelectedItems.Add(DensityUnit.ToString());

        // strain
        DropDownItems.Add(Units.FilteredStrainUnits);
        SelectedItems.Add(StrainUnit.ToString());

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0) // change is made to grade
      {
        Grade = (ConcreteMaterial.ConcreteGrade)Enum.Parse(typeof(ConcreteMaterial.ConcreteGrade), SelectedItems[i]);
      }
      else if (i == 1) // change is made to weight type
      {
        Type = (ConcreteMaterial.WeightType)Enum.Parse(typeof(ConcreteMaterial.WeightType), SelectedItems[i]);
      }
      else if (i == 2) // change is made to density unit
      {
        DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), SelectedItems[i]);
      }
      else if (i == 3) // change is made to strain unit
      {
        StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), SelectedItems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      Grade = (ConcreteMaterial.ConcreteGrade)Enum.Parse(typeof(ConcreteMaterial.ConcreteGrade), SelectedItems[0]);
      Type = (ConcreteMaterial.WeightType)Enum.Parse(typeof(ConcreteMaterial.WeightType), SelectedItems[1]);
      DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), SelectedItems[2]);
      StrainUnit = (StrainUnit)Enum.Parse(typeof(StrainUnit), SelectedItems[3]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity density = new Density(0, DensityUnit);
      string densityUnitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));

      string strainUnitAbbreviation = Oasys.Units.Strain.GetAbbreviation(StrainUnit);

      // optional
      pManager.AddGenericParameter("Dry Density [" + densityUnitAbbreviation + "]", "DD", "(Optional) Dry Density", GH_ParamAccess.item);
      pManager.AddGenericParameter("Steel/Concrete Modular Ratios", "SCMR", "(Optional) Steel/Concrete Modular Ratios", GH_ParamAccess.item);
      pManager.AddNumberParameter("Imposed Load Percentage [%]", "PoILALT", "(Optional) Percentage of imposed load acting long term", GH_ParamAccess.item, 33);
      pManager.AddGenericParameter("Shrinkage Strain [" + strainUnitAbbreviation + "]", "SS", "(Optional) Shrinkage Strain", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Concrete Material", "CMt", "Concrete Material for Concrete Slab", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Density dryDensity = GetInput.Density(this, DA, 0, DensityUnit, true);

      SteelConcreteModularRatio steelConcreteModularRatio = new SteelConcreteModularRatio();
      DA.GetData(1, ref steelConcreteModularRatio);

      double percentageOfImposedLoadActingLongTerm = 33;
      DA.GetData(2, ref percentageOfImposedLoadActingLongTerm);

      Strain shrinkageStrain = GetInput.Strain(this, DA, 3, StrainUnit, true);

      ConcreteMaterial concreteMaterial = new ConcreteMaterial();
      concreteMaterial.Grade = Grade;
      concreteMaterial.Type = Type;
      concreteMaterial.DryDensity = dryDensity;
      concreteMaterial.SteelConcreteModularRatio = steelConcreteModularRatio;
      concreteMaterial.PercentageOfImposedLoadActingLongTerm = percentageOfImposedLoadActingLongTerm;
      concreteMaterial.ShrinkageStrain = shrinkageStrain;

      DA.SetData(0, concreteMaterial);
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropDownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropDownItems, ref SelectedItems, ref SpacerDescriptions);

      UpdateUIFromSelectedItems();

      first = false;

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
      IQuantity density = new Density(0, DensityUnit);
      string densityUnitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));
      Params.Input[0].Name = "Density [" + densityUnitAbbreviation + "]";

      // todo: add the rest of the dropdowns
      IQuantity strain = new Strain(0, StrainUnit);
      string strainUnitAbbreviation = string.Concat(strain.ToString().Where(char.IsLetter));
      Params.Input[3].Name = "Strain [" + strainUnitAbbreviation + "]";
    }
    #endregion
  }
}
