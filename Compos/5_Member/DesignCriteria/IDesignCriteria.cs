using System.Collections.Generic;

namespace ComposAPI
{
  public interface IDesignCriteria
  {
    // beam size
    IBeamSizeLimits BeamSizeLimits { get; }

    // deflection limits
    IDeflectionLimit ConstructionDeadLoad { get; }
    IDeflectionLimit AdditionalDeadLoad { get; }
    IDeflectionLimit FinalLiveLoad { get; }
    IDeflectionLimit TotalLoads { get; }
    /// <summary>
    /// Total loads minus construction dead load
    /// </summary>
    IDeflectionLimit PostConstruction { get; }

    /// <summary>
    /// True to optimise for minimum weight, else (if false) minimum depth will be used as criterion
    /// </summary>
    OptimiseOption OptimiseOption { get; }

    IFrequencyLimits FrequencyLimits { get; }

    IList<int> CatalogueSectionTypes { get; }
    string ToCoaString(string name, ComposUnits units);
  }
}
