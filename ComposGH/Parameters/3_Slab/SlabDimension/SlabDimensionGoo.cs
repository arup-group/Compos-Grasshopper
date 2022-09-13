using Grasshopper.Kernel.Types;
using ComposAPI;
using OasysGH.Parameters;
using Grasshopper.Kernel;
using System;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure ComposAPI <see cref="ISlabDimension"/> class can be used in Grasshopper.
  /// </summary>
  public class SlabDimensionGoo : GH_OasysGoo<ISlabDimension>
  {
    public static string Name => "Slab Dimension";
    public static string NickName => "SDm";
    public static string Description => "Compos Slab Dimensions";
    public SlabDimensionGoo(ISlabDimension item) : base(item) { }
    public override IGH_Goo Duplicate() => new SlabDimensionGoo(this.Value);
  }

  /// <summary>
  /// /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class SlabDimensionParam : GH_Param<SlabDimensionGoo>
  {
    public SlabDimensionParam()
      : base(new GH_InstanceDescription(
        SlabDimensionGoo.Name,
        SlabDimensionGoo.NickName,
        SlabDimensionGoo.Description + " parameter",
        Components.Ribbon.CategoryName.Name(),
        Components.Ribbon.SubCategoryName.Cat10()))
    { }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + SlabDimensionGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? SlabDimensionGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("ae87f978-1810-441c-b0d8-cb63c3cbb94e");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SlabDimensionParam;
  }
}
