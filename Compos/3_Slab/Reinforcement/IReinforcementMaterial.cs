using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Custom interface that defines the basic properties and methods for our custom class
  /// </summary>
  public interface IReinforcementMaterial
  {
    Pressure Fu { get; }
  }
}
