using UnitsNet.Units;

namespace ComposAPI
{
  public interface ILoad
  {
    LoadType Type { get; }

    string ToCoaString(string name, ForceUnit forceUnit, LengthUnit lengthUnit);
  }
}
