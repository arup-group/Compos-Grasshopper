﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public class DeckingTest
  {
    [Theory]
    [InlineData("RLD", "Ribdeck AL (0.9)", DeckingSteelGrade.S280, 90, true, true, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (0.9)	S280	90.0000	DECKING_JOINTED	JOINT_WELDED\n")]
    [InlineData("RLD", "Ribdeck AL (1.0)", DeckingSteelGrade.S280, 91, true, false, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (1.0)	S280	91.0000	DECKING_JOINTED	JOINT_NOT_WELD\n")]
    [InlineData("RLD", "Ribdeck AL (1.2)", DeckingSteelGrade.S350, 92, false, true, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (1.2)	S350	92.0000	DECKING_CONTINUED	JOINT_WELDED\n")]
    [InlineData("RLD", "Ribdeck E60 (0.9)", DeckingSteelGrade.S350, 93, false, false, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck E60 (0.9)	S350	93.0000	DECKING_CONTINUED	JOINT_NOT_WELD\n")]
    public void CatalogueDeckingToCoaStringTest(string catalogue, string profile, DeckingSteelGrade deckingSteelGrade, double angle, bool isDiscontinous, bool isWelded, string expected_coaString)
    {
      Decking decking = new CatalogueDecking(catalogue, profile, deckingSteelGrade, new DeckingConfiguration(new Angle(angle, AngleUnit.Degree), isDiscontinous, isWelded));
      string coaString = decking.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    [Theory]
    [InlineData(0.3, 0.12, 0.14, 0.01, 0.04, 0.05, 0.0012, 2.75E8, 90, true, true, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	0.300000	0.120000	0.140000	0.0500000	0.00120000	0.0100000	0.0400000	DECKING_JOINTED	JOINT_WELDED\n")]
    [InlineData(0.3, 0.12, 0.14, 0.01, 0.04, 0.05, 0.0012, 2.75E8, 90, true, false, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	0.300000	0.120000	0.140000	0.0500000	0.00120000	0.0100000	0.0400000	DECKING_JOINTED	JOINT_NOT_WELD\n")]
    [InlineData(0.3, 0.12, 0.14, 0.01, 0.04, 0.05, 0.0012, 2.75E8, 90, false, true, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	0.300000	0.120000	0.140000	0.0500000	0.00120000	0.0100000	0.0400000	DECKING_CONTINUED	JOINT_WELDED\n")]
    [InlineData(0.3, 0.12, 0.14, 0.01, 0.04, 0.05, 0.0012, 2.75E8, 90, false, false, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	0.300000	0.120000	0.140000	0.0500000	0.00120000	0.0100000	0.0400000	DECKING_CONTINUED	JOINT_NOT_WELD\n")]
    [InlineData(300, 120, 140, 10, 40, 50, 0.12, 2.75E8, 90, true, true, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	300.000	120.000	140.000	50.0000	0.120000	10.0000	40.0000	DECKING_JOINTED	JOINT_WELDED\n")]
    public void CustomDeckingToCoaStringTest(double b1, double b2, double b3, double b4, double b5, double depth, double thickness, double stress, double angle, bool isDiscontinous, bool isWelded, string expected_coaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();
      Decking decking = new CustomDecking(new Length(b1, units.Section), new Length(b2, units.Section), new Length(b3, units.Section), new Length(b4, units.Section), new Length(b5, units.Section), new Length(depth, units.Section), new Length(thickness, units.Section), new Pressure(stress, units.Stress), new DeckingConfiguration(new Angle(angle, AngleUnit.Degree), isDiscontinous, isWelded));
      string coaString = decking.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());

      Assert.Equal(expected_coaString, coaString);
    }

    public void TestConstructor()
    {
      // todo
    }
  }
}