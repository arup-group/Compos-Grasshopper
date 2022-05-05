namespace ComposAPI
{
  /// <summary>
  /// Define the Steel/Concrete Modular Ratio used for a <see cref="IConcreteMaterial"/> interface.
  /// </summary>
  public interface IERatio
  {
    double ShortTerm { get; }
    double LongTerm { get; }
    double Vibration { get; }
    double Shrinkage { get; }
    bool UserDefined { get; }
  }
}
