using ComposAPI;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDeckingConfiguration"/> class can be used in Grasshopper.
  /// </summary>
  public class DeckingConfigurationGoo : GH_OasysGoo<IDeckingConfiguration> {
    public static string Description => "Compos Steel Decking Configurations";
    public static string Name => "Decking Config.";
    public static string NickName => "DCf";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public DeckingConfigurationGoo(IDeckingConfiguration item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new DeckingConfigurationGoo(Value);
    }
  }
}
