using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISlabDimension"/> class can be used in Grasshopper.
  /// </summary>
  public class SlabDimensionGoo : GH_OasysGoo<ISlabDimension>
  {
    public static string Name => "Slab Dimension";
    public static string NickName => "SlD";
    public static string Description => "Compos Slab Dimensions";
    public SlabDimensionGoo(ISlabDimension item) : base(item) { }
    public override IGH_Goo Duplicate() => new SlabDimensionGoo(this.Value);
  }
}
