using System;
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

namespace ComposGH.Components {
  public class CreateStudSpecEN : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("467bb1c3-ea5e-4c63-a012-d088158fb173");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardStudSpecsEN;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public CreateStudSpecEN() : base("StandardEN" + StudSpecificationGoo.Name.Replace(" ", string.Empty),
      "StudSpecsEN",
      "Look up a Standard EN " + StudSpecificationGoo.Description + " for a " + StudGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];
      if (LengthUnit.ToString() == _selectedItems[i]) {
        return;
      }

      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[0].Name = "No Stud Zone Start [" + unitAbbreviation + "]";
      Params.Input[1].Name = "No Stud Zone End [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Rebar Pos [" + unitAbbreviation + "]";
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

      pManager.AddGenericParameter("No Stud Zone Start [" + unitAbbreviation + "]",
          "NSZS", "Length of zone without shear studs at the start of the beam (default = 0)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter("No Stud Zone End [" + unitAbbreviation + "]",
          "NSZE", "Length of zone without shear studs at the end of the beam (default = 0)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter("Rebar Pos [" + unitAbbreviation + "]",
          "RbP", "Reinforcement position distance below underside of stud head (default = 30mm)", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Welded", "Wld", "Welded through profiled steel sheeting", GH_ParamAccess.item, true);
      pManager.AddBooleanParameter("NCCI Limits", "NCCI", "Use NCCI limits on minimum percentage of interaction if applicable. " +
          "(Imposed load criteria will not be verified)", GH_ParamAccess.item, false);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new StudSpecificationParam(), StudSpecificationGoo.Name, StudSpecificationGoo.NickName, "EN " + StudSpecificationGoo.Description + " for a " + StudGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      // get default length inputs used for all cases
      IQuantity noStudZoneStart = Length.Zero;
      if (Params.Input[0].Sources.Count > 0) {
        noStudZoneStart = Input.LengthOrRatio(this, DA, 0, LengthUnit, true);
      }
      IQuantity noStudZoneEnd = Length.Zero;
      if (Params.Input[1].Sources.Count > 0) {
        noStudZoneEnd = Input.LengthOrRatio(this, DA, 1, LengthUnit, true);
      }

      // get rebar position
      var rebarPos = new Length(30, LengthUnit.Millimeter);
      if (Params.Input[2].Sources.Count > 0) {
        rebarPos = (Length)Input.UnitNumber(this, DA, 2, LengthUnit, true);
      }
      bool welded = true;
      DA.GetData(3, ref welded);
      bool ncci = false;
      DA.GetData(4, ref ncci);
      var specEN = new StudSpecification(
          noStudZoneStart, noStudZoneEnd, rebarPos, welded, ncci);
      Output.SetItem(this, DA, 0, new StudSpecificationGoo(specEN));
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
