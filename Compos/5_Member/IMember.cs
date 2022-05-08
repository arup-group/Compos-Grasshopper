using System.Collections.Generic;
using Oasys.Units;
using UnitsNet.Units;

namespace ComposAPI
{
  public interface IMember
  {
    IBeam Beam { get; }
    IStud Stud { get; }
    ISlab Slab { get; }
    List<ILoad> Loads { get; }
    IDesignCode DesignCode { get; }
    string Name { get; }
    string GridReference { get; }
    string Note { get; }

    string ToCoaString(AngleUnit angleUnit, DensityUnit densityUnit, ForceUnit forceUnit, LengthUnit lengthGeometryUnit, LengthUnit lengthSectionUnit, PressureUnit pressureUnit, StrainUnit strainUnit);
  }
}
