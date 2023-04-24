using System.Collections.Generic;
using OasysUnits;

namespace ComposAPI {
  public interface IResult {
    IBeamClassification BeamClassification { get; }
    IBeamStressResult BeamStresses { get; }
    ICapacityResult Capacities { get; }
    IDeflectionResult Deflections { get; }
    IInternalForceResult InternalForces { get; }
    List<Length> Positions { get; }
    ICompositeSectionProperties SectionProperties { get; }
    ISlabStressResult SlabStresses { get; }
    IStudResult StudResults { get; }
    ITransverseRebarResult TransverseRebarResults { get; }
    IUtilisation Utilisations { get; }
  }
}
