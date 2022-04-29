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
using Grasshopper.Kernel.Parameters;

namespace ComposGH.Components
{
  public class CreateSteelMaterial : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("2C3C07F4-C395-4747-A111-D5A67B250104");
    public CreateSteelMaterial()
      : base("Steel Material", "SteelMat", "Create Standard Steel Material for a Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateSteelMaterial;
    #endregion

    #region Custom UI
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // SteelType
        dropdownitems.Add(Enum.GetValues(typeof(ComposSteelMaterial.SteelType)).Cast<ComposSteelMaterial.SteelType>().Select(x => x.ToString()).ToList());
        selecteditems.Add(ST.ToString());

        // WeldMaterial
        dropdownitems.Add(Enum.GetValues(typeof(ComposSteelMaterial.WeldMat)).Cast<ComposSteelMaterial.WeldMat>().Select(x => x.ToString()).ToList());
        selecteditems.Add(WM.ToString());

        // Stress
        dropdownitems.Add(Units.FilteredStressUnits);
        selecteditems.Add(stressUnit.ToString());

        // Density
        dropdownitems.Add(Units.FilteredDensityUnits);
        selecteditems.Add(densityUnit.ToString());

        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0)  // change is made to code 
      {
        if (ST.ToString() == selecteditems[i])
          return; // return if selected value is same as before

        ST = (ComposSteelMaterial.SteelType)Enum.Parse(typeof(ComposSteelMaterial.SteelType), selecteditems[i]);

        //ToggleInput();
      }
      if (i == 1)  // change is made to code 
      {
        if (WM.ToString() == selecteditems[i])
          return; // return if selected value is same as before

        WM = (ComposSteelMaterial.WeldMat)Enum.Parse(typeof(ComposSteelMaterial.WeldMat), selecteditems[i]);

        //ToggleInput();
      }
      if (i == 2)
      {
        stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
      }
      if (i == 3)
      {
        densityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), selecteditems[i]);
      }

        // update name of inputs (to display unit on sliders)
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }


    private void UpdateUIFromSelectedItems()
    {
      ST = (ComposSteelMaterial.SteelType)Enum.Parse(typeof(ComposSteelMaterial.SteelType), selecteditems[1]);
      WM = (ComposSteelMaterial.WeldMat)Enum.Parse(typeof(ComposSteelMaterial.WeldMat), selecteditems[2]);
      stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[3]);
      densityUnit = (DensityUnit)Enum.Parse(typeof(DensityUnit), selecteditems[4]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output

    // list of lists with all dropdown lists content
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Steel Type",
            "Weld Material",
            "StressUnit",
            "DensityUnit"
    });

    private bool first = true;
    private PressureUnit stressUnit = Units.StressUnit;
    private DensityUnit densityUnit = Units.DensityUnit;
    private ComposSteelMaterial.SteelType ST = ComposSteelMaterial.SteelType.S235;
    private ComposSteelMaterial.WeldMat WM = ComposSteelMaterial.WeldMat.Grade35;

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity stress1 = new Pressure(0, stressUnit);
      IQuantity stress2 = new Pressure(0, stressUnit);
      IQuantity density = new Density(0, densityUnit);

      string stress1unitAbbreviation = string.Concat(stress1.ToString().Where(char.IsLetter));
      string stress2unitAbbreviation = string.Concat(stress2.ToString().Where(char.IsLetter));
      string densityunitAbbreviation = string.Concat(density.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Strength [" + stress1unitAbbreviation + "]", "fy", "Steel Yield Strength", GH_ParamAccess.item);
      pManager.AddGenericParameter("Strength [" + stress2unitAbbreviation + "]", "E", "Steel Young's Modulus", GH_ParamAccess.item);
      pManager.AddGenericParameter("Density [" + densityunitAbbreviation + "]", "ρ", "Steel Density", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Reduction Factor", "RF", "Apply reduction factor for plastic moment capacity", GH_ParamAccess.item, false);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("StandardSteelMaterial", "SSM", "Standard Steel Material for a Compos Beam", GH_ParamAccess.item);
    }


    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool redFact = new bool();

      DA.GetData(3, ref redFact);

      if (this.Params.Input[0].Sources.Count > 0)
      {
        selecteditems[0] = "Custom";
        DA.SetData(0, new ComposSteelMaterialGoo(new ComposSteelMaterial(GetInput.Stress(this, DA, 0, stressUnit), GetInput.Stress(this, DA, 1, stressUnit), GetInput.Density(this, DA, 2, densityUnit), selecteditems[1].ToString(), true, redFact)));
      }
      else
        DA.SetData(0, new ComposSteelMaterialGoo(new ComposSteelMaterial(ST)));
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

    }
    #endregion

  }
}
