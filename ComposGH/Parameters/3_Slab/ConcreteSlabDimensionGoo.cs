using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Rhino.Collections;
using UnitsNet;
using UnitsNet.Units;
using ComposAPI.ConcreteSlab;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ConcreteSlabDimensionGoo : GH_Goo<ConcreteSlabDimension>
  {
    #region constructors
    public ConcreteSlabDimensionGoo()
    {
      this.Value = new ConcreteSlabDimension();
    }

    public ConcreteSlabDimensionGoo(ConcreteSlabDimension item)
    {
      if (item == null)
        item = new ConcreteSlabDimension();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ConcreteSlabDimensionGoo DuplicateGoo()
    {
      return new ConcreteSlabDimensionGoo(this.Value == null ? new ConcreteSlabDimension() : this.Value.Duplicate());
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Concrete Slab";
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
      if (this.Value == null)
        return "Null";
      else
        return "Compos " + this.TypeName + " {" + this.Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(ConcreteSlabDimension)))
      {
        if (this.Value == null)
          target = default;
        else
          target = (Q)(object)this.Value;
        return true;
      }

      //if (typeof(Q).IsAssignableFrom(typeof(string)))
      //{
      //  if (Value == null)
      //    target = default;
      //  else
      //    target = (Q)(object)Value.SectionDescription;
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

      //Cast from custom type
      if (typeof(ConcreteSlabDimension).IsAssignableFrom(source.GetType()))
      {
        this.Value = (ConcreteSlabDimension)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
