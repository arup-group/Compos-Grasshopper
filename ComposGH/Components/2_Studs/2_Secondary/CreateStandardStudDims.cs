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
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateStandardStudDimensions : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c97c7e52-7aa3-438f-900a-33f6ca889b3c");
    public CreateStandardStudDimensions()
      : base("Standard Stud Dimensions", "StdStudDim", "Create Standard Stud Dimensions for a Compos Stud",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardStudDims;
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
            "Unit"
    });
    private bool First = true;
    private ForceUnit ForceUnit = Units.ForceUnit;
    private StandardSize StdSize = ComposAPI.StandardSize.D19mmH100mm;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // spacing
        DropdownItems.Add(Enum.GetValues(typeof(StandardSize)).Cast<StandardSize>()
            .Select(x => x.ToString()).ToList());
        for (int i = 0; i < DropdownItems[0].Count; i++)
          DropdownItems[0][i] = DropdownItems[0][i].Replace("D", "Ø").Replace("mmH", "/");

        SelectedItems.Add(StdSize.ToString().Replace("D", "Ø").Replace("mmH", "/"));

        // strength
        DropdownItems.Add(Units.FilteredForceUnits);
        SelectedItems.Add(ForceUnit.ToString());

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
        string sz = SelectedItems[i].Replace("Ø", "D").Replace("/", "mmH");
        StdSize = (StandardSize)Enum.Parse(typeof(StandardSize), sz);
      }
      else if (i == 1) // change is made to grade
      {
        ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), SelectedItems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      string sz = SelectedItems[0].Replace("Ø", "D").Replace("/", "mmH");
      StdSize = (StandardSize)Enum.Parse(typeof(StandardSize), sz);
      ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), SelectedItems[1]);

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
      IQuantity force = new Force(0, ForceUnit);
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
      DA.SetData(0, new StudDimensionsGoo(new StudDimensions(StdSize, GetInput.Force(this, DA, 0, ForceUnit))));
    }

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
      IQuantity force = new Force(0, ForceUnit);
      string forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

      Params.Input[0].Name = "Strength [" + forceunitAbbreviation + "]";
    }
    #endregion
  }
}
