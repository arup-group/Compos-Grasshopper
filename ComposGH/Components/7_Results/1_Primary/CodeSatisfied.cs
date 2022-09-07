using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ComposAPI;
using ComposGH.Parameters;
using OasysGH.Components;

namespace ComposGH.Components
{
  /// <summary>
  /// Component to check if a Compos model satisfies the chosen code
  /// </summary>
  public class CodeSatisfied : GH_OasysComponent, IGH_VariableParameterComponent
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

    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
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
        base.DestroyIconCache();
      }
      if (member != null)
      {
        this.Message = member.GetCodeSatisfiedMessage();
        this.Status = member.CodeSatisfied();
        if(this.Status == 1)
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The natural frequency is lower than that required");
        DA.SetData(0, new GH_Number(Status));
      }
    }
    int Status = 4;
    protected override Bitmap Icon
    {
      get
      {
        if (Status < 2)
          return Properties.Resources.CodeReqMet;
        else if (Status == 2)
          return Properties.Resources.CodeReqNotMet;
        else
          return Properties.Resources.CodeReqNotAvailable;
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