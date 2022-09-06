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
  /// Component to read results from a Compos model
  /// </summary>
  public class ReadResults : GH_OasysDropDownComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("3517ea0c-5e7f-42b0-aef4-78301b097b2b");
    public ReadResults()
      : base("Read Results", "Results", "Reads results from a Compos model",
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
      //pManager.AddIntegerParameter("Position", "Pos", "(Optional) Position number", GH_ParamAccess.item, 0);
      //pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "Result", GH_ParamAccess.list);
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
        List<GH_Number> result = new List<GH_Number>();
        for (short pos = 0; pos < member.NumIntermediatePos(); pos++)
        {
          result.Add(new GH_Number(member.Result(this.Option.ToString(), Convert.ToInt16(pos))));
        }

        SetOutput.List(this, DA, 0, result);
      }
    }

    #region Custom UI
    private ResultOption Option = ResultOption.CRITI_SECT_DIST;

    internal override void InitialiseDropdowns()
    {
      SpacerDescriptions = new List<string>(new string[] { "Result option" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(Enum.GetValues(typeof(ResultOption)).Cast<ResultOption>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(this.Option.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.Option = (ResultOption)Enum.Parse(typeof(ResultOption), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.Option = (ResultOption)Enum.Parse(typeof(ResultOption), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
