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
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateCustomTransverseReinforcementLayout : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public CreateCustomTransverseReinforcementLayout()
      : base("Custom Rebar Layout", "CustTransRbL", "Create Custom Transverse Reinforcement Layout for Compos Transverse Reinforcement",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat3())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override Guid ComponentGuid => new Guid("19322156-8b1a-4849-9772-813411af965c");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.TransverseReinforcement;
    #endregion

    #region Custom UI
    // This region overrides the typical component layout
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
    private LengthUnit lengthUnit = Units.LengthUnitSection;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthUnit);
      string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Material", "RMt", "Reinforcement Material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Start Pos x [" + unitAbbreviation + "]", "PxS", "Start Position where this Rebar Spacing Groups begins on Beam (beam local x-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("End Pos x [" + unitAbbreviation + "]", "PxE", "End Position where this Rebar Spacing Groups begins on Beam (beam local x-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Diameter [" + unitAbbreviation + "]", "Ø", "Transverse rebar diameter", GH_ParamAccess.item);
      pManager.AddGenericParameter("Spacing [" + unitAbbreviation + "]", "S", "The centre/centre distance between rebars in this group (along beam local x-axis)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Cover [" + unitAbbreviation + "]", "Cov", "Reinforcement cover", GH_ParamAccess.item);

    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Reinforcement", "Rb", "Transverse Reinforcement for Compos Slab", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      Length start = GetInput.Length(this, DA, 0, lengthUnit);
      Length end = GetInput.Length(this, DA, 1, lengthUnit);
      Length dia = GetInput.Length(this, DA, 2, lengthUnit);
      Length spacing = GetInput.Length(this, DA, 3, lengthUnit);
      Length cov = GetInput.Length(this, DA, 4, lengthUnit);

      DA.SetData(0, new CustomTransverseReinforcmentLayoutGoo(new ComposAPI.CustomTransverseReinforcementLayout(start, end, dia, spacing, cov)));
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

      Params.Input[0].Name = "Start Pos x [" + unitAbbreviation + "]";
      Params.Input[1].Name = "End Pos x [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Diameter [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Spacing [" + unitAbbreviation + "]";
      Params.Input[4].Name = "Cover [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
