using System.Collections.Generic;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public interface IBeam
  {
    Length Length { get; }
    IRestraint Restraint { get; }
    ISteelMaterial Material { get; }
    List<IBeamSection> BeamSections { get; }
    List<IWebOpening> WebOpenings { get; }

    string ToCoaString(string name, Code code, DensityUnit densityUnit, LengthUnit lengthUnit, PressureUnit pressureUnit);
  }
}
