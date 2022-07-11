using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

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
    List<List<string>> DropDownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Intermediate Sup.",
      "Unit"
    });
    List<bool> OverrideDropDownItems;
    private bool First = true;
    private IntermediateRestraint RestraintType = IntermediateRestraint.None;
    private LengthUnit LengthUnit = Units.LengthUnitGeometry;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // type
        DropDownItems.Add(Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList());
        DropDownItems[0].RemoveAt(DropDownItems[0].Count - 1);
        SelectedItems.Add(DropDownItems[0][0]);

        // length
        DropDownItems.Add(Units.FilteredLengthUnits);
        SelectedItems.Add(LengthUnit.ToString());

        OverrideDropDownItems = new List<bool>() { false, false };
        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropDownItems[i][j];

      if (i == 0)
      {
        this.ParseRestraintType(this.SelectedItems[0]);
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
      this.ParseRestraintType(this.SelectedItems[0]);
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[1]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      this.Params.OnParametersChanged();
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
      pManager.AddGenericParameter("Int. Support", "ISup", "(Optional) Intermediate support", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Support conditions", "Sup", "Support conditions for a Compos Restraint", GH_ParamAccess.item);
    }
    #endregion

    private void ParseRestraintType(string value)
    {
      if (value != "-")
      {
        value = value.ToString().Replace("-", "__").Replace(" ", "_");
        this.RestraintType = (IntermediateRestraint)Enum.Parse(typeof(IntermediateRestraint), value);
      }
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // override intermediate support?
      if (this.Params.Input[3].Sources.Count > 0)
      {
        string restraintType = "";
        DA.GetData(3, ref restraintType);
        try
        {
          this.ParseRestraintType(restraintType);
          this.DropDownItems[0] = new List<string>();
          this.SelectedItems[0] = "-";
          this.OverrideDropDownItems[0] = true;
        }
        catch (ArgumentException)
        {
          string text = "Could not parse intermediate restraint. Valid options are ";
          foreach (string g in Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>().Select(x => x.ToString()).ToList())
          {
            text += g + ", ";
          }
          text = text.Remove(text.Length - 2);
          text += ".";
          this.DropDownItems[0] = Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>().Select(x => x.ToString()).ToList();
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
        }
      }
      else if (this.OverrideDropDownItems[0])
      {
        this.DropDownItems[0] = Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>().Select(x => x.ToString()).ToList();
        this.OverrideDropDownItems[0] = false;
      }

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
      IQuantity length = new Length(0, LengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      Params.Input[2].Name = "Restraint Pos [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
