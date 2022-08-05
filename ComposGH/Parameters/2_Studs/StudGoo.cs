using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStud"/> class can be used in Grasshopper.
  /// </summary>
  public class StudGoo : GH_OasysGoo<IStud>
  {
    public static string Name => "Stud";
    public static string NickName => "Stu";
    public static string Description => "Compos Shear Stud";
    public StudGoo(IStud item) : base(item) { }
    public override IGH_Goo Duplicate() => new StudGoo(this.Value);
  }
}
