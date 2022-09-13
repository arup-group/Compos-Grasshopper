using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ITransverseReinforcement"/> class can be used in Grasshopper.
  /// </summary>
  public class TransverseReinforcementGoo : GH_OasysGoo<ITransverseReinforcement>
  {
    public static string Name => "Transverse Reinforcement";
    public static string NickName => "RfT";
    public static string Description => "Compos Transverse Reinforcement";
    public TransverseReinforcementGoo(ITransverseReinforcement item) : base(item) { }
    public override IGH_Goo Duplicate() => new TransverseReinforcementGoo(this.Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class TransverseReinforcementParam : GH_Param<TransverseReinforcementGoo>
  {
    public TransverseReinforcementParam()
      : base(new GH_InstanceDescription(
        TransverseReinforcementGoo.Name,
        TransverseReinforcementGoo.NickName,
        TransverseReinforcementGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + TransverseReinforcementGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? TransverseReinforcementGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("85fae3b5-8b00-47b0-9ff1-ec874b89ab3f");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.TransverseReinforcementParam;
  }
}
