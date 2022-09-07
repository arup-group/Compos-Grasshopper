using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components
{
  /// <summary>
  /// Component to read utilisations from a Compos model
  /// </summary>
  public class ReadUtilisations : GH_OasysDropDownComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("3afd2171-a09d-4a97-8371-fc27572ea5c1");
    public ReadUtilisations()
      : base("Read Utilisations", "Util", "Read utilisations from a Compos model",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override Bitmap Icon => Properties.Resources.ReadResult;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
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
      IMember member = null;
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ == null) { return; }

        if (gh_typ.Value is MemberGoo)
        {
          MemberGoo goo = (MemberGoo)gh_typ.Value;
          member = (IMember)goo.Value;
          this.Message = "";
        }
      }
      if (member != null)
      {
        Output.SetItem(this, DA, 0, new GH_Number(member.UtilisationFactor(this.Option)));
      }
    }

    #region Custom UI
    private UtilisationFactorOption Option = UtilisationFactorOption.FinalMoment;

    public override void InitialiseDropdowns()
    {
      SpacerDescriptions = new List<string>(new string[] { "Utilisation factor option" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(Enum.GetValues(typeof(UtilisationFactorOption)).Cast<UtilisationFactorOption>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(this.Option.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.Option = (UtilisationFactorOption)Enum.Parse(typeof(UtilisationFactorOption), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.Option = (UtilisationFactorOption)Enum.Parse(typeof(UtilisationFactorOption), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
