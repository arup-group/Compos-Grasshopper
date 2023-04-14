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
  public class CreateWebOpeningStiffener : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4e7a2c23-0504-46d2-8fe1-846bf4ef6a37");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.Stiffener;
    public CreateWebOpeningStiffener()
      : base("Create" + WebOpeningStiffenersGoo.Name.Replace(" ", string.Empty),
          WebOpeningStiffenersGoo.Name.Replace(" ", string.Empty),
          "Create a " + WebOpeningStiffenersGoo.Description + " for a " + WebOpeningGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddBooleanParameter("Both Sides", "BS", "Set to true to apply horizontal stiffeners on both sides of web", GH_ParamAccess.item);
      pManager.AddGenericParameter("Dist. z [" + unitAbbreviation + "]", "Dz", "Vertical distance above/below opening edge to centre of stiffener (beam local z-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Top Width [" + unitAbbreviation + "]", "Wt", "Top Stiffener Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Top Thickness [" + unitAbbreviation + "]", "Tt", "Top Stiffener Thickness", GH_ParamAccess.item);
      pManager.AddGenericParameter("Bottom Width [" + unitAbbreviation + "]", "Wb", "Bottom Stiffener Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Bottom Thickness [" + unitAbbreviation + "]", "Tb", "Bottom Stiffener Thickness", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new WebOpeningStiffenersParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool bothSides = false;
      DA.GetData(0, ref bothSides);
      Length start = (Length)Input.UnitNumber(this, DA, 1, LengthUnit);
      Length topWidth = (Length)Input.UnitNumber(this, DA, 2, LengthUnit);
      Length topTHK = (Length)Input.UnitNumber(this, DA, 3, LengthUnit);
      if (OpeningType == Stiff_types.Web_Opening)
      {
        Length bottomWidth = (Length)Input.UnitNumber(this, DA, 4, LengthUnit);
        Length bottomTHK = (Length)Input.UnitNumber(this, DA, 5, LengthUnit);
        Output.SetItem(this, DA, 0, new WebOpeningStiffenersGoo(new WebOpeningStiffeners(
            start, topWidth, topTHK, bottomWidth, bottomTHK, bothSides)));
      }
      else
      {
        Output.SetItem(this, DA, 0, new WebOpeningStiffenersGoo(new WebOpeningStiffeners(
            start, topWidth, topTHK, bothSides)));
      }
    }

    #region Custom UI
    private enum Stiff_types
    {
      Web_Opening,
      Notch,
    }
    private Stiff_types OpeningType = Stiff_types.Web_Opening;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    protected override void InitialiseDropdowns()
    {
      _spacerDescriptions = new List<string>(new string[] { "Type", "Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // type
      _dropDownItems.Add(Enum.GetValues(typeof(Stiff_types)).Cast<Stiff_types>()
          .Select(x => x.ToString().Replace('_', ' ')).ToList());
      _selectedItems.Add(Stiff_types.Web_Opening.ToString().Replace('_', ' '));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)
      {
        if (_selectedItems[i] == OpeningType.ToString().Replace('_', ' '))
          return;
        OpeningType = (Stiff_types)Enum.Parse(typeof(Stiff_types), _selectedItems[i].Replace(' ', '_'));
        ModeChangeClicked();
      }
      else if (i == 1) // change is made to length unit
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      OpeningType = (Stiff_types)Enum.Parse(typeof(Stiff_types), _selectedItems[0].Replace(' ', '_'));
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      CreateAttributes();
      ModeChangeClicked();
      base.UpdateUI();
    }

    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      if (OpeningType == Stiff_types.Web_Opening)
      {
        if (Params.Input.Count == 6)
          return;

        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_GenericObject());
      }
      if (OpeningType == Stiff_types.Notch)
      {
        if (Params.Input.Count == 4)
          return;

        Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
        Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
      }
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      Params.Input[1].Name = "Dist. z [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Top Width [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Top Thickness [" + unitAbbreviation + "]";

      if (OpeningType == Stiff_types.Web_Opening)
      {
        int i = 4;
        Params.Input[i].Name = "Bottom Width [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Wb";
        Params.Input[i].Description = "Bottom Stiffener Width";
        Params.Input[i].Optional = false;
        i++;
        Params.Input[i].Name = "Bottom Thickness [" + unitAbbreviation + "]";
        Params.Input[i].NickName = "Tb";
        Params.Input[i].Description = "Bottom Stiffener Thickness";
        Params.Input[i].Optional = false;
      }
    }
    #endregion
  }
}
