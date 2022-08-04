using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISafetyFactors"/> class can be used in Grasshopper.
  /// </summary>
  public class SafetyFactorsGoo : GH_OasysGoo<ISafetyFactors>
  {
    public static string Name => "Safety Factors";
    public static string NickName => "Saf";
    public static string Description => "Compos Material and Load Safety Factors.";
    public SafetyFactorsGoo(ISafetyFactors item) : base(item) { }
    public override IGH_Goo Duplicate() => new SafetyFactorsGoo(this.Value);
  }
}
