﻿using System;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components
{
  public class CreateCustomDeck : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8859723E-D8BD-4AC5-A341-81D1B5708F43");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CustomDecking;
    public CreateCustomDeck()
      : base("Custom" + DeckingGoo.Name.Replace(" ", string.Empty),
          DeckingGoo.Name.Replace(" ", string.Empty),
          "Create a " + DeckingGoo.Description + " for a " + SlabGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      string stressunitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);

      pManager.AddGenericParameter("b1 [" + unitAbbreviation + "]", "b1", "Lenght of b1 deck parameter(Deck_Spacing). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b2 [" + unitAbbreviation + "]", "b2", "Lenght of b2 deck parameter(Deck_UpperWidth). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b3 [" + unitAbbreviation + "]", "b3", "Lenght of b3 deck parameter(Deck_LowerWidth). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b4 [" + unitAbbreviation + "]", "b4", "Lenght of b4 deck parameter(Deck_Proj_Height). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("b5 [" + unitAbbreviation + "]", "b5", "Lenght of b5 deck parameter(Deck_Proj_width). See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth [" + unitAbbreviation + "]", "D", "Depth of a deck. See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thickness [" + unitAbbreviation + "]", "Th", "Thickness of a deck sheet. See the decking picture in helps", GH_ParamAccess.item);
      pManager.AddGenericParameter("Strength [" + stressunitAbbreviation + "]", "fu", "characteristic strength of Steel Deck", GH_ParamAccess.item);
      pManager.AddGenericParameter(DeckingConfigurationGoo.Name, DeckingConfigurationGoo.NickName, "(Optional)" + DeckingConfigurationGoo.Description, GH_ParamAccess.item);
      pManager[8].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposDeckingParameter(), "Custom " + DeckingGoo.Name, DeckingGoo.NickName, "Custom " + DeckingGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length distB1 = (Length)Input.UnitNumber(this, DA, 0, this.LengthUnit);
      Length distB2 = (Length)Input.UnitNumber(this, DA, 1, this.LengthUnit);
      Length distB3 = (Length)Input.UnitNumber(this, DA, 2, this.LengthUnit);
      Length distB4 = (Length)Input.UnitNumber(this, DA, 3, this.LengthUnit);
      Length distB5 = (Length)Input.UnitNumber(this, DA, 4, this.LengthUnit);
      Length depth = (Length)Input.UnitNumber(this, DA, 5, this.LengthUnit);
      Length thickness = (Length)Input.UnitNumber(this, DA, 6, this.LengthUnit);
      Pressure stress = (Pressure)Input.UnitNumber(this, DA, 7, this.StressUnit);
      DeckingConfigurationGoo dconf = (DeckingConfigurationGoo)Input.GenericGoo<DeckingConfigurationGoo>(this, DA, 8);

      Output.SetItem(this, DA, 0, new DeckingGoo(new CustomDecking(distB1, distB2, distB3, distB4, distB5, depth, thickness, stress, (dconf == null) ? new DeckingConfiguration() : dconf.Value)));
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    private PressureUnit StressUnit = DefaultUnits.MaterialStrengthUnit;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] { "Length Unit", "Strength Unit" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // length
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this._selectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      // strength
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      this._selectedItems.Add(Pressure.GetAbbreviation(this.StressUnit));

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0) // change is made to length unit
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this._selectedItems[i]);
      else
        this.StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this._selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this._selectedItems[0]);
      this.StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this._selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      string stressunitAbbreviation = Pressure.GetAbbreviation(this.StressUnit);

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
