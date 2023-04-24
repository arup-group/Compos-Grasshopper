namespace ComposAPI {
  /// <summary>
  /// Define the Steel/Concrete Modular Ratio used for a <see cref="IConcreteMaterial"/> interface.
  /// </summary>
  public interface IERatio {
    double LongTerm { get; }
    double ShortTerm { get; }
    double Shrinkage { get; }
    bool UserDefined { get; }
    double Vibration { get; }
  }
}
