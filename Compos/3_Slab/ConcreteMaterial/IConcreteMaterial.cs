using OasysUnits;
using static ComposAPI.ConcreteMaterial;

namespace ComposAPI {
  /// <summary>
  /// A Concrete Material interface that provides information about the material
  /// such as strength, grade, weight type, Young's modulus ratio, etc.
  /// /// </summary>
  public interface IConcreteMaterial {
    DensityClass Class { get; }
    Density DryDensity { get; }
    IERatio ERatio { get; }
    string Grade { get; }
    Ratio ImposedLoadPercentage { get; }
    Strain ShrinkageStrain { get; }
    WeightType Type { get; }
    bool UserDensity { get; }
    bool UserStrain { get; }

    string ToCoaString(string name, ComposUnits units);
  }
}
