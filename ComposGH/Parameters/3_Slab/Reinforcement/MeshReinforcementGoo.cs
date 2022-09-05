using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IMeshReinforcement"/> class can be used in Grasshopper.
  /// </summary>
  public class MeshReinforcementGoo : GH_OasysGoo<IMeshReinforcement>
  {
    public static string Name => "Mesh Reinforcement";
    public static string NickName => "RfM";
    public static string Description => "Compos Mesh Reinforcement";
    public MeshReinforcementGoo(IMeshReinforcement item) : base(item) { }
    public override IGH_Goo Duplicate() => new MeshReinforcementGoo(this.Value);
  }
}
