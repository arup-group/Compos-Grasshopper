using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Properties;
using OasysGH.Components;

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
      pManager.AddGenericParameter(SlabGoo.Name, SlabGoo.NickName, SlabGoo.Description, GH_ParamAccess.item);
      pManager.AddGenericParameter(LoadGoo.Name + "(s)", LoadGoo.NickName, LoadGoo.Description, GH_ParamAccess.list);
      pManager.AddGenericParameter(DesignCodeGoo.Name, DesignCodeGoo.NickName, DesignCodeGoo.Description, GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Set Member Name", GH_ParamAccess.item);
      pManager.AddTextParameter("GridRef", "Grd", "(Optional) Set Member's Grid Reference", GH_ParamAccess.item);
      pManager.AddTextParameter("Note", "Nt", "(Optional) Set Notes about the Member", GH_ParamAccess.item);
      pManager[6].Optional = true;
      pManager[7].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter(MemberGoo.Name, MemberGoo.NickName, MemberGoo.Description, GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      BeamGoo beam = (BeamGoo)GetInput.GenericGoo<BeamGoo>(this, DA, 0);
      StudGoo stud = (StudGoo)GetInput.GenericGoo<StudGoo>(this, DA, 1);
      SlabGoo slab = (SlabGoo)GetInput.GenericGoo<SlabGoo>(this, DA, 2);
      List<LoadGoo> loads = GetInput.GenericGooList<LoadGoo>(this, DA, 3);
      DesignCodeGoo code = (DesignCodeGoo)GetInput.GenericGoo<DesignCodeGoo>(this, DA, 4);

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
