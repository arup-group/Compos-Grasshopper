using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISafetyFactorsEN"/> class can be used in Grasshopper.
  /// </summary>
  public class SafetyFactorsENGoo : GH_OasysGoo<ISafetyFactorsEN>
  {
    public static string Name => "EN Safety Factors";
    public static string NickName => "SEN";
    public static string Description => "Compos Material and Load Safety Factors to EN1994-1-1";
    public SafetyFactorsENGoo(ISafetyFactorsEN item) : base(item) { }
    public override IGH_Goo Duplicate() => new SafetyFactorsENGoo(this.Value);
  }
}
