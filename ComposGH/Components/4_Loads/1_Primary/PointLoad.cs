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
using ComposAPI;

namespace ComposGH.Components
{
  public class PointLoad : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public PointLoad()
      : base("Create Point Load", "PointLoad", "Create a concentrated point Compos Load",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.PointLoad;
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
      pManager.AddGenericParameter("Const. Dead [" + unitAbbreviation + "]", "dl", "Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live [" + unitAbbreviation + "]", "ll", "Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead [" + unitAbbreviation + "]", "DL", "Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live [" + unitAbbreviation + "]", "LL", "Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos [" + lengthunitAbbreviation + "]", "Px", "Position on beam where Point Load acts (beam local x-axis)", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Load", "Ld", "Compos Point Load", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Force constDead = GetInput.Force(this, DA, 0, forceUnit);
      Force constLive = GetInput.Force(this, DA, 1, forceUnit);
      Force finalDead = GetInput.Force(this, DA, 2, forceUnit);
      Force finalLive = GetInput.Force(this, DA, 3, forceUnit);
      Length pos = GetInput.Length(this, DA, 4, lengthUnit);

      Load load = new ComposAPI.PointLoad(constDead, constLive, finalDead, finalLive, pos);
      DA.SetData(0, new LoadGoo(load));
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
      Params.Input[i++].Name = "Const. Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Pos [" + lengthunitAbbreviation + "]";
    }
    #endregion
  }
}
