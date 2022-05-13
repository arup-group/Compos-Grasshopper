using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateStandardStudDimensionsEN : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f012d853-af53-45b9-b080-723661b9c2ad");
    public CreateStandardStudDimensionsEN()
      : base("Standard Stud EN Dimensions", "StdStudDimEN", "Create Standard Stud Dimensions to EN1994-1-1 for a Compos Stud",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardStudDimsEN;
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
            "Standard Size",
            "Grade",
            "Force Unit",
            "Length Unit"
    });
    List<string> StandardSize = new List<string>(new string[]
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

    private bool First = true;
    private PressureUnit StressUnit = Units.StressUnit;
    private LengthUnit LengthUnit = Units.LengthUnitSection;
    private StandardStudGrade StdGrd = StandardStudGrade.SD1_EN13918;
    private StandardStudSize StdSize = ComposAPI.StandardStudSize.D19mmH100mm;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // spacing
        DropdownItems.Add(StandardSize);
        SelectedItems.Add(StdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

        // grade
        DropdownItems.Add(Enum.GetValues(typeof(StandardStudGrade)).Cast<StandardStudGrade>().Select(x => x.ToString()).ToList());
        SelectedItems.Add(StdGrd.ToString());

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

      if (i == 0) // change is made to size
      {
        if (SelectedItems[0] != StandardSize[0])
        {
          string sz = SelectedItems[i].Replace("Ø", "D").Replace("/", "mmH");
          StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
          if (DropdownItems.Count > 3)
          {
            // remove length dropdown
            DropdownItems.RemoveAt(DropdownItems.Count - 1);
            SelectedItems.RemoveAt(SelectedItems.Count - 1);
            ModeChangeClicked();
          }
        }
        else if (DropdownItems.Count < 4)
        {
          // add length dropdown
          DropdownItems.Add(Units.FilteredLengthUnits);
          SelectedItems.Add(LengthUnit.ToString());
          ModeChangeClicked();
        }
      }
      if (i == 1) // change is made to grade
      {
        StdGrd = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), SelectedItems[i]);
      }
      if (i == 2) // change is made to grade
      {
        StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[i]);
      }
      if (i == 3) // change is made to length
      {
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);
      }

        // update name of inputs (to display unit on sliders)
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      if (SelectedItems[0] != StandardSize[0])
      {
        string sz = SelectedItems[0].Replace("Ø", "D").Replace("/", "mmH");
        StdSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), sz);
        StdGrd = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), SelectedItems[1]);
      }
      else
      {
        ModeChangeClicked();
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[3]);
      }

      StressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[2]);

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
      IQuantity stress = new Pressure(0, StressUnit);
      string stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Grade [" + stressUnitAbbreviation + "]", "fu", "(Optional) Custom Stud Steel Grade", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Stud Dims", "Sdm", "Compos Shear Stud Dimensions", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      if (SelectedItems[0] == StandardSize[0]) // custom size
      {
        Length dia = GetInput.Length(this, DA, 0, LengthUnit);
        Length h = GetInput.Length(this, DA, 1, LengthUnit);

        if (this.Params.Input[2].Sources.Count > 0)
        {
          SelectedItems[1] = "Custom";
          Pressure strengthS = GetInput.Stress(this, DA, 2, StressUnit);
          DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthS)));
        }
        else
          DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, StdGrd)));
      }
      else
      {
        if (this.Params.Input[0].Sources.Count > 0)
        {
          SelectedItems[1] = "Custom";
          Pressure strengthS = GetInput.Stress(this, DA, 0, StressUnit);
          DA.SetData(0, new StudDimensionsGoo(new StudDimensions(StdSize, strengthS)));
        }
        else
          DA.SetData(0, new StudDimensionsGoo(new StudDimensions(StdSize, StdGrd)));
      }
    }

    #region update input params
    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (SelectedItems[0] == StandardSize[0]) // custom size
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
      if (SelectedItems[0] != StandardSize[0]) // standard size
      {
        if (this.Params.Input.Count == 1)
          return;

        // remove params
        Params.UnregisterInputParameter(Params.Input[0], true);
        Params.UnregisterInputParameter(Params.Input[0], true);
      }
    }
    #endregion

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
      IQuantity stress = new Pressure(0, StressUnit);
      string stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

      if (SelectedItems[0] == StandardSize[0]) // custom size
      {
        IQuantity length = new Length(0, LengthUnit);
        string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

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
      if (SelectedItems[0] != StandardSize[0]) // standard size
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
