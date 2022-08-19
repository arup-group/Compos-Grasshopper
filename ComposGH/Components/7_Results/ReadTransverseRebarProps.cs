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
  /// Component to read transverse rebar properties from a Compos model
  /// </summary>
  public class ReadTransverseRebarProps : GH_OasysDropDownComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("10f23315-ff32-43aa-8f0f-7fd6bdd2d003");
    public ReadTransverseRebarProps()
      : base("Read Transverse Rebar Properties", "TranRebar", "Read transverse rebar properties from a Compos model",
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
      pManager.AddGenericParameter("TranRebarProp", "TRP", "Transverse rebar property", GH_ParamAccess.list);
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
        for (short num = 0; num < member.NumTranRebar(); num++)
        {
          result.Add(new GH_Number(member.TranRebarProp(this.Option, Convert.ToInt16(num))));
        }

        DA.SetDataList(0, result);
      }
    }

    #region Custom UI
    private TransverseRebarOption Option = TransverseRebarOption.REBAR_DIST_LEFT_SIDE;

    internal override void InitialiseDropdowns()
    {
      SpacerDescriptions = new List<string>(new string[] { "Transverse rebar option" });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      this.DropDownItems.Add(Enum.GetValues(typeof(TransverseRebarOption)).Cast<TransverseRebarOption>().Select(x => x.ToString()).ToList());
      this.SelectedItems.Add(this.Option.ToString());

      this.IsInitialised = true;
    }

    internal override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0)
        this.Option = (TransverseRebarOption)Enum.Parse(typeof(TransverseRebarOption), this.SelectedItems[i]);

      base.UpdateUI();
    }

    internal override void UpdateUIFromSelectedItems()
    {
      this.Option = (TransverseRebarOption)Enum.Parse(typeof(TransverseRebarOption), this.SelectedItems[0]);

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
