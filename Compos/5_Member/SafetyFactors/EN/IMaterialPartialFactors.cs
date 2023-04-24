namespace ComposAPI {
  /// <summary>
  /// Interface for custom material factors according to EN1994-1-1:2004. These data can be omitted, if they are omitted, code specified safety factor will be used.
  /// </summary>
  public interface IMaterialPartialFactors {
    double Gamma_C { get; }
    double Gamma_Deck { get; }
    double Gamma_M0 { get; }
    double Gamma_M1 { get; }
    double Gamma_M2 { get; }
    double Gamma_S { get; }
    double Gamma_vs { get; }

    string ToCoaString(string name);
  }
}
