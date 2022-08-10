using System;

using ComposGH.Parameters;
using Grasshopper.Kernel;
using ComposAPI;
using System.Collections.Generic;
using UnitsNet.Units;
using UnitsNet;

namespace ComposGH.Components
{
  public class CreateBeamSizeLimits : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("a1c37716-886d-4816-afa3-ef0b9ab42f79");
    public CreateBeamSizeLimits()
      : base("Create" + BeamSizeLimitsGoo.Name.Replace(" ", string.Empty),
          BeamSizeLimitsGoo.Name.Replace(" ", string.Empty),
          "Create a " + BeamSizeLimitsGoo.Description + " for a " + DesignCriteriaGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.BeamSizeLimits;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbb = Length.GetAbbreviation(this.LengthUnit);
      pManager.AddGenericParameter("Min Depth [" + unitAbb + "]", "Dmin", "Minimum Depth", GH_ParamAccess.item);
      pManager.AddGenericParameter("Max Depth [" + unitAbb + "]", "Dmax", "Maximum Depth", GH_ParamAccess.item);
      pManager.AddGenericParameter("Min Width [" + unitAbb + "]", "Wmin", "Minimum Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Max Width [" + unitAbb + "]", "Wmax", "Maximum Width", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(BeamSizeLimitsGoo.Name, BeamSizeLimitsGoo.NickName, BeamSizeLimitsGoo.Description + " for a " + DesignCriteriaGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      BeamSizeLimits beamSizeLimits = new BeamSizeLimits()
      {
        MinDepth = GetInput.Length(this, DA, 0, this.LengthUnit),
        MaxDepth = GetInput.Length(this, DA, 1, this.LengthUnit),
        MinWidth = GetInput.Length(this, DA, 2, this.LengthUnit),
        MaxWidth = GetInput.Length(this, DA, 3, this.LengthUnit)
      };

      DA.SetData(0, new BeamSizeLimitsGoo(beamSizeLimits));
    }

    #region Custom UI
    private LengthUnit LengthUnit = Units.LengthUnitSection;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (this.LengthUnit.ToString() == this.SelectedItems[i])
        return;

      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);

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
