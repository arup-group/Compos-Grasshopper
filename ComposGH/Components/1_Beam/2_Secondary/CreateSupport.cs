using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using ComposGH.Parameters;
using UnitsNet;
using UnitsNet.Units;
using System.Linq;
using Grasshopper.Kernel.Parameters;
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
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateSupport;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // type
        dropdownitems.Add(Enum.GetValues(typeof(IntermediateRestraint)).Cast<IntermediateRestraint>()
            .Select(x => x.ToString().Replace("__", "-").Replace("_", " ")).ToList());
        dropdownitems[0].RemoveAt(dropdownitems[0].Count - 1);
        selecteditems.Add(dropdownitems[0][0]);

        // length
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (i == 0)
      {
        string typ = selecteditems[i].ToString().Replace("-", "__").Replace(" ", "_");
        resType = (IntermediateRestraint)Enum.Parse(typeof(IntermediateRestraint), typ);
      }
      if (i == 1)
        lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      string typ = selecteditems[0].ToString().Replace("-", "__").Replace(" ", "_");
      resType = (IntermediateRestraint)Enum.Parse(typeof(IntermediateRestraint), typ);
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
      "Intermediate Sup.",
      "Unit"
    });

    private bool first = true;
    private IntermediateRestraint resType = IntermediateRestraint.None;
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthUnit);
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
        List<Length> restrs = GetInput.Lengths(this, DA, 2, lengthUnit);
        selecteditems[0] = "Custom";
        Supports sup = new Supports(restrs, smir, ffre);
        DA.SetData(0, new SupportsGoo(sup));
      }
      else
      {
        Supports sup = new Supports(resType, smir, ffre);
        DA.SetData(0, new SupportsGoo(sup));
      }
    }

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Helpers.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Helpers.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

      UpdateUIFromSelectedItems();

      first = false;

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
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      Params.Input[2].Name = "Restraint Pos [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
