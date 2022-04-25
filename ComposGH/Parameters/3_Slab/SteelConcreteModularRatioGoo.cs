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
  public class SteelConcreteModularRatio
  {
    public double ShortTerm { get; set; }

    public double LongTerm { get; set; }

    public double Vibration { get; set; }

    public double Shrinkage { get; set; }

    #region constructors
    public SteelConcreteModularRatio() { }

    public SteelConcreteModularRatio(double shortTerm, double longTerm, double vibration)
    {
      this.ShortTerm = shortTerm;
      this.LongTerm = longTerm;
      this.Vibration = vibration;
    }

    public SteelConcreteModularRatio(double shortTerm, double longTerm, double vibration, double shrinkage) : this(shortTerm, longTerm, vibration)
    {
      this.Shrinkage = shrinkage;
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
    internal SteelConcreteModularRatio(string coaString)
    {
      // to do - implement from coa string method
    }
    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }
    #endregion

    #region methods
    public SteelConcreteModularRatio Duplicate()
    {
      if (this == null) { return null; }
      SteelConcreteModularRatio dup = (SteelConcreteModularRatio)this.MemberwiseClone();
      return dup;
    }

    public override string ToString()
    {
      // check value of optional field

      return "ST: " + ShortTerm + ", LT: " + LongTerm + ", ";
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class SteelConcreteModularRatioGoo : GH_Goo<SteelConcreteModularRatio>
  {
    #region constructors
    public SteelConcreteModularRatioGoo()
    {
      this.Value = new SteelConcreteModularRatio();
    }
    public SteelConcreteModularRatioGoo(SteelConcreteModularRatio item)
    {
      if (item == null)
        item = new SteelConcreteModularRatio();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public SteelConcreteModularRatioGoo DuplicateGoo()
    {
      return new SteelConcreteModularRatioGoo(Value == null ? new SteelConcreteModularRatio() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Steel/Concrete Modular Ratios";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); // todo: beef this up to be more informative
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
      // This function is called when Grasshopper needs to convert this instance of our custom class into some other type Q.            
      if (typeof(Q).IsAssignableFrom(typeof(SteelConcreteModularRatio)))
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
      // This function is called when Grasshopper needs to convert other data into our custom class.
      if (source == null) { return false; }

      if (typeof(SteelConcreteModularRatio).IsAssignableFrom(source.GetType()))
      {
        Value = (SteelConcreteModularRatio)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
