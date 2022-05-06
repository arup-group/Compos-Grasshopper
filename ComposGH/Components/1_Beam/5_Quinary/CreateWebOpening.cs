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
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.WebOpening;
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
            "Shape",
            "Unit"
    });

    private bool First = true;
    private WebOpeningShape OpeningType = WebOpeningShape.Rectangular;
    private LengthUnit LengthUnit = Units.LengthUnitSection;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // type
        DropdownItems.Add(Enum.GetValues(typeof(WebOpeningShape)).Cast<WebOpeningShape>()
            .Select(x => x.ToString()).ToList());
        SelectedItems.Add(WebOpeningShape.Rectangular.ToString());

        // length
        DropdownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (i == 0)
      {
        if (SelectedItems[i] == OpeningType.ToString())
          return;
        OpeningType = (WebOpeningShape)Enum.Parse(typeof(WebOpeningShape), SelectedItems[i]);
        ModeChangeClicked();
      }
      else if (i == 1) // change is made to length unit
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
      OpeningType = (WebOpeningShape)Enum.Parse(typeof(WebOpeningShape), SelectedItems[0]);
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);
      CreateAttributes();
      ModeChangeClicked();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, LengthUnit);
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
      Length width_dia = GetInput.Length(this, DA, 0, LengthUnit);
      Length height = (OpeningType == WebOpeningShape.Rectangular) ? GetInput.Length(this, DA, 1, LengthUnit) : Length.Zero;
      Length x = GetInput.Length(this, DA, (OpeningType == WebOpeningShape.Rectangular) ? 2 : 1, LengthUnit);
      Length z = GetInput.Length(this, DA, (OpeningType == WebOpeningShape.Rectangular) ? 3 : 2, LengthUnit);
      WebOpeningStiffenersGoo stiff = (WebOpeningStiffenersGoo)GetInput.GenericGoo<WebOpeningStiffenersGoo>(this, DA, (OpeningType == WebOpeningShape.Rectangular) ? 4 : 3);

      switch (OpeningType)
      {
        case WebOpeningShape.Rectangular:
          DA.SetData(0, new WebOpeningGoo(new WebOpening(width_dia, height, x, z, (stiff == null) ? null : stiff.Value)));
          break;

        case WebOpeningShape.Circular:
          DA.SetData(0, new WebOpeningGoo(new WebOpening(width_dia, x, z, (stiff == null) ? null : stiff.Value)));
          break;
      }
    }
    #region update input params
    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (OpeningType == WebOpeningShape.Rectangular)
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
      if (OpeningType == WebOpeningShape.Circular)
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
      IQuantity length = new Length(0, LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      if (OpeningType == WebOpeningShape.Rectangular)
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
      if (OpeningType == WebOpeningShape.Circular)
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
