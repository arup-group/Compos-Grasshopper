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
  /// Component to read beam results from a Compos model
  /// </summary>
  public class ReadBeamResults : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c91c7cfa-63fc-494f-9956-08aaec46b00c");
    public ReadBeamResults()
      : base("Read Beam Results", "Beam", "Reads beam results from a Compos model",
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
      pManager.AddGenericParameter("Result", "Res", "Beam result", GH_ParamAccess.list);
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

        Output.SetList(this, DA, 0, result);
      }
    }

    #region Custom UI
    private BeamResultOption Option = BeamResultOption.GIRDER_WELD_THICK_T;

    public override void InitialiseDropdowns()
    {
      SpacerDescriptions = new List<string>(new string[] { "Beam result option" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(Enum.GetValues(typeof(BeamResultOption)).Cast<BeamResultOption>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(this.Option.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.Option = (BeamResultOption)Enum.Parse(typeof(BeamResultOption), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.Option = (BeamResultOption)Enum.Parse(typeof(BeamResultOption), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
