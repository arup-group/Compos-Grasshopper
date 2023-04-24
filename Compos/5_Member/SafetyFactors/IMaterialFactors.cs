namespace ComposAPI {
  /// <summary>
  /// Interface for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used.
  /// </summary>
  public interface IMaterialFactors {
    double ConcreteCompression { get; }
    double ConcreteShear { get; }
    double MetalDecking { get; }
    double Reinforcement { get; }
    double ShearStud { get; }
    double SteelBeam { get; }

    string ToCoaString(string name);
  }
}
