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
  public class CreateBeamSizeLimits : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("a1c37716-886d-4816-afa3-ef0b9ab42f79");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.BeamSizeLimits;
    public CreateBeamSizeLimits()
      : base("Create" + BeamSizeLimitsGoo.Name.Replace(" ", string.Empty),
          BeamSizeLimitsGoo.Name.Replace(" ", string.Empty),
          "Create a " + BeamSizeLimitsGoo.Description + " for a " + DesignCriteriaGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbb = Length.GetAbbreviation(this.LengthUnit);
      pManager.AddGenericParameter("Min Depth [" + unitAbb + "]", "Dmin", "(Optional) Minimum Depth (default ≥ 20 cm)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Max Depth [" + unitAbb + "]", "Dmax", "(Optional) Maximum Depth  (default ≤ 100 cm)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Min Width [" + unitAbb + "]", "Wmin", "(Optional) Minimum Width  (default ≥ 10 cm)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Max Width [" + unitAbb + "]", "Wmax", "(Optional) Maximum Width  (default ≤ 50 cm)", GH_ParamAccess.item);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new BeamSizeLimitsParam());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length minDepth = new Length(20, LengthUnit.Centimeter);
      if (this.Params.Input[0].Sources.Count > 0)
        minDepth = (Length)Input.UnitNumber(this, DA, 0, this.LengthUnit);

      Length maxDepth = new Length(100, LengthUnit.Centimeter);
      if (this.Params.Input[1].Sources.Count > 0)
        maxDepth = (Length)Input.UnitNumber(this, DA, 1, this.LengthUnit);

      Length minWidth = new Length(10, LengthUnit.Centimeter);
      if (this.Params.Input[2].Sources.Count > 0)
        minWidth = (Length)Input.UnitNumber(this, DA, 2, this.LengthUnit);

      Length maxWidth = new Length(50, LengthUnit.Centimeter);
      if (this.Params.Input[3].Sources.Count > 0)
        maxWidth = (Length)Input.UnitNumber(this, DA, 3, this.LengthUnit);

      BeamSizeLimits beamSizeLimits = new BeamSizeLimits()
      {
        MinDepth = minDepth,
        MaxDepth = maxDepth,
        MinWidth = minWidth,
        MaxWidth = maxWidth
      };

      Output.SetItem(this, DA, 0, new BeamSizeLimitsGoo(beamSizeLimits));
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (this.LengthUnit.ToString() == this.SelectedItems[i])
        return;

      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbb = Length.GetAbbreviation(this.LengthUnit);
      int i = 0;
      Params.Input[i++].Name = "Min Depth [" + unitAbb + "]";
      Params.Input[i++].Name = "Max Depth [" + unitAbb + "]";
      Params.Input[i++].Name = "Min Width [" + unitAbb + "]";
      Params.Input[i++].Name = "Max Width [" + unitAbb + "]";
    }
    #endregion
  }
}
