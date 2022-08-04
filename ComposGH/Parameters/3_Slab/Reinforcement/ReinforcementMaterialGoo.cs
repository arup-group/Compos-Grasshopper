using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IReinforcementMaterial"/> class can be used in Grasshopper.
  /// </summary>
  public class ReinforcementMaterialGoo : GH_OasysGoo<IReinforcementMaterial>
  {
    public static string Name => "Reinforcement Material";
    public static string NickName => "RMt";
    public static string Description => "Compos Reinforcement Material.";
    public ReinforcementMaterialGoo(IReinforcementMaterial item) : base(item) { }
    public override IGH_Goo Duplicate() => new ReinforcementMaterialGoo(this.Value);
  }
}
