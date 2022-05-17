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
    IList<IBeamSection> BeamSections { get; }
    IList<IWebOpening> WebOpenings { get; }

    string ToCoaString(string name, Code code, ComposUnits units);
  }
}
