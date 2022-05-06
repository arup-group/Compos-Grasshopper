namespace ComposAPI
{
  public interface IDesignOptions
  {
    bool ProppedDuringConstruction { get; }
    bool InclSteelBeamWeight { get; }
    bool InclThinFlangeSections { get; }
    bool ConsiderShearDeflection { get; }

    string ToCoaString(string name, Code code);
  }
}
