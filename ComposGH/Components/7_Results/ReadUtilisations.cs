using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.UI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace ComposGH.Components
{
  /// <summary>
  /// Component to read utilisations from a Compos model
  /// </summary>
  public class ReadUtilisations : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("3afd2171-a09d-4a97-8371-fc27572ea5c1");
    public ReadUtilisations()
      : base("Get Utilisations", "Util", "Read utilisations from a Compos model",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat6())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => Properties.Resources.SaveModel;
    #endregion

    #region Custom UI
    // This region overrides the typical component layout

    // list of lists with all dropdown lists conctent
    List<List<string>> DropdownItems;
    // list of selected items
    List<string> SelectedItems;
    // list of descriptions 
    List<string> SpacerDescriptions = new List<string>(new string[]
    {
      "Utilisation factor option"
    });

    private bool First = true;
    private UtilisationFactorOption Option = UtilisationFactorOption.FinalMoment;

    public override void CreateAttributes()
    {
      if (this.First)
      {
        this.DropdownItems = new List<List<string>>();
        this.SelectedItems = new List<string>();

        this.DropdownItems.Add(Enum.GetValues(typeof(UtilisationFactorOption)).Cast<UtilisationFactorOption>().Select(x => x.ToString()).ToList());
        this.SelectedItems.Add(this.Option.ToString());

        this.First = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, this.DropdownItems, this.SelectedItems, this.SpacerDescriptions);
    }

    public void SetSelected(int i, int j)
    {
      // change selected item
      this.SelectedItems[i] = this.DropdownItems[i][j];

      if (i == 0)
      {
        this.Option = (UtilisationFactorOption)Enum.Parse(typeof(UtilisationFactorOption), this.SelectedItems[i]);
      }

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      this.Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      this.Option = (UtilisationFactorOption)Enum.Parse(typeof(UtilisationFactorOption), this.SelectedItems[0]);

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
      pManager.AddGenericParameter("Model", "Mod", "Compos model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Member", "Mem", "Compos member", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Util", "U", "Utilisation", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      IComposFile file = null;
      IMember member = null;
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ == null) { return; }
        if (gh_typ.Value is ComposFileGoo)
        {
          ComposFileGoo goo = (ComposFileGoo)gh_typ.Value;
          file = (ComposFile)goo.Value;
          Message = "";
        }
      }
      if (DA.GetData(1, ref gh_typ))
      {
        if (gh_typ == null) { return; }

        if (gh_typ.Value is MemberGoo)
        {
          MemberGoo goo = (MemberGoo)gh_typ.Value;
          member = (IMember)goo.Value;
          this.Message = "";
        }
      }
      if (file != null && member != null)
      {
        // should we check code satisfaction here?
        int status = file.CodeSatisfied(member.Name);
        /// <returns>The return values are:
        /// 0 - all code requirements are met
        /// 1 - except the natural frequency is lower than that required, other code requirements are met
        /// 2 - one or more code requirements are not met
        /// 3 - the given member name is not valid
        /// 4 - there is no results for the given named member
        /// </returns>

        status = file.Analyse();
        if (status == 1)
        {
          this.Message = "One or more members failed";
          return;
        }
        DA.SetData(0, new GH_Number(file.UtilisationFactor(member.Name, this.Option)));
      }
    }

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
    }
    #endregion
  }
}