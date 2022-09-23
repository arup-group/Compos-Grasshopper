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
      if (SelectedItems[0] == StandardSizes[0]) // custom size
      {
        Length dia = (Length)Input.UnitNumber(this, DA, 0, LengthUnit);
        Length h = (Length)Input.UnitNumber(this, DA, 1, LengthUnit);

        if (this.Params.Input[2].Sources.Count > 0)
        {
          SelectedItems[1] = "Custom";
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
          SelectedItems[1] = "Custom";
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

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] {
            "Standard Size",
            "Grade",
            "Force Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // spacing
      this.DropDownItems.Add(this.StandardSizes);
      this.SelectedItems.Add(this.StdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

      // grade
      this.DropDownItems.Add(Enum.GetValues(typeof(StandardStudGrade)).Cast<StandardStudGrade>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(this.StdGrd.ToString());

      // strength
      this.DropDownItems.Add(FilteredUnits.FilteredStressUnits);
      this.SelectedItems.Add(this.StressUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to size
      {
        if (this.SelectedItems[0] != this.StandardSizes[0])
        {
          string sz = this.SelectedItems[i].Replace("Ø", "D").Replace("/", "mmH");
          this.StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
          if (this.DropDownItems.Count > 3)
          {
            // remove length dropdown
            this.DropDownItems.RemoveAt(this.DropDownItems.Count - 1);
            this.SelectedItems.RemoveAt(this.SelectedItems.Count - 1);
            this.SpacerDescriptions.RemoveAt(this.SpacerDescriptions.Count - 1);
            this.ModeChangeClicked();
          }
        }
        else if (this.DropDownItems.Count < 4)
        {
          // add length dropdown
          this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
          this.SelectedItems.Add(this.LengthUnit.ToString());
          this.SpacerDescriptions.Add("Length Unit");
          this.ModeChangeClicked();
        }
      }
      if (i == 1) // change is made to grade
        this.StdGrd = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), this.SelectedItems[i]);

      if (i == 2) // change is made to grade
        this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[i]);

      if (i == 3) // change is made to length
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      if (this.SelectedItems[0] != this.StandardSizes[0])
      {
        string sz = this.SelectedItems[0].Replace("Ø", "D").Replace("/", "mmH");
        this.StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
        this.StdGrd = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), this.SelectedItems[1]);
      }
      else
      {
        this.ModeChangeClicked();
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[3]);
      }

      this.StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }

    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (this.SelectedItems[0] == this.StandardSizes[0]) // custom size
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
      if (this.SelectedItems[0] != this.StandardSizes[0]) // standard size
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

      if (SelectedItems[0] == StandardSizes[0]) // custom size
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
      if (SelectedItems[0] != StandardSizes[0]) // standard size
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
