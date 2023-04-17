using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ICustomTransverseReinforcementLayout"/> class can be used in Grasshopper.
  /// </summary>
  public class CustomTransverseReinforcementLayoutGoo : GH_OasysGoo<ICustomTransverseReinforcementLayout> {
    public static string Description => "Compos Custom Transverse Reinforcement Layout";
    public static string Name => "Reinforcement Layout";
    public static string NickName => "RfL";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public CustomTransverseReinforcementLayoutGoo(ICustomTransverseReinforcementLayout item) : base(item) { }

    public override IGH_Goo Duplicate() => new CustomTransverseReinforcementLayoutGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class CustomTransverseReinforcementParam : GH_Param<CustomTransverseReinforcementLayoutGoo> {
    public override Guid ComponentGuid => new Guid("ddf7f5e0-b6f2-4e69-adc0-b76c3a0b235c");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + CustomTransverseReinforcementLayoutGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? CustomTransverseReinforcementLayoutGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.RebarLayoutParam;

    public CustomTransverseReinforcementParam()
                                          : base(new GH_InstanceDescription(
    CustomTransverseReinforcementLayoutGoo.Name,
    CustomTransverseReinforcementLayoutGoo.NickName,
    CustomTransverseReinforcementLayoutGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
