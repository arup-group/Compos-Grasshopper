using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStudSpecification"/> class can be used in Grasshopper.
  /// </summary>
  public class StudSpecificationGoo : GH_OasysGoo<IStudSpecification>
  {
    public static string Name => "Stud Specs";
    public static string NickName => "Spc";
    public static string Description => "Compos Shear Stud Specifications.";
    public StudSpecificationGoo(IStudSpecification item) : base(item) { }
    public override IGH_Goo Duplicate() => new StudSpecificationGoo(this.Value);
  }
}
