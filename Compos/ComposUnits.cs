using System.Collections.Generic;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  public class ComposUnits
  {
    public AngleUnit Angle { get; set; }
    public DensityUnit Density { get; set; }
    public MassUnit Mass { get; set; }
    public ForceUnit Force { get; set; }
    public LengthUnit Length { get; set; }
    public LengthUnit Section { get; set; }
    public LengthUnit Displacement { get; set; }
    public PressureUnit Stress { get; set; }
    public StrainUnit Strain { get; set; }

    public ComposUnits() { }

    public static ComposUnits GetStandardUnits()
    {
      ComposUnits units = new ComposUnits
      {
        Angle = AngleUnit.Degree,
        Density = UnitsHelper.DensityUnit,
        Force = UnitsHelper.ForceUnit,
        Length = UnitsHelper.LengthUnitGeometry,
        Section = UnitsHelper.LengthUnitSection,
        Displacement = UnitsHelper.LengthUnitResult,
        Stress = UnitsHelper.StressUnit,
        Strain = UnitsHelper.StrainUnit,
        Mass = UnitsHelper.MassUnit
      };
      return units;
    }

    public string ToCoaString()
    {
      ComposUnits standardUnits = ComposUnits.GetStandardUnits();

      Force force = new Force(1.0, this.Force);
      double forceFactor = 1.0 / force.ToUnit(standardUnits.Force).Value;

      Length length = new Length(1.0, this.Length);
      double lengthFactor = 1.0 / length.ToUnit(standardUnits.Length).Value;

      Length displacement = new Length(1.0, this.Displacement);
      double displacementFactor = 1.0 / displacement.ToUnit(standardUnits.Displacement).Value;

      Length section = new Length(1.0, this.Section);
      double sectionFactor = 1.0 / section.ToUnit(standardUnits.Section).Value;

      Pressure stress = new Pressure(1.0, this.Stress);
      double stressFactor = 1.0 / stress.ToUnit(standardUnits.Stress).Value;

      Mass mass = new Mass(1.0, this.Mass);
      double massFactor = 1.0 / mass.ToUnit(standardUnits.Mass).Value;

      string coaString = "UNIT_DATA\tFORCE\t" + OasysUnits.Force.GetAbbreviation(this.Force) + "\t" + CoaHelper.FormatSignificantFigures(forceFactor, 6) + "\n";
      coaString += "UNIT_DATA\tLENGTH\t" + OasysUnits.Length.GetAbbreviation(this.Length) + "\t" + CoaHelper.FormatSignificantFigures(lengthFactor, 6) + "\n";
      coaString += "UNIT_DATA\tDISP\t" + OasysUnits.Length.GetAbbreviation(this.Displacement) + "\t" + CoaHelper.FormatSignificantFigures(displacementFactor, 6) + "\n";
      coaString += "UNIT_DATA\tSECTION\t" + OasysUnits.Length.GetAbbreviation(this.Section) + "\t" + CoaHelper.FormatSignificantFigures(sectionFactor, 6) + "\n";
      coaString += "UNIT_DATA\tSTRESS\t" + Pressure.GetAbbreviation(this.Stress) + "\t" + CoaHelper.FormatSignificantFigures(stressFactor, 6) + "\n";
      coaString += "UNIT_DATA\tMASS\t" + OasysUnits.Mass.GetAbbreviation(this.Mass) + "\t" + CoaHelper.FormatSignificantFigures(massFactor, 6) + "\n";
      return coaString;
    }

    internal void FromCoaString(List<string> parameters)
    {
      switch (parameters[1])
      {
        case CoaIdentifier.Units.Force:
          this.Force = (ForceUnit)UnitParser.Default.Parse(parameters[2], typeof(ForceUnit));
          break;
        case CoaIdentifier.Units.Length:
          this.Length = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
          break;
        case CoaIdentifier.Units.Displacement:
          this.Displacement = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
          break;
        case CoaIdentifier.Units.Section:
          this.Section = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
          break;
        case CoaIdentifier.Units.Stress:
          this.Stress = (PressureUnit)UnitParser.Default.Parse(parameters[2], typeof(PressureUnit));
          break;
        case CoaIdentifier.Units.Mass:
          this.Mass = (MassUnit)UnitParser.Default.Parse(parameters[2], typeof(MassUnit));
          break;
      }
    }
  }
}
