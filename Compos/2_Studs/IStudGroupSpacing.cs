using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Interface for accessing custom spacing/layout for a <see cref="Stud"/>
  /// </summary>
  public interface IStudGroupSpacing {
    IQuantity DistanceFromStart { get; }
    int NumberOfLines { get; }
    int NumberOfRows { get; }
    Length Spacing { get; }
  }
}
