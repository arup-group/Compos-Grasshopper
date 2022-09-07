using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreateAxialLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("9dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public CreateAxialLoad()
      : base("CreateAxialLoad", "AxialLoad", "Create an Axial Compos Load applied at both end positions.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Resources.AxialLoad;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      pManager.AddGenericParameter("Const. Dead 1 [" + unitAbbreviation + "]", "dl1", "Start Constant dead load; construction stage dead load which are used for construction stage analysis."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 1 [" + unitAbbreviation + "]", "ll1", "Start Constant live load; construction stage live load which are used for construction stage analysis."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 1 [" + unitAbbreviation + "]", "DL1", "Start Final Dead Load."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 1 [" + unitAbbreviation + "]", "LL1", "Start Final Live Load."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth 1 [" + lengthunitAbbreviation + "]", "dz1", "Start Depth below top of steel where axial load is applied (beam local z-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Dead 2 [" + unitAbbreviation + "]", "dl2", "End Constant dead load; construction stage dead load which are used for construction stage analysis."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live 2 [" + unitAbbreviation + "]", "ll2", "End Constant live load; construction stage live load which are used for construction stage analysis."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead 2 [" + unitAbbreviation + "]", "DL2", "End Final Dead Load."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live 2 [" + unitAbbreviation + "]", "LL2", "End Final Live Load."
        + Environment.NewLine + "Positive axial forces are considered as tensile and negative forces are considered as compressive", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth 2 [" + lengthunitAbbreviation + "]", "dz2", "End Depth below top of steel where axial load is applied (beam local z-axis)", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(LoadGoo.Name, LoadGoo.NickName, LoadGoo.Description + " for a " + MemberGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Force constDead1 = GetInput.Force(this, DA, 0, this.ForceUnit);
      Force constLive1 = GetInput.Force(this, DA, 1, this.ForceUnit);
      Force finalDead1 = GetInput.Force(this, DA, 2, this.ForceUnit);
      Force finalLive1 = GetInput.Force(this, DA, 3, this.ForceUnit);
      Length pos1 = GetInput.Length(this, DA, 4, this.LengthUnit);
      Force constDead2 = GetInput.Force(this, DA, 5, this.ForceUnit);
      Force constLive2 = GetInput.Force(this, DA, 6, this.ForceUnit);
      Force finalDead2 = GetInput.Force(this, DA, 7, this.ForceUnit);
      Force finalLive2 = GetInput.Force(this, DA, 8, this.ForceUnit);
      Length pos2 = GetInput.Length(this, DA, 9, this.LengthUnit);

      Load load = new AxialLoad(
        constDead1, constLive1, finalDead1, finalLive1, pos1, constDead2, constLive2, finalDead2, finalLive2, pos2);
      SetOutput.Item(this, DA, 0, new LoadGoo(load));
    }

    #region Custom UI
    private ForceUnit ForceUnit = Units.ForceUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Force Unit", "Length Unit" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // force unit
      this.DropDownItems.Add(Units.FilteredForceUnits);
      this.SelectedItems.Add(this.ForceUnit.ToString());

      // length
      this.DropDownItems.Add(Units.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      if (i == 1)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
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
