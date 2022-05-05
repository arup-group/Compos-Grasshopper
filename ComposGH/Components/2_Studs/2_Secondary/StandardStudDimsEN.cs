using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  public class StandardStudDimensionsEN : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("f012d853-af53-45b9-b080-723661b9c2ad");
    public StandardStudDimensionsEN()
      : base("Standard Stud EN Dimensions", "StdStudDimEN", "Create Standard Stud Dimensions to EN1994-1-1 for a Compos Stud",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardStudDimsEN;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // spacing
        dropdownitems.Add(standardSize);
        selecteditems.Add(stdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

        // grade
        dropdownitems.Add(Enum.GetValues(typeof(StandardGrade)).Cast<StandardGrade>().Select(x => x.ToString()).ToList());
        selecteditems.Add(stdGrd.ToString());

        // strength
        dropdownitems.Add(Units.FilteredStressUnits);
        selecteditems.Add(stressUnit.ToString());

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0) // change is made to size
      {
        if (selecteditems[0] != standardSize[0])
        {
          string sz = selecteditems[i].Replace("Ø", "D").Replace("/", "mmH");
          stdSize = (StandardSize)Enum.Parse(typeof(StandardSize), sz);
          if (dropdownitems.Count > 3)
          {
            // remove length dropdown
            dropdownitems.RemoveAt(dropdownitems.Count - 1);
            selecteditems.RemoveAt(selecteditems.Count - 1);
            ModeChangeClicked();
          }
        }
        else if (dropdownitems.Count < 4)
        {
          // add length dropdown
          dropdownitems.Add(Units.FilteredLengthUnits);
          selecteditems.Add(lengthUnit.ToString());
          ModeChangeClicked();
        }
      }
      if (i == 1) // change is made to grade
      {
        stdGrd = (StandardGrade)Enum.Parse(typeof(StandardGrade), selecteditems[i]);
      }
      if (i == 2) // change is made to grade
      {
        stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
      }
      if (i == 3) // change is made to length
      {
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);
      }

        // update name of inputs (to display unit on sliders)
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      if (selecteditems[0] != standardSize[0])
      {
        string sz = selecteditems[0].Replace("Ø", "D").Replace("/", "mmH");
        stdSize = (StandardSize)Enum.Parse(typeof(StandardSize), sz);
        stdGrd = (StandardGrade)Enum.Parse(typeof(StandardGrade), selecteditems[1]);
      }
      else
      {
        ModeChangeClicked();
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[3]);
      }

      stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[2]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 

    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Standard Size",
            "Grade",
            "Force Unit",
            "Length Unit"
    });
    List<string> standardSize = new List<string>(new string[]
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

    private bool first = true;
    private PressureUnit stressUnit = Units.StressUnit;
    private LengthUnit lengthUnit = Units.LengthUnitSection;
    private StandardGrade stdGrd = StandardGrade.SD1_EN13918;
    private StandardSize stdSize = StandardSize.D19mmH100mm;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity stress = new Pressure(0, stressUnit);
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
      if (selecteditems[0] == standardSize[0]) // custom size
      {
        Length dia = GetInput.Length(this, DA, 0, lengthUnit);
        Length h = GetInput.Length(this, DA, 1, lengthUnit);

        if (this.Params.Input[2].Sources.Count > 0)
        {
          selecteditems[1] = "Custom";
          Pressure strengthS = GetInput.Stress(this, DA, 2, stressUnit);
          DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthS)));
        }
        else
          DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, stdGrd)));
      }
      else
      {
        if (this.Params.Input[0].Sources.Count > 0)
        {
          selecteditems[1] = "Custom";
          Pressure strengthS = GetInput.Stress(this, DA, 0, stressUnit);
          DA.SetData(0, new StudDimensionsGoo(new StudDimensions(stdSize, strengthS)));
        }
        else
          DA.SetData(0, new StudDimensionsGoo(new StudDimensions(stdSize, stdGrd)));
      }
    }
    #region update input params
    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (selecteditems[0] == standardSize[0]) // custom size
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
      if (selecteditems[0] != standardSize[0]) // standard size
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
      Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

      UpdateUIFromSelectedItems();

      first = false;

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
      IQuantity stress = new Pressure(0, stressUnit);
      string stressUnitAbbreviation = string.Concat(stress.ToString().Where(char.IsLetter));

      if (selecteditems[0] == standardSize[0]) // custom size
      {
        IQuantity length = new Length(0, lengthUnit);
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
      if (selecteditems[0] != standardSize[0]) // standard size
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
