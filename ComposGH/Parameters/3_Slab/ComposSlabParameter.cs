using Grasshopper.Kernel;
using System;
using System.Collections.Generic;

namespace ComposGH.Parameters {
  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposSlabParameter : GH_PersistentParam<SlabGoo> {
    public override Guid ComponentGuid => new Guid("e1c8e010-a55d-4f41-8f37-8d6e56976975");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    public bool Hidden {
      get { return true; }
      //set { m_hidden = value; }
    }

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + SlabGoo.Name + " parameter" : base.InstanceDescription;

    public bool IsPreviewCapable {
      get { return false; }
    }

    public override string TypeName => SourceCount == 0 ? SlabGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.SlabParam;

    public ComposSlabParameter()
                                                          : base(new GH_InstanceDescription(
    SlabGoo.Name,
    SlabGoo.NickName,
    SlabGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) {
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

    protected override GH_GetterResult Prompt_Plural(ref List<SlabGoo> values) {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref SlabGoo value) {
      return GH_GetterResult.cancel;
    }
  }
}
