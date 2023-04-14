using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using OasysGH;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDeckingConfiguration"/> class can be used in Grasshopper.
  /// </summary>
  public class DeckingConfigurationGoo : GH_OasysGoo<IDeckingConfiguration>
  {
    public static string Name => "Decking Config.";
    public static string NickName => "DCf";
    public static string Description => "Compos Steel Decking Configurations";
    public DeckingConfigurationGoo(IDeckingConfiguration item) : base(item) { }
    public override IGH_Goo Duplicate() => new DeckingConfigurationGoo(Value);
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
  }
}
