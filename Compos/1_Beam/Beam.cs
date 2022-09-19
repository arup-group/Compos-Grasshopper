using System;
using System.Collections.Generic;
using System.Linq;
using OasysUnitsNet;
using OasysUnitsNet.Units;
using System.Drawing;
using ComposAPI.Helpers;

namespace ComposAPI
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class Beam : IBeam
  {
    public Length Length { get; set; } // span length
    public IRestraint Restraint { get; set; }
    public ISteelMaterial Material { get; set; }
    public IList<IBeamSection> Sections { get; internal set; } = new List<IBeamSection>();
    public IList<IWebOpening> WebOpenings { get; internal set; } = null;

    #region constructors
    public Beam()
    {
      // empty constructor
    }

    public Beam(Length length, IRestraint restraint, ISteelMaterial material, List<IBeamSection> sections, List<IWebOpening> webOpenings = null)
    {
      this.Length = length;
      this.Restraint = restraint;
      this.Material = material;
      this.Sections = sections;
      if (webOpenings != null)
        this.WebOpenings = webOpenings;
    }

    #endregion

    #region coa interop
    internal static IBeam FromCoaString(string coaString, string name, ComposUnits units, Code code)
    {
      Beam beam = new Beam();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return beam;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        switch (parameters[0])
        {
          case (CoaIdentifier.BeamSpanLength):
            beam.Length = CoaHelper.ConvertToLength(parameters[3], units.Length);
            break;

          case (CoaIdentifier.RetraintPoint):
          case (CoaIdentifier.RestraintTopFlange):
          case (CoaIdentifier.Restraint2ndBeam):
          case (CoaIdentifier.EndFlangeFreeRotate):
          case (CoaIdentifier.FinalRestraintPoint):
          case (CoaIdentifier.FinalRestraintNoStud):
          case (CoaIdentifier.FinalRestraint2ndBeam):
          case (CoaIdentifier.FinalEndFlangeFreeRotate):
            if (beam.Restraint == null) { beam.Restraint = new Restraint(); }
            Restraint restraint = (Restraint)beam.Restraint;
            // not static to update the object 
            restraint.FromCoaString(parameters, ComposUnits.GetStandardUnits());
            break;

          case (CoaIdentifier.BeamSteelMaterialStandard):
          case (CoaIdentifier.BeamSteelMaterialUser):
            beam.Material = SteelMaterial.FromCoaString(parameters, units, code);
            break;
          case (CoaIdentifier.BeamWeldingMaterial):
            SteelMaterial steelMaterial = (SteelMaterial)beam.Material;
            steelMaterial.WeldGrade = SteelMaterial.WeldGradeFromCoa(parameters);
            break;

          case (CoaIdentifier.BeamSectionAtX):
            beam.Sections.Add(BeamSection.FromCoaString(parameters, units));
            break;

          case (CoaIdentifier.WebOpeningDimension):
            if (beam.WebOpenings == null) { beam.WebOpenings = new List<IWebOpening>(); }
            beam.WebOpenings.Add(WebOpening.FromCoaString(parameters, units));
            break;

          default:
            // continue;
            break;
        }
      }
      return beam;
    }

    public string ToCoaString(string name, Code code, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      
      string str = this.Material.ToCoaString(name, code, units);
      
      parameters.Add(CoaIdentifier.BeamSpanLength);
      parameters.Add(name);
      // span number always 1 - placeholder for future feature in Compos to have continuous beams.
      parameters.Add(Convert.ToString(1));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.Length.ToUnit(units.Length).Value, 6));

      str += CoaHelper.CreateString(parameters);

      int num = 1;
      foreach (IBeamSection section in this.Sections)
        str += section.ToCoaString(name, this.Sections.Count, num++, units);

      str += this.Restraint.ToCoaString(name, units);

      if (this.WebOpenings != null)
      {
        foreach (IWebOpening webOpening in this.WebOpenings)
        {
          str += webOpening.ToCoaString(name, units);
        }
      }
      return str;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string invalid = "";
      string profile = "";
      if (this.Sections.Count == 0)
      {
        invalid = "Invalid Beam ";
        profile = "(no profile set)";
      }
      else
        profile = (this.Sections.Count > 1) ? string.Join(" : ", this.Sections.Select(x => x.SectionDescription).ToArray()) : this.Sections[0].SectionDescription;

      string mat = "";
      if (this.Material == null)
      {
        invalid = "Invalid Beam ";
        mat = "(no material set)";
      }
      else
        mat = this.Material.ToString();
      string line = "L:" + this.Length.ToUnit(UnitsHelper.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);

      return invalid + line + ", " + profile + ", " + mat;
    }
    #endregion

  }
}
