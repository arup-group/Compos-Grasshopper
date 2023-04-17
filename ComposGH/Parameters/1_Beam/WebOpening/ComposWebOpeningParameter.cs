using Grasshopper.Kernel;
using System;
using System.Collections.Generic;

namespace ComposGH.Parameters {
  public class ComposWebOpeningParameter : GH_PersistentParam<WebOpeningGoo> {
    public override Guid ComponentGuid => new Guid("eb70e868-29d9-4fae-9ef7-c465f3762a43");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    public bool Hidden {
      get { return true; }
      //set { m_hidden = value; }
    }

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + WebOpeningGoo.Name + " parameter" : base.InstanceDescription;

    public bool IsPreviewCapable {
      get { return false; }
    }

    public override string TypeName => SourceCount == 0 ? WebOpeningGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.WebOpeningParam;

    public ComposWebOpeningParameter()
                                                          : base(new GH_InstanceDescription(
    WebOpeningGoo.Name,
    WebOpeningGoo.NickName,
    WebOpeningGoo.Description + " parameter",
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

    protected override GH_GetterResult Prompt_Plural(ref List<WebOpeningGoo> values) {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref WebOpeningGoo value) {
      return GH_GetterResult.cancel;
    }
  }
}
