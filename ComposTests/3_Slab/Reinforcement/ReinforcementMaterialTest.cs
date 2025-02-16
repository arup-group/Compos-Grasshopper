﻿using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helper;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Slabs.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class ReinforcementMaterialTest {

    // 1 setup inputs
    [Theory]
    [InlineData(500)]
    public ReinforcementMaterial ConstructorTest1(double fy) {
      var units = ComposUnits.GetStandardUnits();

      // 2 create object instance with constructor
      var material = new ReinforcementMaterial(new Pressure(fy, units.Stress));

      // 3 check that inputs are set in object's members
      Assert.Equal(fy, material.Fy.Value);
      Assert.True(material.UserDefined);

      return material;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(RebarGrade.AS_D500E)]
    [InlineData(RebarGrade.AS_D500L)]
    [InlineData(RebarGrade.AS_D500N)]
    [InlineData(RebarGrade.AS_R250N)]
    [InlineData(RebarGrade.BS_1770)]
    [InlineData(RebarGrade.BS_250R)]
    [InlineData(RebarGrade.BS_460T)]
    [InlineData(RebarGrade.BS_500X)]
    [InlineData(RebarGrade.EN_500A)]
    [InlineData(RebarGrade.EN_500B)]
    [InlineData(RebarGrade.EN_500C)]
    [InlineData(RebarGrade.HK_250)]
    [InlineData(RebarGrade.HK_460)]
    public void ConstructorTest2(RebarGrade grade) {
      // 2 create object instance with constructor
      var material = new ReinforcementMaterial(grade);

      // 3 check that inputs are set in object's members
      Assert.Equal(grade, material.Grade);
      switch (grade) {
        case RebarGrade.BS_250R:
        case RebarGrade.HK_250:
        case RebarGrade.AS_R250N:
          Assert.Equal(250, material.Fy.Value);
          break;

        case RebarGrade.BS_460T:
        case RebarGrade.HK_460:
          Assert.Equal(460, material.Fy.Value);
          break;

        case RebarGrade.BS_500X:
        case RebarGrade.AS_D500L:
        case RebarGrade.AS_D500N:
        case RebarGrade.AS_D500E:
        case RebarGrade.EN_500A:
        case RebarGrade.EN_500B:
        case RebarGrade.EN_500C:
          Assert.Equal(500, material.Fy.Value);
          break;

        case RebarGrade.BS_1770:
          Assert.Equal(1770, material.Fy.Value);
          break;
      }
      Assert.False(material.UserDefined);
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      ReinforcementMaterial original = ConstructorTest1(30);
      var duplicate = (ReinforcementMaterial)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Theory]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	D500E\n", Code.AS_NZS2327_2017, RebarGrade.AS_D500E, false, 5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	D500L\n", Code.AS_NZS2327_2017, RebarGrade.AS_D500L, false, 5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	D500N\n", Code.AS_NZS2327_2017, RebarGrade.AS_D500N, false, 5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	R250N\n", Code.AS_NZS2327_2017, RebarGrade.AS_R250N, false, 2.5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	1770\n", Code.BS5950_3_1_1990_A1_2010, RebarGrade.BS_1770, false, 17.7E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	250R\n", Code.BS5950_3_1_1990_A1_2010, RebarGrade.BS_250R, false, 2.5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	460T\n", Code.BS5950_3_1_1990_A1_2010, RebarGrade.BS_460T, false, 4.6E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	500X\n", Code.BS5950_3_1_1990_A1_2010, RebarGrade.BS_500X, false, 5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	500A\n", Code.EN1994_1_1_2004, RebarGrade.EN_500A, false, 5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	500B\n", Code.EN1994_1_1_2004, RebarGrade.EN_500B, false, 5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	500C\n", Code.EN1994_1_1_2004, RebarGrade.EN_500C, false, 5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	250\n", Code.HKSUOS_2011, RebarGrade.HK_250, false, 2.5E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	STANDARD	460\n", Code.HKSUOS_2011, RebarGrade.HK_460, false, 4.6E+8)]
    [InlineData("REBAR_MATERIAL	MEMBER-1	USER_DEFINED	4.00000e+008\n", Code.AS_NZS2327_2017, RebarGrade.AS_D500E, true, 4E+8)]
    public void FromCoaStringTest(string coaString, Code code, RebarGrade expected_grade, bool expected_userDefined, double expected_fy) {
      List<string> parameters = CoaHelper.Split(coaString);

      IReinforcementMaterial reinforcementMaterial = ReinforcementMaterial.FromCoaString(parameters, code);

      if (!expected_userDefined) {
        Assert.Equal(expected_grade, reinforcementMaterial.Grade);
      }
      Assert.Equal(expected_userDefined, reinforcementMaterial.UserDefined);
      Assert.True(ComposUnitsHelper.IsEqual(new Pressure(expected_fy, PressureUnit.NewtonPerSquareMeter), reinforcementMaterial.Fy));
    }

    [Theory]
    [InlineData(RebarGrade.AS_D500E, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	D500E\n")]
    [InlineData(RebarGrade.AS_D500L, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	D500L\n")]
    [InlineData(RebarGrade.AS_D500N, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	D500N\n")]
    [InlineData(RebarGrade.AS_R250N, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	R250N\n")]
    [InlineData(RebarGrade.BS_1770, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	1770\n")]
    [InlineData(RebarGrade.BS_250R, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	250R\n")]
    [InlineData(RebarGrade.BS_460T, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	460T\n")]
    [InlineData(RebarGrade.BS_500X, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	500X\n")]
    [InlineData(RebarGrade.EN_500A, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	500A\n")]
    [InlineData(RebarGrade.EN_500B, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	500B\n")]
    [InlineData(RebarGrade.EN_500C, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	500C\n")]
    [InlineData(RebarGrade.HK_250, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	250\n")]
    [InlineData(RebarGrade.HK_460, false, 0, "REBAR_MATERIAL	MEMBER-1	STANDARD	460\n")]
    [InlineData(RebarGrade.AS_D500E, true, 4.00000e+008, "REBAR_MATERIAL	MEMBER-1	USER_DEFINED	4.00000e+008\n")]
    public void ToCoaStringTest(RebarGrade grade, bool userDefined, double fy, string expected_coaString) {
      ReinforcementMaterial reinforcementMaterial;
      if (!userDefined) {
        reinforcementMaterial = new ReinforcementMaterial(grade);
      } else {
        reinforcementMaterial = new ReinforcementMaterial(new Pressure(fy, PressureUnit.NewtonPerSquareMeter));
      }

      string coaString = reinforcementMaterial.ToCoaString("MEMBER-1");

      Assert.Equal(expected_coaString, coaString);
    }
  }
}
