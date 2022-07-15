namespace ComposAPI
{
  public interface IDesignOption
  {
    bool ProppedDuringConstruction { get; }
    bool InclSteelBeamWeight { get; }
    bool InclThinFlangeSections { get; }
    bool ConsiderShearDeflection { get; }
    bool InclConcreteSlabWeight { get; }
  }
}
