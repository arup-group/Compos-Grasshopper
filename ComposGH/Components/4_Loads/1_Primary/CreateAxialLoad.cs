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
  public class CreateAxialLoad : GH_OasysComponent, IGH_VariableParameterComponent
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

    protected override System.Drawing.Bitmap Icon => Properties.Resources.AxialLoad;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 

    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Force Unit",
      "Length Unit"
    });

    private bool First = true;
    private ForceUnit ForceUnit = Units.ForceUnit;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // force unit
        DropDownItems.Add(Units.FilteredForceUnits);
        SelectedItems.Add(ForceUnit.ToString());

        // length
        DropDownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), SelectedItems[i]);
      if (i == 1)
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), SelectedItems[0]);
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[1]);

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
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(LengthUnit);
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
      Force constDead1 = GetInput.Force(this, DA, 0, ForceUnit);
      Force constLive1 = GetInput.Force(this, DA, 1, ForceUnit);
      Force finalDead1 = GetInput.Force(this, DA, 2, ForceUnit);
      Force finalLive1 = GetInput.Force(this, DA, 3, ForceUnit);
      Length pos1 = GetInput.Length(this, DA, 4, LengthUnit);
      Force constDead2 = GetInput.Force(this, DA, 5, ForceUnit);
      Force constLive2 = GetInput.Force(this, DA, 6, ForceUnit);
      Force finalDead2 = GetInput.Force(this, DA, 7, ForceUnit);
      Force finalLive2 = GetInput.Force(this, DA, 8, ForceUnit);
      Length pos2 = GetInput.Length(this, DA, 9, LengthUnit);

      Load load = new AxialLoad(
        constDead1, constLive1, finalDead1, finalLive1, pos1, constDead2, constLive2, finalDead2, finalLive2, pos2);
      DA.SetData(0, new LoadGoo(load));
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, DropDownItems, SelectedItems, SpacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref DropDownItems, ref SelectedItems, ref SpacerDescriptions);

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
      string unitAbbreviation = Force.GetAbbreviation(ForceUnit);
      string lengthunitAbbreviation = Length.GetAbbreviation(LengthUnit);
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
