using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDeflectionLimit"/> class can be used in Grasshopper.
  /// </summary>
  public class DeflectionLimitGoo : GH_OasysGoo<IDeflectionLimit>
  {
    public static string Name => "Deflection Limits";
    public static string NickName => "DLm";
    public static string Description => "Compos Deflection Limit Criteria";
    public DeflectionLimitGoo(IDeflectionLimit item) : base(item) { }
    public override IGH_Goo Duplicate() => new DeflectionLimitGoo(this.Value);
  }
}
