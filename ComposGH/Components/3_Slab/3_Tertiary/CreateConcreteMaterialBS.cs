using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;

namespace ComposGH.Components
{
  public class CreateConcreteMaterialBS : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2ca1b0d6-44a2-441f-bf4b-8367d98d90a8");
    public CreateConcreteMaterialBS()
      : base("BS Concrete Material", "ConcMatBS", "Create concrete material to British Standard (BS) for concrete slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateConcreteMaterialBS;
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
      "Density Unit"
    });
    private bool First = true;
    private ConcreteGrade Grade = ConcreteGrade.C25;
    private ConcreteMaterial.WeightType Type = ConcreteMaterial.WeightType.Normal;
    private DensityUnit DensityUnit = Units.DensityUnit;

    public override void CreateAttributes()
    {
      if (this.First)
      {
        this.DropDownItems = new List<List<string>>();
        this.SelectedItems = new List<string>();

        // grade
        List<string> concreteGrades = Enum.GetValues(typeof(ConcreteGrade)).Cast<ConcreteGrade>().Select(x => x.ToString()).ToList();
        concreteGrades.RemoveAt(0); // C20
        concreteGrades.RemoveAt(2); // C32
        this.DropDownItems.Add(concreteGrades);
        this.SelectedItems.Add(this.Grade.ToString());

        // weight type
        this.DropDownItems.Add(Enum.GetValues(typeof(ConcreteMaterial.WeightType)).Cast<ConcreteMaterial.WeightType>().Select(x => x.ToString()).ToList());
        this.SelectedItems.Add(this.Type.ToString());

        // density unit
        this.DropDownItems.Add(Units.FilteredDensityUnits);
        this.SelectedItems.Add(this.DensityUnit.ToString());

        this.First = false;
      }
      this.m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, this.DropDownItems, this.SelectedItems, this.SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to grade
        this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), this.SelectedItems[i]);

      else if (i == 1) // change is made to weight type
        this.Type = (ConcreteMaterial.WeightType)Enum.Parse(typeof(ConcreteMaterial.WeightType), this.SelectedItems[i]);

      else if (i == 2) // change is made to density unit
        this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      this.Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      this.Grade = (ConcreteGrade)Enum.Parse(typeof(ConcreteGrade), this.SelectedItems[0]);
      this.Type = (ConcreteMaterial.WeightType)Enum.Parse(typeof(ConcreteMaterial.WeightType), this.SelectedItems[1]);
      this.DensityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), this.SelectedItems[2]);

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

      // optional
      pManager.AddNumberParameter("Dry Density [" + densityUnitAbbreviation + "]", "DD", "(Optional) Dry density", GH_ParamAccess.item);
      pManager.AddGenericParameter("E Ratios", "ER", "(Optional) Steel/concrete Young´s modulus ratios", GH_ParamAccess.item);
      pManager.AddNumberParameter("Imposed Load Percentage [%]", "ILP", "(Optional) Percentage of imposed load acting long term", GH_ParamAccess.item, 33);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
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
        if (this.Type == ConcreteMaterial.WeightType.Light)
        dryDensity = new Density(1800, DensityUnit.KilogramPerCubicMeter);

      double imposedLoadPercentage = 33;
      DA.GetData(2, ref imposedLoadPercentage);

      ERatioGoo eRatio = (ERatioGoo)GetInput.GenericGoo<ERatioGoo>(this, DA, 1);

      IConcreteMaterial concreteMaterial = new ConcreteMaterial(this.Grade, this.Type, dryDensity, userDensity, (eRatio == null) ? new ERatio() : eRatio.Value, imposedLoadPercentage);

      DA.SetData(0, new ConcreteMaterialGoo(concreteMaterial));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, this.DropDownItems, this.SelectedItems, this.SpacerDescriptions);
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropDownItems, ref SelectedItems, ref SpacerDescriptions);

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
      IQuantity density = new Density(0, this.DensityUnit);
      string densityUnitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));
      this.Params.Input[0].Name = "Density [" + densityUnitAbbreviation + "]";
    }
    #endregion
  }
}
