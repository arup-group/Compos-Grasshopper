using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Interface for accessing custom spacing/layout for a <see cref="Stud"/>
  /// </summary>
  public interface IStudGroupSpacing
  {
    IQuantity DistanceFromStart { get; }
    int NumberOfRows { get; }
    int NumberOfLines { get; }
    Length Spacing { get; }
  }
}
