using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IRestraint"/> class can be used in Grasshopper.
  /// </summary>
  public class RestraintGoo : GH_OasysGoo<IRestraint>
  {
    public static string Name => "Restraints";
    public static string NickName => "Res";
    public static string Description => "Compos Restraints";
    public RestraintGoo(IRestraint item) : base(item) { }
    public override IGH_Goo Duplicate() => new RestraintGoo(this.Value);
  }
}
