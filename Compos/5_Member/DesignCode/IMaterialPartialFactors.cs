namespace ComposAPI
{
  /// <summary>
  /// Interface for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used.
  /// </summary>
  public interface IMaterialPartialFactors
  {
    double SteelBeam { get; }
    double ConcreteCompression { get; }
    double ConcreteShear { get; }
    double MetalDecking { get; }
    double ShearStud { get; }
    double Reinforcement { get; }
  }
}
