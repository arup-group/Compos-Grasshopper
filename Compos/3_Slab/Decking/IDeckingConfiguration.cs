using OasysUnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Custom interface that defines the basic properties and methods for our custom class
  /// </summary>
  public interface IDeckingConfiguration
  {
    Angle Angle { get; }
    bool IsDiscontinous { get; }
    bool IsWelded { get; }
    bool IsValid { get; }
  }
}
