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
  public class CreateStudSpecAZNZHK : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("1ef0e7f8-bd0a-4a10-b6ed-009745062628");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardStudSpecs;
    public CreateStudSpecAZNZHK()
      : base("Create" + StudSpecificationGoo.Name.Replace(" ", string.Empty),
          StudSpecificationGoo.Name.Replace(" ", string.Empty),
          "Create a " + StudSpecificationGoo.Description + " applicable for AS/NZ or HK codes, for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2()) { Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("No Stud Zone Start [" + unitAbbreviation + "]",
          "NSZS", "Length of zone without shear studs at the start of the beam (default = 0)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter("No Stud Zone End [" + unitAbbreviation + "]",
          "NSZE", "Length of zone without shear studs at the end of the beam (default = 0)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Welded", "Wld", "Welded through profiled steel sheeting", GH_ParamAccess.item, true);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new StudSpecificationParam(), StudSpecificationGoo.Name, StudSpecificationGoo.NickName, StudSpecificationGoo.Description + " applicable for AS/NZ or HK codes, for a " + StudGoo.Description, GH_ParamAccess.item);
    }
    #endregion

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

      bool welded = true;
      DA.GetData(2, ref welded);

      var specOther = new StudSpecification(noStudZoneStart, noStudZoneEnd, welded);
      Output.SetItem(this, DA, 0, new StudSpecificationGoo(specOther));
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] { "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }
    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];
      if (LengthUnit.ToString() == _selectedItems[i]) {
        return;
      }

      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[0].Name = "No Stud Zone Start [" + unitAbbreviation + "]";
      Params.Input[1].Name = "No Stud Zone End [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
