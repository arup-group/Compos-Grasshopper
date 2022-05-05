namespace ComposAPI
{
  public interface IDesignOptions
  {
    bool ProppedDuringConstruction { get; }
    bool InclSteelBeamWeight { get; }
    bool InclThinFlangeSections { get; }
    bool ConsiderShearDeflection { get; }
  }
}
