using System.Collections.Generic;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class ComposUnits {
    public AngleUnit Angle { get; set; }
    public DensityUnit Density { get; set; }
    public LengthUnit Displacement { get; set; }
    public ForceUnit Force { get; set; }
    public LengthUnit Length { get; set; }
    public MassUnit Mass { get; set; }
    public LengthUnit Section { get; set; }
    public StrainUnit Strain { get; set; }
    public PressureUnit Stress { get; set; }

    public ComposUnits() { }

    public static ComposUnits GetStandardUnits() {
      var units = new ComposUnits {
        Angle = AngleUnit.Degree,
        Density = ComposUnitsHelper.DensityUnit,
        Force = ComposUnitsHelper.ForceUnit,
        Length = ComposUnitsHelper.LengthUnitGeometry,
        Section = ComposUnitsHelper.LengthUnitSection,
        Displacement = ComposUnitsHelper.LengthUnitResult,
        Stress = ComposUnitsHelper.StressUnit,
        Strain = ComposUnitsHelper.StrainUnit,
        Mass = ComposUnitsHelper.MassUnit
      };
      return units;
    }

    public string ToCoaString() {
      var standardUnits = ComposUnits.GetStandardUnits();

      var force = new Force(1.0, Force);
      double forceFactor = 1.0 / force.ToUnit(standardUnits.Force).Value;

      var length = new Length(1.0, Length);
      double lengthFactor = 1.0 / length.ToUnit(standardUnits.Length).Value;

      var displacement = new Length(1.0, Displacement);
      double displacementFactor = 1.0 / displacement.ToUnit(standardUnits.Displacement).Value;

      var section = new Length(1.0, Section);
      double sectionFactor = 1.0 / section.ToUnit(standardUnits.Section).Value;

      var stress = new Pressure(1.0, Stress);
      double stressFactor = 1.0 / stress.ToUnit(standardUnits.Stress).Value;

      var mass = new Mass(1.0, Mass);
      double massFactor = 1.0 / mass.ToUnit(standardUnits.Mass).Value;

      string coaString = "UNIT_DATA\tFORCE\t" + OasysUnits.Force.GetAbbreviation(Force) + "\t" + CoaHelper.FormatSignificantFigures(forceFactor, 6) + "\n";
      coaString += "UNIT_DATA\tLENGTH\t" + OasysUnits.Length.GetAbbreviation(Length) + "\t" + CoaHelper.FormatSignificantFigures(lengthFactor, 6) + "\n";
      coaString += "UNIT_DATA\tDISP\t" + OasysUnits.Length.GetAbbreviation(Displacement) + "\t" + CoaHelper.FormatSignificantFigures(displacementFactor, 6) + "\n";
      coaString += "UNIT_DATA\tSECTION\t" + OasysUnits.Length.GetAbbreviation(Section) + "\t" + CoaHelper.FormatSignificantFigures(sectionFactor, 6) + "\n";
      coaString += "UNIT_DATA\tSTRESS\t" + Pressure.GetAbbreviation(Stress) + "\t" + CoaHelper.FormatSignificantFigures(stressFactor, 6) + "\n";
      coaString += "UNIT_DATA\tMASS\t" + OasysUnits.Mass.GetAbbreviation(Mass) + "\t" + CoaHelper.FormatSignificantFigures(massFactor, 6) + "\n";
      return coaString;
    }

    internal void FromCoaString(List<string> parameters) {
      switch (parameters[1]) {
        case CoaIdentifier.Units.Force:
          Force = (ForceUnit)OasysUnitsSetup.Default.UnitParser.Parse(parameters[2], typeof(ForceUnit));
          break;

        case CoaIdentifier.Units.Length:
          Length = (LengthUnit)OasysUnitsSetup.Default.UnitParser.Parse(parameters[2], typeof(LengthUnit));
          break;

        case CoaIdentifier.Units.Displacement:
          Displacement = (LengthUnit)OasysUnitsSetup.Default.UnitParser.Parse(parameters[2], typeof(LengthUnit));
          break;

        case CoaIdentifier.Units.Section:
          Section = (LengthUnit)OasysUnitsSetup.Default.UnitParser.Parse(parameters[2], typeof(LengthUnit));
          break;

        case CoaIdentifier.Units.Stress:
          Stress = (PressureUnit)OasysUnitsSetup.Default.UnitParser.Parse(parameters[2], typeof(PressureUnit));
          break;

        case CoaIdentifier.Units.Mass:
          Mass = (MassUnit)OasysUnitsSetup.Default.UnitParser.Parse(parameters[2], typeof(MassUnit));
          break;
      }
    }
  }
}
