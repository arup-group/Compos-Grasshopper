using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  public class ComposStudParameter : GH_PersistentParam<StudGoo>
  {
    public ComposStudParameter()
      : base(new GH_InstanceDescription(
        StudGoo.Name, 
        StudGoo.NickName, 
        StudGoo.Description + " parameter", 
        Components.Ribbon.CategoryName.Name(), 
        Components.Ribbon.SubCategoryName.Cat10()))
    {
    }

    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + StudGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? StudGoo.Name : base.TypeName;

    public override Guid ComponentGuid => new Guid("e0b6cb52-99c8-4b2a-aec1-7f8a2d720daa");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.StudParam;

    protected override GH_GetterResult Prompt_Plural(ref List<StudGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref StudGoo value)
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
