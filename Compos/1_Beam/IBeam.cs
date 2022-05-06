using System.Collections.Generic;
using UnitsNet;

namespace ComposAPI
{
  public interface IBeam
  {
    Length Length { get; }
    IRestraint Restraint { get; }
    ISteelMaterial Material { get; }
    List<IBeamSection> BeamSections { get; }
    List<IWebOpening> WebOpenings { get; }
  }
}
