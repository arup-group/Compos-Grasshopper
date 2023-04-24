using System;
using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IDesignCode"/> class can be used in Grasshopper.
  /// </summary>
  public class DesignCodeGoo : GH_OasysGoo<IDesignCode> {
    public static string Description => "Compos Design Code";
    public static string Name => "Design Code";
    public static string NickName => "DC";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public DesignCodeGoo(IDesignCode item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new DesignCodeGoo(Value);
    }
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class DesignCodeParam : GH_Param<DesignCodeGoo> {
    public override Guid ComponentGuid => new Guid("029870cb-c510-488b-940e-cabc31045910");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + DesignCodeGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? DesignCodeGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.DesignCodeParameter;

    public DesignCodeParam() : base(new GH_InstanceDescription(
      DesignCodeGoo.Name,
      DesignCodeGoo.NickName,
      DesignCodeGoo.Description + " parameter",
      Components.Ribbon.CategoryName.Name(),
      Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
