using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Web Opening or Notch for a <see cref="IBeam"/> containing information about opening shape and optionally contains a <see cref="IWebOpeningStiffeners"/>.
  /// </summary>
  public interface IWebOpening
  {
    OpeningType WebOpeningType { get; }
    Length Width { get; }
    Length Height { get; }
    Length Diameter { get; }
    Length CentroidPosFromStart { get; }
    Length CentroidPosFromTop { get; }
    IWebOpeningStiffeners OpeningStiffeners { get; }
  }
}