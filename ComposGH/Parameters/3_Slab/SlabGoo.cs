using ComposAPI;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISlab"/> class can be used in Grasshopper.
  /// </summary>
  public class SlabGoo : GH_OasysGoo<ISlab> {
    public static string Description => "Compos Concrete Slab";
    public static string Name => "Slab";
    public static string NickName => "Sla";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public SlabGoo(ISlab item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new SlabGoo(Value);
    }
  }
}
