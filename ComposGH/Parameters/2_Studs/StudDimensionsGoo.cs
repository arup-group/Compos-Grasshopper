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
  /// Goo wrapH class, makes sure our custom class can be used in GrasshopH.
  /// </summary>
  public class StudDimensionsGoo : GH_Goo<StudDimensions>
  {
    #region constructors
    public StudDimensionsGoo()
    {
      this.Value = new StudDimensions();
    }
    public StudDimensionsGoo(StudDimensions item)
    {
      if (item == null)
        item = new StudDimensions();
      this.Value = item.Duplicate() as StudDimensions;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public StudDimensionsGoo DuplicateGoo()
    {
      return new StudDimensionsGoo(Value == null ? new StudDimensions() : Value.Duplicate() as StudDimensions);
    }
    #endregion

    #region proHties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Stud Dimension";
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
      // This function is called when GrasshopH needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(StudDimensions)))
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
      // This function is called when GrasshopH needs to convert other data 
      // into our custom class.

      if (source == null) { return false; }

      //Cast from GsaMaterial
      if (typeof(StudDimensions).IsAssignableFrom(source.GetType()))
      {
        Value = (StudDimensions)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
