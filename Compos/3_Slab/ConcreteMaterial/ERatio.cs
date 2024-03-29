﻿namespace ComposAPI {
  /// <summary>
  /// Define the Steel/Concrete Modular Ratio used for a <see cref="ConcreteMaterial"/> object.
  /// </summary>
  public class ERatio : IERatio {
    public double LongTerm { get; set; }
    public double ShortTerm { get; set; } //	user defined steel to concrete modulus ratio for short term londing
    public double Shrinkage { get; set; }
    // user defined steel to concrete modulus ratio for shrinkage concrete
    public bool UserDefined { get; set; } = false;
    // user defined steel to concrete modulus ratio for long term londing
    public double Vibration { get; set; } // user defined steel to concrete modulus ratio for vibration calculation
                                          // code or user defined steel to concrete Young's modulus ratio

    public ERatio() { }

    public ERatio(double shortTerm, double longTerm, double vibration) : this(shortTerm, longTerm, vibration, double.NaN) {
    }

    public ERatio(double shortTerm, double longTerm, double vibration, double shrinkage) {
      ShortTerm = shortTerm;
      LongTerm = longTerm;
      Vibration = vibration;
      Shrinkage = shrinkage;
      UserDefined = true;
    }

    public override string ToString() {
      string str = "ST: " + ShortTerm + ", LT: " + LongTerm + ", V: " + Vibration;
      if (Shrinkage > 0) {
        str += ", S: " + Shrinkage;
      }
      return str;
    }
  }
}
