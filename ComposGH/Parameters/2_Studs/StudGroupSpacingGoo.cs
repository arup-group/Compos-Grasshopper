using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStudGroupSpacing"/> class can be used in Grasshopper.
  /// </summary>
  public class StudGroupSpacingGoo : GH_OasysGoo<IStudGroupSpacing>
  {
    public static string Name => "Stud Spacing";
    public static string NickName => "Spa";
    public static string Description => "Compos Custom Shear Stud Spacing.";
    public StudGroupSpacingGoo(IStudGroupSpacing item) : base(item) { }
    public override IGH_Goo Duplicate() => new StudGroupSpacingGoo(this.Value);
  }
}
