using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposDeckingParameter : GH_PersistentParam<DeckingGoo>
  {
    public ComposDeckingParameter()
      : base(new GH_InstanceDescription(
        DeckingGoo.Name, 
        DeckingGoo.NickName, 
        DeckingGoo.Description + " parameter", 
        Components.Ribbon.CategoryName.Name(), 
        Components.Ribbon.SubCategoryName.Cat10()))
    {
    }
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + DeckingGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? DeckingGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("81411C5C-6EF7-4782-B173-CFB2C7355F4F");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.DeckingParam;

    protected override GH_GetterResult Prompt_Plural(ref List<DeckingGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref DeckingGoo value)
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
