using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISupports"/> class can be used in Grasshopper.
  /// </summary>
  public class SupportsGoo : GH_OasysGoo<ISupports> {
    public static string Description => "Compos Support conditions";
    public static string Name => "Supports";
    public static string NickName => "Sup";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public SupportsGoo(ISupports item) : base(item) { }

    public override IGH_Goo Duplicate() => new SupportsGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class SupportsParam : GH_Param<SupportsGoo> {
    public override Guid ComponentGuid => new Guid("e1de523d-a6db-400c-8550-49e25cfcd9bd");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + SupportsGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? SupportsGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.SupportParam;

    public SupportsParam()
                                          : base(new GH_InstanceDescription(
    SupportsGoo.Name,
    SupportsGoo.NickName,
    SupportsGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
