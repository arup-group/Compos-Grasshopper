using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
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
  }
}
