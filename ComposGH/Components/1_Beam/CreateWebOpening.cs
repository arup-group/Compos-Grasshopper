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

namespace ComposGH.Components
{
  public class CreateWebOpening : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("084fa2ab-d50e-4213-8f44-2affc9f41752");
    public CreateWebOpening()
      : base("Web Opening", "WebOpen", "Create Web Opening for a Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.WebOpening;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // type
        dropdownitems.Add(Enum.GetValues(typeof(ComposWebOpening.WebOpeningShape)).Cast<ComposWebOpening.WebOpeningShape>()
            .Select(x => x.ToString()).ToList());
        selecteditems.Add(ComposWebOpening.WebOpeningShape.Rectangular.ToString());

        // length
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0)
      {
        if (selecteditems[i] == openingType.ToString())
          return;
        openingType = (ComposWebOpening.WebOpeningShape)Enum.Parse(typeof(ComposWebOpening.WebOpeningShape), selecteditems[i]);
        ModeChangeClicked();
      }
      else if (i == 1) // change is made to length unit
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
      openingType = (ComposWebOpening.WebOpeningShape)Enum.Parse(typeof(ComposWebOpening.WebOpeningShape), selecteditems[0]);
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);
      CreateAttributes();
      ModeChangeClicked();
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
            "Shape",
            "Unit"
    });

    private bool first = true;
    private ComposWebOpening.WebOpeningShape openingType = ComposWebOpening.WebOpeningShape.Rectangular;
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Web Opening Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Web Opening Height", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x [" + unitAbbreviation + "]", "Px", "Position of opening Centroid from Start of Beam (beam local x-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos z [" + unitAbbreviation + "]", "Pz", "Position of opening Centroid from Top of Beam (beam local z-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Stiffeners", "WS", "(Optional) Web Opening Stiffeners", GH_ParamAccess.item);
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("WebOpening", "WO", "Web Opening for a Compos Beam", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length width_dia = GetInput.Length(this, DA, 0, lengthUnit);
      Length height = (openingType == ComposWebOpening.WebOpeningShape.Rectangular) ? GetInput.Length(this, DA, 1, lengthUnit) : Length.Zero;
      Length x = GetInput.Length(this, DA, (openingType == ComposWebOpening.WebOpeningShape.Rectangular) ? 2 : 1, lengthUnit);
      Length z = GetInput.Length(this, DA, (openingType == ComposWebOpening.WebOpeningShape.Rectangular) ? 3 : 2, lengthUnit);
      WebOpeningStiffeners stiff = GetInput.WebOpeningStiffeners(this, DA, (openingType == ComposWebOpening.WebOpeningShape.Rectangular) ? 4 : 3, true);

      switch (openingType)
      {
        case ComposWebOpening.WebOpeningShape.Rectangular:
          DA.SetData(0, new ComposWebOpeningGoo(new ComposWebOpening(width_dia, height, x, z, stiff)));
          break;

        case ComposWebOpening.WebOpeningShape.Circular:
          DA.SetData(0, new ComposWebOpeningGoo(new ComposWebOpening(width_dia, x, z, stiff)));
          break;
      }
    }
    #region update input params
    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (openingType == ComposWebOpening.WebOpeningShape.Rectangular)
      {
        if (this.Params.Input.Count == 5)
          return;

        // remove parameters until first
        IGH_Param stiff = Params.Input[3];
        Params.UnregisterInputParameter(Params.Input[3], false);
        IGH_Param z = Params.Input[2];
        Params.UnregisterInputParameter(Params.Input[2], false);
        IGH_Param x = Params.Input[1];
        Params.UnregisterInputParameter(Params.Input[1], false);

        // add new param at idd 1
        Params.RegisterInputParam(new Param_GenericObject());

        // re-add other params
        Params.RegisterInputParam(x);
        Params.RegisterInputParam(z);
        Params.RegisterInputParam(stiff);
      }
      if (openingType == ComposWebOpening.WebOpeningShape.Circular)
      {
        if (this.Params.Input.Count == 4)
          return;

        // remove height param
        Params.UnregisterInputParameter(Params.Input[1], true);
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
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      if (openingType == ComposWebOpening.WebOpeningShape.Rectangular)
      {
        int i = 0;
        Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "B";
        Params.Input[i].Description = "Web Opening Width";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Height [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "H";
        Params.Input[i].Description = "Web Opening Height";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Pos x [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Px";
        Params.Input[i].Description = "Position of opening Centroid from Start of Beam (beam local x-axis)";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Pos z [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Pz";
        Params.Input[i].Description = "Position of opening Centroid from Top of Beam (beam local z-axis)";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Stiffeners";
        Params.Input[i].NickName = "WS";
        Params.Input[i].Description = "(Optional) Web Opening Stiffeners";
        Params.Input[i].Optional = true;
      }
      if (openingType == ComposWebOpening.WebOpeningShape.Circular)
      {
        int i = 0;
        Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Ø";
        Params.Input[i].Description = "Web Opening Diameter";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Pos x [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Px";
        Params.Input[i].Description = "Position of opening Centroid from Start of Beam (beam local x-axis)";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Pos z [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Pz";
        Params.Input[i].Description = "Position of opening Centroid from Top of Beam (beam local z-axis)";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Stiffeners";
        Params.Input[i].NickName = "WS";
        Params.Input[i].Description = "(Optional) Web Opening Stiffeners";
        Params.Input[i].Optional = true;
      }
    }
    #endregion
  }
}
