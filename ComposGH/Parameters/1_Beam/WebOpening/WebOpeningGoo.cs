using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IWebOpening"/> class can be used in Grasshopper.
  /// </summary>
  public class WebOpeningGoo : GH_OasysGoo<IWebOpening>
  {
    public static string Name => "Web Opening";
    public static string NickName => "WO";
    public static string Description => "Compos Web Opening or Notch.";
    public WebOpeningGoo(IWebOpening item) : base(item) { }
    public override IGH_Goo Duplicate() => new WebOpeningGoo(this.Value);
  }
}
