namespace ComposAPI {
  public interface IDesignOption {
    bool ConsiderShearDeflection { get; }
    bool InclConcreteSlabWeight { get; }
    bool InclSteelBeamWeight { get; }
    bool InclThinFlangeSections { get; }
    bool ProppedDuringConstruction { get; }
  }
}
