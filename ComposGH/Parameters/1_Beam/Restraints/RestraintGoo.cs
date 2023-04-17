using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IRestraint"/> class can be used in Grasshopper.
  /// </summary>
  public class RestraintGoo : GH_OasysGoo<IRestraint> {
    public static string Description => "Compos Restraints";
    public static string Name => "Restraints";
    public static string NickName => "Res";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public RestraintGoo(IRestraint item) : base(item) { }

    public override IGH_Goo Duplicate() => new RestraintGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class RestraintParam : GH_Param<RestraintGoo> {
    public override Guid ComponentGuid => new Guid("55ba5231-c519-45a3-b326-afccbe55bd3e");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + RestraintGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? RestraintGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.RestraintParam;

    public RestraintParam()
                                          : base(new GH_InstanceDescription(
    RestraintGoo.Name,
    RestraintGoo.NickName,
    RestraintGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
