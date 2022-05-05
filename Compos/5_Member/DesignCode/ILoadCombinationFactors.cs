namespace ComposAPI
{
  /// <summary>
  /// Interface for custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public interface ILoadCombinationFactors
  {
    double xi { get; }
    double psi_0 { get; }
    double gamma_G { get; }
    double gamma_Q { get; }
  }
}
