using OasysUnits;

namespace ComposAPI
{
  public interface IDeflectionLimit
  {
    Length AbsoluteDeflection { get; }
    Ratio SpanOverDeflectionRatio { get; }
    string ToCoaString(string name, DeflectionLimitLoadType type, ComposUnits units);
  }
}
