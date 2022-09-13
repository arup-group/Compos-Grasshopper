using System;
using System.Collections.Generic;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH.Components;
using OasysGH.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Components
{
  public class CreatePointLoad : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4dfed0d2-3ad1-49e6-a8d8-d5a5fd851a64");
    public CreatePointLoad()
      : base("CreatePointLoad", "PointLoad", "Create a concentrated Compos Point Load.",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat4())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Resources.PointLoad;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      pManager.AddGenericParameter("Const. Dead [" + unitAbbreviation + "]", "dl", "Constant dead load; construction stage dead load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Const. Live [" + unitAbbreviation + "]", "ll", "Constant live load; construction stage live load which are used for construction stage analysis", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Dead [" + unitAbbreviation + "]", "DL", "Final Dead Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Final Live [" + unitAbbreviation + "]", "LL", "Final Live Load", GH_ParamAccess.item);
      pManager.AddGenericParameter("Pos [" + lengthunitAbbreviation + "]", "Px", "Position on beam where Point Load acts (beam local x-axis)."
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set position as percentage", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposLoadParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Force constDead = (Force)Input.UnitNumber(this, DA, 0, this.ForceUnit);
      Force constLive = (Force)Input.UnitNumber(this, DA, 1, this.ForceUnit);
      Force finalDead = (Force)Input.UnitNumber(this, DA, 2, this.ForceUnit);
      Force finalLive = (Force)Input.UnitNumber(this, DA, 3, this.ForceUnit);
      IQuantity pos = Input.LengthOrRatio(this, DA, 4, this.LengthUnit);

      Load load = new PointLoad(constDead, constLive, finalDead, finalLive, pos);
      Output.SetItem(this, DA, 0, new LoadGoo(load));
    }

    #region Custom UI
    private ForceUnit ForceUnit = Units.ForceUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void InitialiseDropdowns()
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

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      if (i == 1)
        this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);

      base.UpdateUIFromSelectedItems();
    }
    
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
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
