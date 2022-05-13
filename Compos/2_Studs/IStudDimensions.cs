using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Interface for accessing dimensions and strength for a <see cref="IStud"/>
  /// </summary>
  public interface IStudDimensions
  {
    Length Diameter { get; }
    Length Height { get; }
    Force CharacterStrength { get; }
    Pressure Fu { get; }
    bool isStandard { get; }
  }
}
