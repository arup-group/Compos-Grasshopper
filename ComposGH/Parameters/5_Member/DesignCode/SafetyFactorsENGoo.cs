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
  public class SafetyFactorENParam : GH_Param<SafetyFactorsENGoo> {
    public override Guid ComponentGuid => new Guid("62158c83-58bf-49cd-b1c3-e2343025521b");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + SafetyFactorsENGoo.Name + " parameter" : base.InstanceDescription;

    public override string TypeName => SourceCount == 0 ? SafetyFactorsENGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.SafetyFactorParam;

    public SafetyFactorENParam() : base(new GH_InstanceDescription(
      SafetyFactorsENGoo.Name,
      SafetyFactorsENGoo.NickName,
      SafetyFactorsENGoo.Description + " parameter",
      Components.Ribbon.CategoryName.Name(),
      Components.Ribbon.SubCategoryName.Cat10())) { }
  }

  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISafetyFactorsEN"/> class can be used in Grasshopper.
  /// </summary>
  public class SafetyFactorsENGoo : GH_OasysGoo<ISafetyFactorsEN> {
    public static string Description => "Compos Material and Load Safety Factors to EN1994-1-1";
    public static string Name => "EN Safety Factors";
    public static string NickName => "SEN";
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;

    public SafetyFactorsENGoo(ISafetyFactorsEN item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new SafetyFactorsENGoo(Value);
    }
  }
}
