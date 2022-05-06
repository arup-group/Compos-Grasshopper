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
  public class CreateUniformLoad : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public CreateUniformLoad()
      : base("Create Uniform Load", "UniformLoad", "Create a uniformly distributed Compos Load",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.UniformLoad;
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
      "Distribution",
      "Unit"
    });

    private bool First = true;
    private ForcePerLengthUnit ForcePerLengthUnit = Units.ForcePerLengthUnit;
    private PressureUnit ForcePerAreaUnit = Units.StressUnit;
    private LoadDistribution DistributionType = LoadDistribution.Area;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // type
        DropdownItems.Add(Enum.GetValues(typeof(LoadDistribution)).Cast<LoadDistribution>().Select(x => x.ToString()).ToList()); 
        SelectedItems.Add(LoadDistribution.Area.ToString());

        // force unit
        DropdownItems.Add(Units.FilteredForcePerAreaUnits);
        SelectedItems.Add(ForcePerAreaUnit.ToString());

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
        DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), SelectedItems[i]);
        if (DistributionType == LoadDistribution.Line)
        {
          DropdownItems[1] = Units.FilteredForcePerLengthUnits;
          SelectedItems[1] = ForcePerLengthUnit.ToString();
        }
        else
        {
          DropdownItems[1] = Units.FilteredForcePerAreaUnits;
          SelectedItems[1] = ForcePerAreaUnit.ToString();
        }
      }
      if (i == 1)
      {
        if (DistributionType == LoadDistribution.Line)
          ForcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), SelectedItems[i]);
        else
          ForcePerAreaUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), SelectedItems[0]);
      if (DistributionType == LoadDistribution.Line)
        ForcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), SelectedItems[1]);
      else
        ForcePerAreaUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[1]);

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
      IQuantity force = new Pressure(0, ForcePerAreaUnit);
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
      switch (DistributionType)
      {
        case LoadDistribution.Line:
          ForcePerLength constDeadL = GetInput.ForcePerLength(this, DA, 0, ForcePerLengthUnit);
          ForcePerLength constLiveL = GetInput.ForcePerLength(this, DA, 1, ForcePerLengthUnit);
          ForcePerLength finalDeadL = GetInput.ForcePerLength(this, DA, 2, ForcePerLengthUnit);
          ForcePerLength finalLiveL = GetInput.ForcePerLength(this, DA, 3, ForcePerLengthUnit);
          Load loadL = new UniformLoad(constDeadL, constLiveL, finalDeadL, finalLiveL);
          DA.SetData(0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          Pressure constDeadA = GetInput.Stress(this, DA, 0, ForcePerAreaUnit);
          Pressure constLiveA = GetInput.Stress(this, DA, 1, ForcePerAreaUnit);
          Pressure finalDeadA = GetInput.Stress(this, DA, 2, ForcePerAreaUnit);
          Pressure finalLiveA = GetInput.Stress(this, DA, 3, ForcePerAreaUnit);
          Load loadA = new UniformLoad(constDeadA, constLiveA, finalDeadA, finalLiveA);
          DA.SetData(0, new LoadGoo(loadA));
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
      string unitAbbreviation = "";
      if (DistributionType == LoadDistribution.Line)
      {
        IQuantity force = new ForcePerLength(0, ForcePerLengthUnit);
        unitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      }
      else
      {
        IQuantity force = new Pressure(0, ForcePerAreaUnit);
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
