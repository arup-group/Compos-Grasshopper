using OasysUnits;

namespace ComposAPI {
  public interface ICustomTransverseReinforcementLayout {
    Length Cover { get; }
    Length Diameter { get; }
    IQuantity EndPosition { get; }
    Length Spacing { get; }
    IQuantity StartPosition { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
