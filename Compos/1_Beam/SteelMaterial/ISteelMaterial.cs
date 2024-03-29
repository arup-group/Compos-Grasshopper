﻿using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Steel Material interface for a <see cref="IBeam"/>. Provides information about strength, density and Young's Modulus, as well as grade.
  /// </summary>
  public interface ISteelMaterial {
    Density Density { get; }
    Pressure E { get; }
    Pressure Fy { get; }
    StandardSteelGrade Grade { get; }
    bool IsCustom { get; }
    bool ReductionFactorMpl { get; }
    WeldMaterialGrade WeldGrade { get; }

    string ToCoaString(string name, Code code, ComposUnits units);
  }
}
