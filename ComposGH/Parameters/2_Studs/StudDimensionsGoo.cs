using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;
using OasysGH;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStudDimensions"/> class can be used in Grasshopper.
  /// </summary>
  public class StudDimensionsGoo : GH_OasysGoo<IStudDimensions>
  {
    public static string Name => "Stud Dimension";
    public static string NickName => "StD";
    public static string Description => "Compos Shear Stud Dimensions";
    public StudDimensionsGoo(IStudDimensions item) : base(item) { }
    public override IGH_Goo Duplicate() => new StudDimensionsGoo(Value);
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class StudDimensionsParam : GH_Param<StudDimensionsGoo>
  {
    public StudDimensionsParam()
      : base(new GH_InstanceDescription(
        StudDimensionsGoo.Name,
        StudDimensionsGoo.NickName,
        StudDimensionsGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + StudDimensionsGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? StudDimensionsGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("573cbc43-c33a-4047-82c2-f0aeb700513d");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.StudDimParam;
  }
}
