using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;

namespace ComposGH.Components
{
  public class CreateStud : GH_OasysDropDownComponent
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

    protected override System.Drawing.Bitmap Icon => Resources.CreateStud;
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
      if (studDimensions == null) { return; } // return here on non-optional inputs
      StudSpecificationGoo studSpec = (StudSpecificationGoo)GetInput.GenericGoo<StudSpecificationGoo>(this, DA, 1);
      if (studSpec == null) { return; } // return here on non-optional inputs
      double minSav = 0.2;
      switch (SpacingType)
      {
        case StudSpacingType.Automatic:
        case StudSpacingType.Min_Num_of_Studs:
          DA.GetData(2, ref minSav);
          SetOutput.Item(this, DA, 0, new StudGoo(
              new Stud(studDimensions.Value, studSpec.Value, minSav, SpacingType)));
          break;

        case StudSpacingType.Partial_Interaction:
          DA.GetData(2, ref minSav);
          double interaction = 0.85;
          DA.GetData(3, ref interaction);
          SetOutput.Item(this, DA, 0, new StudGoo(
              new Stud(studDimensions.Value, studSpec.Value, minSav, interaction)));
          break;

        case StudSpacingType.Custom:
          List<StudGroupSpacingGoo> spacings = GetInput.GenericGooList<StudGroupSpacingGoo>(this, DA, 2);
          bool check = false;
          DA.GetData(3, ref check);
          SetOutput.Item(this, DA, 0, new StudGoo(
              new Stud(studDimensions.Value, studSpec.Value, spacings?.Select(x => x.Value as IStudGroupSpacing).ToList(), check)));
          break;
      }
    }
    
    #region Custom UI
    private StudSpacingType SpacingType = StudSpacingType.Min_Num_of_Studs;

    internal override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] { "Spacing Type" });
      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();
      // spacing
      this.DropDownItems.Add(Enum.GetValues(typeof(StudSpacingType)).Cast<StudSpacingType>()
          .Select(x => x.ToString().Replace("_", " ")).ToList());
      this.SelectedItems.Add(SpacingType.ToString().Replace("_", " "));
      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (SpacingType.ToString().Replace("_", " ") == this.SelectedItems[i])
        return;
      this.SpacingType = (StudSpacingType)Enum.Parse(typeof(StudSpacingType), this.SelectedItems[i].Replace(" ", "_"));
      this.ModeChangeClicked();
      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.SpacingType = (StudSpacingType)Enum.Parse(typeof(StudSpacingType), this.SelectedItems[0].Replace(" ", "_"));
      this.ModeChangeClicked();
      base.UpdateUIFromSelectedItems();
    }

    private void ModeChangeClicked()
    {
      RecordUndoEvent("Changed Parameters");

      switch (this.SpacingType)
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

    public override void VariableParameterMaintenance()
    {
      switch (this.SpacingType)
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
