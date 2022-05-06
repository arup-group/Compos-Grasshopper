﻿using System;
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
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateCustomStudSpacing : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("49328e6d-eebe-405c-b58c-060b8bdc1bef");
    public CreateCustomStudSpacing()
      : base("Custom Stud Spacing", "CustStudSpac", "Create Custom Stud Spacing for a Compos Stud",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CustomStudSpacing;
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
      "Unit",
    });

    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitSection;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

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


      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);


      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[0]);

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

      pManager.AddGenericParameter("Pos x [" + unitAbbreviation + "]", "Px", "Start Position where this Stud Spacing Groups begins on Beam (beam local x-axis)", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Rows", "R", "Number of rows (across the top flange)", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Lines", "L", "Number of lines (along the length of the beam", GH_ParamAccess.item);
      pManager.AddGenericParameter("Spacing [" + unitAbbreviation + "]", "S", "Spacing of studs in this group (distance between each line)", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Stud Spacing", "Spa", "Custom Compos Shear Stud Spacing", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length start = GetInput.Length(this, DA, 0, LengthUnit);
      int rows = 1;
      DA.GetData(1, ref rows);
      int lines = 1;
      DA.GetData(2, ref lines);
      Length spacing = GetInput.Length(this, DA, 3, LengthUnit);

      DA.SetData(0, new StudGroupSpacingGoo(new StudGroupSpacing(start, rows, lines, spacing)));
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
      IQuantity length = new Length(0, LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      Params.Input[0].Name = "Pos x [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Spacing [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
