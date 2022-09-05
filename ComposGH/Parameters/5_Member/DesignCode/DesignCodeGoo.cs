using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDesignCode"/> class can be used in Grasshopper.
  /// </summary>
  public class DesignCodeGoo : GH_OasysGoo<IDesignCode>
  {
    public static string Name => "Design Code";
    public static string NickName => "DC";
    public static string Description => "Compos Design Code";
    public DesignCodeGoo(IDesignCode item) : base(item) { }
    public override IGH_Goo Duplicate() => new DesignCodeGoo(this.Value);
  }
}
