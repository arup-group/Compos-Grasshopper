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

namespace ComposGH.Components
{
  public class StandardStudDimensions : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c97c7e52-7aa3-438f-900a-33f6ca889b3c");
    public StandardStudDimensions()
      : base("Standard Stud Dimensions", "StdStudDim", "Create Standard Stud Dimensions for a Compos Stud",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardStudDims;
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
        dropdownitems.Add(Enum.GetValues(typeof(StudDimensions.StandardSize)).Cast<StudDimensions.StandardSize>()
            .Select(x => x.ToString()).ToList());
        for (int i = 0; i < dropdownitems[0].Count; i++)
          dropdownitems[0][i] = dropdownitems[0][i].Replace("D", "Ø").Replace("mmH", "/");

        selecteditems.Add(stdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

        // strength
        dropdownitems.Add(Units.FilteredForceUnits);
        selecteditems.Add(forceUnit.ToString());

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
        string sz = selecteditems[i].Replace("Ø", "D").Replace("/", "mmH");
        stdSize = (StudDimensions.StandardSize)Enum.Parse(typeof(StudDimensions.StandardSize), sz);
      }
      else if (i == 1) // change is made to grade
      {
        forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      string sz = selecteditems[0].Replace("Ø", "D").Replace("/", "mmH");
      stdSize = (StudDimensions.StandardSize)Enum.Parse(typeof(StudDimensions.StandardSize), sz);
      forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[1]);

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
            "Unit"
    });
    List<string> standardSize = new List<string>(new string[]
    {
            "Ø13/H65mm",
            "Ø16/H70mm",
            "Ø16/H75mm",
            "Ø19/H75mm",
            "Ø19/H95mm",
            "Ø19/H100mm",
            "Ø19/H125mm",
            "Ø22/H95mm",
            "Ø22/H100mm",
            "Ø25/H95mm",
            "Ø25/H100mm"
    });
    private bool first = true;
    private ForceUnit forceUnit = Units.ForceUnit;
    private StudDimensions.StandardSize stdSize = StudDimensions.StandardSize.D19mmH100mm;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity force = new Force(0, forceUnit);
      string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Grade [" + forceunitAbbreviation + "]", "fu", "Stud Character strength", GH_ParamAccess.item);
      pManager[0].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Stud Dims", "Sdm", "Compos Shear Stud Dimensions", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      DA.SetData(0, new StudDimensionsGoo(new StudDimensions(stdSize, GetInput.Force(this, DA, 0, forceUnit))));
    }

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
      IQuantity force = new Force(0, forceUnit);
      string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

      Params.Input[0].Name = "Strength [" + forceunitAbbreviation + "]";
    }
    #endregion
  }
}
