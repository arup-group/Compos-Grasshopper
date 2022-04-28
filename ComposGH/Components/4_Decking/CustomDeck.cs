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

namespace ComposGH.Components
{
  public class CustomDeck : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8859723E-D8BD-4AC5-A341-81D1B5708F43");
    public CustomDeck()
      : base("Custom Decking", "CustDeck", "Create Custom Decking for a Compos Slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    //protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStudZoneLength;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // length
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

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


      if (i == 0) // change is made to length unit
      {
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
      }
      else
      {
        stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
      }

        //lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
        //stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);

        // update name of inputs (to display unit on sliders)
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[0]);
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
            "Length Unit",
            "Strength Unit"
    });

    private bool first = true;
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    private PressureUnit stressUnit = Units.StressUnit;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      IQuantity stress = new Pressure(0, stressUnit);
      string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("b1 [" + unitAbbreviation + "]", "b1", "Lenght of b1 deck parameter(Deck_Spacing). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b2 [" + unitAbbreviation + "]", "b2", "Lenght of b2 deck parameter(Deck_UpperWidth). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b3 [" + unitAbbreviation + "]", "b3", "Lenght of b3 deck parameter(Deck_LowerWidth). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b4 [" + unitAbbreviation + "]", "b4", "Lenght of b4 deck parameter(Deck_Proj_Height). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b5 [" + unitAbbreviation + "]", "b5", "Lenght of b5 deck parameter(Deck_Proj_width). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth [" + unitAbbreviation + "]", "D", "Depth of a deck. See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thickness [" + unitAbbreviation + "]", "Th", "Thickness of a deck sheet. See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Strength [" + stressunitAbbreviation + "]", "fu", "characteristic strength of Steel Deck", GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "RMt", "Reinforcement Material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Deck Config", "DeckConfig", "Compos Deck Configuration setup", GH_ParamAccess.item);
      
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Custom Deck", "Deck", "Custom Compos Deck", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length distB1 = GetInput.Length(this, DA, 0, lengthUnit);
      Length distB2 = GetInput.Length(this, DA, 1, lengthUnit);
      Length distB3 = GetInput.Length(this, DA, 2, lengthUnit);
      Length distB4 = GetInput.Length(this, DA, 3, lengthUnit);
      Length distB5 = GetInput.Length(this, DA, 4, lengthUnit);
      Length depth = GetInput.Length(this, DA, 5, lengthUnit);
      Length thickness = GetInput.Length(this, DA, 6, lengthUnit);
      Pressure stress = GetInput.Stress(this, DA, 7, stressUnit);
      DeckConfiguration dconf = GetInput.DeckConfiguration(this, DA, 0);

      DA.SetData(0, new ComposDeckGoo(new ComposDeck(distB1, distB2, distB3, distB4, distB5, depth, thickness, stress, dconf)));
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
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      IQuantity stress = new Pressure(0, stressUnit);
      string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

      Params.Input[0].Name = "b1 [" + unitAbbreviation + "]";
      Params.Input[1].Name = "b2 [" + unitAbbreviation + "]";
      Params.Input[2].Name = "b3 [" + unitAbbreviation + "]";
      Params.Input[3].Name = "b4 [" + unitAbbreviation + "]";
      Params.Input[4].Name = "b5 [" + unitAbbreviation + "]";
      Params.Input[5].Name = "Depth [" + unitAbbreviation + "]";
      Params.Input[6].Name = "Thickness [" + unitAbbreviation + "]";
      Params.Input[7].Name = "Strength [" + stressunitAbbreviation + "]";

    }
    #endregion
  }
}
