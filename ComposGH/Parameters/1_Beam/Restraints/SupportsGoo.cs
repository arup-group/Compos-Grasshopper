using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISupports"/> class can be used in Grasshopper.
  /// </summary>
  public class SupportsGoo : GH_OasysGoo<ISupports>
  {
    public static string Name => "Supports";
    public static string NickName => "Sup";
    public static string Description => "Compos Support conditions";
    public SupportsGoo(ISupports item) : base(item) { }
    public override IGH_Goo Duplicate() => new SupportsGoo(this.Value);
  }
}
