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
using System;
using System.Collections.Generic;

namespace ComposGH.Components {
  public class CreateCustomTransverseReinforcementLayout : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("19322156-8b1a-4849-9772-813411af965c");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CustomRebarLayout;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public CreateCustomTransverseReinforcementLayout()
          : base("Create" + CustomTransverseReinforcementLayoutGoo.Name.Replace(" ", string.Empty),
      CustomTransverseReinforcementLayoutGoo.Name.Replace(" ", string.Empty),
      "Create a " + CustomTransverseReinforcementLayoutGoo.Description + " for a " + TransverseReinforcementGoo.Description,
        Ribbon.CategoryName.Name(),
        Ribbon.SubCategoryName.Cat3()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];
      if (LengthUnit.ToString() == _selectedItems[i])
        return;

      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      int i = 0;
      Params.Input[i++].Name = "Start Pos x [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "End Pos x [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Diameter [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Spacing [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Cover [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Start Pos x [" + unitAbbreviation + "]", "PxS", "Start Position where this Rebar Spacing Groups begins on Beam (beam local x-axis)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set positions as percentage (-0.5 => 50%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("End Pos x [" + unitAbbreviation + "]", "PxE", "End Position where this Rebar Spacing Groups begins on Beam (beam local x-axis)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set positions as percentage (-0.5 => 50 %)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Transverse rebar diameter", GH_ParamAccess.item);
      pManager.AddGenericParameter("Spacing [" + unitAbbreviation + "]", "S", "The centre/centre distance between rebars in this group (along beam local x-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new CustomTransverseReinforcementParam());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      IQuantity start = Input.LengthOrRatio(this, DA, 0, LengthUnit);
      IQuantity end = Input.LengthOrRatio(this, DA, 1, LengthUnit);
      Length dia = (Length)Input.UnitNumber(this, DA, 2, LengthUnit);
      Length spacing = (Length)Input.UnitNumber(this, DA, 3, LengthUnit);
      Length cov = (Length)Input.UnitNumber(this, DA, 4, LengthUnit);

      Output.SetItem(this, DA, 0, new CustomTransverseReinforcementLayoutGoo(new CustomTransverseReinforcementLayout(start, end, dia, spacing, cov)));
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
