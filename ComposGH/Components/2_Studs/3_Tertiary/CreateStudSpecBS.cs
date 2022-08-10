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
  public class CreateStudSpecBS : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("418590eb-52b0-455b-96e7-36df966d328f");
    public CreateStudSpecBS()
      : base("StandardBS" + StudSpecificationGoo.Name.Replace(" ", string.Empty),
          "StudSpecsBS",
          "Look up a Standard BS " + StudSpecificationGoo.Description + " for a " + StudGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StandardStudSpecsBS;
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
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (LengthUnit.ToString() == SelectedItems[i])
        return;

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

      pManager.AddGenericParameter("No Stud Zone Start [" + unitAbbreviation + "]",
          "NSZS", "Length of zone without shear studs at the start of the beam (default = 0)", GH_ParamAccess.item);
      pManager.AddGenericParameter("No Stud Zone End [" + unitAbbreviation + "]",
          "NSZE", "Length of zone without shear studs at the end of the beam (default = 0)", GH_ParamAccess.item);
      pManager.AddBooleanParameter("EC4 Limit", "Lim", "Use 'Eurocode 4'limit on minimum percentage of shear interaction if it is worse than BS5950", GH_ParamAccess.item, true);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(StudSpecificationGoo.Name, StudSpecificationGoo.NickName, "BS " + StudSpecificationGoo.Description + " for a " + StudGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // get default length inputs used for all cases
      Length noStudZoneStart = Length.Zero;
      if (this.Params.Input[0].Sources.Count > 0)
        noStudZoneStart = GetInput.Length(this, DA, 0, LengthUnit, true);
      Length noStudZoneEnd = Length.Zero;
      if (this.Params.Input[1].Sources.Count > 0)
        noStudZoneEnd = GetInput.Length(this, DA, 1, LengthUnit, true);

      bool ec4 = true;
      DA.GetData(2, ref ec4);

      StudSpecification specBS = new StudSpecification(
          ec4, noStudZoneStart, noStudZoneEnd);
      DA.SetData(0, new StudSpecificationGoo(specBS));
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
      Params.Input[0].Name = "No Stud Zone Start [" + unitAbbreviation + "]";
      Params.Input[1].Name = "No Stud Zone End [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
