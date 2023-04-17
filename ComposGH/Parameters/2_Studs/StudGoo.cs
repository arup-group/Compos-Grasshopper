using ComposAPI;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStud"/> class can be used in Grasshopper.
  /// </summary>
  public class StudGoo : GH_OasysGoo<IStud> {
    public static string Description => "Compos Shear Stud";
    public static string Name => "Stud";
    public static string NickName => "Stu";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public StudGoo(IStud item) : base(item) { }

    public override IGH_Goo Duplicate() => new StudGoo(Value);
  }
}
