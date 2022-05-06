using UnitsNet.Units;

namespace ComposAPI
{
  public interface ILoad
  {
    LoadType Type { get; }

    string ToCoaString(ForceUnit forceUnit, LengthUnit lengthUnit, PressureUnit pressureUnit);
  }
}
