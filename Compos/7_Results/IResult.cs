using System.Collections.Generic;
using OasysUnits;

namespace ComposAPI
{
  public interface IResult
  {
    List<Length> Positions { get; }

    IUtilisation Utilisations { get; }

    ICompositeSectionProperties SectionProperties { get; }
    IInternalForceResult InternalForces { get; }
    ICapacityResult Capacities { get; }
    IDeflectionResult Deflections { get; }

    IBeamClassification BeamClassification { get; }
    IBeamStressResult BeamStresses { get; }
    IStudResult StudResults { get; }
    ISlabStressResult SlabStresses { get; }
    ITransverseRebarResult TransverseRebarResults { get; } 
  }
}
