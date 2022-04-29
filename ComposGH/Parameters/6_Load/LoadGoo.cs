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
using ComposAPI.Loads;

namespace ComposGH.Parameters
{

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class LoadGoo : GH_Goo<Load> // needs to be upgraded to GeometryGoo eventually....
  {
    #region constructors
    public LoadGoo()
    {
      this.Value = new Load();
    }
    public LoadGoo(Load item)
    {
      if (item == null)
        item = new Load();
      this.Value = item;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public LoadGoo DuplicateGoo()
    {
      return new LoadGoo(base.Value == null ? new Load() : base.Value);
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Load";
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

      if (typeof(Q).IsAssignableFrom(typeof(Load)))
      {
        if (base.Value == null)
          target = default;
        else
          target = (Q)(object)base.Value;
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
      if (typeof(Load).IsAssignableFrom(source.GetType()))
      {
        base.Value = (Load)source;
        return true;
      }

      return false;
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposLoadParameter : GH_PersistentParam<LoadGoo>
  {
    public ComposLoadParameter()
      : base(new GH_InstanceDescription("Load", "Ld", "Compos Load", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10()))
    {
    }

    public override Guid ComponentGuid => new Guid("3dc51bc1-9abb-4f26-845f-ca1e66236e9e");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.LoadParam;

    protected override GH_GetterResult Prompt_Plural(ref List<LoadGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref LoadGoo value)
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
