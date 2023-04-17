using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ComposGH.Parameters {
  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposBeamParameter : GH_PersistentGeometryParam<BeamGoo>, IGH_PreviewObject {
    public BoundingBox ClippingBox {
      get {
        return Preview_ComputeClippingBox();
      }
    }

    public override Guid ComponentGuid => new Guid("dc61e94b-c326-4789-92f2-e0fe3caea4c7");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    public bool Hidden {
      get { return m_hidden; }
      set { m_hidden = value; }
    }

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + BeamGoo.Name + " parameter" : base.InstanceDescription;

    public bool IsPreviewCapable {
      get { return true; }
    }

    public override string TypeName => SourceCount == 0 ? BeamGoo.Name : base.TypeName;

    protected override Bitmap Icon => Properties.Resources.BeamParam;

    private bool m_hidden = false;

    public ComposBeamParameter()
                                                                          : base(new GH_InstanceDescription(
    BeamGoo.Name,
    BeamGoo.NickName,
    BeamGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) {
    }

    public void DrawViewportMeshes(IGH_PreviewArgs args) {
      //Use a standard method to draw gunk, you don't have to specifically implement
      Preview_DrawMeshes(args);
    }

    public void DrawViewportWires(IGH_PreviewArgs args) {
      //Use a standard method to draw gunk, you don't have to specifically implement
      Preview_DrawWires(args);
    }

    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem() {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem() {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    //We do not allow users to pick parameter,
    //therefore the following 4 methods disable all this ui.
    protected override GH_GetterResult Prompt_Plural(ref List<BeamGoo> values) {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref BeamGoo value) {
      return GH_GetterResult.cancel;
    }
  }
}
