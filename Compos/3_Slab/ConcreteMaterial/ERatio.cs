using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposAPI
{
  /// <summary>
  /// Define the Steel/Concrete Modular Ratio used for a <see cref="ConcreteMaterial"/> object.
  /// </summary>
  public class ERatio
  {
    public double ShortTerm { get; set; } //	user defined steel to concrete modulus ratio for short term londing
    public double LongTerm { get; set; } // user defined steel to concrete modulus ratio for long term londing
    public double Vibration { get; set; } // user defined steel to concrete modulus ratio for vibration calculation
    public double Shrinkage { get; set; } // user defined steel to concrete modulus ratio for shrinkage concrete
    public bool UserDefined { get; } = false; // code or user defined steel to concrete Young's modulus ratio

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
}
