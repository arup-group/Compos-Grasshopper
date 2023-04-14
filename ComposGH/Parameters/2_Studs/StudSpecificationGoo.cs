using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;
using OasysGH;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStudSpecification"/> class can be used in Grasshopper.
  /// </summary>
  public class StudSpecificationGoo : GH_OasysGoo<IStudSpecification>
  {
    public static string Name => "Stud Specs";
    public static string NickName => "Spc";
    public static string Description => "Compos Shear Stud Specifications";
    public StudSpecificationGoo(IStudSpecification item) : base(item) { }
    public override IGH_Goo Duplicate() => new StudSpecificationGoo(Value);
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class StudSpecificationParam : GH_Param<StudSpecificationGoo>
  {
    public StudSpecificationParam()
      : base(new GH_InstanceDescription(
        StudSpecificationGoo.Name,
        StudSpecificationGoo.NickName,
        StudSpecificationGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + StudSpecificationGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? StudSpecificationGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("632f67ea-1fa2-4062-998b-5232029086f3");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.StudSpecParam;
  }
}
