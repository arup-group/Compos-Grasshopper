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
using static ComposAPI.Load;

namespace ComposGH.Components
{
  public class TriLinearLoad : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("7dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public TriLinearLoad()
      : base("Create Tri-Linear Load", "TriLinearLoad", "Create a tri-linearly varying distributed Compos Load starting from left end of the beam and ending at the right." +
          Environment.NewLine + "The two peak load points can be defined at any positions along the span Compos Load",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.TriLinearLoad;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // type
        dropdownitems.Add(Enum.GetValues(typeof(LoadDistribution)).Cast<LoadDistribution>().Select(x => x.ToString()).ToList()); 
        selecteditems.Add(LoadDistribution.Area.ToString());

        // force unit
        dropdownitems.Add(Units.FilteredForcePerAreaUnits);
        selecteditems.Add(stressUnit.ToString());

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
      {
        distribution = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), selecteditems[i]);
        if (distribution == LoadDistribution.Line)
        {
          dropdownitems[1] = Units.FilteredForcePerLengthUnits;
          selecteditems[1] = forceUnit.ToString();
        }
        else
        {
          dropdownitems[1] = Units.FilteredForcePerAreaUnits;
          selecteditems[1] = stressUnit.ToString();
        }
      }
      if (i == 1)
      {
        if (distribution == LoadDistribution.Line)
          forceUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[i]);
        else
          stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);
      }
      if (i == 2)
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      distribution = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), selecteditems[0]);
      if (distribution == LoadDistribution.Line)
        forceUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[1]);
      else
        stressUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[1]);
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[2]);

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
      "Distribution",
      "Force Unit",
      "Length Unit"
    });

    private bool first = true;
    private ForcePerLengthUnit forceUnit = Units.ForcePerLengthUnit;
    private PressureUnit stressUnit = Units.StressUnit;
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    private LoadDistribution distribution = LoadDistribution.Area;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity force = new Pressure(0, stressUnit);
      string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      IQuantity length = new Length(0, lengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      pManager.AddGenericParameter("Const. Dead 1 [" + unitAbbreviation + "]", "dl1", "Start Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 1 [" + unitAbbreviation + "]", "ll1", "Start Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 1 [" + unitAbbreviation + "]", "DL1", "Start Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 1 [" + unitAbbreviation + "]", "LL1", "Start Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x1 [" + lengthunitAbbreviation + "]", "Px1", "Start Position on beam where line load begins (beam local x-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Dead 2 [" + unitAbbreviation + "]", "dl2", "End Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 2 [" + unitAbbreviation + "]", "ll2", "End Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 2 [" + unitAbbreviation + "]", "DL2", "End Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 2 [" + unitAbbreviation + "]", "LL2", "End Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x2 [" + lengthunitAbbreviation + "]", "Px2", "End Position on beam where line load ends (beam local x-axis)", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Load", "Ld", "Compos Point Load", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length pos1 = GetInput.Length(this, DA, 4, lengthUnit);
      Length pos2 = GetInput.Length(this, DA, 9, lengthUnit);

      switch (distribution)
      {
        case LoadDistribution.Line:
          ForcePerLength constDeadL1 = GetInput.ForcePerLength(this, DA, 0, forceUnit);
          ForcePerLength constLiveL1 = GetInput.ForcePerLength(this, DA, 1, forceUnit);
          ForcePerLength finalDeadL1 = GetInput.ForcePerLength(this, DA, 2, forceUnit);
          ForcePerLength finalLiveL1 = GetInput.ForcePerLength(this, DA, 3, forceUnit);
          ForcePerLength constDeadL2 = GetInput.ForcePerLength(this, DA, 5, forceUnit);
          ForcePerLength constLiveL2 = GetInput.ForcePerLength(this, DA, 6, forceUnit);
          ForcePerLength finalDeadL2 = GetInput.ForcePerLength(this, DA, 7, forceUnit);
          ForcePerLength finalLiveL2 = GetInput.ForcePerLength(this, DA, 8, forceUnit);
          Load loadL = new ComposAPI.TriLinearLoad(
            constDeadL1, constLiveL1, finalDeadL1, finalLiveL1, pos1, constDeadL2, constLiveL2, finalDeadL2, finalLiveL2, pos2);
          DA.SetData(0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          Pressure constDeadA1 = GetInput.Stress(this, DA, 0, stressUnit);
          Pressure constLiveA1 = GetInput.Stress(this, DA, 1, stressUnit);
          Pressure finalDeadA1 = GetInput.Stress(this, DA, 2, stressUnit);
          Pressure finalLiveA1 = GetInput.Stress(this, DA, 3, stressUnit);
          Pressure constDeadA2 = GetInput.Stress(this, DA, 4, stressUnit);
          Pressure constLiveA2 = GetInput.Stress(this, DA, 5, stressUnit);
          Pressure finalDeadA2 = GetInput.Stress(this, DA, 6, stressUnit);
          Pressure finalLiveA2 = GetInput.Stress(this, DA, 7, stressUnit);
          Load loadA = new ComposAPI.TriLinearLoad(
            constDeadA1, constLiveA1, finalDeadA1, finalLiveA1, pos1, constDeadA2, constLiveA2, finalDeadA2, finalLiveA2, pos2);
          DA.SetData(0, new LoadGoo(loadA));
          break;
      }
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
      string unitAbbreviation = "";
      if (distribution == LoadDistribution.Line)
      {
        IQuantity force = new ForcePerLength(0, forceUnit);
        unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      }
      else
      {
        IQuantity force = new Pressure(0, stressUnit);
        unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      }
      IQuantity length = new Length(0, lengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      int i = 0;
      Params.Input[i++].Name = "Const. Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 1 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Pos x1 [" + lengthunitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live 2 [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Pos x2 [" + lengthunitAbbreviation + "]";
    }
    #endregion
  }
}
