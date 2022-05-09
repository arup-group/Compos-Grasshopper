using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  /// <summary>
  /// Steel Material interface for a <see cref="IBeam"/>. Provides information about strength, density and Young's Modulus, as well as grade.
  /// </summary>
  public interface ISteelMaterial
  {
    Pressure fy { get; }
    Pressure E { get; }
    Density Density { get; }
    bool isCustom { get; }
    bool ReductionFactorMpl { get; }
    SteelMaterialGrade Grade { get; }
    WeldMaterialGrade WeldGrade { get; }

    string ToCoaString(string name, Code code, ComposUnits units);
  }
}
