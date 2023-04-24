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
  public class CreateBeamSection : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("de792051-ae6a-4249-8699-7ea0cfe8c528");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.BeamSection;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public CreateBeamSection() : base("Create" + BeamSectionGoo.Name.Replace(" ", string.Empty),
      BeamSectionGoo.Name.Replace(" ", string.Empty),
      "Create a " + BeamSectionGoo.Description + " for a " + BeamGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[1].Name = "Start [" + unitAbbreviation + "]";
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

      pManager.AddGenericParameter(BeamSectionGoo.Name, BeamSectionGoo.NickName, BeamSectionGoo.Description + " parameter or a text string in the format of either 'CAT IPE IPE200', 'STD I(cm) 20. 19. 8.5 1.27' or 'STD GI 400 300 250 12 25 20'", GH_ParamAccess.item);
      pManager.AddGenericParameter("Start [" + unitAbbreviation + "]", "Px", "(Optional) Start Position of this profile (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Taper Next", "Tp", "Taper to next (default = false)", GH_ParamAccess.item, false);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new BeamSectionParam());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      string profile = Helpers.CustomInput.BeamSection(this, DA, 0, false);
      profile = profile.Trim();

      IQuantity start = new Ratio(0, RatioUnit.Percent);
      if (Params.Input[1].Sources.Count > 0) {
        start = Input.LengthOrRatio(this, DA, 1, LengthUnit);
      }

      bool taper = false;
      if (DA.GetData(2, ref taper)) {
        if (taper & profile.StartsWith("CAT")) {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Catalogue profiles cannot taper - use a custom welded section instead");
        }
      }

      var beamSection = new BeamSection(profile) {
        StartPosition = start,
        TaperedToNext = taper
      };
      Output.SetItem(this, DA, 0, new BeamSectionGoo(beamSection));
    }

    protected override void UpdateUIFromSelectedItems() {
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
