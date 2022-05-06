using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateCustomDeck : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8859723E-D8BD-4AC5-A341-81D1B5708F43");
    public CreateCustomDeck()
      : base("Custom Decking", "CustDeck", "Create Custom Decking for a Compos Slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CustomDecking;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Length Unit",
      "Strength Unit"
    });

    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;
    private PressureUnit StressUnit = Units.StressUnit;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // length
        DropdownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        // strength
        DropdownItems.Add(Units.FilteredStressUnits);
        SelectedItems.Add(StressUnit.ToString());

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (i == 0) // change is made to length unit
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);
      else
        StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[i]);

        // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[0]);
      StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[1]);

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
      IQuantity length = new Length(0, LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      IQuantity stress = new Pressure(0, StressUnit);
      string stressunitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("b1 [" + unitAbbreviation + "]", "b1", "Lenght of b1 deck parameter(Deck_Spacing). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b2 [" + unitAbbreviation + "]", "b2", "Lenght of b2 deck parameter(Deck_UpperWidth). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b3 [" + unitAbbreviation + "]", "b3", "Lenght of b3 deck parameter(Deck_LowerWidth). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b4 [" + unitAbbreviation + "]", "b4", "Lenght of b4 deck parameter(Deck_Proj_Height). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b5 [" + unitAbbreviation + "]", "b5", "Lenght of b5 deck parameter(Deck_Proj_width). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth [" + unitAbbreviation + "]", "D", "Depth of a deck. See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thickness [" + unitAbbreviation + "]", "Th", "Thickness of a deck sheet. See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Strength [" + stressunitAbbreviation + "]", "fu", "characteristic strength of Steel Deck", GH_ParamAccess.item);
      pManager.AddGenericParameter("Deck Config", "DC", "Compos Deck Configuration", GH_ParamAccess.item);
      pManager[8].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Decking", "Dk", "Compos Decking", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length distB1 = GetInput.Length(this, DA, 0, LengthUnit);
      Length distB2 = GetInput.Length(this, DA, 1, LengthUnit);
      Length distB3 = GetInput.Length(this, DA, 2, LengthUnit);
      Length distB4 = GetInput.Length(this, DA, 3, LengthUnit);
      Length distB5 = GetInput.Length(this, DA, 4, LengthUnit);
      Length depth = GetInput.Length(this, DA, 5, LengthUnit);
      Length thickness = GetInput.Length(this, DA, 6, LengthUnit);
      Pressure stress = GetInput.Stress(this, DA, 7, StressUnit);
      DeckingConfigurationGoo dconf = (DeckingConfigurationGoo)GetInput.GenericGoo<DeckingConfigurationGoo>(this, DA, 8);

      DA.SetData(0, new DeckingGoo(new CustomDecking(distB1, distB2, distB3, distB4, distB5, depth, thickness, stress, (dconf == null) ? new DeckingConfiguration() : dconf.Value)));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropdownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropdownItems, ref SelectedItems, ref SpacerDescriptions);

      UpdateUIFromSelectedItems();

      First = false;

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
      IQuantity length = new Length(0, LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      IQuantity stress = new Pressure(0, StressUnit);
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
