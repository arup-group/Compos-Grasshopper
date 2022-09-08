using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IWebOpeningStiffeners"/> class can be used in Grasshopper.
  /// </summary>
  public class WebOpeningStiffenersGoo : GH_OasysGoo<IWebOpeningStiffeners>
  {
    public static string Name => "Stiffener";
    public static string NickName => "Stf";
    public static string Description => "Compos Web Opening Stiffener";
    public WebOpeningStiffenersGoo(IWebOpeningStiffeners item) : base(item) { }
    public override IGH_Goo Duplicate() => new WebOpeningStiffenersGoo(this.Value);
  }
}
