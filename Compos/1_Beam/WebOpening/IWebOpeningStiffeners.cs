using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Define Stiffener Plate information used for a <see cref="IWebOpening"/>.
  /// </summary>
  public interface IWebOpeningStiffeners
  {
    Length DistanceFrom { get; }
    Length TopStiffenerWidth { get; }
    Length TopStiffenerThickness { get; }
    Length BottomStiffenerWidth { get; }
    Length BottomStiffenerThickness { get; }
    bool isBothSides { get; }
    bool isNotch { get; }
  }
}
