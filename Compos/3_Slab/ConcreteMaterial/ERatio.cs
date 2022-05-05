using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposAPI
{
  /// <summary>
  /// Define the Steel/Concrete Modular Ratio used for a <see cref="ConcreteMaterial"/> object.
  /// </summary>
  public class ERatio : IERatio
  {
    public double ShortTerm { get; set; }
    public double LongTerm { get; set; }
    public double Vibration { get; set; }
    public double Shrinkage { get; set; }
    public bool UserDefined { get; set; } = false;

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
