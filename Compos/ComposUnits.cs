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

    public ComposUnits () { }

    public static ComposUnits GetStandardUnits()
    {
      ComposUnits units = new ComposUnits
      {
        Angle = AngleUnit.Degree,
        Density = Units.DensityUnit,
        Force = Units.ForceUnit,
        Length = Units.LengthUnitGeometry,
        Section = Units.LengthUnitSection,
        Displacement = Units.LengthUnitResult,
        Stress = Units.StressUnit,
        Strain = Units.StrainUnit,
        Mass = Units.MassUnit
      };
      return units;
    }

    internal void Change(List<string> parameters)
    {
      switch (parameters[1])
      {
        case CoaIdentifier.Units.Force:
          this.Force = (ForceUnit)UnitParser.Default.Parse(parameters[2], typeof(ForceUnit));
          break;
        case CoaIdentifier.Units.Length_Geometry:
          this.Length = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
          break;
        case CoaIdentifier.Units.Length_Section:
          this.Section = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
          break;
        //case CoaIdentifier.Units.Length_Results:
        //  lengtResultUnit = (LengthUnit)UnitParser.Default.Parse(parameters[2], typeof(LengthUnit));
        //break;
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
