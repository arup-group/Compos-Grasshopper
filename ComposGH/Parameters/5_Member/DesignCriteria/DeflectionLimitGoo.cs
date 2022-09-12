using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDeflectionLimit"/> class can be used in Grasshopper.
  /// </summary>
  public class DeflectionLimitGoo : GH_OasysGoo<IDeflectionLimit>
  {
    public static string Name => "Deflection Limits";
    public static string NickName => "DLm";
    public static string Description => "Compos Deflection Limit Criteria";
    public DeflectionLimitGoo(IDeflectionLimit item) : base(item) { }
    public override IGH_Goo Duplicate() => new DeflectionLimitGoo(this.Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class DeflectionLimitParam : GH_Param<DeflectionLimitGoo>
  {
    public DeflectionLimitParam()
      : base(new GH_InstanceDescription(
        DeflectionLimitGoo.Name,
        DeflectionLimitGoo.NickName,
        DeflectionLimitGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + DeflectionLimitGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? DeflectionLimitGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("bef8188b-0874-43c4-94b1-4285fbbdec2e");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.DeflectionLimit;
  }
}
