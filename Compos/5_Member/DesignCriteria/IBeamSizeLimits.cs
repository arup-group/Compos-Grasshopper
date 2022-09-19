using OasysUnitsNet;

namespace ComposAPI
{
  public interface IBeamSizeLimits
  {
    Length MinDepth { get; }
    Length MaxDepth { get; }
    Length MinWidth { get; }
    Length MaxWidth { get; }
    string ToCoaString(string name, ComposUnits units);
  }
}
