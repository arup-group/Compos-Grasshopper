using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;
using OasysGH;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IWebOpeningStiffeners"/> class can be used in Grasshopper.
  /// </summary>
  public class WebOpeningStiffenersGoo : GH_OasysGoo<IWebOpeningStiffeners>
  {
    public static string Name => "Stiffener";
    public static string NickName => "Stf";
    public static string Description => "Compos Web Opening Stiffener";
    public WebOpeningStiffenersGoo(IWebOpeningStiffeners item) : base(item) { }
    public override IGH_Goo Duplicate() => new WebOpeningStiffenersGoo(Value);
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class WebOpeningStiffenersParam : GH_Param<WebOpeningStiffenersGoo>
  {
    public WebOpeningStiffenersParam()
      : base(new GH_InstanceDescription(
        WebOpeningStiffenersGoo.Name,
        WebOpeningStiffenersGoo.NickName,
        WebOpeningStiffenersGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + WebOpeningStiffenersGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? WebOpeningStiffenersGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("8c6de6af-baae-40b0-aaba-4273051e0265");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.WebOpeningStiffenerParam;
  }
}
