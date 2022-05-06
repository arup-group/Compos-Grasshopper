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
  public class CreateNotch : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("de802051-ae6a-4249-8699-7ea0cfe8c528");
    public CreateNotch()
      : base("Beam Notch", "Notch", "Create Notch for a Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.Notch;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
            "Position",
            "Unit"
    });
    private enum notch_types
    {
      Both_ends,
      Start,
      End
    }

    private bool First = true;
    private notch_types OpeningType = notch_types.Both_ends;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // type
        DropdownItems.Add(Enum.GetValues(typeof(notch_types)).Cast<notch_types>()
            .Select(x => x.ToString().Replace('_', ' ')).ToList());
        SelectedItems.Add(notch_types.Both_ends.ToString().Replace('_', ' '));

        // length
        DropdownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropdownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropdownItems[i][j];

      if (i == 0)
      {
        if (SelectedItems[i] == OpeningType.ToString().Replace('_', ' '))
          return;
        OpeningType = (notch_types)Enum.Parse(typeof(notch_types), SelectedItems[i].Replace(' ', '_'));
      }
      else if (i == 1) // change is made to length unit
      {
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);
      }

        // update name of inputs (to display unit on sliders)
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      OpeningType = (notch_types)Enum.Parse(typeof(notch_types), SelectedItems[0].Replace(' ', '_'));
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);

      CreateAttributes();
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

      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Web Opening Width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Web Opening Height", GH_ParamAccess.item);
      pManager.AddGenericParameter("Stiffeners", "WS", "(Optional) Web Opening Stiffeners", GH_ParamAccess.item);
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("WebOpening", "WO", "Notch Web Opening for a Compos Beam", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length width = GetInput.Length(this, DA, 0, LengthUnit);
      Length height = GetInput.Length(this, DA, 1, LengthUnit);
      WebOpeningStiffenersGoo stiff = (WebOpeningStiffenersGoo)GetInput.GenericGoo<WebOpeningStiffenersGoo>(this, DA, 2);
      if (stiff != null)
      {
        if (stiff.Value.BottomStiffenerWidth != Length.Zero)
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "For Beam Notches only top stiffener(s) will be used.");
      }

      switch (OpeningType)
      {
        case notch_types.Start:
          DA.SetData(0, new WebOpeningGoo(new WebOpening(width, height, NotchPosition.Start, (stiff == null) ? null : stiff.Value)));
          break;

        case notch_types.End:
          DA.SetData(0, new WebOpeningGoo(new WebOpening(width, height, NotchPosition.End, (stiff == null) ? null : stiff.Value)));
          break;

        case notch_types.Both_ends:
          List<WebOpeningGoo> both = new List<WebOpeningGoo>();
          both.Add(new WebOpeningGoo(new WebOpening(width, height, NotchPosition.Start, (stiff == null) ? null : stiff.Value)));
          both.Add(new WebOpeningGoo(new WebOpening(width, height, NotchPosition.End, (stiff == null) ? null : stiff.Value)));
          DA.SetDataList(0, both);
          break;
      }
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropdownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropdownItems, ref SelectedItems, ref SpacerDescriptions);

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
      if (OpeningType == notch_types.Both_ends)
        Params.Output[0].Access = GH_ParamAccess.list;
      else
        Params.Output[0].Access = GH_ParamAccess.item;

      IQuantity length = new Length(0, LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      Params.Input[0].Name = "Width [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Height [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
