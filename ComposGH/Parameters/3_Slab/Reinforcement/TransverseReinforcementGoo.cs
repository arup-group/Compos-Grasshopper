using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ITransverseReinforcement"/> class can be used in Grasshopper.
  /// </summary>
  public class TransverseReinforcementGoo : GH_OasysGoo<ITransverseReinforcement> {
    public static string Description => "Compos Transverse Reinforcement";
    public static string Name => "Transverse Reinforcement";
    public static string NickName => "RfT";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public TransverseReinforcementGoo(ITransverseReinforcement item) : base(item) { }

    public override IGH_Goo Duplicate() => new TransverseReinforcementGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class TransverseReinforcementParam : GH_Param<TransverseReinforcementGoo> {
    public override Guid ComponentGuid => new Guid("85fae3b5-8b00-47b0-9ff1-ec874b89ab3f");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + TransverseReinforcementGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? TransverseReinforcementGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.TransverseReinforcementParam;

    public TransverseReinforcementParam()
                                          : base(new GH_InstanceDescription(
    TransverseReinforcementGoo.Name,
    TransverseReinforcementGoo.NickName,
    TransverseReinforcementGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
