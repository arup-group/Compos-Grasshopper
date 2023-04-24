using System.Collections.Generic;
using ComposAPI.Helpers;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class ComposUnitsTest {

    [Theory]
    // Force
    [InlineData("UNIT_DATA	FORCE	N	1.00000\n", ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    [InlineData("UNIT_DATA	FORCE	kN	0.00100000\n", ForceUnit.Kilonewton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    // Length
    [InlineData("UNIT_DATA	LENGTH	m	1.00000\n", ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    [InlineData("UNIT_DATA	LENGTH	mm	1000.00\n", ForceUnit.Newton, LengthUnit.Millimeter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    // Displacement
    [InlineData("UNIT_DATA	DISP	m	1.00000\n", ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    [InlineData("UNIT_DATA	DISP	mm	1000.00\n", ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Millimeter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    // Section
    [InlineData("UNIT_DATA	SECTION	m	1.00000\n", ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    [InlineData("UNIT_DATA	SECTION	mm	1000.00\n", ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Millimeter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    // Stress
    [InlineData("UNIT_DATA	STRESS	N/m²	1.00000\n", ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    // Mass
    [InlineData("UNIT_DATA	MASS	kg	1.00000\n", ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram)]
    public void FromCoaStringTest(string coaString, ForceUnit expected_forceUnit, LengthUnit expected_lengthUnit, LengthUnit expected_displacementUnit, LengthUnit expected_sectionUnit, PressureUnit expected_stressUnit, MassUnit expected_massUnit) {
      List<string> parameters = CoaHelper.Split(coaString);

      var units = ComposUnits.GetStandardUnits();
      units.FromCoaString(parameters);

      Assert.Equal(expected_forceUnit, units.Force);
      Assert.Equal(expected_lengthUnit, units.Length);
      Assert.Equal(expected_displacementUnit, units.Displacement);
      Assert.Equal(expected_sectionUnit, units.Section);
      Assert.Equal(expected_stressUnit, units.Stress);
      Assert.Equal(expected_massUnit, units.Mass);
    }

    [Theory]
    [InlineData(ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram, "UNIT_DATA	FORCE	N	1.00000\nUNIT_DATA	LENGTH	m	1.00000\nUNIT_DATA	DISP	m	1.00000\nUNIT_DATA	SECTION	m	1.00000\nUNIT_DATA	STRESS	N/m²	1.00000\nUNIT_DATA	MASS	kg	1.00000\n")]
    [InlineData(ForceUnit.Kilonewton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram, "UNIT_DATA	FORCE	kN	0.00100000\nUNIT_DATA	LENGTH	m	1.00000\nUNIT_DATA	DISP	m	1.00000\nUNIT_DATA	SECTION	m	1.00000\nUNIT_DATA	STRESS	N/m²	1.00000\nUNIT_DATA	MASS	kg	1.00000\n")]
    [InlineData(ForceUnit.Newton, LengthUnit.Millimeter, LengthUnit.Meter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram, "UNIT_DATA	FORCE	N	1.00000\nUNIT_DATA	LENGTH	mm	1000.00\nUNIT_DATA	DISP	m	1.00000\nUNIT_DATA	SECTION	m	1.00000\nUNIT_DATA	STRESS	N/m²	1.00000\nUNIT_DATA	MASS	kg	1.00000\n")]
    [InlineData(ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Millimeter, LengthUnit.Meter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram, "UNIT_DATA	FORCE	N	1.00000\nUNIT_DATA	LENGTH	m	1.00000\nUNIT_DATA	DISP	mm	1000.00\nUNIT_DATA	SECTION	m	1.00000\nUNIT_DATA	STRESS	N/m²	1.00000\nUNIT_DATA	MASS	kg	1.00000\n")]
    [InlineData(ForceUnit.Newton, LengthUnit.Meter, LengthUnit.Meter, LengthUnit.Millimeter, PressureUnit.NewtonPerSquareMeter, MassUnit.Kilogram, "UNIT_DATA	FORCE	N	1.00000\nUNIT_DATA	LENGTH	m	1.00000\nUNIT_DATA	DISP	m	1.00000\nUNIT_DATA	SECTION	mm	1000.00\nUNIT_DATA	STRESS	N/m²	1.00000\nUNIT_DATA	MASS	kg	1.00000\n")]
    public void ToCoaStringTest(ForceUnit forceUnit, LengthUnit lengthUnit, LengthUnit displacementUnit, LengthUnit sectionUnit, PressureUnit stressUnit, MassUnit massUnit, string expected_coaString) {
      var units = new ComposUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      units.Displacement = displacementUnit;
      units.Section = sectionUnit;
      units.Stress = stressUnit;
      units.Mass = massUnit;
      string coaString = units.ToCoaString();

      Assert.Equal(expected_coaString, coaString);
    }
  }
}
