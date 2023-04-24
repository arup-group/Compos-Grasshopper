using System;
using System.Drawing;
using ComposAPI;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Components;

namespace ComposGH.Components {
  /// <summary>
  /// Component to check if a Compos model satisfies the chosen code
  /// </summary>
  public class CodeSatisfied : GH_OasysComponent, IGH_VariableParameterComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c402ae16-b8c0-4203-86c7-43c3f2917075");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override Bitmap Icon {
      get {
        if (Status < 2) {
          return Properties.Resources.CodeReqMet;
        } else if (Status == 2) {
          return Properties.Resources.CodeReqNotMet;
        } else {
          return Properties.Resources.CodeReqNotAvailable;
        }
      }
    }

    private int Status = 4;

    public CodeSatisfied() : base("Code Satisfied?", "Code", "Check if a Compos model satisfies the chosen code",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat7()) { Hidden = true; } // sets the initial state of the component to hidden

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    void IGH_VariableParameterComponent.VariableParameterMaintenance() {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Result", "Res", "Result", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      var gh_typ = new GH_ObjectWrapper();
      IMember member = null;
      if (DA.GetData(0, ref gh_typ)) {
        if (gh_typ == null) { return; }

        if (gh_typ.Value is MemberGoo goo) {
          member = (IMember)goo.Value;
          Message = "";
        }
        base.DestroyIconCache();
      }
      if (member != null) {
        Message = member.DesignCode.Code.ToString();
        Status = member.CodeSatisfied();
        if (Status == 1) {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The natural frequency is lower than that required");
        }
        DA.SetData(0, member.GetCodeSatisfiedMessage());
      }
    }
  }
}
