using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IComposFile"/> class can be used in Grasshopper.
  /// </summary>
  public class ComposFileGoo : GH_OasysGoo<IComposFile>
  {
    public static string Name => "Compos File";
    public static string NickName => ".cob";
    public static string Description => "Compos File containing one or more Members.";
    public ComposFileGoo(IComposFile item) : base(item) { }
    public override IGH_Goo Duplicate() => new ComposFileGoo(this.Value);
  }
}
