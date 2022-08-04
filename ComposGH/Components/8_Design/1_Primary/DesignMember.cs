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
  public class DesignMember : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c422ae16-b8c0-4203-86c7-43c3f2917075");
    public DesignMember()
      : base("Design Member", "Design", "Design (size) the steel beam of a Compos Member",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary;

    //protected override Bitmap Icon => Properties.Resources.CodeSatisfied;
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
      MemberGoo goo = (MemberGoo)GetInput.GenericGoo<MemberGoo>(this, DA, 0);
      IMember member = goo.Value;
      
      if (member != null)
      {
        //member.File.
        //DA.SetData(0, new GH_Number(status));
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