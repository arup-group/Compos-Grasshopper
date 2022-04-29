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
using ComposAPI.Loads;
using static ComposAPI.Loads.Load;

namespace ComposGH.Components
{
  public class UniformLoad : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public UniformLoad()
      : base("Create Uniform Load", "UniformLoad", "Create a uniformly distributed Compos Load",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat6())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.UniformLoad;
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
      "Unit"
    });

    private bool first = true;
    private ForcePerLengthUnit forceUnit = Units.ForcePerLengthUnit;
    private PressureUnit stressUnit = Units.StressUnit;
    private LoadDistribution distribution = LoadDistribution.Area;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity force = new Pressure(0, stressUnit);
      string unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      pManager.AddGenericParameter("Const. Dead [" + unitAbbreviation + "]", "dl", "Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live [" + unitAbbreviation + "]", "ll", "Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead [" + unitAbbreviation + "]", "DL", "Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live [" + unitAbbreviation + "]", "LL", "Final Live Load", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Load", "Ld", "Compos Point Load", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      switch (distribution)
      {
        case LoadDistribution.Line:
          ForcePerLength constDeadL = GetInput.ForcePerLength(this, DA, 0, forceUnit);
          ForcePerLength constLiveL = GetInput.ForcePerLength(this, DA, 1, forceUnit);
          ForcePerLength finalDeadL = GetInput.ForcePerLength(this, DA, 2, forceUnit);
          ForcePerLength finalLiveL = GetInput.ForcePerLength(this, DA, 3, forceUnit);
          Load loadL = new ComposAPI.Loads.UniformLoad(constDeadL, constLiveL, finalDeadL, finalLiveL);
          DA.SetData(0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          Pressure constDeadA = GetInput.Stress(this, DA, 0, stressUnit);
          Pressure constLiveA = GetInput.Stress(this, DA, 1, stressUnit);
          Pressure finalDeadA = GetInput.Stress(this, DA, 2, stressUnit);
          Pressure finalLiveA = GetInput.Stress(this, DA, 3, stressUnit);
          Load loadA = new ComposAPI.Loads.UniformLoad(constDeadA, constLiveA, finalDeadA, finalLiveA);
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
      int i = 0;
      Params.Input[i++].Name = "Const. Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Const. Live [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Dead [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Final Live [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
