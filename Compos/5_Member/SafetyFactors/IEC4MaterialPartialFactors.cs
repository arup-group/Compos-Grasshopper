namespace ComposAPI
{
  /// <summary>
  /// Interface for custom material factors according to EN1994-1-1:2004. These data can be omitted, if they are omitted, code specified safety factor will be used.
  /// </summary>
  public interface IEC4MaterialPartialFactors
  {
    double gamma_M0 { get; }
    double gamma_M1 { get; }
    double gamma_M2 { get; }
    double gamma_C { get; }
    double gamma_Deck { get; }
    double gamma_vs { get; }
    double gamma_S { get; }
    string ToCoaString(string name);
  }
}
