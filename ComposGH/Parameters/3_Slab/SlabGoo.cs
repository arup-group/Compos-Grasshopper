using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using OasysGH;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISlab"/> class can be used in Grasshopper.
  /// </summary>
  public class SlabGoo : GH_OasysGoo<ISlab>
  {
    public static string Name => "Slab";
    public static string NickName => "Sla";
    public static string Description => "Compos Concrete Slab";
    public SlabGoo(ISlab item) : base(item) { }
    public override IGH_Goo Duplicate() => new SlabGoo(Value);
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
  }
}
