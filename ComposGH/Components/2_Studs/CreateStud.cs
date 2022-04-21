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

namespace ComposGH.Components
{
  public class CreateStud : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("1451E11C-69D0-47D3-8730-FCA80E838E25");
    public CreateStud()
      : base("Create Stud", "Stud", "Create Compos Stud",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStud;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // spacing
        dropdownitems.Add(Enum.GetValues(typeof(StudGroupSpacing.StudSpacingType)).Cast<StudGroupSpacing.StudSpacingType>()
            .Select(x => x.ToString().Replace("_", " ")).ToList());
        selecteditems.Add(StudGroupSpacing.StudSpacingType.Automatic.ToString().Replace("_", " "));

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      if (spacingType.ToString().Replace("_", " ") == selecteditems[i])
        return;

      spacingType = (StudGroupSpacing.StudSpacingType)Enum.Parse(typeof(StudGroupSpacing.StudSpacingType), selecteditems[i].Replace(" ", "_"));

      ModeChangeClicked();

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      spacingType = (StudGroupSpacing.StudSpacingType)Enum.Parse(typeof(StudGroupSpacing.StudSpacingType), selecteditems[0]);

      ModeChangeClicked();

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
            "Spacing Type",
    });

    private bool first = true;
    private StudGroupSpacing.StudSpacingType spacingType = StudGroupSpacing.StudSpacingType.Automatic;
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Stud Dims", "Sdm", "Compos Shear Stud Dimensions", GH_ParamAccess.item);
      pManager.AddGenericParameter("Stud Spec", "Spc", "Compos Shear Stud Specification", GH_ParamAccess.item);
      pManager.AddNumberParameter("Min Saving", "Msm", "Fraction for Minimum Savnig for using Multiple Zones (Default = 0.2 (20%))", GH_ParamAccess.item, 0.2);
      pManager[2].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Stud", "Stu", "Compos Shear Stud", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      StudDimensions studDimensions = GetInput.StudDim(this, DA, 0);
      StudSpecification studSpec = GetInput.StudSpec(this, DA, 1);
      double minSav = 0.2;
      switch (spacingType)
      {
        case StudGroupSpacing.StudSpacingType.Automatic:
        case StudGroupSpacing.StudSpacingType.Min_Num_of_Studs:
          DA.GetData(2, ref minSav);
          DA.SetData(0, new ComposStudGoo(
              new ComposStud(studDimensions, studSpec, minSav, spacingType)));
          break;

        case StudGroupSpacing.StudSpacingType.Partial_Interaction:
          DA.GetData(2, ref minSav);
          double interaction = 0.85;
          DA.GetData(3, ref interaction);
          DA.SetData(0, new ComposStudGoo(
              new ComposStud(studDimensions, studSpec, minSav, interaction)));
          break;

        case StudGroupSpacing.StudSpacingType.Custom:
          List<StudGroupSpacing> spacings = GetInput.StudSpacings(this, DA, 2);
          bool check = false;
          DA.GetData(3, ref check);
          DA.SetData(0, new ComposStudGoo(
              new ComposStud(studDimensions, studSpec, spacings, check)));
          break;
      }
    }
    #region update input params
    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      switch (spacingType)
      {
        case StudGroupSpacing.StudSpacingType.Automatic:
        case StudGroupSpacing.StudSpacingType.Min_Num_of_Studs:
          //remove input parameters
          while (Params.Input.Count > 2)
            Params.UnregisterInputParameter(Params.Input[2], true);

          //add input parameters
          Params.RegisterInputParam(new Param_Number());
          break;

        case StudGroupSpacing.StudSpacingType.Partial_Interaction:
          //remove input parameters
          while (Params.Input.Count > 2)
            Params.UnregisterInputParameter(Params.Input[2], true);

          //add input parameters
          Params.RegisterInputParam(new Param_Number());
          Params.RegisterInputParam(new Param_Number());
          break;

        case StudGroupSpacing.StudSpacingType.Custom:
          //remove input parameters
          while (Params.Input.Count > 2)
            Params.UnregisterInputParameter(Params.Input[2], true);

          //add input parameters
          Params.RegisterInputParam(new Param_GenericObject());
          Params.RegisterInputParam(new Param_Boolean());
          break;
      }
    }
    #endregion

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
      switch (spacingType)
      {
        case StudGroupSpacing.StudSpacingType.Automatic:
        case StudGroupSpacing.StudSpacingType.Min_Num_of_Studs:
          Params.Input[2].Name = "Min Saving";
          Params.Input[2].NickName = "Msm";
          Params.Input[2].Description = "Fraction for Minimum Savnig for using Multiple Zones (Default = 0.2 (20%))";
          Params.Input[2].Optional = true;
          break;

        case StudGroupSpacing.StudSpacingType.Partial_Interaction:
          Params.Input[2].Name = "Min Saving";
          Params.Input[2].NickName = "Msm";
          Params.Input[2].Description = "Fraction for Minimum Savnig for using Multiple Zones (Default = 0.2 (20%))";
          Params.Input[2].Optional = true;
          Params.Input[3].Name = "Interaction";
          Params.Input[3].NickName = "Int";
          Params.Input[3].Description = "Fraction for percentage of interaction for automatic stud spacing (Default = 0.85 (85%))";
          Params.Input[3].Optional = true;
          break;

        case StudGroupSpacing.StudSpacingType.Custom:
          Params.Input[2].Name = "Stud Spacings";
          Params.Input[2].NickName = "Spa";
          Params.Input[2].Description = "List of Custom Compos Shear Stud Spacing";
          Params.Input[2].Access = GH_ParamAccess.list;
          Params.Input[3].Name = "Check Spacing";
          Params.Input[3].NickName = "Chk";
          Params.Input[3].Description = "Check Shear Stud Spacing (default = false)";
          Params.Input[3].Optional = true;
          break;
      }
    }
    #endregion
  }
}
