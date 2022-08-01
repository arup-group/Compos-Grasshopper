using UnitsNet;

namespace ComposAPI
{
  public interface IDeflectionLimits
  {
    Length AbsoluteDeflection { get; }
    Ratio SpanRatio { get; }
    string ToCoaString(string name);
  }
}
