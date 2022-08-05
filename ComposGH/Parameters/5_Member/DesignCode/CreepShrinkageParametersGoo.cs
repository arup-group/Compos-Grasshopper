using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ICreepShrinkageParameters"/> class can be used in Grasshopper.
  /// </summary>
  public class CreepShrinkageParametersGoo : GH_OasysGoo<ICreepShrinkageParameters>
  {
    public static string Name => "Creep & Shrinkage";
    public static string NickName => "CSP";
    public static string Description => "Compos Creep and Shrinkage Parameters";
    public CreepShrinkageParametersGoo(ICreepShrinkageParameters item) : base(item) { }
    public override IGH_Goo Duplicate() => new CreepShrinkageParametersGoo(this.Value);
  }
}
