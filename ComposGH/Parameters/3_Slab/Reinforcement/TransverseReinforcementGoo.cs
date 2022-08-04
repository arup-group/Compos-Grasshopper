using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ITransverseReinforcement"/> class can be used in Grasshopper.
  /// </summary>
  public class TransverseReinforcementGoo : GH_OasysGoo<ITransverseReinforcement>
  {
    public static string Name => "Transverse Reinforcement";
    public static string NickName => "RfT";
    public static string Description => "Compos Transverse Reinforcement.";
    public TransverseReinforcementGoo(ITransverseReinforcement item) : base(item) { }
    public override IGH_Goo Duplicate() => new TransverseReinforcementGoo(this.Value);
  }
}
