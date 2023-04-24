using System;
using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IReinforcementMaterial"/> class can be used in Grasshopper.
  /// </summary>
  public class ReinforcementMaterialGoo : GH_OasysGoo<IReinforcementMaterial> {
    public static string Description => "Compos Reinforcement Material";
    public static string Name => "Reinforcement Material";
    public static string NickName => "RMt";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public ReinforcementMaterialGoo(IReinforcementMaterial item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new ReinforcementMaterialGoo(Value);
    }
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ReinforcementMaterialParam : GH_Param<ReinforcementMaterialGoo> {
    public override Guid ComponentGuid => new Guid("9fd80717-a8db-4f42-9900-b0cfcf164fc0");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + ReinforcementMaterialGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? ReinforcementMaterialGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.RebarMaterialParam;

    public ReinforcementMaterialParam() : base(new GH_InstanceDescription(
      ReinforcementMaterialGoo.Name,
      ReinforcementMaterialGoo.NickName,
      ReinforcementMaterialGoo.Description + " parameter",
      Components.Ribbon.CategoryName.Name(),
      Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
