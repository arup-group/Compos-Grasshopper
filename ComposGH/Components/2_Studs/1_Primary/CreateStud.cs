using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposGH.Parameters;
using System.Linq;
using Grasshopper.Kernel.Parameters;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateStud : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("1451E11C-69D0-47D3-8730-FCA80E838E25");
    public CreateStud()
      : base("Create" + StudGoo.Name.Replace(" ", string.Empty),
          StudGoo.Name.Replace(" ", string.Empty),
          "Create a " + StudGoo.Description + " for a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateStud;
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
      "Spacing Type",
    });

    private bool First = true;
    private StudSpacingType SpacingType = StudSpacingType.Automatic;

    public override void CreateAttributes()
    {
      if (First)
      {
        DropDownItems = new List<List<string>>();
        SelectedItems = new List<string>();

        // spacing
        DropDownItems.Add(Enum.GetValues(typeof(StudSpacingType)).Cast<StudSpacingType>()
            .Select(x => x.ToString().Replace("_", " ")).ToList());
        SelectedItems.Add(StudSpacingType.Automatic.ToString().Replace("_", " "));

        First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, DropDownItems, SelectedItems, SpacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      SelectedItems[i] = DropDownItems[i][j];

      if (SpacingType.ToString().Replace("_", " ") == SelectedItems[i])
        return;

      SpacingType = (StudSpacingType)Enum.Parse(typeof(StudSpacingType), SelectedItems[i].Replace(" ", "_"));

      ModeChangeClicked();

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      SpacingType = (StudSpacingType)Enum.Parse(typeof(StudSpacingType), SelectedItems[0].Replace(" ", "_"));

      ModeChangeClicked();

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
      pManager.AddGenericParameter(StudDimensionsGoo.Name, StudDimensionsGoo.NickName, StudDimensionsGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(StudSpecificationGoo.Name, StudSpecificationGoo.NickName, StudSpecificationGoo.Description, GH_ParamAccess.item);
      pManager.AddNumberParameter("Min Saving", "Msm", "Fraction for Minimum Saving for using Multiple Zones (Default = 0.2 (20%))", GH_ParamAccess.item, 0.2);
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(StudGoo.Name, StudGoo.NickName, StudGoo.Description + " for a " + MemberGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      StudDimensionsGoo studDimensions = (StudDimensionsGoo)GetInput.GenericGoo<StudDimensionsGoo>(this, DA, 0);
      if(studDimensions == null) { return; } // return here on non-optional inputs
      StudSpecificationGoo studSpec = (StudSpecificationGoo)GetInput.GenericGoo<StudSpecificationGoo>(this, DA, 1);
      if (studSpec == null) { return; } // return here on non-optional inputs
      double minSav = 0.2;
      switch (SpacingType)
      {
        case StudSpacingType.Automatic:
        case StudSpacingType.Min_Num_of_Studs:
          DA.GetData(2, ref minSav);
          DA.SetData(0, new StudGoo(
              new Stud(studDimensions.Value, studSpec.Value, minSav, SpacingType)));
          break;

        case StudSpacingType.Partial_Interaction:
          DA.GetData(2, ref minSav);
          double interaction = 0.85;
          DA.GetData(3, ref interaction);
          DA.SetData(0, new StudGoo(
              new Stud(studDimensions.Value, studSpec.Value, minSav, interaction)));
          break;

        case StudSpacingType.Custom:
          List<StudGroupSpacingGoo> spacings = GetInput.GenericGooList<StudGroupSpacingGoo>(this, DA, 2);
          bool check = false;
          DA.GetData(3, ref check);
          DA.SetData(0, new StudGoo(
              new Stud(studDimensions.Value, studSpec.Value, (spacings == null) ? null : spacings.Select(x => x.Value as IStudGroupSpacing).ToList(), check)));
          break;
      }
    }
    #region update input params
    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      switch (SpacingType)
      {
        case StudSpacingType.Automatic:
        case StudSpacingType.Min_Num_of_Studs:
          //remove input parameters
          while (Params.Input.Count > 2)
            Params.UnregisterInputParameter(Params.Input[2], true);

          //add input parameters
          Params.RegisterInputParam(new Param_Number());
          break;

        case StudSpacingType.Partial_Interaction:
          //remove input parameters
          while (Params.Input.Count > 2)
            Params.UnregisterInputParameter(Params.Input[2], true);

          //add input parameters
          Params.RegisterInputParam(new Param_Number());
          Params.RegisterInputParam(new Param_Number());
          break;

        case StudSpacingType.Custom:
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
      switch (SpacingType)
      {
        case StudSpacingType.Automatic:
        case StudSpacingType.Min_Num_of_Studs:
          Params.Input[2].Name = "Min Saving";
          Params.Input[2].NickName = "Msm";
          Params.Input[2].Description = "Fraction for Minimum Savnig for using Multiple Zones (Default = 0.2 (20%))";
          Params.Input[2].Optional = true;
          break;

        case StudSpacingType.Partial_Interaction:
          Params.Input[2].Name = "Min Saving";
          Params.Input[2].NickName = "Msm";
          Params.Input[2].Description = "Fraction for Minimum Savnig for using Multiple Zones (Default = 0.2 (20%))";
          Params.Input[2].Optional = true;
          Params.Input[3].Name = "Interaction";
          Params.Input[3].NickName = "Int";
          Params.Input[3].Description = "Fraction for percentage of interaction for automatic stud spacing (Default = 0.85 (85%))";
          Params.Input[3].Optional = true;
          break;

        case StudSpacingType.Custom:
          Params.Input[2].Name = StudGroupSpacingGoo.Name + "(s)";
          Params.Input[2].NickName = StudGroupSpacingGoo.NickName;
          Params.Input[2].Description = "(Optional) " + StudGroupSpacingGoo.Description;
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
