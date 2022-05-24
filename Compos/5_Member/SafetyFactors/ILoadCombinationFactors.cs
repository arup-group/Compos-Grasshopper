namespace ComposAPI
{
  /// <summary>
  /// Interface for custom Load Combination factors according to EN1994-1-1:2004. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public interface ILoadCombinationFactors
  {
    double Constantxi { get; }
    double Constantpsi_0 { get; }
    double Constantgamma_G { get; }
    double Constantgamma_Q { get; }
    double Finalxi { get; }
    double Finalpsi_0 { get; }
    double Finalgamma_G { get; }
    double Finalgamma_Q { get; }
    string ToCoaString(string name);
  }
}
