using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDesignCriteria"/> class can be used in Grasshopper.
  /// </summary>
  public class DesignCriteriaGoo : GH_OasysGoo<IDesignCriteria> {
    public static string Description => "Compos Design Criteria";
    public static string Name => "Design Criteria";
    public static string NickName => "Crt";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public DesignCriteriaGoo(IDesignCriteria item) : base(item) { }

    public override IGH_Goo Duplicate() => new DesignCriteriaGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class DesignCriteriaParam : GH_Param<DesignCriteriaGoo> {
    public override Guid ComponentGuid => new Guid("6f04c4ce-cf10-47e2-b17b-407ce3c53e47");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + DesignCriteriaGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? DesignCriteriaGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.DesignCriteriaParam;

    public DesignCriteriaParam()
                                          : base(new GH_InstanceDescription(
    DesignCriteriaGoo.Name,
    DesignCriteriaGoo.NickName,
    DesignCriteriaGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
