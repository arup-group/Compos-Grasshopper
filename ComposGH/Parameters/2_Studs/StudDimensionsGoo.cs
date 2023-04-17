using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using System;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStudDimensions"/> class can be used in Grasshopper.
  /// </summary>
  public class StudDimensionsGoo : GH_OasysGoo<IStudDimensions> {
    public static string Description => "Compos Shear Stud Dimensions";
    public static string Name => "Stud Dimension";
    public static string NickName => "StD";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public StudDimensionsGoo(IStudDimensions item) : base(item) { }

    public override IGH_Goo Duplicate() => new StudDimensionsGoo(Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class StudDimensionsParam : GH_Param<StudDimensionsGoo> {
    public override Guid ComponentGuid => new Guid("573cbc43-c33a-4047-82c2-f0aeb700513d");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + StudDimensionsGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? StudDimensionsGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StudDimParam;

    public StudDimensionsParam()
                                          : base(new GH_InstanceDescription(
    StudDimensionsGoo.Name,
    StudDimensionsGoo.NickName,
    StudDimensionsGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
