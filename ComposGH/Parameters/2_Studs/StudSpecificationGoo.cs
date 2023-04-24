using System;
using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="IStudSpecification"/> class can be used in Grasshopper.
  /// </summary>
  public class StudSpecificationGoo : GH_OasysGoo<IStudSpecification> {
    public static string Description => "Compos Shear Stud Specifications";
    public static string Name => "Stud Specs";
    public static string NickName => "Spc";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public StudSpecificationGoo(IStudSpecification item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new StudSpecificationGoo(Value);
    }
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class StudSpecificationParam : GH_Param<StudSpecificationGoo> {
    public override Guid ComponentGuid => new Guid("632f67ea-1fa2-4062-998b-5232029086f3");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + StudSpecificationGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? StudSpecificationGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StudSpecParam;

    public StudSpecificationParam() : base(new GH_InstanceDescription(
      StudSpecificationGoo.Name,
      StudSpecificationGoo.NickName,
      StudSpecificationGoo.Description + " parameter",
      Components.Ribbon.CategoryName.Name(),
      Components.Ribbon.SubCategoryName.Cat10())) { }
  }
}
