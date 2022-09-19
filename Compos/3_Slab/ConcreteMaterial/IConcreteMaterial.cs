using OasysUnitsNet;
using OasysUnitsNet.Units;
using static ComposAPI.ConcreteMaterial;

namespace ComposAPI
{
  /// <summary>
  /// A Concrete Material interface that provides information about the material
  /// such as strength, grade, weight type, Young's modulus ratio, etc.
  /// /// </summary>
  public interface IConcreteMaterial
  {
    string Grade { get; }
    WeightType Type { get; }
    DensityClass Class { get; }
    Density DryDensity { get; }
    bool UserDensity { get; }
    IERatio ERatio { get; }
    Ratio ImposedLoadPercentage { get; }
    Strain ShrinkageStrain { get; }
    bool UserStrain { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
