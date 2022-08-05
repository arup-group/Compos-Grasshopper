using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IConcreteMaterial"/> class can be used in Grasshopper.
  /// </summary>
  public class ConcreteMaterialGoo : GH_OasysGoo<IConcreteMaterial>
  {
    public static string Name => "Concrete Material";
    public static string NickName => "CMt";
    public static string Description => "Compos Concrete Material";
    public ConcreteMaterialGoo(IConcreteMaterial item) : base(item) { }
    public override IGH_Goo Duplicate() => new ConcreteMaterialGoo(this.Value);
  }
}
