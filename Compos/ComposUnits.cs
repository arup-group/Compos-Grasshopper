using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComposAPI.Helpers;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

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

      string coaString = "UNIT_DATA\tFORCE\t" + ComposUnits.GetAbbreviation(this.Force) + "\t" + CoaHelper.FormatSignificantFigures(forceFactor, 6) + "\n";
      coaString += "UNIT_DATA\tLENGTH\t" + ComposUnits.GetAbbreviation(this.Length) + "\t" + CoaHelper.FormatSignificantFigures(lengthFactor, 6) + "\n";
      coaString += "UNIT_DATA\tDISP\t" + ComposUnits.GetAbbreviation(this.Displacement) + "\t" + CoaHelper.FormatSignificantFigures(displacementFactor, 6) + "\n";
      coaString += "UNIT_DATA\tSECTION\t" + ComposUnits.GetAbbreviation(this.Section) + "\t" + CoaHelper.FormatSignificantFigures(sectionFactor, 6) + "\n";
      coaString += "UNIT_DATA\tSTRESS\t" + ComposUnits.GetAbbreviation(this.Stress) + "\t" + CoaHelper.FormatSignificantFigures(stressFactor, 6) + "\n";
      coaString += "UNIT_DATA\tMASS\t" + ComposUnits.GetAbbreviation(this.Mass) + "\t" + CoaHelper.FormatSignificantFigures(massFactor, 6) + "\n";
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
          try
          {
            this.Stress = (PressureUnit)UnitParser.Default.Parse(parameters[2], typeof(PressureUnit));
          }
          catch (Exception)
          {
            parameters[2] = parameters[2].Remove(parameters[2].Length - 1);
            parameters[2] += "²";
            this.Stress = (PressureUnit)UnitParser.Default.Parse(parameters[2], typeof(PressureUnit));
          }
          break;
        case CoaIdentifier.Units.Mass:
          this.Mass = (MassUnit)UnitParser.Default.Parse(parameters[2], typeof(MassUnit));
          break;
      }
    }

    internal static string GetAbbreviation(DensityUnit unit)
    {
      switch (unit)
      {
        case (DensityUnit.KilogramPerCubicMeter):
          return "kg/m³";

        default:
          throw new NotImplementedException("");
      }
    }

    internal static string GetAbbreviation(MassUnit unit)
    {
      switch (unit)
      {
        case (MassUnit.Kilogram):
          return "kg";

        default:
          throw new NotImplementedException("");
      }
    }

    internal static string GetAbbreviation(ForceUnit unit)
    {
      switch (unit)
      {
        case (ForceUnit.Newton):
          return "N";
        case (ForceUnit.Kilonewton):
          return "kN";

        default:
          throw new NotImplementedException("");
      }
    }

    internal static string GetAbbreviation(LengthUnit unit)
    {
      switch (unit)
      {
        case (LengthUnit.Millimeter):
          return "mm";
        case (LengthUnit.Centimeter):
          return "cm";
        case (LengthUnit.Meter):
          return "m";

        default:
          throw new NotImplementedException("");
      }
    }

    internal static string GetAbbreviation(PressureUnit unit)
    {
      switch (unit)
      {
        case (PressureUnit.NewtonPerSquareMeter):
          return "N/m²";

        default:
          throw new NotImplementedException("");
      }
    }

    internal static string GetAbbreviation(StrainUnit unit)
    {
      return Oasys.Units.Strain.GetAbbreviation(unit);
    }
  }
}
