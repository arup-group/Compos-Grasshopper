using System.Collections.Generic;
using UnitsNet;

namespace ComposAPI
{
  public interface IResult
  {
    List<Length> Positions { get; }
    ICompositeSectionProperties SectionProperties { get; }

    IBeamClassification BeamClassification { get; }

    ICapacityResult Capacities { get; }

    IInternalForceResult InternalForces { get; }

    IUtilisation Utilisations { get; }

    IBeamStressResult BeamStresses { get; }

    ISlabStressResult SlabStresses { get; }

    ITransverseRebarResult TransverseRebarResults { get; }

    IStudResult StudResults { get; }

    IDeflectionResult Deflections { get; }
  }
}
