using System.Collections.Generic;
using UnitsNet;

namespace ComposAPI
{
  public interface IDesignCriteria
  {
    // beam size
    IBeamSizeLimits BeamSizeLimits { get; }

    // deflection limits
    IDeflectionLimits ConstructionDeadLoad { get; }
    IDeflectionLimits AdditionalDeadLoad { get; }
    IDeflectionLimits FinalLiveLoad { get; }
    IDeflectionLimits TotalLoads { get; }
    /// <summary>
    /// Total loads minus construction dead load
    /// </summary>
    IDeflectionLimits PostConstruction { get; }
    
    /// <summary>
    /// True to optimise for minimum weight, else (if false) minimum depth will be used as criterion
    /// </summary>
    bool OptimiseForWeight { get; }

    IFrequencyLimits FrequencyLimits { get; }

    IList<int> CatalogueSectionTypes { get; }
    string ToCoaString(string name);
  }
}
