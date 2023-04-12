using System.Collections.Generic;
using System.IO;
using ComposAPI.Helpers;
using ComposGHTests.Helpers;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Slabs.Tests
{
  [Collection("ComposAPI Fixture collection")]
  public class DeckingTest
  {
    [Theory]
    [InlineData("RLD", "Ribdeck AL (0.9)", DeckingSteelGrade.S280, 90, true, true, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (0.9)	S280	90.0000	DECKING_JOINTED	JOINT_WELDED\n")]
    [InlineData("RLD", "Ribdeck AL (1.0)", DeckingSteelGrade.S280, 91, true, false, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (1.0)	S280	91.0000	DECKING_JOINTED	JOINT_NOT_WELD\n")]
    [InlineData("RLD", "Ribdeck AL (1.2)", DeckingSteelGrade.S350, 92, false, true, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (1.2)	S350	92.0000	DECKING_CONTINUE	JOINT_WELDED\n")]
    [InlineData("RLD", "Ribdeck E60 (0.9)", DeckingSteelGrade.S350, 93, false, false, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck E60 (0.9)	S350	93.0000	DECKING_CONTINUE	JOINT_NOT_WELD\n")]
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

    // 1 setup inputs
    [Theory]
    [InlineData("RLD", "Ribdeck AL (1.2)", DeckingSteelGrade.S280,
      300, 120, 140, 10, 40, 50, 1.2)]
    [InlineData("Kingspan", "Multideck 50 (0.85)", DeckingSteelGrade.S350,
      150, 40, 135, 0, 0, 50, 0.85)]
    public void CatalogeDeckingConstructorTest(string catalogue, string profile,
      DeckingSteelGrade deckingSteelGrade,
      double b1_expected, double b2_expected, double b3_expected, double b4_expected, double b5_expected, double depth_expected, double thickness_expected)
    {
      // 2 create object instance with constructor
      DeckingConfiguration configuration = new DeckingConfiguration();
      CatalogueDecking decking = new CatalogueDecking(catalogue, profile, deckingSteelGrade, configuration);

      // 3 check that inputs are set in object's members
      Assert.Equal(b1_expected, decking.b1.Millimeters);
      Assert.Equal(b2_expected, decking.b2.Millimeters);
      Assert.Equal(b3_expected, decking.b3.Millimeters);
      Assert.Equal(b4_expected, decking.b4.Millimeters);
      Assert.Equal(b5_expected, decking.b5.Millimeters);
      Assert.Equal(depth_expected, decking.Depth.Millimeters);
      Assert.Equal(thickness_expected, decking.Thickness.Millimeters);
      Assert.Equal(catalogue, decking.Catalogue);
      Assert.Equal(profile, decking.Profile);
      Assert.Equal(deckingSteelGrade, decking.Grade);
      Assert.Equal(configuration, decking.DeckingConfiguration);
      Assert.Equal(DeckingType.Catalogue, decking.Type);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8)]
    public CustomDecking CustomDeckingConstructorTest(double b1, double b2, double b3, double b4, double b5, double depth, double thickness, double strength)
    {
      // 2 create object instance with constructor
      DeckingConfiguration configuration = new DeckingConfiguration();
      ComposUnits units = ComposUnits.GetStandardUnits();
      CustomDecking decking = new CustomDecking(new Length(b1, units.Length), new Length(b2, units.Length), new Length(b3, units.Length), new Length(b4, units.Length), new Length(b5, units.Length), new Length(depth, units.Length), new Length(thickness, units.Length), new Pressure(strength, units.Stress), configuration);

      // 3 check that inputs are set in object's members
      Assert.Equal(b1, decking.b1.Value);
      Assert.Equal(b2, decking.b2.Value);
      Assert.Equal(b3, decking.b3.Value);
      Assert.Equal(b4, decking.b4.Value);
      Assert.Equal(b5, decking.b5.Value);
      Assert.Equal(depth, decking.Depth.Value);
      Assert.Equal(thickness, decking.Thickness.Value);
      Assert.Equal(strength, decking.Strength.Value);
      Assert.Equal(configuration, decking.DeckingConfiguration);
      Assert.Equal(DeckingType.Custom, decking.Type);

      return decking;
    }
    [Fact]
    public void DuplicateCustomDeckingTest()
    {
      // 1 create with constructor and duplicate
      CustomDecking original = CustomDeckingConstructorTest(8, 7, 6, 5, 4, 3, 2, 1);
      CustomDecking duplicate = (CustomDecking)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Theory]
    [InlineData("RLD", "Ribdeck AL (0.9)", DeckingSteelGrade.S280, 90, true, true, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (0.9)	S280	90.0000	DECKING_JOINTED	JOINT_WELDED\n")]
    [InlineData("RLD", "Ribdeck AL (1.0)", DeckingSteelGrade.S280, 91, true, false, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (1.0)	S280	91.0000	DECKING_JOINTED	JOINT_NOT_WELD\n")]
    [InlineData("RLD", "Ribdeck AL (1.2)", DeckingSteelGrade.S350, 92, false, true, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (1.2)	S350	92.0000	DECKING_CONTINUED	JOINT_WELDED\n")]
    [InlineData("RLD", "Ribdeck E60 (0.9)", DeckingSteelGrade.S350, 93, false, false, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck E60 (0.9)	S350	93.0000	DECKING_CONTINUED	JOINT_NOT_WELD\n")]
    public CatalogueDecking CatalogueDeckingFromCoaStringTest(string catalogue, string profile, DeckingSteelGrade deckingSteelGrade, double angle, bool isDiscontinous, bool isWelded, string expected_coaString)
    {
      // Assemble
      List<string> parameters = CoaHelper.Split(expected_coaString);
      ComposUnits units = ComposUnits.GetStandardUnits();

      // Act
      CatalogueDecking decking = (CatalogueDecking)CatalogueDecking.FromCoaString(parameters, units);

      // Assert
      Assert.Equal(catalogue, decking.Catalogue);
      Assert.Equal(profile, decking.Profile);
      Assert.Equal(deckingSteelGrade, decking.Grade);
      Assert.Equal(angle, decking.DeckingConfiguration.Angle.Degrees);
      Assert.Equal(isDiscontinous, decking.DeckingConfiguration.IsDiscontinous);
      Assert.Equal(isWelded, decking.DeckingConfiguration.IsWelded);
      Assert.Equal(DeckingType.Catalogue, decking.Type);

      return decking;
    }
    [Fact]
    public void DuplicateCatDeckingTest()
    {
      // 1 create with constructor and duplicate
      CatalogueDecking original = CatalogueDeckingFromCoaStringTest("RLD", "Ribdeck AL (0.9)", DeckingSteelGrade.S280, 90, true, true, "DECKING_CATALOGUE	MEMBER-1	RLD	Ribdeck AL (0.9)	S280	90.0000	DECKING_JOINTED	JOINT_WELDED\n");
      CatalogueDecking duplicate = (CatalogueDecking)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Theory]
    [InlineData(0.3, 0.12, 0.14, 0.01, 0.04, 0.05, 0.0012, 2.75E8, 90, true, true, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	0.300000	0.120000	0.140000	0.0500000	0.00120000	0.0100000	0.0400000	DECKING_JOINTED	JOINT_WELDED\n")]
    [InlineData(0.3, 0.12, 0.14, 0.01, 0.04, 0.05, 0.0012, 2.75E8, 90, true, false, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	0.300000	0.120000	0.140000	0.0500000	0.00120000	0.0100000	0.0400000	DECKING_JOINTED	JOINT_NOT_WELD\n")]
    [InlineData(0.3, 0.12, 0.14, 0.01, 0.04, 0.05, 0.0012, 2.75E8, 90, false, true, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	0.300000	0.120000	0.140000	0.0500000	0.00120000	0.0100000	0.0400000	DECKING_CONTINUED	JOINT_WELDED\n")]
    [InlineData(0.3, 0.12, 0.14, 0.01, 0.04, 0.05, 0.0012, 2.75E8, 90, false, false, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	0.300000	0.120000	0.140000	0.0500000	0.00120000	0.0100000	0.0400000	DECKING_CONTINUED	JOINT_NOT_WELD\n")]
    [InlineData(300, 120, 140, 10, 40, 50, 0.12, 2.75E8, 90, true, true, "DECKING_USER	MEMBER-1	USER_DEFINED	2.75000e+008	90.0000	300.000	120.000	140.000	50.0000	0.120000	10.0000	40.0000	DECKING_JOINTED	JOINT_WELDED\n")]
    public void CustomDeckingFromCoaStringTest(double b1_exp, double b2_exp, double b3_exp, double b4_exp, double b5_exp, double depth_exp, double thickness_exp, double stress_exp, double angle_exp, bool isDiscontinous_exp, bool isWelded_exp, string coaString)
    {
      // Assemble
      List<string> parameters = CoaHelper.Split(coaString);
      ComposUnits units = ComposUnits.GetStandardUnits();

      // Act
      CustomDecking decking = (CustomDecking)CustomDecking.FromCoaString(parameters, units);

      // Assert
      Assert.Equal(b1_exp, decking.b1.Value);
      Assert.Equal(b2_exp, decking.b2.Value);
      Assert.Equal(b3_exp, decking.b3.Value);
      Assert.Equal(b4_exp, decking.b4.Value);
      Assert.Equal(b5_exp, decking.b5.Value);
      Assert.Equal(depth_exp, decking.Depth.Value);
      Assert.Equal(thickness_exp, decking.Thickness.Value);
      Assert.Equal(stress_exp, decking.Strength.Value);
      Assert.Equal(angle_exp, decking.DeckingConfiguration.Angle.Degrees);
      Assert.Equal(isDiscontinous_exp, decking.DeckingConfiguration.IsDiscontinous);
      Assert.Equal(isWelded_exp, decking.DeckingConfiguration.IsWelded);
      Assert.Equal(DeckingType.Custom, decking.Type);
    }
  }
}
