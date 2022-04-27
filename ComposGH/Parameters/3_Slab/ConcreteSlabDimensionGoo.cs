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
using UnitsNet.Units;
using System.Globalization;
using System.IO;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class ConcreteSlabDimension
  {
    public Length StartPosition { get; set; } = Length.Zero;

    // Dimensions
    public Length OverallDepth { get; set; }
    public Length AvailableWidthLeft { get; set; }
    public Length AvailableWidtRight { get; set; }
    public bool OverrideEffectiveWidth { get; set; }
    public Length EffectiveWidthLeft { get; set; }
    public Length EffectiveWidthRight { get; set; }

    public bool TaperedToNext { get; set; }

    #region constructors
    public ConcreteSlabDimension()
    {
      // empty constructor
    }

    public ConcreteSlabDimension(Length startPosition, Length overallDepth, Length availableWidthLeft, Length availableWidthRight, bool overrideEffectiveWidth, 
      Length effectiveWidthLeft, Length effectiveWidthRight, bool taperedToNext = false)
    {
      this.StartPosition = startPosition;
      this.OverallDepth = overallDepth;
      this.AvailableWidthLeft = availableWidthLeft;
      this.AvailableWidtRight = availableWidthRight;
      this.OverrideEffectiveWidth = overrideEffectiveWidth;
      this.EffectiveWidthLeft = effectiveWidthLeft;
      this.EffectiveWidthRight = effectiveWidthRight;
      this.TaperedToNext = taperedToNext;
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

    #region methods

    public ConcreteSlabDimension Duplicate()
    {
      if (this == null) { return null; }
      ConcreteSlabDimension dup = (ConcreteSlabDimension)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string start = "";
      if (this.StartPosition != Length.Zero)
        start = ", Px:" + this.StartPosition.ToString("f0").Replace(" ", string.Empty);
      string tapered = "";
      if (this.TaperedToNext)
        tapered = ", Tapered";

      //return (this.SectionDescription == null) ? "Null profile" : this.SectionDescription + start + tapered;
      return "";
    }
    #endregion
  }

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
    public override bool IsValid => true;
    public override string TypeName => "Concrete Slab";
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
