using ComposAPI;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IERatio"/> class can be used in Grasshopper.
  /// </summary>
  public class ERatioGoo : GH_OasysGoo<IERatio> {
    public static string Description => "Compos Steel to concrete Young´s modulus ratios";
    public static string Name => "E-Ratio";
    public static string NickName => "ER";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public ERatioGoo(IERatio item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new ERatioGoo(Value);
    }
  }
}
