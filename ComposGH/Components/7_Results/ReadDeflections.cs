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
  /// Component to read deflections from a Compos model
  /// </summary>
  public class ReadDeflections : GH_OasysDropDownComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("809777e9-97ba-4ccf-a2ca-5d332c734d43");
    public ReadDeflections()
      : base("Read Deflections", "Deflect", "Reads deflections from a Compos model",
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
      pManager.AddGenericParameter("Deflection", "Def", "Deflection", GH_ParamAccess.list);
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
    private DeflectionOption Option = DeflectionOption.DEFL_CONS_DEAD_LOAD;

    public override void InitialiseDropdowns()
    {
      SpacerDescriptions = new List<string>(new string[] { "Deflection option" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(Enum.GetValues(typeof(DeflectionOption)).Cast<DeflectionOption>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(this.Option.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.Option = (DeflectionOption)Enum.Parse(typeof(DeflectionOption), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.Option = (DeflectionOption)Enum.Parse(typeof(DeflectionOption), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
