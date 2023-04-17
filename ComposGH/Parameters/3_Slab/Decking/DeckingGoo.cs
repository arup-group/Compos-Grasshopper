using ComposAPI;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDecking"/> class can be used in Grasshopper.
  /// </summary>
  public class DeckingGoo : GH_OasysGoo<IDecking> {
    public static string Description => "Compos Steel Decking";
    public static string Name => "Decking";
    public static string NickName => "Dec";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public DeckingGoo(IDecking item) : base(item) { }

    public override IGH_Goo Duplicate() => new DeckingGoo(Value);
  }
}
