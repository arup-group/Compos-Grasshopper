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
}
