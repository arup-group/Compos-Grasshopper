using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ICustomTransverseReinforcementLayout"/> class can be used in Grasshopper.
  /// </summary>
  public class CustomTransverseReinforcementLayoutGoo : GH_OasysGoo<ICustomTransverseReinforcementLayout>
  {
    public static string Name => "Reinforcement Layout";
    public static string NickName => "RfL";
    public static string Description => "Compos Custom Transverse Reinforcement Layout.";
    public CustomTransverseReinforcementLayoutGoo(ICustomTransverseReinforcementLayout item) : base(item) { }
    public override IGH_Goo Duplicate() => new CustomTransverseReinforcementLayoutGoo(this.Value);
  }
}
