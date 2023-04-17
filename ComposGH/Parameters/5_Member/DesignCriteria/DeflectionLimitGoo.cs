using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDeflectionLimit"/> class can be used in Grasshopper.
  /// </summary>
  public class DeflectionLimitGoo : GH_OasysGoo<IDeflectionLimit> {
    public static string Description => "Compos Deflection Limit Criteria";
    public static string Name => "Deflection Limits";
    public static string NickName => "DLm";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public DeflectionLimitGoo(IDeflectionLimit item) : base(item) { }

    public override IGH_Goo Duplicate() => new DeflectionLimitGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class DeflectionLimitParam : GH_Param<DeflectionLimitGoo> {
    public override Guid ComponentGuid => new Guid("f226c2c8-6524-45a2-8579-77bfa8d0542e");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + DeflectionLimitGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? DeflectionLimitGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.DeflectionLimit;

    public DeflectionLimitParam()
                                          : base(new GH_InstanceDescription(
    DeflectionLimitGoo.Name,
    DeflectionLimitGoo.NickName,
    DeflectionLimitGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
