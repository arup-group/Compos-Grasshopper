using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;
using OasysGH;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISteelMaterial"/> class can be used in Grasshopper.
  /// </summary>
  public class SteelMaterialGoo : GH_OasysGoo<ISteelMaterial>
  {
    public static string Name => "Steel Material";
    public static string NickName => "SMt";
    public static string Description => "Compos Steel Material";
    public SteelMaterialGoo(ISteelMaterial item) : base(item) { }
    public override IGH_Goo Duplicate() => new SteelMaterialGoo(Value);
    public override OasysPluginInfo PluginInfo => ComposGH.PluginInfo.Instance;
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class SteelMaterialParam : GH_Param<SteelMaterialGoo>
  {
    public SteelMaterialParam()
      : base(new GH_InstanceDescription(
        SteelMaterialGoo.Name,
        SteelMaterialGoo.NickName,
        SteelMaterialGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + SteelMaterialGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? SteelMaterialGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("be27b097-85d4-4211-967b-83e47c94433f");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SteelMaterialParam;
  }
}
