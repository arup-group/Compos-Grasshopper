using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

namespace ComposGH.Parameters
{
  public class ComposWebOpeningParameter : GH_PersistentParam<WebOpeningGoo>
  {
    public ComposWebOpeningParameter()
      : base(new GH_InstanceDescription(
        WebOpeningGoo.Name, 
        WebOpeningGoo.NickName, 
        WebOpeningGoo.Description + " parameter", 
        Components.Ribbon.CategoryName.Name(), 
        Components.Ribbon.SubCategoryName.Cat10()))
    {
    }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + WebOpeningGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? WebOpeningGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("eb70e868-29d9-4fae-9ef7-c465f3762a43");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.WebOpeningParam;

    protected override GH_GetterResult Prompt_Plural(ref List<WebOpeningGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref WebOpeningGoo value)
    {
      return GH_GetterResult.cancel;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    #region preview methods

    public bool Hidden
    {
      get { return true; }
      //set { m_hidden = value; }
    }
    public bool IsPreviewCapable
    {
      get { return false; }
    }
    #endregion
  }

}
