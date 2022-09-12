using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStudGroupSpacing"/> class can be used in Grasshopper.
  /// </summary>
  public class StudGroupSpacingGoo : GH_OasysGoo<IStudGroupSpacing>
  {
    public static string Name => "Stud Spacing";
    public static string NickName => "Spa";
    public static string Description => "Compos Custom Shear Stud Spacing";
    public StudGroupSpacingGoo(IStudGroupSpacing item) : base(item) { }
    public override IGH_Goo Duplicate() => new StudGroupSpacingGoo(this.Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class StudGroupSpacingParam : GH_Param<StudGroupSpacingGoo>
  {
    public StudGroupSpacingParam()
      : base(new GH_InstanceDescription(
        StudGroupSpacingGoo.Name,
        StudGroupSpacingGoo.NickName,
        StudGroupSpacingGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + StudGroupSpacingGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? StudGroupSpacingGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("01ab719c-941a-4d92-974a-7af53f2af28c");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.StudGrpDimParam;
  }
}
