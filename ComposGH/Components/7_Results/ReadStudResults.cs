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
  /// Component to read stud results from a Compos model
  /// </summary>
  public class ReadStudResults : GH_OasysDropDownComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("b7542409-348f-4c6e-b082-9509f9f4d78c");
    public ReadStudResults()
      : base("Read Stud Results", "Studs", "Reads stud results from a Compos model",
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
      pManager.AddGenericParameter("Result", "Res", "Stud results", GH_ParamAccess.list);
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

        DA.SetDataList(0, result);
      }
    }

    #region Custom UI
    private StudResultOption Option = StudResultOption.STUD_CONCRTE_FORCE;

    internal override void InitialiseDropdowns()
    {
      SpacerDescriptions = new List<string>(new string[] { "Stud result option" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(Enum.GetValues(typeof(StudResultOption)).Cast<StudResultOption>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(this.Option.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.Option = (StudResultOption)Enum.Parse(typeof(StudResultOption), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.Option = (StudResultOption)Enum.Parse(typeof(StudResultOption), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
