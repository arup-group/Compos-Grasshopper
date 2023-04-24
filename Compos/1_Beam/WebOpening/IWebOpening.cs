using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Web Opening or Notch for a <see cref="IBeam"/> containing information about opening shape and optionally contains a <see cref="IWebOpeningStiffeners"/>.
  /// </summary>
  public interface IWebOpening {
    IQuantity CentroidPosFromStart { get; }
    IQuantity CentroidPosFromTop { get; }
    Length Diameter { get; }
    Length Height { get; }
    IWebOpeningStiffeners OpeningStiffeners { get; }
    OpeningType WebOpeningType { get; }
    Length Width { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
