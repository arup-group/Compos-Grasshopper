﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public class ReinforcementMaterialTest
  {
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
    public void ToCoaStringTest(RebarGrade grade, bool userDefined, double fy, string expected_coaString)
    {
      ReinforcementMaterial reinforcementMaterial;
      if (!userDefined)
        reinforcementMaterial = new ReinforcementMaterial(grade);
      else
        reinforcementMaterial = new ReinforcementMaterial(new Pressure(fy, PressureUnit.NewtonPerSquareMeter));

      string coaString = reinforcementMaterial.ToCoaString("MEMBER-1");

      Assert.Equal(expected_coaString, coaString);
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
    [InlineData("REBAR_MATERIAL	MEMBER-1	USER_DEFINED	4.00000e+008\n", null, RebarGrade.AS_D500E, true, 4E+8)]
    public void CoaConstructorTest(string coaString, Code code, RebarGrade expected_grade, bool expected_userDefined, double expected_fy)
    {
      List<string> parameters = CoaHelper.Split(coaString);

      ReinforcementMaterial reinforcementMaterial = new ReinforcementMaterial(parameters, code);

      if (!expected_userDefined)
        Assert.Equal(expected_grade, reinforcementMaterial.Grade);
      Assert.Equal(expected_userDefined, reinforcementMaterial.UserDefined);
      Assert.Equal(new Pressure(expected_fy, PressureUnit.NewtonPerSquareMeter), reinforcementMaterial.Fy);
    }
  }
}