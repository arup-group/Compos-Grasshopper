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
  public class CreateWebOpening : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("084fa2ab-d50e-4213-8f44-2affc9f41752");
    public CreateWebOpening()
      : base("Create" + WebOpeningGoo.Name.Replace(" ", string.Empty),
          WebOpeningGoo.Name.Replace(" ", string.Empty),
          "Create a " + WebOpeningGoo.Description + " for a " + BeamGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.WebOpening;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Web Opening Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Web Opening Height", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x [" + unitAbbreviation + "]", "Px", "Position of opening Centroid from Start of Beam (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos z [" + unitAbbreviation + "]", "Pz", "Position of opening Centroid from Top of Beam (beam local z-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter(WebOpeningStiffenersGoo.Name, WebOpeningStiffenersGoo.NickName, "(Optional) " + WebOpeningStiffenersGoo.Description, GH_ParamAccess.item);
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(WebOpeningGoo.Name, WebOpeningGoo.NickName, WebOpeningGoo.Description + " for a " + BeamGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length width_dia = GetInput.Length(this, DA, 0, this.LengthUnit);
      
      int i = 1;

      Length height = Length.Zero;
      if (this.OpeningType == WebOpeningShape.Rectangular)
        height = GetInput.Length(this, DA, i++, this.LengthUnit);
      
      IQuantity x = GetInput.LengthOrRatio(this, DA, i++, this.LengthUnit);
      
      IQuantity z = GetInput.LengthOrRatio(this, DA, i++, this.LengthUnit);
      
      WebOpeningStiffenersGoo stiff = (WebOpeningStiffenersGoo)GetInput.GenericGoo<WebOpeningStiffenersGoo>(this, DA, i++);

      switch (this.OpeningType)
      {
        case WebOpeningShape.Rectangular:
          SetOutput.Item(this, DA, 0, new WebOpeningGoo(new WebOpening(width_dia, height, x, z, (stiff == null) ? null : stiff.Value)));
          break;

        case WebOpeningShape.Circular:
          SetOutput.Item(this, DA, 0, new WebOpeningGoo(new WebOpening(width_dia, x, z, (stiff == null) ? null : stiff.Value)));
          break;
      }
    }

    #region Custom UI
    private WebOpeningShape OpeningType = WebOpeningShape.Rectangular;
    private LengthUnit LengthUnit = Units.LengthUnitSection;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Shape", "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // type
      this.DropDownItems.Add(Enum.GetValues(typeof(WebOpeningShape)).Cast<WebOpeningShape>()
          .Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(WebOpeningShape.Rectangular.ToString());

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
      {
        if (this.SelectedItems[i] == OpeningType.ToString())
          return;
        this.OpeningType = (WebOpeningShape)Enum.Parse(typeof(WebOpeningShape), this.SelectedItems[i]);
        ModeChangeClicked();
      }
      else if (i == 1) // change is made to length unit
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      OpeningType = (WebOpeningShape)Enum.Parse(typeof(WebOpeningShape), SelectedItems[0]);
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);

      CreateAttributes();
      ModeChangeClicked();
      base.UpdateUI();
    }

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
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

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
        Params.Input[i].Name = WebOpeningStiffenersGoo.Name + "(s)";
        Params.Input[i].NickName = WebOpeningStiffenersGoo.NickName;
        Params.Input[i].Description = "(Optional) " + WebOpeningStiffenersGoo.Description;
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
        Params.Input[i].Name = WebOpeningStiffenersGoo.Name + "(s)";
        Params.Input[i].NickName = WebOpeningStiffenersGoo.NickName;
        Params.Input[i].Description = "(Optional) " + WebOpeningStiffenersGoo.Description;
        Params.Input[i].Optional = true;
      }
    }
    #endregion
  }
}
