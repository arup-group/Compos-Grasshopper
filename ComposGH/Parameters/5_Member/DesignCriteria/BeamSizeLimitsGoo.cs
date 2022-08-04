using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IBeamSizeLimits"/> class can be used in Grasshopper.
  /// </summary>
  public class BeamSizeLimitsGoo : GH_OasysGoo<IBeamSizeLimits>
  {
    public static string Name => "Beam Size Limits";
    public static string NickName => "BLm";
    public static string Description => "Compos Beam Size Limit Criteria.";
    public BeamSizeLimitsGoo(IBeamSizeLimits item) : base(item) { }
    public override IGH_Goo Duplicate() => new BeamSizeLimitsGoo(this.Value);
  }
}
