using System.Collections.Generic;
using UnitsNet;

namespace ComposAPI
{
  public interface IResult
  {
    List<Length> Positions { get; }
    ICompositeSectionProperties SectionProperties { get; } // 1

    IBeamClassification BeamClassification { get; } // 2

    ICapacityResult Capacities { get; } // 1

    IInternalForceResult InternalForces { get; } // 1

    IUtilisation Utilisations { get; } // 0

    IBeamStressResult BeamStresses { get; } // 2

    ISlabStressResult SlabStresses { get; } // 2

    ITransverseRebarResult TransverseRebarResults { get; } // 2

    IStudResult StudResults { get; } // 2

    IDeflectionResult Deflections { get; } // 1
  }
}
