using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using Grasshopper.Kernel;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components {
  public class CreateMember : GH_OasysComponent {
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("e7eafcec-ede6-4d60-9fcb-5392cb878581");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Resources.CreateMember;

    public CreateMember() : base("Create" + MemberGoo.Name.Replace(" ", string.Empty),
      MemberGoo.Name.Replace(" ", string.Empty),
      "Create a " + MemberGoo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5()) { Hidden = false; } // sets the initial state of the component to hidden

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new ComposBeamParameter());
      pManager.AddParameter(new ComposStudParameter());
      pManager.AddParameter(new ComposSlabParameter());
      pManager.AddParameter(new ComposLoadParameter(), LoadGoo.Name + "(s)", LoadGoo.NickName, LoadGoo.Description, GH_ParamAccess.list);
      pManager.AddParameter(new DesignCodeParam());
      pManager.AddTextParameter("Name", "Na", "Set Member Name", GH_ParamAccess.item);
      pManager.AddTextParameter("GridRef", "Grd", "(Optional) Set Member's Grid Reference", GH_ParamAccess.item);
      pManager.AddTextParameter("Note", "Nt", "(Optional) Set Notes about the Member", GH_ParamAccess.item);
      pManager[6].Optional = true;
      pManager[7].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new ComposMemberParameter());
    }

    protected override void SolveInstance(IGH_DataAccess DA) {
      var beam = (BeamGoo)Input.GenericGoo<BeamGoo>(this, DA, 0);
      var stud = (StudGoo)Input.GenericGoo<StudGoo>(this, DA, 1);
      var slab = (SlabGoo)Input.GenericGoo<SlabGoo>(this, DA, 2);
      List<LoadGoo> loads = Input.GenericGooList<LoadGoo>(this, DA, 3);
      var code = (DesignCodeGoo)Input.GenericGoo<DesignCodeGoo>(this, DA, 4);

      string name = "";
      DA.GetData(5, ref name);
      name = name.Trim();
      string gridRef = "";
      DA.GetData(6, ref gridRef);
      gridRef = gridRef.Trim();
      string note = "";
      DA.GetData(7, ref note);
      note = note.Trim();

      var member = new Member(name, gridRef, note, code.Value, beam.Value, stud.Value, slab.Value, loads.Select(x => x.Value).ToList());

      DA.SetData(0, new MemberGoo(member));
    }
  }
}
