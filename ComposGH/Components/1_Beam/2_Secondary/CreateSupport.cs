using System;
using System.Collections.Generic;
using System.Linq;
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
  public class CreateSupport : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("71c87cde-f442-475b-9131-8f2974c42499");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateSupport;
    public CreateSupport()
      : base("Create" + SupportsGoo.Name.Replace(" ", string.Empty), 
          SupportsGoo.Name.Replace(" ", string.Empty), 
          "Create a " + SupportsGoo.Description + " for a " + RestraintGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { Hidden = true; } // sets the initial state of the component to hidden
    #endregion
    
    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddBooleanParameter("Sec. mem. interm. res.", "SMIR", "Take secondary member as intermediate restraint (default = true)", GH_ParamAccess.item, true);
      pManager.AddBooleanParameter("Flngs. free rot. ends", "FFRE", "Both flanges are free to rotate on plan at end restraints (default = true)", GH_ParamAccess.item, true);
      pManager.AddGenericParameter("Restraint Pos [" + unitAbbreviation + "]", "RPxs", "(Optional) List of customly defined intermediate restraint positions along the beam (beam x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set positions as percentage (-0.5 => 50%)", GH_ParamAccess.list);
      pManager.AddTextParameter("Int. Support", "ISup", "(Optional) Intermediate support as a text string.", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new SupportsParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // override intermediate support?
      if (Params.Input[3].Sources.Count > 0)
      {
        string restraintType = "";
        DA.GetData(3, ref restraintType);
        try
        {
          ParseRestraintType(restraintType);
          _dropDownItems[0] = new List<string>();
          _selectedItems[0] = "-";
          Override_dropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse intermediate restraint. Valid options are (click to copy to clipboard):";
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
          List<string> options = new List<string>();
          foreach (string g in Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList())
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, g);
          _dropDownItems[0] = Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList();
        }
      }
      else if (Override_dropDownItems[0])
      {
        _dropDownItems[0] = Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList();
        _selectedItems[0] = RestraintType.ToString().Replace("__", "-").Replace("_", " ");
        Override_dropDownItems[0] = false;
      }

      bool smir = true;
      DA.GetData(0, ref smir);
      bool ffre = true;
      DA.GetData(1, ref ffre);

      if (Params.Input[2].Sources.Count > 0)
      {
        List<IQuantity> restrs = Input.LengthsOrRatios(this, DA, 2, LengthUnit);
        _selectedItems[0] = "Custom";
        Supports sup = new Supports(restrs, smir, ffre);
        Output.SetItem(this, DA, 0, new SupportsGoo(sup));
      }
      else
      {
        Supports sup = new Supports(RestraintType, smir, ffre);
        Output.SetItem(this, DA, 0, new SupportsGoo(sup));
      }
    }

    #region Custom UI
    List<bool> Override_dropDownItems;
    private IntermediateRestraint RestraintType = IntermediateRestraint.None;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    protected override void InitialiseDropdowns()
    {
      _spacerDescriptions = new List<string>(new string[]
        {
          "Intermediate Sup.",
          "Unit"
        });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      // type
      _dropDownItems.Add(Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
          .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList());
      _dropDownItems[0].RemoveAt(_dropDownItems[0].Count - 1);
      _selectedItems.Add(_dropDownItems[0][0]);

      // length
      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(LengthUnit));

      Override_dropDownItems = new List<bool>() { false, false };
      _isInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      _selectedItems[i] = _dropDownItems[i][j];

      if (i == 0)
        ParseRestraintType(_selectedItems[0]);
      if (i == 1)
        LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

      base.UpdateUI();
    }

    protected override void UpdateUIFromSelectedItems()
    {
      ParseRestraintType(_selectedItems[0]);
      LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    private void ParseRestraintType(string value)
    {
      if (value != "-")
      {
        value = value.ToString().Replace("-", "__").Replace(" ", "_");
        RestraintType = (IntermediateRestraint)Enum.Parse(typeof(IntermediateRestraint), value);
      }
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      Params.Input[2].Name = "Restraint Pos [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
