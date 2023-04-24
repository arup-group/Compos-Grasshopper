using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;

namespace ComposGH.Parameters {
  public class ComposWebOpeningParameter : GH_PersistentParam<WebOpeningGoo> {
    public override Guid ComponentGuid => new Guid("eb70e868-29d9-4fae-9ef7-c465f3762a43");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    public bool Hidden => true;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + WebOpeningGoo.Name + " parameter" : base.InstanceDescription;

    public bool IsPreviewCapable => false;

    public override string TypeName => SourceCount == 0 ? WebOpeningGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.WebOpeningParam;

    public ComposWebOpeningParameter() : base(new GH_InstanceDescription(
    WebOpeningGoo.Name,
    WebOpeningGoo.NickName,
    WebOpeningGoo.Description + " parameter",
    Components.Ribbon.CategoryName.Name(),
    Components.Ribbon.SubCategoryName.Cat10())) {
    }

    protected override ToolStripMenuItem Menu_CustomMultiValueItem() {
      var item = new ToolStripMenuItem {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    protected override ToolStripMenuItem Menu_CustomSingleValueItem() {
      var item = new ToolStripMenuItem {
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
