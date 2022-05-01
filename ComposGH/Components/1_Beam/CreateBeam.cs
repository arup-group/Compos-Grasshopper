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
using ComposAPI.SteelBeam;

namespace ComposGH.Components
{
  public class CreateBeam : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("060641e49fc648eb8d7699f2d6697111");
    public CreateBeam()
      : base("Create Beam", "Beam", "Create a Compos Beam",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateBeam;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

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


      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);


      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[0]);

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
      "Unit",
    });

    private bool first = true;
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddCurveParameter("Line [" + unitAbbreviation + "]", "L", "Line drawn to selected units to create Compos Beam from", GH_ParamAccess.item);
      pManager.AddGenericParameter("Restraint", "Res", "Compos Restraint", GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "SMt", "Compos Steel Material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Beam Sections", "Bs", "Compos Beam Sections or Profile string descriptions like 'CAT IPE IPE200', 'STD I(cm) 20. 19. 8.5 1.27' or 'STD GI 400 300 250 12 25 20'", GH_ParamAccess.list);
      pManager.AddGenericParameter("WebOpening", "WO", "Compos Web Openings or Notches", GH_ParamAccess.list);
      pManager[4].Optional = true;

    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Beam", "Bm", "Compos Beam", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Line ghln = new GH_Line();
      if (DA.GetData(0, ref ghln))
      {
        if (ghln == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Line input is null"); }
        Line ln = new Line();
        if (GH_Convert.ToLine(ghln, ref ln, GH_Conversion.Both))
        {
          Restraint res = GetInput.Restraint(this, DA, 1);

          SteelMaterial mat = GetInput.SteelMaterial(this, DA, 2);

          List<BeamSection> beamSections = GetInput.BeamSections(this, DA, 3);
          try
          {
            if (this.Params.Input[4].Sources.Count > 0)
            {
              List<WebOpening> webOpenings = GetInput.WebOpenings(this, DA, 4);
              DA.SetData(0, new BeamGoo(new LineCurve(ln), lengthUnit, res, mat, beamSections, webOpenings));
            }
            else
            {
              DA.SetData(0, new BeamGoo(new LineCurve(ln), lengthUnit, res, mat, beamSections));
            }
          }
          catch (Exception e)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            return;
          }
        }
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

      Params.Input[0].Name = "Line [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
