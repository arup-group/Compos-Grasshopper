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
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class StudGroupSpacingGoo : GH_Goo<IStudGroupSpacing>
  {
    #region constructors
    public StudGroupSpacingGoo()
    {
      this.Value = new StudGroupSpacing();
    }
    public StudGroupSpacingGoo(IStudGroupSpacing item)
    {
      if (item == null)
        item = new StudGroupSpacing();
      this.Value = item; //.Duplicate() as StudGroupSpacing;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public StudGroupSpacingGoo DuplicateGoo()
    {
      return new StudGroupSpacingGoo(Value == null ? new StudGroupSpacing() : Value);// .Duplicate() as StudGroupSpacing);
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Stud Spacing";
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

      if (typeof(Q).IsAssignableFrom(typeof(StudGroupSpacing)))
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
      if (typeof(StudGroupSpacing).IsAssignableFrom(source.GetType()))
      {
        Value = (StudGroupSpacing)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
