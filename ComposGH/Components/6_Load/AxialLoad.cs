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

namespace ComposGH.Components
{
  public class AxialLoad : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("9dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public AxialLoad()
      : base("Create Axial Load", "AxalLoad", "Create an axial Compos Load applied at both end positions",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat6())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.AxialLoad;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // force unit
        dropdownitems.Add(Units.FilteredForceUnits);
        selecteditems.Add(forceUnit.ToString());

        // length
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0)
        forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[i]);
      if (i == 1)
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[0]);
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 

    List<string> spacerDescriptions = new List<string>(new string[]
    {
      "Force Unit",
      "Length Unit"
    });

    private bool first = true;
    private ForceUnit forceUnit = Units.ForceUnit;
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity force = new Force(0, forceUnit);
      string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      IQuantity length = new Length(0, lengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      pManager.AddGenericParameter("Const. Dead 1 [" + unitAbbreviation + "]", "dl1", "Start Constant dead load; construction stage dead load which are used for construction stage analysis."
        + System.Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 1 [" + unitAbbreviation + "]", "ll1", "Start Constant live load; construction stage live load which are used for construction stage analysis."
        + System.Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 1 [" + unitAbbreviation + "]", "DL1", "Start Final Dead Load."
        + System.Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 1 [" + unitAbbreviation + "]", "LL1", "Start Final Live Load."
        + System.Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth 1 [" + lengthunitAbbreviation + "]", "dz1", "Start Depth below top of steel where axial load is applied (beam local z-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Dead 2 [" + unitAbbreviation + "]", "dl2", "End Constant dead load; construction stage dead load which are used for construction stage analysis."
        + System.Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 2 [" + unitAbbreviation + "]", "ll2", "End Constant live load; construction stage live load which are used for construction stage analysis."
        + System.Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 2 [" + unitAbbreviation + "]", "DL2", "End Final Dead Load."
        + System.Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 2 [" + unitAbbreviation + "]", "LL2", "End Final Live Load."
        + System.Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth 2 [" + lengthunitAbbreviation + "]", "dz2", "End Depth below top of steel where axial load is applied (beam local z-axis)", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Load", "Ld", "Compos Point Load", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Force constDead1 = GetInput.Force(this, DA, 0, forceUnit);
      Force constLive1 = GetInput.Force(this, DA, 1, forceUnit);
      Force finalDead1 = GetInput.Force(this, DA, 2, forceUnit);
      Force finalLive1 = GetInput.Force(this, DA, 3, forceUnit);
      Length pos1 = GetInput.Length(this, DA, 4, lengthUnit);
      Force constDead2 = GetInput.Force(this, DA, 5, forceUnit);
      Force constLive2 = GetInput.Force(this, DA, 6, forceUnit);
      Force finalDead2 = GetInput.Force(this, DA, 7, forceUnit);
      Force finalLive2 = GetInput.Force(this, DA, 8, forceUnit);
      Length pos2 = GetInput.Length(this, DA, 9, lengthUnit);

      Parameters.AxialLoad load = new Parameters.AxialLoad(
        constDead1, constLive1, finalDead1, finalLive1, pos1, constDead2, constLive2, finalDead2, finalLive2, pos2);
      DA.SetData(0, new ComposLoadGoo(load));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

      UpdateUIFromSelectedItems();

      first = false;

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
      IQuantity force = new Force(0, forceUnit);
      string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      IQuantity length = new Length(0, lengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      int i = 0;
      Params.Input[i++].Name = "Const. Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Depth 1 [" + lengthunitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Depth 2 [" + lengthunitAbbreviation + "]";
    }
    #endregion
  }
}
