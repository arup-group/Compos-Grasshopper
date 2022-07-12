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
  public class ComposFileGoo : GH_Goo<IComposFile>
  {
    #region constructors
    //public ComposFileGoo()
    //{
    //  this.Value = new ComposFile();
    //}

    public ComposFileGoo(IComposFile item)
    {
      if (item == null)
        item = new ComposFile();
      this.Value = item;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }

    public ComposFileGoo DuplicateGoo()
    {
      return new ComposFileGoo(Value == null ? new ComposFile() : Value);
    }
    #endregion

    #region properties
    public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "File";
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

      //if (typeof(Q).IsAssignableFrom(typeof(Stud)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //    target = (Q)(object)Value;
      //  return true;
      //}

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into our custom class.

      if (source == null) { return false; }

      ////Cast from GsaMaterial
      //if (typeof(Stud).IsAssignableFrom(source.GetType()))
      //{
      //  Value = (Stud)source;
      //  return true;
      //}

      return false;
    }
    #endregion
  }

  /// <summary>
  /// This class provides a Parameter interface for the CustomGoo type.
  /// </summary>

  public class ComposFileParameter : GH_PersistentParam<ComposFileGoo>
  {
    public ComposFileParameter()
      : base(new GH_InstanceDescription("Compos File", "Cob", "Maintains a collection of Compos File data", Components.Ribbon.CategoryName.Name(), Components.Ribbon.SubCategoryName.Cat10()))
    {
    }

    public override Guid ComponentGuid => new Guid("074492a1-8a5f-4823-972c-1c0abedd7498");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    //protected override System.Drawing.Bitmap Icon => Properties.Resources.ComposLogo128;

    protected override GH_GetterResult Prompt_Plural(ref List<ComposFileGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref ComposFileGoo value)
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
