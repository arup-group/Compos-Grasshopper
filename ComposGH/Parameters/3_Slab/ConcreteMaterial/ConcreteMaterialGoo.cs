using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IConcreteMaterial"/> class can be used in Grasshopper.
  /// </summary>
  public class ConcreteMaterialGoo : GH_OasysGoo<IConcreteMaterial> {
    public static string Description => "Compos Concrete Material";
    public static string Name => "Concrete Material";
    public static string NickName => "CMt";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public ConcreteMaterialGoo(IConcreteMaterial item) : base(item) { }

    public override IGH_Goo Duplicate() => new ConcreteMaterialGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ConcreteMaterialParam : GH_Param<ConcreteMaterialGoo> {
    public override Guid ComponentGuid => new Guid("fd8b76b2-67a9-4a4f-ada5-c0c70f49a38e");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + ConcreteMaterialGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? ConcreteMaterialGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.ConcreteMaterialParam;

    public ConcreteMaterialParam()
                                          : base(new GH_InstanceDescription(
    ConcreteMaterialGoo.Name,
    ConcreteMaterialGoo.NickName,
    ConcreteMaterialGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
