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

namespace ComposGH.Parameters
{

  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class ComposRestraint
  {
    public Supports ConstructionStageSupports { get; set; }
    public Supports FinalStageSupports { get; set; }
    public bool TopFlangeRestrained { get; set; }
    private bool finalSupportsSet;

    #region constructors
    public ComposRestraint()
    {
      // empty constructore
    }
    public ComposRestraint(bool topFlangeRestrained, Supports constructionStageSupports, Supports finalStageSupports)
    {
      this.TopFlangeRestrained = topFlangeRestrained;
      this.ConstructionStageSupports = constructionStageSupports;
      this.FinalStageSupports = finalStageSupports;
      this.finalSupportsSet = true;
    }
    public ComposRestraint(bool topFlangeRestrained, Supports constructionStageSupports)
    {
      this.TopFlangeRestrained = topFlangeRestrained;
      this.ConstructionStageSupports = constructionStageSupports;
      this.finalSupportsSet = false;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region coa interop
    internal ComposRestraint(string coaString)
    {
      // to be done
    }

    internal string Coa()
    {
      // to be done
      return "";
    }
    #endregion

    #region methods

    public ComposRestraint Duplicate()
    {
      if (this == null) { return null; }
      ComposRestraint dup = (ComposRestraint)this.MemberwiseClone();
      dup.ConstructionStageSupports = this.ConstructionStageSupports.Duplicate();
      if (this.finalSupportsSet)
        dup.FinalStageSupports = this.FinalStageSupports.Duplicate();
      return dup;
    }

    public override string ToString()
    {
      string top = (TopFlangeRestrained) ? "Top flng. rest., " : "";
      string con = "Constr.: " + this.ConstructionStageSupports.ToString();
      string fin = ", Final: " + this.FinalStageSupports.ToString();
      return top + con + fin;
    }

    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ComposRestraintGoo : GH_Goo<ComposRestraint>
  {
    #region constructors
    public ComposRestraintGoo()
    {
      this.Value = new ComposRestraint();
    }
    public ComposRestraintGoo(ComposRestraint item)
    {
      if (item == null)
        item = new ComposRestraint();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ComposRestraintGoo DuplicateGoo()
    {
      return new ComposRestraintGoo(Value == null ? new ComposRestraint() : Value.Duplicate());
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

      if (typeof(Q).IsAssignableFrom(typeof(ComposRestraint)))
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
      if (typeof(ComposRestraint).IsAssignableFrom(source.GetType()))
      {
        Value = (ComposRestraint)source;
        return true;
      }

      return false;
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>
  public class ComposRestraintParameter : GH_PersistentParam<ComposRestraintGoo>
  {
    public ComposRestraintParameter()
      : base(new GH_InstanceDescription("Restraint", "Res", "Compos Restraint", ComposGH.Components.Ribbon.CategoryName.Name(), ComposGH.Components.Ribbon.SubCategoryName.Cat10()))
    {
    }
    public override Guid ComponentGuid => new Guid("496ab030-c665-4fbf-a3b9-15fa25713b20");

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => ComposGH.Properties.Resources.DesignCodeParameter;

    protected override GH_GetterResult Prompt_Plural(ref List<ComposRestraintGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref ComposRestraintGoo value)
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
