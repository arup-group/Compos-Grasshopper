using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStudDimensions"/> class can be used in Grasshopper.
  /// </summary>
  public class StudDimensionsGoo : GH_OasysGoo<IStudDimensions>
  {
    public static string Name => "Stud Dimension";
    public static string NickName => "StD";
    public static string Description => "Compos Shear Stud Dimensions.";
    public StudDimensionsGoo(IStudDimensions item) : base(item) { }
    public override IGH_Goo Duplicate() => new StudDimensionsGoo(this.Value);
  }
}
