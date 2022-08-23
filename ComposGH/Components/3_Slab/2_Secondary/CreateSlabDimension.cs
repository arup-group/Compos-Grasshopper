using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateSlabDimension : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("3da0ace2-b5a0-4a6a-8bf0-d669800c1f08");
    public CreateSlabDimension()
      : base("Create" + SlabDimensionGoo.Name.Replace(" ", string.Empty),
          SlabDimensionGoo.Name.Replace(" ", string.Empty),
          "Create a " + SlabDimensionGoo.Description + " for a " + SlabGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.SlabDimensions;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Start [" + unitAbbreviation + "]", "Px", "(Optional) Start Position of this profile (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddNumberParameter("Depth [" + unitAbbreviation + "]", "D", "Overall depth", GH_ParamAccess.item);
      pManager.AddNumberParameter("Width Left [" + unitAbbreviation + "]", "WL", "Available width left", GH_ParamAccess.item);
      pManager.AddNumberParameter("Width Right [" + unitAbbreviation + "]", "WR", "Available width right", GH_ParamAccess.item);
      pManager.AddNumberParameter("Effective width Left [" + unitAbbreviation + "]", "EWL", "(Optional) Effective width left", GH_ParamAccess.item);
      pManager.AddNumberParameter("Effective width Right [" + unitAbbreviation + "]", "EWR", "(Optional) Effective width right", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Tapered", "Tp", "Taper to next (default = false)", GH_ParamAccess.item, false);
      pManager[0].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
      pManager[6].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(SlabDimensionGoo.Name, SlabDimensionGoo.NickName, SlabDimensionGoo.Description + " for a " + SlabGoo.Description, GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length start = GetInput.Length(this, DA, 0, this.LengthUnit, true);
      Length overallDepth = GetInput.Length(this, DA, 1, this.LengthUnit, true);
      Length availableWidthLeft = GetInput.Length(this, DA, 2, this.LengthUnit, true);
      Length availableWidthRight = GetInput.Length(this, DA, 3, this.LengthUnit, true);

      bool customEffectiveWidth = false;
      Length effectiveWidthLeft = Length.Zero;
      Length effectiveWidthRight = Length.Zero;

      if (this.Params.Input[4].Sources.Count > 0 && this.Params.Input[5].Sources.Count > 0)
      {
        customEffectiveWidth = true;
        effectiveWidthLeft = GetInput.Length(this, DA, 4, this.LengthUnit, true);
        effectiveWidthRight = GetInput.Length(this, DA, 5, this.LengthUnit, true);
      }
      bool taperedToNext = false;
      DA.GetData(6, ref taperedToNext);

      SlabDimension slabDimension;
      if (customEffectiveWidth)
         slabDimension = new SlabDimension(start, overallDepth, availableWidthLeft, availableWidthRight, effectiveWidthLeft, effectiveWidthRight, taperedToNext);
      else
         slabDimension = new SlabDimension(start, overallDepth, availableWidthLeft, availableWidthRight, taperedToNext);

      SetOutput.Item(this, DA, 0, new SlabDimensionGoo(slabDimension));
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
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      this.Params.Input[0].Name = "Start [" + unitAbbreviation + "]";
      this.Params.Input[1].Name = "Overall depth [" + unitAbbreviation + "]";
      this.Params.Input[2].Name = "Available width Left [" + unitAbbreviation + "]";
      this.Params.Input[3].Name = "Available width Right [" + unitAbbreviation + "]";
      this.Params.Input[4].Name = "Effective width Left [" + unitAbbreviation + "]";
      this.Params.Input[5].Name = "Effective width Right [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
