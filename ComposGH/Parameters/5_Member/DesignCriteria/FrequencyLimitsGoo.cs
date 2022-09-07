using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IFrequencyLimits"/> class can be used in Grasshopper.
  /// </summary>
  public class FrequencyLimitsGoo : GH_OasysGoo<IFrequencyLimits>
  {
    public static string Name => "Frequency Limits";
    public static string NickName => "fLm";
    public static string Description => "Compos Frequency Limit Criteria";
    public FrequencyLimitsGoo(IFrequencyLimits item) : base(item) { }
    public override IGH_Goo Duplicate() => new FrequencyLimitsGoo(this.Value);
  }
}
