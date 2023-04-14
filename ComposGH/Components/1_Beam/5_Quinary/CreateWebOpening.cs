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
  public class CreateWebOpening : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("084fa2ab-d50e-4213-8f44-2affc9f41752");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.WebOpening;
    public CreateWebOpening()
      : base("Create" + WebOpeningGoo.Name.Replace(" ", string.Empty),
          WebOpeningGoo.Name.Replace(" ", string.Empty),
          "Create a " + WebOpeningGoo.Description + " for a " + BeamGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Web Opening Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Web Opening Height", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x [" + unitAbbreviation + "]", "Px", "Position of opening Centroid from Start of Beam (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos z [" + unitAbbreviation + "]", "Pz", "Position of opening Centroid from Top of Beam (beam local z-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddParameter(new WebOpeningStiffenersParam(), WebOpeningStiffenersGoo.Name + "(s)", WebOpeningStiffenersGoo.NickName, "(Optional) " + WebOpeningStiffenersGoo.Description, GH_ParamAccess.item);
      pManager[4].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposWebOpeningParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length width_dia = (Length)Input.UnitNumber(this, DA, 0, LengthUnit);
      
      int i = 1;

      Length height = Length.Zero;
      if (OpeningType == WebOpeningShape.Rectangular)
        height = (Length)Input.UnitNumber(this, DA, i++, LengthUnit);
      
      IQuantity x = Input.LengthOrRatio(this, DA, i++, LengthUnit);
      
      IQuantity z = Input.LengthOrRatio(this, DA, i++, LengthUnit);
      
      WebOpeningStiffenersGoo stiff = (WebOpeningStiffenersGoo)Input.GenericGoo<WebOpeningStiffenersGoo>(this, DA, i++);

      switch (OpeningType)
      {
        case WebOpeningShape.Rectangular:
          Output.SetItem(this, DA, 0, new WebOpeningGoo(new WebOpening(width_dia, height, x, z, (stiff == null) ? null : stiff.Value)));
          break;

        case WebOpeningShape.Circular:
          Output.SetItem(this, DA, 0, new WebOpeningGoo(new WebOpening(width_dia, x, z, (stiff == null) ? null : stiff.Value)));
          break;
      }
    }

    #region Custom UI
    private WebOpeningShape OpeningType = WebOpeningShape.Rectangular;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    protected override void InitialiseDropdowns()
    {
      _spacerDescriptions = new List<string>(new string[] { "Shape", "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // type
      _dropDownItems.Add(Enum.GetValues(typeof(WebOpeningShape)).Cast<WebOpeningShape>()
          .Select(x => x.ToString()).ToList());
      _selectedItems.Add(WebOpeningShape.Rectangular.ToString());

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)
      {
        if (_selectedItems[i] == OpeningType.ToString())
          return;
        OpeningType = (WebOpeningShape)Enum.Parse(typeof(WebOpeningShape), _selectedItems[i]);
        ModeChangeClicked();
      }
      else if (i == 1) // change is made to length unit
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      OpeningType = (WebOpeningShape)Enum.Parse(typeof(WebOpeningShape), _selectedItems[0]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      CreateAttributes();
      ModeChangeClicked();
      base.UpdateUI();
    }

    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (OpeningType == WebOpeningShape.Rectangular)
      {
        if (Params.Input.Count == 5)
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
        if (Params.Input.Count == 4)
          return;

        // remove height param
        Params.UnregisterInputParameter(Params.Input[1], true);
      }
    }
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

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
