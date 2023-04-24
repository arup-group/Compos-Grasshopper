using System;
using ComposAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace ComposGH.Parameters {
  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class SafetyFactorParam : GH_Param<SafetyFactorsGoo> {
    public override Guid ComponentGuid => new Guid("57c67486-4ba0-4396-b62c-822e192328b6");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + SafetyFactorsGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? SafetyFactorsGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.SafetyFactorParam;

    public SafetyFactorParam() : base(new GH_InstanceDescription(
      SafetyFactorsGoo.Name,
      SafetyFactorsGoo.NickName,
      SafetyFactorsGoo.Description + " parameter",
      Components.Ribbon.CategoryName.Name(),
      Components.Ribbon.SubCategoryName.Cat10())) { }
  }

  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISafetyFactors"/> class can be used in Grasshopper.
  /// </summary>
  public class SafetyFactorsGoo : GH_OasysGoo<ISafetyFactors> {
    public static string Description => "Compos Material and Load Safety Factors";
    public static string Name => "Safety Factors";
    public static string NickName => "Saf";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public SafetyFactorsGoo(ISafetyFactors item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new SafetyFactorsGoo(Value);
    }
  }
}
