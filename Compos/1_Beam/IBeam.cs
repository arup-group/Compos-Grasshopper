using System.Collections.Generic;
using OasysUnits;

namespace ComposAPI
{
  public interface IBeam
  {
    Length Length { get; }
    IRestraint Restraint { get; }
    ISteelMaterial Material { get; }
    IList<IBeamSection> Sections { get; }
    IList<IWebOpening> WebOpenings { get; }

    string ToCoaString(string name, Code code, ComposUnits units);
  }
}
