using ComposAPI;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IWebOpening"/> class can be used in Grasshopper.
  /// </summary>
  public class WebOpeningGoo : GH_OasysGoo<IWebOpening> {
    public static string Description => "Compos Web Opening or Notch";
    public static string Name => "Web Opening";
    public static string NickName => "WO";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public WebOpeningGoo(IWebOpening item) : base(item) { }

    public override IGH_Goo Duplicate() => new WebOpeningGoo(Value);
  }
}
