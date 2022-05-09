using System;
using System.Collections.Generic;
using System.Linq;
using ComposGH.Parameters;
using Grasshopper.Kernel;
using ComposAPI;

namespace ComposGH.Components
{
  public class CreateMember : GH_Component
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("e7eafcec-ede6-4d60-9fcb-5392cb878581");
    public CreateMember()
      : base("Create Member", "Member", "Create a Compos Member",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = false; } // sets the initial state of the component to hidden

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMember;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Beam", "Bm", "Compos Steel Beam", GH_ParamAccess.item);
      pManager.AddGenericParameter("Stud", "Stu", "Compos Shear Stud", GH_ParamAccess.item);
      pManager.AddGenericParameter("Slab", "Sla", "Compos Concrete slab", GH_ParamAccess.item);
      pManager.AddGenericParameter("Loads", "Ld", "Compos Loads", GH_ParamAccess.list);
      pManager.AddGenericParameter("Design Code", "DC", "Compos Design Code", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Set Member Name", GH_ParamAccess.item);
      pManager.AddTextParameter("GridRef", "Grd", "(Optional) Set Member's Grid Reference", GH_ParamAccess.item);
      pManager.AddTextParameter("Note", "Nt", "(Optional) Set Notes about the Member", GH_ParamAccess.item);
      pManager[6].Optional = true;
      pManager[7].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Member", "Mem", "Compos Member", GH_ParamAccess.item);
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

      IMember member = new Member(name, gridRef, note, code.Value, beam.Value, stud.Value, slab.Value, loads.Select(x => x.Value).ToList());
      
      DA.SetData(0, new MemberGoo(member));
    }
  }
}
