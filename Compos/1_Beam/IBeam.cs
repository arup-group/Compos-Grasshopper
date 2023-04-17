using OasysUnits;
using System.Collections.Generic;

namespace ComposAPI {
  public interface IBeam {
    Length Length { get; }
    ISteelMaterial Material { get; }
    IRestraint Restraint { get; }
    IList<IBeamSection> Sections { get; }
    IList<IWebOpening> WebOpenings { get; }

    string ToCoaString(string name, Code code, ComposUnits units);
  }
}
