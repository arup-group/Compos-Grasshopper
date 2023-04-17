using System.Collections.Generic;

namespace ComposAPI {
  public interface IDesignCriteria {
    IDeflectionLimit AdditionalDeadLoad { get; }
    // beam size
    IBeamSizeLimits BeamSizeLimits { get; }

    IList<int> CatalogueSectionTypes { get; }
    // deflection limits
    IDeflectionLimit ConstructionDeadLoad { get; }
    IDeflectionLimit FinalLiveLoad { get; }
    IFrequencyLimits FrequencyLimits { get; }
    /// <summary>
    /// True to optimise for minimum weight, else (if false) minimum depth will be used as criterion
    /// </summary>
    OptimiseOption OptimiseOption { get; }
    /// <summary>
    /// Total loads minus construction dead load
    /// </summary>
    IDeflectionLimit PostConstruction { get; }
    IDeflectionLimit TotalLoads { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
