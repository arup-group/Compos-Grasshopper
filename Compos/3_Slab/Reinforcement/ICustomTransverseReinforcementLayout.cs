using UnitsNet;

namespace ComposAPI
{
  public interface ICustomTransverseReinforcementLayout
  {
    IQuantity StartPosition { get; }
    IQuantity EndPosition { get; }
    Length Diameter { get; }
    Length Spacing { get; }
    Length Cover { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
