namespace ComposAPI {
  /// <summary>
  /// Interface for custom Load Combination factors according to EN1994-1-1:2004. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public interface ILoadCombinationFactors {
    double Constantgamma_G { get; }
    double Constantgamma_Q { get; }
    double ConstantPsi { get; }
    double ConstantXi { get; }
    double Finalgamma_G { get; }
    double Finalgamma_Q { get; }
    double FinalPsi { get; }
    double FinalXi { get; }
    LoadCombination LoadCombination { get; set; }

    string ToCoaString(string name);
  }
}
