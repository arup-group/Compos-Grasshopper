using ComposAPI;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ICreepShrinkageParameters"/> class can be used in Grasshopper.
  /// </summary>
  public class CreepShrinkageParametersGoo : GH_OasysGoo<ICreepShrinkageParameters> {
    public static string Description => "Compos Creep and Shrinkage Parameters";
    public static string Name => "Creep & Shrinkage";
    public static string NickName => "CSP";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public CreepShrinkageParametersGoo(ICreepShrinkageParameters item) : base(item) { }

    public override IGH_Goo Duplicate() => new CreepShrinkageParametersGoo(Value);
  }
}
