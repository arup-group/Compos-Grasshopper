using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IERatio"/> class can be used in Grasshopper.
  /// </summary>
  public class ERatioGoo : GH_OasysGoo<IERatio>
  {
    public static string Name => "E-Ratio";
    public static string NickName => "ER";
    public static string Description => "Compos Steel to concrete Young´s modulus ratios";
    public ERatioGoo(IERatio item) : base(item) { }
    public override IGH_Goo Duplicate() => new ERatioGoo(this.Value);
  }
}
