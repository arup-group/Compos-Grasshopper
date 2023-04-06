using System;
using System.Linq;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysUnits;
using OasysUnits.Units;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysGH;

namespace ComposGH.Components
{
  public class CreateStandardStudDimensionsEN : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f012d853-af53-45b9-b080-723661b9c2ad");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.StandardStudDimsEN;
    public CreateStandardStudDimensionsEN()
      : base("StandardEN" + StudDimensionsGoo.Name.Replace(" ", string.Empty),
          "StudDimsEN",
          "Look up a Standard " + StudDimensionsGoo.Description + " for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string stressUnitAbbreviation = Pressure.GetAbbreviation(StressUnit);

      pManager.AddGenericParameter("Grade [" + stressUnitAbbreviation + "]", "fu", "(Optional) Custom Stud Steel Grade", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new StudDimensionsParam(), StudDimensionsGoo.Name, StudDimensionsGoo.NickName, "EN " + StudDimensionsGoo.Description + " for a " + StudGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (_selectedItems[0] == StandardSizes[0]) // custom size
      {
        Length dia = (Length)Input.UnitNumber(this, DA, 0, LengthUnit);
        Length h = (Length)Input.UnitNumber(this, DA, 1, LengthUnit);

        if (this.Params.Input[2].Sources.Count > 0)
        {
          _selectedItems[1] = "Custom";
          Pressure strengthS = (Pressure)Input.UnitNumber(this, DA, 2, StressUnit);
          Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthS)));
        }
        else
          Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(dia, h, StdGrd)));
      }
      else
      {
        if (this.Params.Input[0].Sources.Count > 0)
        {
          _selectedItems[1] = "Custom";
          Pressure strengthS = (Pressure)Input.UnitNumber(this, DA, 0, StressUnit);
          Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(StdSize, strengthS)));
        }
        else
          Output.SetItem(this, DA, 0, new StudDimensionsGoo(new StudDimensions(StdSize, StdGrd)));
      }
    }

    #region Custom UI
    List<string> StandardSizes = new List<string>(new string[]
    {
            "Custom",
            "Ø13/65mm",
            "Ø16/75mm",
            "Ø19/75mm",
            "Ø19/100mm",
            "Ø19/125mm",
            "Ø22/100mm",
            "Ø25/100mm"
    });
    private PressureUnit StressUnit = DefaultUnits.MaterialStrengthUnit;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;
    private StandardStudGrade StdGrd = StandardStudGrade.SD1_EN13918;
    private StandardStudSize StdSize = StandardStudSize.D19mmH100mm;

    protected override void InitialiseDropdowns()
    {
      this._spacerDescriptions = new List<string>(new string[] {
            "Standard Size",
            "Grade",
            "Force Unit" });

      this._dropDownItems = new List<List<string>>();
      this._selectedItems = new List<string>();

      // spacing
      this._dropDownItems.Add(this.StandardSizes);
      this._selectedItems.Add(this.StdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

      // grade
      this._dropDownItems.Add(Enum.GetValues(typeof(StandardStudGrade)).Cast<StandardStudGrade>().Select(x => x.ToString()).ToList());
      this._selectedItems.Add(this.StdGrd.ToString());

      // strength
      this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      this._selectedItems.Add(Pressure.GetAbbreviation(this.StressUnit));

      this._isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this._selectedItems[i] = this._dropDownItems[i][j];

      if (i == 0) // change is made to size
      {
        if (this._selectedItems[0] != this.StandardSizes[0])
        {
          string sz = this._selectedItems[i].Replace("Ø", "D").Replace("/", "mmH");
          this.StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
          if (this._dropDownItems.Count > 3)
          {
            // remove length dropdown
            this._dropDownItems.RemoveAt(this._dropDownItems.Count - 1);
            this._selectedItems.RemoveAt(this._selectedItems.Count - 1);
            this._spacerDescriptions.RemoveAt(this._spacerDescriptions.Count - 1);
            this.ModeChangeClicked();
          }
        }
        else if (this._dropDownItems.Count < 4)
        {
          // add length dropdown
          this._dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
          this._selectedItems.Add(Length.GetAbbreviation(this.LengthUnit));
          this._spacerDescriptions.Add("Length Unit");
          this.ModeChangeClicked();
        }
      }
      if (i == 1) // change is made to grade
        this.StdGrd = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), this._selectedItems[i]);

      if (i == 2) // change is made to grade
        this.StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this._selectedItems[i]);

      if (i == 3) // change is made to length
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this._selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      if (this._selectedItems[0] != this.StandardSizes[0])
      {
        string sz = this._selectedItems[0].Replace("Ø", "D").Replace("/", "mmH");
        this.StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
        this.StdGrd = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), this._selectedItems[1]);
      }
      else
      {
        this.ModeChangeClicked();
        this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this._selectedItems[3]);
      }

      this.StressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this._selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }

    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (this._selectedItems[0] == this.StandardSizes[0]) // custom size
      {
        if (this.Params.Input.Count == 3)
          return;

        // temp remove input 1
        IGH_Param fu = Params.Input[0];
        Params.UnregisterInputParameter(Params.Input[0], false);

        // add new params
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());

        // re-add fu
        Params.RegisterInputParam(fu);

      }
      if (this._selectedItems[0] != this.StandardSizes[0]) // standard size
      {
        if (this.Params.Input.Count == 1)
          return;

        // remove params
        Params.UnregisterInputParameter(Params.Input[0], true);
        Params.UnregisterInputParameter(Params.Input[0], true);
      }
    }

    public override void VariableParameterMaintenance()
    {
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
    #endregion
  }
}
