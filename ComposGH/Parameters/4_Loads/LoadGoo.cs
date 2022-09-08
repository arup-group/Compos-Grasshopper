using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ILoad"/> class can be used in Grasshopper.
  /// </summary>
  public class LoadGoo : GH_OasysGoo<ILoad>
  {
    public static string Name => "Load";
    public static string NickName => "Ld";
    public static string Description => "Compos Load";
    public LoadGoo(ILoad item) : base(item) { }
    public override IGH_Goo Duplicate() => new LoadGoo(this.Value);
  }
}