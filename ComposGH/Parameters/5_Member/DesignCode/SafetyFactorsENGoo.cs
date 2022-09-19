using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;
using OasysGH;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISafetyFactorsEN"/> class can be used in Grasshopper.
  /// </summary>
  public class SafetyFactorsENGoo : GH_OasysGoo<ISafetyFactorsEN>
  {
    public static string Name => "EN Safety Factors";
    public static string NickName => "SEN";
    public static string Description => "Compos Material and Load Safety Factors to EN1994-1-1";
    public SafetyFactorsENGoo(ISafetyFactorsEN item) : base(item) { }
    public override IGH_Goo Duplicate() => new SafetyFactorsENGoo(this.Value);
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class SafetyFactorENParam : GH_Param<SafetyFactorsENGoo>
  {
    public SafetyFactorENParam()
      : base(new GH_InstanceDescription(
        SafetyFactorsENGoo.Name,
        SafetyFactorsENGoo.NickName,
        SafetyFactorsENGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + SafetyFactorsENGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? SafetyFactorsENGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("62158c83-58bf-49cd-b1c3-e2343025521b");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SafetyFactorParam;
  }
}
