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
  public class WebOpeningStiffeners
  {
    public Length DistanceFrom { get; set; }
    public Length TopStiffenerWidth { get; set; }
    public Length TopStiffenerThickness { get; set; }
    public Length BottomStiffenerWidth { get; set; }
    public Length BottomStiffenerThickness { get; set; }
    public bool isBothSides { get; set; }
    public bool isNotch { get; set; }

    #region constructors
    public WebOpeningStiffeners()
    {
      // empty constructor
    }
    /// <summary>
    /// Create web opening stiffeners
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="topWidth"></param>
    /// <param name="topTHK"></param>
    /// <param name="bottomWidth"></param>
    /// <param name="bottomTHK"></param>
    /// <param name="bothSides"></param>
    public WebOpeningStiffeners(Length distance, Length topWidth, Length topTHK, Length bottomWidth, Length bottomTHK, bool bothSides)
    {
      this.DistanceFrom = distance;
      this.TopStiffenerWidth = topWidth;
      this.TopStiffenerThickness = topTHK;
      this.BottomStiffenerWidth = bottomWidth;
      this.BottomStiffenerThickness = bottomTHK;
      this.isBothSides = bothSides;
      this.isNotch = false;
    }
    /// <summary>
    /// Create notch stiffener
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="topWidth"></param>
    /// <param name="topTHK"></param>
    /// <param name="bothSides"></param>
    public WebOpeningStiffeners(Length distance, Length topWidth, Length topTHK, bool bothSides)
    {
      this.isBothSides = bothSides;
      this.DistanceFrom = distance;
      this.TopStiffenerWidth = topWidth;
      this.TopStiffenerThickness = topTHK;
      this.isNotch = true;
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

    public WebOpeningStiffeners Duplicate()
    {
      if (this == null) { return null; }
      WebOpeningStiffeners dup = (WebOpeningStiffeners)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string start = (this.DistanceFrom.Value == 0) ? "" : "d:" + this.DistanceFrom.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      string top = (this.TopStiffenerWidth.Value == 0) ? "" : "Top:" + this.TopStiffenerWidth.As(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty)
          + "x" + this.TopStiffenerThickness.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      string bottom = (this.BottomStiffenerWidth.Value == 0) ? "" : "Bottom:" + this.BottomStiffenerWidth.As(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty)
          + "x" + this.BottomStiffenerThickness.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);

      return string.Join(", ", start, top, bottom).Trim(' ').TrimEnd(',');
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class WebOpeningStiffenersGoo : GH_Goo<WebOpeningStiffeners>
  {
    #region constructors
    public WebOpeningStiffenersGoo()
    {
      this.Value = new WebOpeningStiffeners();
    }
    public WebOpeningStiffenersGoo(WebOpeningStiffeners item)
    {
      if (item == null)
        item = new WebOpeningStiffeners();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public WebOpeningStiffenersGoo DuplicateGoo()
    {
      return new WebOpeningStiffenersGoo(Value == null ? new WebOpeningStiffeners() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Web Opening Stiffeners";
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

      if (typeof(Q).IsAssignableFrom(typeof(WebOpeningStiffeners)))
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
      if (typeof(WebOpeningStiffeners).IsAssignableFrom(source.GetType()))
      {
        Value = (WebOpeningStiffeners)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
