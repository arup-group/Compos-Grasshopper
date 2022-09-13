using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IConcreteMaterial"/> class can be used in Grasshopper.
  /// </summary>
  public class ConcreteMaterialGoo : GH_OasysGoo<IConcreteMaterial>
  {
    public static string Name => "Concrete Material";
    public static string NickName => "CMt";
    public static string Description => "Compos Concrete Material";
    public ConcreteMaterialGoo(IConcreteMaterial item) : base(item) { }
    public override IGH_Goo Duplicate() => new ConcreteMaterialGoo(this.Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ConcreteMaterialParam : GH_Param<ConcreteMaterialGoo>
  {
    public ConcreteMaterialParam()
      : base(new GH_InstanceDescription(
        ConcreteMaterialGoo.Name,
        ConcreteMaterialGoo.NickName,
        ConcreteMaterialGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + ConcreteMaterialGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? ConcreteMaterialGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("fd8b76b2-67a9-4a4f-ada5-c0c70f49a38e");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConcreteMaterialParam;
  }
}
