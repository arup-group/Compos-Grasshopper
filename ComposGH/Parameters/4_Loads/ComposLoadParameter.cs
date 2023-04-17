using Grasshopper.Kernel;
using System;
using System.Collections.Generic;

namespace ComposGH.Parameters {
  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposLoadParameter : GH_PersistentParam<LoadGoo> {
    public override Guid ComponentGuid => new Guid("3dc51bc1-9abb-4f26-845f-ca1e66236e9e");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    public bool Hidden {
      get { return true; }
      //set { m_hidden = value; }
    }

    public override string InstanceDescription => m_data.DataCount == 0 ? "Empty " + LoadGoo.Name + " parameter" : base.InstanceDescription;

    public bool IsPreviewCapable {
      get { return false; }
    }

    public override string TypeName => SourceCount == 0 ? LoadGoo.Name : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.LoadParam;

    public ComposLoadParameter()
                                                          : base(new GH_InstanceDescription(LoadGoo.Name, LoadGoo.NickName, "Maintains a collection of " + LoadGoo.Description + " data", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10())) {
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

    protected override GH_GetterResult Prompt_Plural(ref List<LoadGoo> values) {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref LoadGoo value) {
      return GH_GetterResult.cancel;
    }
  }
}
