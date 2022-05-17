using UnitsNet;

namespace ComposAPI
{
  public interface ICustomTransverseReinforcementLayout
  {
    Length DistanceFromStart { get; }
    Length DistanceFromEnd { get; }
    Length Diameter { get; }
    Length Spacing { get; }
    Length Cover { get; }
  }
}
