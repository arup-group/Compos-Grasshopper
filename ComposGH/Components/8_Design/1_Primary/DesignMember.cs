using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using System;
using System.Drawing;

namespace ComposGH.Components {
  /// <summary>
  /// Component to check if a Compos model satisfies the chosen code
  /// </summary>
  public class DesignMember : GH_OasysComponent {
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c422ae16-b8c0-4203-86c7-43c3f2917075");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.DesignMember;

    public DesignMember()
      : base("DesignMember", "Design", "Design (size) the Steel Beam of a Compos Member",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat8()) { Hidden = true; } // sets the initial state of the component to hidden

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter());
      pManager.AddParameter(new DesignCriteriaParam());
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter(), MemberGoo.Name, MemberGoo.NickName, "Designed " + MemberGoo.Description, GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      MemberGoo memGoo = (MemberGoo)Input.GenericGoo<MemberGoo>(this, DA, 0);
      DesignCriteriaGoo critGoo = (DesignCriteriaGoo)Input.GenericGoo<DesignCriteriaGoo>(this, DA, 1);
      Message = "";
      if (memGoo.Value != null) {
        Member designedMember = (Member)memGoo.Value.Duplicate();
        designedMember.DesignCriteria = critGoo.Value;
        Message = designedMember.GetCodeSatisfiedMessage();
        if (!designedMember.Design()) {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to design member");
          return;
        }
        string[] oldProfile = memGoo.Value.Beam.Sections[0].SectionDescription.Split(' ');
        string[] newProfile = designedMember.Beam.Sections[0].SectionDescription.Split(' ');
        if (newProfile[2] == oldProfile[2]) {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to design member");
          return;
        }
        Message = newProfile[2];
        DA.SetData(0, new MemberGoo(designedMember));
      }
    }
  }
}
