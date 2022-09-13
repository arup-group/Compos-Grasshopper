using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;
using OasysGH.Helpers;

namespace ComposGH.Components
{
  public class CreateMember : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("e7eafcec-ede6-4d60-9fcb-5392cb878581");
    public CreateMember()
      : base("Create" + MemberGoo.Name.Replace(" ", string.Empty),
          MemberGoo.Name.Replace(" ", string.Empty),
          "Create a " + MemberGoo.Description,
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Resources.CreateMember;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
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

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new ComposMemberParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      BeamGoo beam = (BeamGoo)Input.GenericGoo<BeamGoo>(this, DA, 0);
      StudGoo stud = (StudGoo)Input.GenericGoo<StudGoo>(this, DA, 1);
      SlabGoo slab = (SlabGoo)Input.GenericGoo<SlabGoo>(this, DA, 2);
      List<LoadGoo> loads = Input.GenericGooList<LoadGoo>(this, DA, 3);
      DesignCodeGoo code = (DesignCodeGoo)Input.GenericGoo<DesignCodeGoo>(this, DA, 4);

      string name = "";
      DA.GetData(5, ref name);
      string gridRef = "";
      DA.GetData(6, ref gridRef);
      string note = "";
      DA.GetData(7, ref note);

      Member member = new Member(name, gridRef, note, code.Value, beam.Value, stud.Value, slab.Value, loads.Select(x => x.Value).ToList());
      
      DA.SetData(0, new MemberGoo(member));
    }
  }
}
