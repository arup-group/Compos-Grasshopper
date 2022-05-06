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
  public class CreateSupport : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("71c87cde-f442-475b-9131-8f2974c42499");
    public CreateSupport()
      : base("Restraint Support", "Support", "Create Support for a Compos Restraint",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateSupport;
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
      "Intermediate Sup.",
      "Unit"
    });

    private bool First = true;
    private IntermediateRestraint RestraintType = IntermediateRestraint.None;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropdownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // type
        DropdownItems.Add(Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList());
        DropdownItems[0].RemoveAt(DropdownItems[0].Count - 1);
        SelectedItems.Add(DropdownItems[0][0]);

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
        string typ = SelectedItems[i].ToString().Replace("-", "__").Replace(" ", "_");
        RestraintType = (IntermediateRestraint)Enum.Parse(typeof(IntermediateRestraint), typ);
      }
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
      string typ = SelectedItems[0].ToString().Replace("-", "__").Replace(" ", "_");
      RestraintType = (IntermediateRestraint)Enum.Parse(typeof(IntermediateRestraint), typ);
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
      IQuantity length = new Length(0, LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddBooleanParameter("Sec. mem. interm. res.", "SMIR", "Take secondary member as intermediate restraint (default = true)", GH_ParamAccess.item, true);
      pManager.AddBooleanParameter("Flngs. free rot. ends", "FFRE", "Both flanges are free to rotate on plan at end restraints (default = true)", GH_ParamAccess.item, true);
      pManager.AddGenericParameter("Restraint Pos [" + unitAbbreviation + "]", "RPxs", "(Optional) Custom defined intermediate restraints Positions along the beam (beam x-axis)", GH_ParamAccess.list);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Support conditions", "Sup", "Support conditions for a Compos Restraint", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      bool smir = true;
      DA.GetData(0, ref smir);
      bool ffre = true;
      DA.GetData(1, ref ffre);

      if (this.Params.Input[2].Sources.Count > 0)
      {
        List<Length> restrs = GetInput.Lengths(this, DA, 2, LengthUnit);
        SelectedItems[0] = "Custom";
        Supports sup = new Supports(restrs, smir, ffre);
        DA.SetData(0, new SupportsGoo(sup));
      }
      else
      {
        Supports sup = new Supports(RestraintType, smir, ffre);
        DA.SetData(0, new SupportsGoo(sup));
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
      IQuantity length = new Length(0, LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      Params.Input[2].Name = "Restraint Pos [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
