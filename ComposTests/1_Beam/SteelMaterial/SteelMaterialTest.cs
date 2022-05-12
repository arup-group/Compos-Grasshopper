using System.Collections.Generic;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public partial class SteelMaterialTest
  {
    [Theory]
    [InlineData(1000, 2000, 3000, true, WeldMaterialGrade.Grade_42, false, Code.BS5950_3_1_1990_A1_2010, "BEAM_STEEL_MATERIAL_USER	MEMBER-1	1000.00	2000.00	3000.00\nBEAM_WELDING_MATERIAL	MEMBER-1	Grade 42\n")]
    [InlineData(1, 2, 3, true, WeldMaterialGrade.Grade_35, false, Code.BS5950_3_1_1990_A1_2010, "BEAM_STEEL_MATERIAL_USER	MEMBER-1	1.00000	2.00000	3.00000\nBEAM_WELDING_MATERIAL	MEMBER-1	Grade 35\n")]
    public void ToCoaStringTest1(double fy, double E, double density, bool isCustom, WeldMaterialGrade weldGrade, bool reductionFacorMpl, Code code, string expected_coaString)
    {
      SteelMaterial steelMaterial = new SteelMaterial(new Pressure(fy, PressureUnit.NewtonPerSquareMeter), new Pressure(E, PressureUnit.NewtonPerSquareMeter), new Density(density, DensityUnit.KilogramPerCubicMeter), weldGrade, isCustom, reductionFacorMpl);
      string coaString = steelMaterial.ToCoaString("MEMBER-1", code, ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Theory]
    [InlineData(StandardSteelGrade.S235, Code.BS5950_3_1_1990_A1_2010, "BEAM_STEEL_MATERIAL_STD	MEMBER-1	S235\nBEAM_WELDING_MATERIAL	MEMBER-1	Grade 35\n")]
    [InlineData(StandardSteelGrade.S355, Code.EN1994_1_1_2004, "BEAM_STEEL_MATERIAL_STD	MEMBER-1	S355 (EN)\nBEAM_WELDING_MATERIAL	MEMBER-1	Grade 42\n")]
    public void ToCoaStringTest2(StandardSteelGrade steelMaterialGrade, Code code, string expected_coaString)
    {
      SteelMaterial steelMaterial = new SteelMaterial(steelMaterialGrade);
      string coaString = steelMaterial.ToCoaString("MEMBER-1", code, ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestConstructor()
    {
      // todo
    }
  }
}
