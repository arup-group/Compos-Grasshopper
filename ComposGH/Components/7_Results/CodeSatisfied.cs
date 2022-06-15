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
  /// Component to check if a Compos model satisfies the chosen code
  /// </summary>
  public class CodeSatisfied : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c402ae16-b8c0-4203-86c7-43c3f2917075");
    public CodeSatisfied()
      : base("Code Satisfied?", "Code", "Check if a Compos model satisfies the chosen code",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat7())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary;

    //protected override Bitmap Icon => Properties.Resources.CodeSatisfied;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Model", "Mod", "Compos model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Member", "Mem", "Compos member", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "Result", GH_ParamAccess.item);
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
        int status = file.CodeSatisfied(member.Name);
        switch (status)
        {
          case 0:
            this.Message = "all code requirements are met";
            break;
          case 1:
            this.Message = "except the natural frequency is lower than that required, other code requirements are met";
            break;
          case 2:
            this.Message = "one or more code requirements are not metf";
            break;
          case 3:
            this.Message = "the given member name is not valid";
            break;
          case 4:
            this.Message = "there is no results for the given named member";
            break;

        }
        DA.SetData(0, new GH_Number(status));
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