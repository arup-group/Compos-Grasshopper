using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateRebarMaterial: GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("E91D37A1-81D4-427D-9910-E8A514466F3C");
    public CreateRebarMaterial()
      : base("Rebar Material", "RebarMat", "Create Rebar Material for Compos Transverse Reinforcement",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.RebarMaterial;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // grade
        dropdownitems.Add(Enum.GetValues(typeof(ReinforcementMaterial.StandardGrade)).Cast<ReinforcementMaterial.StandardGrade>().Select(x => x.ToString()).ToList());
        selecteditems.Add(mat.ToString());

        // strength
        dropdownitems.Add(Units.FilteredStressUnits);
        selecteditems.Add(stressUnit.ToString());

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0) // change is made to grade
      {
        mat = (ReinforcementMaterial.StandardGrade)Enum.Parse(typeof(ReinforcementMaterial.StandardGrade), selecteditems[i]);
      }
      if (i == 1) // change is made to unit
      {
        stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
      }


      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      if (selecteditems[0] != "Custom")
        mat = (ReinforcementMaterial.StandardGrade)Enum.Parse(typeof(ReinforcementMaterial.StandardGrade), selecteditems[0]);

      stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[1]);


      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 

    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Grade",
            "Unit"
    });

    private bool first = true;
    private PressureUnit stressUnit = Units.StressUnit;
    private ReinforcementMaterial.StandardGrade mat = ReinforcementMaterial.StandardGrade.EN_500B;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity stress = new Pressure(0, stressUnit);
      string stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
      pManager.AddGenericParameter("Strength [" + stressUnitAbbreviation + "]", "fu", "(Optional) Custom Characteristic Steel Strength", GH_ParamAccess.item);
      pManager[0].Optional = true;

    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Material", "RMt", "Reinforcement Material for Transverse Reinforcement", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (this.Params.Input[0].Sources.Count > 0)
      {
        selecteditems[0] = "Custom";
        DA.SetData(0, new ReinforcementMaterialGoo(new ReinforcementMaterial(GetInput.Stress(this, DA, 0, stressUnit))));
      }
      else
        DA.SetData(0, new ReinforcementMaterialGoo(new ReinforcementMaterial(mat)));
    }


    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

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
      IQuantity stress = new Pressure(0, stressUnit);
      string stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));
      Params.Input[0].Name = "Strength [" + stressUnitAbbreviation + "]";
    }
    #endregion
  }
}
