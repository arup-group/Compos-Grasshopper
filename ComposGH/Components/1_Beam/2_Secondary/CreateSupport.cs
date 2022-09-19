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
using OasysUnitsNet;
using OasysUnitsNet.Units;

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
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion
    
    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

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
      if (this.Params.Input[3].Sources.Count > 0)
      {
        string restraintType = "";
        DA.GetData(3, ref restraintType);
        try
        {
          this.ParseRestraintType(restraintType);
          this.DropDownItems[0] = new List<string>();
          this.SelectedItems[0] = "-";
          this.OverrideDropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse intermediate restraint. Valid options are (click to copy to clipboard):";
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
          List<string> options = new List<string>();
          foreach (string g in Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList())
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, g);
          this.DropDownItems[0] = Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList();
        }
      }
      else if (this.OverrideDropDownItems[0])
      {
        this.DropDownItems[0] = Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList();
        this.SelectedItems[0] = RestraintType.ToString().Replace("__", "-").Replace("_", " ");
        this.OverrideDropDownItems[0] = false;
      }

      bool smir = true;
      DA.GetData(0, ref smir);
      bool ffre = true;
      DA.GetData(1, ref ffre);

      if (this.Params.Input[2].Sources.Count > 0)
      {
        List<IQuantity> restrs = Input.LengthsOrRatios(this, DA, 2, LengthUnit);
        SelectedItems[0] = "Custom";
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
    List<bool> OverrideDropDownItems;
    private IntermediateRestraint RestraintType = IntermediateRestraint.None;
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Intermediate Sup.",
          "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // type
      this.DropDownItems.Add(Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
          .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList());
      this.DropDownItems[0].RemoveAt(this.DropDownItems[0].Count - 1);
      this.SelectedItems.Add(this.DropDownItems[0][0]);

      // length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(LengthUnit.ToString());

      this.OverrideDropDownItems = new List<bool>() { false, false };
      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.ParseRestraintType(this.SelectedItems[0]);
      if (i == 1)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.ParseRestraintType(this.SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    private void ParseRestraintType(string value)
    {
      if (value != "-")
      {
        value = value.ToString().Replace("-", "__").Replace(" ", "_");
        this.RestraintType = (IntermediateRestraint)Enum.Parse(typeof(IntermediateRestraint), value);
      }
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[2].Name = "Restraint Pos [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
