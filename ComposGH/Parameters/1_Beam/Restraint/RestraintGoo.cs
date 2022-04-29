using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Documentation;
using Rhino.Collections;
using UnitsNet;
using ComposAPI.SteelBeam;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class RestraintGoo : GH_Goo<Restraint>
  {
    #region constructors
    public RestraintGoo()
    {
      this.Value = new Restraint();
    }
    public RestraintGoo(Restraint item)
    {
      if (item == null)
        item = new Restraint();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public RestraintGoo DuplicateGoo()
    {
      return new RestraintGoo(Value == null ? new Restraint() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Restraints";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
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

      if (typeof(Q).IsAssignableFrom(typeof(Restraint)))
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
      if (typeof(Restraint).IsAssignableFrom(source.GetType()))
      {
        Value = (Restraint)source;
        return true;
      }

      return false;
    }
    #endregion
  }

  ///// <summary>
  ///// This class provides a Parameter interface for the CustomGoo type.
  ///// </summary>
  //public class RestraintParameter : GH_PersistentParam<RestraintGoo>
  //{
  //  public RestraintParameter()
  //    : base(new GH_InstanceDescription("Restraint", "Res", "Compos Restraint", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
  //  {
  //  }
  //  public override Guid ComponentGuid => new Guid("496ab030-c665-4fbf-a3b9-15fa25713b20");

  //  public override GH_Exposure Exposure => GH_Exposure.secondary;

  //  protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.RestraintParam;

  //  protected override GH_GetterResult Prompt_Plural(ref List<RestraintGoo> values)
  //  {
  //    return GH_GetterResult.cancel;
  //  }
  //  protected override GH_GetterResult Prompt_Singular(ref RestraintGoo value)
  //  {
  //    return GH_GetterResult.cancel;
  //  }
  //  protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
  //  {
  //    System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
  //    {
  //      Text = "Not available",
  //      Visible = false
  //    };
  //    return item;
  //  }
  //  protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
  //  {
  //    System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
  //    {
  //      Text = "Not available",
  //      Visible = false
  //    };
  //    return item;
  //  }

  //  #region preview methods

  //  public bool Hidden
  //  {
  //    get { return true; }
  //    //set { m_hidden = value; }
  //  }
  //  public bool IsPreviewCapable
  //  {
  //    get { return false; }
  //  }
  //  #endregion
  //}

}
