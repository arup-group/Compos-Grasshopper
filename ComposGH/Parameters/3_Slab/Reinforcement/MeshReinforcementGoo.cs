using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IMeshReinforcement"/> class can be used in Grasshopper.
  /// </summary>
  public class MeshReinforcementGoo : GH_OasysGoo<IMeshReinforcement> {
    public static string Description => "Compos Mesh Reinforcement";
    public static string Name => "Mesh Reinforcement";
    public static string NickName => "RfM";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public MeshReinforcementGoo(IMeshReinforcement item) : base(item) { }

    public override IGH_Goo Duplicate() => new MeshReinforcementGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class MeshReinforcementParam : GH_Param<MeshReinforcementGoo> {
    public override Guid ComponentGuid => new Guid("5600a6c7-9c7f-4f5b-9e7c-e04945719a21");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + MeshReinforcementGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? MeshReinforcementGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.MeshReinforcementParam;

    public MeshReinforcementParam()
                                          : base(new GH_InstanceDescription(
    MeshReinforcementGoo.Name,
    MeshReinforcementGoo.NickName,
    MeshReinforcementGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
