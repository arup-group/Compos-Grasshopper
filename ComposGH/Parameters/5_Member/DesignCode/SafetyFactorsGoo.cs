using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISafetyFactors"/> class can be used in Grasshopper.
  /// </summary>
  public class SafetyFactorsGoo : GH_OasysGoo<ISafetyFactors>
  {
    public static string Name => "Safety Factors";
    public static string NickName => "Saf";
    public static string Description => "Compos Material and Load Safety Factors";
    public SafetyFactorsGoo(ISafetyFactors item) : base(item) { }
    public override IGH_Goo Duplicate() => new SafetyFactorsGoo(this.Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class SafetyFactorParam : GH_Param<SafetyFactorsGoo>
  {
    public SafetyFactorParam()
      : base(new GH_InstanceDescription(
        SafetyFactorsGoo.Name,
        SafetyFactorsGoo.NickName,
        SafetyFactorsGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + SafetyFactorsGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? SafetyFactorsGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("57c67486-4ba0-4396-b62c-822e192328b6");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SafetyFactorParam;
  }
}
