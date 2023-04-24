using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components {
  public class CreateStandardStudDimensionsEN : GH_OasysDropDownComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f012d853-af53-45b9-b080-723661b9c2ad");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardStudDimsEN;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    private List<string> StandardSizes = new List<string>(new string[] {
      "Custom",
      "Ø13/65mm",
      "Ø16/75mm",
      "Ø19/75mm",
      "Ø19/100mm",
      "Ø19/125mm",
      "Ø22/100mm",
      "Ø25/100mm"
    });

    private StandardStudGrade StdGrd = StandardStudGrade.SD1_EN13918;

    private StandardStudSize StdSize = StandardStudSize.D19mmH100mm;

    private PressureUnit StressUnit = DefaultUnits.MaterialStrengthUnit;

    public CreateStandardStudDimensionsEN() : base("StandardEN" + StudDimensionsGoo.Name.Replace(" ", string.Empty),
      "StudDimsEN",
      "Look up a Standard " + StudDimensionsGoo.Description + " for a " + StudGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2()) { Hidden = true; } // sets the initial state of the component to hidden

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0) // change is made to size
      {
        if (_selectedItems[0] != StandardSizes[0]) {
          string sz = _selectedItems[i].Replace("Ø", "D").Replace("/", "mmH");
          StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
          if (_dropDownItems.Count > 3) {
            // remove length dropdown
            _dropDownItems.RemoveAt(_dropDownItems.Count - 1);
            _selectedItems.RemoveAt(_selectedItems.Count - 1);
            _spacerDescriptions.RemoveAt(_spacerDescriptions.Count - 1);
            ModeChangeClicked();
          }
        } else if (_dropDownItems.Count < 4) {
          // add length dropdown
          _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          _selectedItems.Add(Length.GetAbbreviation(LengthUnit));
          _spacerDescriptions.Add("Length Unit");
          ModeChangeClicked();
        }
      }
      if (i == 1) {
        // change is made to grade
        StdGrd = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), _selectedItems[i]);
      }
      if (i == 2) {
        // change is made to grade
        StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
      }
      if (i == 3) {
        // change is made to length
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      }
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(StressUnit);

      if (_selectedItems[0] == StandardSizes[0]) // custom size
      {
        string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

        Params.Input[0].Name = "Diameter [" + unitAbbreviation + "]";
        Params.Input[0].NickName = "Ø";
        Params.Input[0].Description = "Diameter of stud head";
        Params.Input[0].Optional = false;

        Params.Input[1].Name = "Height [" + unitAbbreviation + "]";
        Params.Input[1].NickName = "H";
        Params.Input[1].Description = "Height of stud";
        Params.Input[1].Optional = false;

        Params.Input[2].Name = "Grade [" + stressUnitAbbreviation + "]";
        Params.Input[2].NickName = "fu";
        Params.Input[2].Description = "Stud Steel Grade";
        Params.Input[2].Optional = true;
      }
      if (_selectedItems[0] != StandardSizes[0]) // standard size
      {
        Params.Input[0].Name = "Grade [" + stressUnitAbbreviation + "]";
        Params.Input[0].NickName = "fu";
        Params.Input[0].Description = "(Optional) Custom Stud Steel Grade";
        Params.Input[0].Optional = true;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
            "Standard Size",
            "Grade",
            "Force Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // spacing
      _dropDownItems.Add(StandardSizes);
      _selectedItems.Add(StdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

      // grade
      _dropDownItems.Add(Enum.GetValues(typeof(StandardStudGrade)).Cast<StandardStudGrade>().Select(x => x.ToString()).ToList());
      _selectedItems.Add(StdGrd.ToString());

      // strength
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(Pressure.GetAbbreviation(StressUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(StressUnit);

      pManager.AddGenericParameter("Grade [" + stressUnitAbbreviation + "]", "fu", "(Optional) Custom Stud Steel Grade", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new StudDimensionsParam(), StudDimensionsGoo.Name, StudDimensionsGoo.NickName, "EN " + StudDimensionsGoo.Description + " for a " + StudGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      if (_selectedItems[0] == StandardSizes[0]) // custom size
      {
        var dia = (Length)Input.UnitNumber(this, DA, 0, LengthUnit);
        var h = (Length)Input.UnitNumber(this, DA, 1, LengthUnit);

        if (Params.Input[2].Sources.Count > 0) {
          _selectedItems[1] = "Custom";
          var strengthS = (Pressure)Input.UnitNumber(this, DA, 2, StressUnit);
          Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthS)));
        } else {
          Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(dia, h, StdGrd)));
        }
      } else {
        if (Params.Input[0].Sources.Count > 0) {
          _selectedItems[1] = "Custom";
          var strengthS = (Pressure)Input.UnitNumber(this, DA, 0, StressUnit);
          Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(StdSize, strengthS)));
        } else {
          Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(StdSize, StdGrd)));
        }
      }
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] != StandardSizes[0]) {
        string sz = _selectedItems[0].Replace("Ø", "D").Replace("/", "mmH");
        StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
        StdGrd = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), _selectedItems[1]);
      } else {
        ModeChangeClicked();
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[3]);
      }

      StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }

    private void ModeChangeClicked() {
      RecordUndoEvent("Changed Parameters");

      if (_selectedItems[0] == StandardSizes[0]) // custom size
      {
        if (Params.Input.Count == 3) {
          return;
        }

        // temp remove input 1
        IGH_Param fu = Params.Input[0];
        Params.UnregisterInputParameter(Params.Input[0], false);

        // add new params
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());

        // re-add fu
        Params.RegisterInputParam(fu);
      }
      if (_selectedItems[0] != StandardSizes[0]) // standard size
      {
        if (Params.Input.Count == 1) {
          return;
        }

        // remove params
        Params.UnregisterInputParameter(Params.Input[0], true);
        Params.UnregisterInputParameter(Params.Input[0], true);
      }
    }
  }
}
