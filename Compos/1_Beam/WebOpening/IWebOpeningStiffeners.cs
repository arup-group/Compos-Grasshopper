using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Define Stiffener Plate information used for a <see cref="IWebOpening"/>.
  /// </summary>
  public interface IWebOpeningStiffeners {
    Length BottomStiffenerThickness { get; }
    Length BottomStiffenerWidth { get; }
    Length DistanceFrom { get; }
    bool isBothSides { get; }
    bool isNotch { get; }
    Length TopStiffenerThickness { get; }
    Length TopStiffenerWidth { get; }
  }
}
