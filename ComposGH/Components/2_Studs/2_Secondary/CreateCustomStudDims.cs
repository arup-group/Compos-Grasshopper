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
  public class CreateCustomStudDimensions : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("e70db6bb-b4bf-4033-a3d0-3ad131fe09b1");
    public CreateCustomStudDimensions()
      : base("Custom" + StudDimensionsGoo.Name.Replace(" ", string.Empty),
          "StudDimsCustom",
          "Create a Custom " + StudDimensionsGoo.Description + " for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CustomStudDims;
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
            "Length Unit",
            "Strength Unit"
    });

    private bool First = true;
    private LengthUnit LengthUnit = Units.LengthUnitSection;
    private ForceUnit ForceUnit = Units.ForceUnit;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // length
        DropDownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        // strength
        DropDownItems.Add(Units.FilteredForceUnits);
        SelectedItems.Add(ForceUnit.ToString());

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to length unit
      {
        LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[i]);
      }
      if (i == 1)
      {
        ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), SelectedItems[i]);
      }

        // update name of inputs (to display unit on sliders)
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), SelectedItems[0]);
      ForceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), SelectedItems[1]);

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
      string forceunitAbbreviation = new Force(0, ForceUnit).ToString("a");
      string unitAbbreviation = Length.GetAbbreviation(LengthUnit);

      pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Diameter of stud head", GH_ParamAccess.item);
      pManager.AddGenericParameter("Height [" + unitAbbreviation + "]", "H", "Height of stud", GH_ParamAccess.item);
      pManager.AddGenericParameter("Strength [" + forceunitAbbreviation + "]", "fu", "Stud Character strength", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(StudDimensionsGoo.Name, StudDimensionsGoo.NickName, StudDimensionsGoo.Description + " for a " + StudGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length dia = GetInput.Length(this, DA, 0, LengthUnit, true);
      Length h = GetInput.Length(this, DA, 1, LengthUnit, true);
      Force strengthF = GetInput.Force(this, DA, 2, ForceUnit);
      DA.SetData(0, new StudDimensionsGoo(new StudDimensions(dia, h, strengthF)));
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
      Params.Input[0].Name = "Diameter [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Height [" + unitAbbreviation + "]";

      string forceunitAbbreviation = new Force(0, ForceUnit).ToString("a");
      Params.Input[2].Name = "Strength [" + forceunitAbbreviation + "]";
    }
    #endregion
  }
}
