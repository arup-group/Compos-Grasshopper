namespace ComposAPI {
  public interface ICreepShrinkageParameters {
    /// <summary>
    /// Creep multiplier used for calculating E ratio for long term and shrinkage (see clause 5.4.2.2 of EN 1994-1-1:2004)
    /// </summary>
    double CreepCoefficient { get; set; }
  }
}
