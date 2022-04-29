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
using ComposAPI.ConcreteSlab;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ReinforcementGoo : GH_Goo<Reinforcement>
  {
    #region constructors
    public ReinforcementGoo()
    {
      this.Value = new Reinforcement();
    }
    public ReinforcementGoo(Reinforcement item)
    {
      if (item == null)
        item = new Reinforcement();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ReinforcementGoo DuplicateGoo()
    {
      return new ReinforcementGoo(Value == null ? new Reinforcement() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Reinforcement";
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

      if (typeof(Q).IsAssignableFrom(typeof(Reinforcement)))
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
      if (typeof(Reinforcement).IsAssignableFrom(source.GetType()))
      {
        Value = (Reinforcement)source;
        return true;
      }

      return false;
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposReinforcementParameter : GH_PersistentParam<ReinforcementGoo>
  {
    public ComposReinforcementParameter()
      : base(new GH_InstanceDescription("Reinforcement", "Rb", "Compos Slab Reinforcement", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10()))
    {
    }

    public override Guid ComponentGuid => new Guid("c6261340-24f2-4d79-9894-5cd945023163");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.ReinforcementParam;

    protected override GH_GetterResult Prompt_Plural(ref List<ReinforcementGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref ReinforcementGoo value)
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
