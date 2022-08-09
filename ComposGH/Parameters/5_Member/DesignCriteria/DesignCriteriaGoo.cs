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
  public class DesignCriteriaGoo : GH_Goo<IDesignCriteria>
  {
    #region constructors
    public DesignCriteriaGoo()
    {
      this.Value = new DesignCriteria();
    }
    public DesignCriteriaGoo(IDesignCriteria item)
    {
      if (item == null)
        item = new DesignCriteria();
      this.Value = item; //.Duplicate() as DesignCode;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public DesignCriteriaGoo DuplicateGoo()
    {
      return new DesignCriteriaGoo(Value == null ? new DesignCriteria() : Value);// .Duplicate() as DesignCode);
    }
    #endregion

    #region properties
    public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "DesignCriteria";
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
      if (Value == null)
        return "Null";
      else
        return "Compos " + TypeName + " {" + Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(DesignCriteria)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into our custom class.

      if (source == null) { return false; }

      //Cast from GsaMaterial
      if (typeof(DesignCriteria).IsAssignableFrom(source.GetType()))
      {
        Value = (DesignCriteria)source;
        return true;
      }

      return false;
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class DesignCriteriaParameter : GH_PersistentParam<DesignCriteriaGoo>
  {
    public DesignCriteriaParameter()
      : base(new GH_InstanceDescription("DesignCriteria", "Crt", "Maintains a collection of Compos Design Criteria data", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10()))
    {
    }
    public override Guid ComponentGuid => new Guid("48fe6e4a-2d32-415c-8ad9-1e467bdfbd01");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.DesignCodeParameter;

    protected override GH_GetterResult Prompt_Plural(ref List<DesignCriteriaGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref DesignCriteriaGoo value)
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
