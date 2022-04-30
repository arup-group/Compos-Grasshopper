using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Collections;
using Rhino.Geometry;
using UnitsNet;
using ComposAPI.ConcreteSlab;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ERatioGoo : GH_Goo<ERatio>
  {
    #region constructors
    public ERatioGoo()
    {
      this.Value = new ERatio();
    }
    public ERatioGoo(ERatio item)
    {
      if (item == null)
        item = new ERatio();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ERatioGoo DuplicateGoo()
    {
      return new ERatioGoo(Value == null ? new ERatio() : Value.Duplicate());
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Steel/Concrete Modular Ratios";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid) { return string.Empty; }
        return IsValid.ToString(); // todo: beef this up to be more informative
      }
    }

    public override string ToString()
    {
      if (Value == null)
        return "Null";
      else
        return "Compos " + this.TypeName + " {" + Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this instance of our custom class into some other type Q.            
      if (typeof(Q).IsAssignableFrom(typeof(ERatio)))
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

      if (typeof(ERatio).IsAssignableFrom(source.GetType()))
      {
        this.Value = (ERatio)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
