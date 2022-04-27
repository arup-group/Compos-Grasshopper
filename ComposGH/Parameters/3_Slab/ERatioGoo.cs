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
  public class ERatio
  {
    public double ShortTerm { get; set; }
    public double LongTerm { get; set; }
    public double Vibration { get; set; }
    public double Shrinkage { get; set; }
    public bool UserDefined { get; } = false;

    #region constructors
    public ERatio() { }

    public ERatio(double shortTerm, double longTerm, double vibration) : this(shortTerm, longTerm, vibration, double.NaN)
    {
    }

    public ERatio(double shortTerm, double longTerm, double vibration, double shrinkage)
    {
      this.ShortTerm = shortTerm;
      this.LongTerm = longTerm;
      this.Vibration = vibration;
      this.Shrinkage = shrinkage;
      this.UserDefined = true;
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
    internal ERatio(string coaString)
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
    public ERatio Duplicate()
    {
      if (this == null) { return null; }
      ERatio dup = (ERatio)this.MemberwiseClone();
      return dup;
    }

    public override string ToString()
    {
      string str = "ST: " + this.ShortTerm + ", LT: " + this.LongTerm + ", V: " + this.Vibration;
      if (this.Shrinkage > 0)
        str += ", S: " + this.Shrinkage;
      return str;
    }
    #endregion
  }

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
