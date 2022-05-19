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

    // 1 setup inputs
    [Theory]
    [InlineData(500, 1000, 2500, WeldMaterialGrade.None, false, false)]
    [InlineData(500, 1000, 2500, WeldMaterialGrade.Grade_35, true, false)]
    [InlineData(500, 1000, 2500, WeldMaterialGrade.Grade_42, false, true)]
    [InlineData(500, 1000, 2500, WeldMaterialGrade.Grade_50, true, true)]
    public void ConstructorTest1(double fy, double E, double density, WeldMaterialGrade weldGrade, bool isCustom, bool reductionFacorMpl)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      SteelMaterial material = new SteelMaterial(new Pressure(fy, units.Stress), new Pressure(E, units.Stress), new Density(density, units.Density), weldGrade, isCustom, reductionFacorMpl);

      // 3 check that inputs are set in object's members
      Assert.Equal(fy, material.fy.Value);
      Assert.Equal(E, material.E.Value);
      Assert.Equal(density, material.Density.Value);
      Assert.Equal(weldGrade, material.WeldGrade);
      Assert.Equal(isCustom, material.isCustom);
      Assert.Equal(reductionFacorMpl, material.ReductionFactorMpl);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(StandardSteelGrade.S235)]
    public void ConstructorTest2(StandardSteelGrade grade)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      SteelMaterial material = new SteelMaterial(grade);

      // 3 check that inputs are set in object's members
      Assert.Equal(205, material.E.Value);
      Assert.Equal(7850, material.Density.Value);
      switch (grade)
      {
        case StandardSteelGrade.S235:
          Assert.Equal(235, material.fy.Value);
          Assert.Equal(WeldMaterialGrade.Grade_35, material.WeldGrade);
          Assert.False(material.isCustom);
          break;

        case StandardSteelGrade.S275:
          Assert.Equal(275, material.fy.Value);
          Assert.Equal(WeldMaterialGrade.Grade_35, material.WeldGrade);
          Assert.False(material.isCustom);
          break;

        case StandardSteelGrade.S355:
          Assert.Equal(355, material.fy.Value);
          Assert.Equal(WeldMaterialGrade.Grade_42, material.WeldGrade);
          Assert.False(material.isCustom);
          break;

        case StandardSteelGrade.S450:
          Assert.Equal(450, material.fy.Value);
          Assert.Equal(WeldMaterialGrade.Grade_50, material.WeldGrade);
          Assert.False(material.isCustom);
          break;

        case StandardSteelGrade.S460:
          Assert.Equal(460, material.fy.Value);
          Assert.Equal(WeldMaterialGrade.Grade_50, material.WeldGrade);
          Assert.False(material.isCustom);
          break;

        default:
          Assert.True(material.isCustom);
          break;
      }
    }
  }
}
