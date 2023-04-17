using OasysUnits;

namespace ComposAPI {
  public interface IBeamSizeLimits {
    Length MaxDepth { get; }
    Length MaxWidth { get; }
    Length MinDepth { get; }
    Length MinWidth { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
