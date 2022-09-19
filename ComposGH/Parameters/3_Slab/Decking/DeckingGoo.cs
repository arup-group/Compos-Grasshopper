using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using OasysGH;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDecking"/> class can be used in Grasshopper.
  /// </summary>
  public class DeckingGoo : GH_OasysGoo<IDecking>
  {
    public static string Name => "Decking";
    public static string NickName => "Dec";
    public static string Description => "Compos Steel Decking";
    public DeckingGoo(IDecking item) : base(item) { }
    public override IGH_Goo Duplicate() => new DeckingGoo(this.Value);
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
  }
}
