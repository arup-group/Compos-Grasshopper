using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;

namespace ComposGH.Parameters {
  public class ComposStudParameter : GH_PersistentParam<StudGoo> {
    public override Guid ComponentGuid => new Guid("e0b6cb52-99c8-4b2a-aec1-7f8a2d720daa");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    public bool Hidden => true;

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + StudGoo.Name + " parameter" : base.InstanceDescription;

    public bool IsPreviewCapable => false;

    public override string TypeName => SourceCount == 0 ? StudGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StudParam;

    public ComposStudParameter() : base(new GH_InstanceDescription(
      StudGoo.Name,
      StudGoo.NickName,
      StudGoo.Description + " parameter",
      Components.Ribbon.CategoryName.Name(),
      Components.Ribbon.SubCategoryName.Cat10())) {
    }

    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem() {
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

    protected override GH_GetterResult Prompt_Plural(ref List<StudGoo> values) {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref StudGoo value) {
      return GH_GetterResult.cancel;
    }
  }
}
