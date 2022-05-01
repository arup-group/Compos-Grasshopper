using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;
using System.Drawing;

namespace ComposAPI.SteelBeam
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class SteelBeam
  {

    public Length Length { get; set; }
    public Restraint Restraint { get; set; }
    public SteelMaterial Material { get; set; }
    public List<BeamSection> BeamSections { get; internal set; } = new List<BeamSection>();
    public List<WebOpening> WebOpenings { get; internal set; } = null;

    #region constructors
    public SteelBeam()
    {
      // empty constructor
    }

    public SteelBeam(Length length, Restraint restraint, SteelMaterial material, List<BeamSection> beamSections, List<WebOpening> webOpenings = null)
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
    internal SteelBeam(string coaString)
    {
      // to do - implement from coa string method
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }
    #endregion

    #region methods

    public SteelBeam Duplicate()
    {
      if (this == null) { return null; }
      SteelBeam dup = (SteelBeam)this.MemberwiseClone();
      dup.Material = this.Material.Duplicate();
      dup.Restraint = this.Restraint.Duplicate();
      dup.BeamSections = this.BeamSections.ToList();
      if (this.WebOpenings != null)
        dup.WebOpenings = this.WebOpenings.ToList();
      return dup;
    }

    public override string ToString()
    {
      string profile = (this.BeamSections.Count > 1) ? string.Join(" : ", this.BeamSections.Select(x => x.SectionDescription).ToArray()) : this.BeamSections[0].SectionDescription;
      string mat = this.Material.ToString();
      string line = "L:" + this.Length.ToUnit(Helpers.Units.FileUnits.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      return line + ", " + profile + ", " + mat;
    }
    #endregion

  }
}
