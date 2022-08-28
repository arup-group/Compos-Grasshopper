namespace ComposAPI
{
  public interface IResult
  {
    ICompositeSectionProperties SectionProperties { get; }

    ICapacityResult Capacities { get; }

    IInternalForceResult InternalForces { get; }

    IBeamStressResult BeamStresses { get; }

    ISlabStressResult SlabStresses { get; }

    IStudResult StudResults { get; }

    IDeflectionResult Deflections { get; }

    
  }
}
