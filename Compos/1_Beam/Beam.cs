using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;
using System.Drawing;

namespace ComposAPI
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class Beam : IBeam
  {
    public Length Length { get; set; }
    public IRestraint Restraint { get; set; }
    public ISteelMaterial Material { get; set; }
    public List<IBeamSection> BeamSections { get; internal set; } = new List<IBeamSection>();
    public List<IWebOpening> WebOpenings { get; internal set; } = null;

    #region constructors
    public Beam()
    {
      // empty constructor
    }

    public Beam(Length length, IRestraint restraint, ISteelMaterial material, List<IBeamSection> beamSections, List<IWebOpening> webOpenings = null)
    {
      this.Length = length;
      this.Restraint = restraint;
      this.Material = material;
      this.BeamSections = beamSections.ToList();
      if (webOpenings != null)
        this.WebOpenings = webOpenings.ToList();
    }

    #endregion

    #region coa interop
    internal Beam(string coaString)
    {
      // to do - implement from coa string method
    }

    public string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string profile = (this.BeamSections.Count > 1) ? string.Join(" : ", this.BeamSections.Select(x => x.SectionDescription).ToArray()) : this.BeamSections[0].SectionDescription;
      string mat = this.Material.ToString();
      string line = "L:" + this.Length.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      return line + ", " + profile + ", " + mat;
    }
    #endregion

  }
}
