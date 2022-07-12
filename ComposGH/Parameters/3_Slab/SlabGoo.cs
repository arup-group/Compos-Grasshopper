using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class SlabGoo : GH_Goo<ISlab>
  {
    #region constructors
    //public SlabGoo()
    //{
    //  this.Value = new Slab();
    //}

    public SlabGoo(ISlab item)
    {
      if (item == null)
        item = new Slab();
      this.Value = item; //.Duplicate() as ISlab;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public SlabGoo DuplicateGoo()
    {
      return new SlabGoo(this.Value == null ? new Slab() : this.Value);// .Duplicate() as ISlab);
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Slab";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid) { return string.Empty; }
        return IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }
    public override string ToString()
    {
      if (this.Value == null)
        return "Null";
      else
        return "Compos " + this.TypeName + " {" + this.Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this instance of our custom class into some other type Q.            
      if (typeof(Q).IsAssignableFrom(typeof(Slab)))
      {
        if (this.Value == null)
          target = default;
        else
          target = (Q)(object)this.Value;
        return true;
      }

      target = default;
      return false;
    }

    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data into our custom class.
      if (source == null) { return false; }

      if (typeof(Slab).IsAssignableFrom(source.GetType()))
      {
        this.Value = (Slab)source;
        return true;
      }

      return false;
    }
    #endregion
  }
  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>

  public class ComposSlabParameter : GH_PersistentParam<SlabGoo>
  {
    public ComposSlabParameter()
      : base(new GH_InstanceDescription("Slab", "Sla", "Maintains a collection of Compos Slab data", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10()))
    {
    }

    public override Guid ComponentGuid => new Guid("e1c8e010-a55d-4f41-8f37-8d6e56976975");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.SlabParam;

    protected override GH_GetterResult Prompt_Plural(ref List<SlabGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref SlabGoo value)
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
