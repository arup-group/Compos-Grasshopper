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
  public class CreateCustomTransverseReinforcementLayout : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public CreateCustomTransverseReinforcementLayout()
      : base("Create" + CustomTransverseReinforcementLayoutGoo.Name.Replace(" ", string.Empty),
          CustomTransverseReinforcementLayoutGoo.Name.Replace(" ", string.Empty),
          "Create a " + CustomTransverseReinforcementLayoutGoo.Description + " for a " + TransverseReinforcementGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override Guid ComponentGuid => new Guid("19322156-8b1a-4849-9772-813411af965c");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CustomRebarLayout;
    #endregion

    #region Custom UI
    // This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Unit",
    });

    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitSection;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

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
      SelectedItems[i] = DropDownItems[i][j];

      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[0]);

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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Start Pos x [" + unitAbbreviation + "]", "PxS", "Start Position where this Rebar Spacing Groups begins on Beam (beam local x-axis)" 
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set positions as percentage (-0.5 => 50%)", GH_ParamAccess.item);
      pManager.AddGenericParameter("End Pos x [" + unitAbbreviation + "]", "PxE", "End Position where this Rebar Spacing Groups begins on Beam (beam local x-axis)"
        + System.Environment.NewLine + "HINT: You can input a negative decimal fraction value to set positions as percentage (-0.5 => 50 %)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Transverse rebar diameter", GH_ParamAccess.item);
      pManager.AddGenericParameter("Spacing [" + unitAbbreviation + "]", "S", "The centre/centre distance between rebars in this group (along beam local x-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);

    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(CustomTransverseReinforcementLayoutGoo.Name, CustomTransverseReinforcementLayoutGoo.NickName, CustomTransverseReinforcementLayoutGoo.Description + " for a " + TransverseReinforcementGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      IQuantity start = GetInput.LengthOrRatio(this, DA, 0, LengthUnit);
      IQuantity end = GetInput.LengthOrRatio(this, DA, 1, LengthUnit);
      Length dia = GetInput.Length(this, DA, 2, LengthUnit);
      Length spacing = GetInput.Length(this, DA, 3, LengthUnit);
      Length cov = GetInput.Length(this, DA, 4, LengthUnit);

      DA.SetData(0, new CustomTransverseReinforcementLayoutGoo(new CustomTransverseReinforcementLayout(start, end, dia, spacing, cov)));
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
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);
      int i = 0;
      Params.Input[i++].Name = "Start Pos x [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "End Pos x [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Diameter [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Spacing [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "Cover [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
