using System;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposGH.Components
{
  public class CreatePointLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.PointLoad;
    public CreatePointLoad()
      : base("CreatePointLoad", "PointLoad", "Create a concentrated Compos Point Load.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(LengthUnit);
      pManager.AddGenericParameter("Const. Dead [" + unitAbbreviation + "]", "dl", "Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live [" + unitAbbreviation + "]", "ll", "Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead [" + unitAbbreviation + "]", "DL", "Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live [" + unitAbbreviation + "]", "LL", "Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos [" + lengthunitAbbreviation + "]", "Px", "Position on beam where Point Load acts (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposLoadParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Force constDead = (Force)Input.UnitNumber(this, DA, 0, ForceUnit);
      Force constLive = (Force)Input.UnitNumber(this, DA, 1, ForceUnit);
      Force finalDead = (Force)Input.UnitNumber(this, DA, 2, ForceUnit);
      Force finalLive = (Force)Input.UnitNumber(this, DA, 3, ForceUnit);
      IQuantity pos = Input.LengthOrRatio(this, DA, 4, LengthUnit);

      Load load = new PointLoad(constDead, constLive, finalDead, finalLive, pos);
      Output.SetItem(this, DA, 0, new LoadGoo(load));
    }

    #region Custom UI
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    protected override void InitialiseDropdowns()
    {
      _spacerDescriptions = new List<string>(new string[] { "Force Unit", "Length Unit" });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // force unit
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(ForceUnit));

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      _isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)
        ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
      if (i == 1)
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
    
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(LengthUnit);
      int i = 0;
      Params.Input[i++].Name = "Const. Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Pos [" + lengthunitAbbreviation + "]";
    }
    #endregion
  }
}
