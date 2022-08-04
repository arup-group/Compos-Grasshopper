﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreatePatchLoad : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public CreatePatchLoad()
      : base("Create Patch Load", "PatchLoad", "Create a distributed patch Compos load; the positions and load values of the start and end points can be defined at any point along the beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.PatchLoad;
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
      "Force Unit",
      "Length Unit"
    });

    private bool First = true;
    private ForcePerLengthUnit ForcePerLengthUnit = Units.ForcePerLengthUnit;
    private PressureUnit ForcePerAreaUnit = Units.StressUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;
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
      if (i == 2)
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);

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
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[2]);

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
      IQuantity length = new Length(0, LengthUnit);
      string lengthunitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      pManager.AddGenericParameter("Const. Dead 1 [" + unitAbbreviation + "]", "dl1", "Start Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 1 [" + unitAbbreviation + "]", "ll1", "Start Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 1 [" + unitAbbreviation + "]", "DL1", "Start Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 1 [" + unitAbbreviation + "]", "LL1", "Start Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x1 [" + lengthunitAbbreviation + "]", "Px1", "Start Position on beam where line load begins (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Dead 2 [" + unitAbbreviation + "]", "dl2", "End Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 2 [" + unitAbbreviation + "]", "ll2", "End Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 2 [" + unitAbbreviation + "]", "DL2", "End Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 2 [" + unitAbbreviation + "]", "LL2", "End Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos x2 [" + lengthunitAbbreviation + "]", "Px2", "End Position on beam where line load ends (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Load", "Ld", "Compos Point Load", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IQuantity pos1 = GetInput.LengthOrRatio(this, DA, 4, LengthUnit);
      IQuantity pos2 = GetInput.LengthOrRatio(this, DA, 9, LengthUnit);

      switch (DistributionType)
      {
        case LoadDistribution.Line:
          ForcePerLength constDeadL1 = GetInput.ForcePerLength(this, DA, 0, ForcePerLengthUnit);
          ForcePerLength constLiveL1 = GetInput.ForcePerLength(this, DA, 1, ForcePerLengthUnit);
          ForcePerLength finalDeadL1 = GetInput.ForcePerLength(this, DA, 2, ForcePerLengthUnit);
          ForcePerLength finalLiveL1 = GetInput.ForcePerLength(this, DA, 3, ForcePerLengthUnit);
          ForcePerLength constDeadL2 = GetInput.ForcePerLength(this, DA, 5, ForcePerLengthUnit);
          ForcePerLength constLiveL2 = GetInput.ForcePerLength(this, DA, 6, ForcePerLengthUnit);
          ForcePerLength finalDeadL2 = GetInput.ForcePerLength(this, DA, 7, ForcePerLengthUnit);
          ForcePerLength finalLiveL2 = GetInput.ForcePerLength(this, DA, 8, ForcePerLengthUnit);
          Load loadL = new PatchLoad(
            constDeadL1, constLiveL1, finalDeadL1, finalLiveL1, pos1, constDeadL2, constLiveL2, finalDeadL2, finalLiveL2, pos2);
          DA.SetData(0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          Pressure constDeadA1 = GetInput.Stress(this, DA, 0, ForcePerAreaUnit);
          Pressure constLiveA1 = GetInput.Stress(this, DA, 1, ForcePerAreaUnit);
          Pressure finalDeadA1 = GetInput.Stress(this, DA, 2, ForcePerAreaUnit);
          Pressure finalLiveA1 = GetInput.Stress(this, DA, 3, ForcePerAreaUnit);
          Pressure constDeadA2 = GetInput.Stress(this, DA, 4, ForcePerAreaUnit);
          Pressure constLiveA2 = GetInput.Stress(this, DA, 5, ForcePerAreaUnit);
          Pressure finalDeadA2 = GetInput.Stress(this, DA, 6, ForcePerAreaUnit);
          Pressure finalLiveA2 = GetInput.Stress(this, DA, 7, ForcePerAreaUnit);
          Load loadA = new PatchLoad(
            constDeadA1, constLiveA1, finalDeadA1, finalLiveA1, pos1, constDeadA2, constLiveA2, finalDeadA2, finalLiveA2, pos2);
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
      IQuantity length = new Length(0, LengthUnit);
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
