using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDesignCriteria"/> class can be used in Grasshopper.
  /// </summary>
  public class DesignCriteriaGoo : GH_OasysGoo<IDesignCriteria>
  {
    public static string Name => "Design Criteria";
    public static string NickName => "Crt";
    public static string Description => "Compos Design Criteria";
    public DesignCriteriaGoo(IDesignCriteria item) : base(item) { }
    public override IGH_Goo Duplicate() => new DesignCriteriaGoo(this.Value);
  }
}
