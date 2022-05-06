using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateSlabDimension : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("3da0ace2-b5a0-4a6a-8bf0-d669800c1f08");
    public CreateSlabDimension()
      : base("Slab Dimension", "SlabDimension", "Create slab dimension for concrete slab",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.SlabDimensions;
    #endregion

    #region Custom UI
    // This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Unit"
    });

    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void CreateAttributes()
    {
      if (First)
      {
        this.DropdownItems = new List<List<string>>();
        this.SelectedItems = new List<string>();

        // length
        this.DropdownItems.Add(Units.FilteredLengthUnits);
        this.SelectedItems.Add(this.LengthUnit.ToString());

        this.First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, this.SetSelected, this.DropdownItems, this.SelectedItems, this.SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropdownItems[i][j];

      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);

      this.CreateAttributes();
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

      pManager.AddGenericParameter("Start [" + unitAbbreviation + "]", "Px", "(Optional) Start Position of this profile (beam local x-axis)", GH_ParamAccess.item);
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
      pManager.AddGenericParameter("Slab Dimension", "SD", "Slab dimension for a concrete slab", GH_ParamAccess.list);
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

      DA.SetData(0, new SlabDimensionGoo(slabDimension));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, this.DropdownItems, this.SelectedItems, this.SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref this.DropdownItems, ref this.SelectedItems, ref this.SpacerDescriptions);

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
      IQuantity length = new Length(0, this.LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
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
