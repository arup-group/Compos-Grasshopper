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
  public class CreatePatchLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public CreatePatchLoad()
      : base("CreatePatchLoad", "PatchLoad", "Create a distributed patch Compos load; the positions and load values of the start and end points can be defined at any point along the beam.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.PatchLoad;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Pressure.GetAbbreviation(this.ForcePerAreaUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
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
      pManager.AddGenericParameter(LoadGoo.Name, LoadGoo.NickName, LoadGoo.Description + " for a " + MemberGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IQuantity pos1 = GetInput.LengthOrRatio(this, DA, 4, this.LengthUnit);
      IQuantity pos2 = GetInput.LengthOrRatio(this, DA, 9, this.LengthUnit);

      switch (DistributionType)
      {
        case LoadDistribution.Line:
          ForcePerLength constDeadL1 = GetInput.ForcePerLength(this, DA, 0, this.ForcePerLengthUnit);
          ForcePerLength constLiveL1 = GetInput.ForcePerLength(this, DA, 1, this.ForcePerLengthUnit);
          ForcePerLength finalDeadL1 = GetInput.ForcePerLength(this, DA, 2, this.ForcePerLengthUnit);
          ForcePerLength finalLiveL1 = GetInput.ForcePerLength(this, DA, 3, this.ForcePerLengthUnit);
          ForcePerLength constDeadL2 = GetInput.ForcePerLength(this, DA, 5, this.ForcePerLengthUnit);
          ForcePerLength constLiveL2 = GetInput.ForcePerLength(this, DA, 6, this.ForcePerLengthUnit);
          ForcePerLength finalDeadL2 = GetInput.ForcePerLength(this, DA, 7, this.ForcePerLengthUnit);
          ForcePerLength finalLiveL2 = GetInput.ForcePerLength(this, DA, 8, this.ForcePerLengthUnit);
          Load loadL = new PatchLoad(
            constDeadL1, constLiveL1, finalDeadL1, finalLiveL1, pos1, constDeadL2, constLiveL2, finalDeadL2, finalLiveL2, pos2);
          SetOutput.Item(this, DA, 0, new LoadGoo(loadL));
          break;

        case LoadDistribution.Area:
          Pressure constDeadA1 = GetInput.Stress(this, DA, 0, this.ForcePerAreaUnit);
          Pressure constLiveA1 = GetInput.Stress(this, DA, 1, this.ForcePerAreaUnit);
          Pressure finalDeadA1 = GetInput.Stress(this, DA, 2, this.ForcePerAreaUnit);
          Pressure finalLiveA1 = GetInput.Stress(this, DA, 3, this.ForcePerAreaUnit);
          Pressure constDeadA2 = GetInput.Stress(this, DA, 5, this.ForcePerAreaUnit);
          Pressure constLiveA2 = GetInput.Stress(this, DA, 6, this.ForcePerAreaUnit);
          Pressure finalDeadA2 = GetInput.Stress(this, DA, 7, this.ForcePerAreaUnit);
          Pressure finalLiveA2 = GetInput.Stress(this, DA, 8, this.ForcePerAreaUnit);
          Load loadA = new PatchLoad(
            constDeadA1, constLiveA1, finalDeadA1, finalLiveA1, pos1, constDeadA2, constLiveA2, finalDeadA2, finalLiveA2, pos2);
          SetOutput.Item(this, DA, 0, new LoadGoo(loadA));
          break;
      }
    }

    #region Custom UI
    private ForcePerLengthUnit ForcePerLengthUnit = Units.ForcePerLengthUnit;
    private PressureUnit ForcePerAreaUnit = Units.StressUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;
    private LoadDistribution DistributionType = LoadDistribution.Area;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] {
        "Distribution",
        "Force Unit",
        "Length Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // type
      this.DropDownItems.Add(Enum.GetValues(typeof(LoadDistribution)).Cast<LoadDistribution>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(LoadDistribution.Area.ToString());

      // force unit
      this.DropDownItems.Add(Units.FilteredForcePerAreaUnits);
      this.SelectedItems.Add(this.ForcePerAreaUnit.ToString());

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
      {
        this.DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), this.SelectedItems[i]);
        if (this.DistributionType == LoadDistribution.Line)
        {
          this.DropDownItems[1] = Units.FilteredForcePerLengthUnits;
          this.SelectedItems[1] = this.ForcePerLengthUnit.ToString();
        }
        else
        {
          this.DropDownItems[1] = Units.FilteredForcePerAreaUnits;
          this.SelectedItems[1] = this.ForcePerAreaUnit.ToString();
        }
      }
      if (i == 1)
      {
        if (this.DistributionType == LoadDistribution.Line)
          this.ForcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), this.SelectedItems[i]);
        else
          this.ForcePerAreaUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[i]);
      }
      if (i == 2)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.DistributionType = (LoadDistribution)Enum.Parse(typeof(LoadDistribution), this.SelectedItems[0]);
      if (this.DistributionType == LoadDistribution.Line)
        this.ForcePerLengthUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), this.SelectedItems[1]);
      else
        this.ForcePerAreaUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[1]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation;
      if (DistributionType == LoadDistribution.Line)
        unitAbbreviation = ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);
      else
        unitAbbreviation = Pressure.GetAbbreviation(this.ForcePerAreaUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

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
