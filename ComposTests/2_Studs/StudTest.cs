using System.Collections.Generic;
using ComposAPI.Tests;
using UnitsNet;
using UnitsNet.Units;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH;

namespace ComposAPI.Studs.Tests
{
  [Collection("ComposAPI Fixture collection")]
  public partial class StudTest
  {
    [Fact]
    public Stud TestConstructorStudCustomSpacing()
    {
      // 1 setup inputs
      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D13mmH65mm, StandardStudGrade.SD1_EN13918);
      IStudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);
      List<IStudGroupSpacing> studSpacings = new List<IStudGroupSpacing>();
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 2, 1, new Length(25, LengthUnit.Centimeter)));
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 1, 2, new Length(35, LengthUnit.Centimeter)));

      // 2 create object instance with constructor
      Stud stud = new Stud(dimensions, specification, studSpacings, true);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.Dimensions);
      Assert.Equal(13, stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(65, stud.Dimensions.Height.Millimeters);
      Assert.Equal(400, stud.Dimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.Dimensions.CharacterStrength);
      // specification
      Assert.NotNull(stud.Specification);
      Assert.Equal(Length.Zero, stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, stud.Specification.NoStudZoneEnd);
      Assert.True(stud.Specification.Welding);
      // spacings
      Assert.NotNull(stud.CustomSpacing);
      Assert.Equal(2, stud.CustomSpacing.Count);
      Assert.NotNull(stud.CustomSpacing[0]);
      Assert.Equal(Length.Zero, stud.CustomSpacing[0].DistanceFromStart);
      Assert.Equal(2, stud.CustomSpacing[0].NumberOfRows);
      Assert.Equal(1, stud.CustomSpacing[0].NumberOfLines);
      Assert.Equal(25, stud.CustomSpacing[0].Spacing.Centimeters);
      Assert.NotNull(stud.CustomSpacing[1]);
      Assert.Equal(Length.Zero, stud.CustomSpacing[1].DistanceFromStart);
      Assert.Equal(1, stud.CustomSpacing[1].NumberOfRows);
      Assert.Equal(2, stud.CustomSpacing[1].NumberOfLines);
      Assert.Equal(35, stud.CustomSpacing[1].Spacing.Centimeters);
      //other
      Assert.Equal(StudSpacingType.Custom, stud.StudSpacingType);
      Assert.True(stud.CheckStudSpacing);
      Assert.Equal(double.NaN, stud.Interaction);
      Assert.Equal(double.NaN, stud.MinSavingMultipleZones);

      return stud;
    }
    [Fact]
    public void DuplicateStudCustomSpacingTest()
    {
      // 1 create with constructor and duplicate
      Stud original = TestConstructorStudCustomSpacing();
      Stud duplicate = (Stud)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(StudSpacingType.Min_Num_of_Studs, 0.2)]
    [InlineData(StudSpacingType.Automatic, 0.3)]
    public Stud TestConstructorStudAutomaticOrMinSpacing(StudSpacingType type, double minSaving)
    {
      // 1b setup inputs
      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D13mmH65mm, StandardStudGrade.SD1_EN13918);
      IStudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);

      // 2 create object instance with constructor
      Stud stud = new Stud(dimensions, specification, minSaving, type);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.Dimensions);
      Assert.Equal(13, stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(65, stud.Dimensions.Height.Millimeters);
      Assert.Equal(400, stud.Dimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.Dimensions.CharacterStrength);
      // specification
      Assert.NotNull(stud.Specification);
      Assert.Equal(Length.Zero, stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, stud.Specification.NoStudZoneEnd);
      Assert.True(stud.Specification.Welding);
      // spacings
      Assert.Null(stud.CustomSpacing);
      //other
      Assert.Equal(minSaving, stud.MinSavingMultipleZones);
      Assert.Equal(type, stud.StudSpacingType);
      Assert.Equal(double.NaN, stud.Interaction);

      return stud;
    }

    [Theory]
    [InlineData(StudSpacingType.Custom)]
    [InlineData(StudSpacingType.Partial_Interaction)]
    public void TestConstructorStudAutomaticOrMinSpacingExceptions(StudSpacingType type)
    {
      // check that exceptions are thrown if inputs does not comply with allowed
      Assert.Throws<System.ArgumentException>(() => TestConstructorStudAutomaticOrMinSpacing(type, 0.2));
    }

    // 1 setup inputs
    [Theory]
    [InlineData(0.2, 0.95)]
    [InlineData(0.3, 0.85)]
    public Stud TestConstructorStudPartialSpacing(double minSaving, double interaction)
    {
      // 1b setup inputs
      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D13mmH65mm, StandardStudGrade.SD1_EN13918);
      IStudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);

      // 2 create object instance with constructor
      Stud stud = new Stud(dimensions, specification, minSaving, interaction);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.Dimensions);
      Assert.Equal(13, stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(65, stud.Dimensions.Height.Millimeters);
      Assert.Equal(400, stud.Dimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.Dimensions.CharacterStrength);
      // specification
      Assert.NotNull(stud.Specification);
      Assert.Equal(Length.Zero, stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, stud.Specification.NoStudZoneEnd);
      Assert.True(stud.Specification.Welding);
      // spacings
      Assert.Null(stud.CustomSpacing);
      //other
      Assert.Equal(minSaving, stud.MinSavingMultipleZones);
      Assert.Equal(interaction, stud.Interaction);
      Assert.Equal(StudSpacingType.Partial_Interaction, stud.StudSpacingType);

      return stud;
    }

    [Fact]
    public void TestStudDuplicate()
    {
      // 1 create with constructor and duplicate
      Stud original = TestConstructorStudCustomSpacing();
      Stud duplicate = (Stud)original.Duplicate();

      // 2 check that duplicate has duplicated values
      // dimensions
      Assert.NotNull(duplicate.Dimensions);
      Assert.Equal(13, duplicate.Dimensions.Diameter.Millimeters);
      Assert.Equal(65, duplicate.Dimensions.Height.Millimeters);
      Assert.Equal(400, duplicate.Dimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, duplicate.Dimensions.CharacterStrength);
      // specification
      Assert.NotNull(duplicate.Specification);
      Assert.Equal(Length.Zero, duplicate.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, duplicate.Specification.NoStudZoneEnd);
      Assert.True(duplicate.Specification.Welding);
      // spacings
      Assert.NotNull(duplicate.CustomSpacing);
      Assert.Equal(2, duplicate.CustomSpacing.Count);
      Assert.NotNull(duplicate.CustomSpacing[0]);
      Assert.Equal(Length.Zero, duplicate.CustomSpacing[0].DistanceFromStart);
      Assert.Equal(2, duplicate.CustomSpacing[0].NumberOfRows);
      Assert.Equal(1, duplicate.CustomSpacing[0].NumberOfLines);
      Assert.Equal(25, duplicate.CustomSpacing[0].Spacing.Centimeters);
      Assert.NotNull(duplicate.CustomSpacing[1]);
      Assert.Equal(Length.Zero, duplicate.CustomSpacing[1].DistanceFromStart);
      Assert.Equal(1, duplicate.CustomSpacing[1].NumberOfRows);
      Assert.Equal(2, duplicate.CustomSpacing[1].NumberOfLines);
      Assert.Equal(35, duplicate.CustomSpacing[1].Spacing.Centimeters);
      //other
      Assert.Equal(StudSpacingType.Custom, duplicate.StudSpacingType);
      Assert.True(duplicate.CheckStudSpacing);
      Assert.Equal(double.NaN, duplicate.Interaction);
      Assert.Equal(double.NaN, duplicate.MinSavingMultipleZones);

      Duplicates.AreEqual(original, duplicate);

      // 3 make some changes to duplicate
      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D25mmH100mm, StandardStudGrade.SD3_EN13918);
      IStudSpecification specification = new StudSpecification(new Length(25, LengthUnit.Centimeter), new Length(35, LengthUnit.Centimeter), false);
      List<IStudGroupSpacing> studSpacings = new List<IStudGroupSpacing>();
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 3, 2, new Length(10, LengthUnit.Centimeter)));

      duplicate.Dimensions = dimensions;
      duplicate.Specification = specification;
      duplicate.CustomSpacing = studSpacings;
      duplicate.CheckStudSpacing = false;

      // 4 check that duplicate has set changes
      // dimensions
      Assert.NotNull(duplicate.Dimensions);
      Assert.Equal(25, duplicate.Dimensions.Diameter.Millimeters);
      Assert.Equal(100, duplicate.Dimensions.Height.Millimeters);
      Assert.Equal(500, duplicate.Dimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, duplicate.Dimensions.CharacterStrength);
      // specification
      Assert.NotNull(duplicate.Specification);
      Assert.Equal(25, duplicate.Specification.NoStudZoneStart.As(LengthUnit.Centimeter));
      Assert.Equal(35, duplicate.Specification.NoStudZoneEnd.As(LengthUnit.Centimeter));
      Assert.False(duplicate.Specification.Welding);
      // spacings
      Assert.NotNull(duplicate.CustomSpacing);
      Assert.Single(duplicate.CustomSpacing);
      Assert.NotNull(duplicate.CustomSpacing[0]);
      Assert.Equal(Length.Zero, duplicate.CustomSpacing[0].DistanceFromStart);
      Assert.Equal(3, duplicate.CustomSpacing[0].NumberOfRows);
      Assert.Equal(2, duplicate.CustomSpacing[0].NumberOfLines);
      Assert.Equal(10, duplicate.CustomSpacing[0].Spacing.Centimeters);
      //other
      Assert.Equal(StudSpacingType.Custom, duplicate.StudSpacingType);
      Assert.False(duplicate.CheckStudSpacing);
      Assert.Equal(double.NaN, duplicate.Interaction);
      Assert.Equal(double.NaN, duplicate.MinSavingMultipleZones);

      // 5 check that original has not been changed
      // dimensions
      Assert.NotNull(original.Dimensions);
      Assert.Equal(13, original.Dimensions.Diameter.Millimeters);
      Assert.Equal(65, original.Dimensions.Height.Millimeters);
      Assert.Equal(400, original.Dimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, original.Dimensions.CharacterStrength);
      // specification
      Assert.NotNull(original.Specification);
      Assert.Equal(Length.Zero, original.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, original.Specification.NoStudZoneEnd);
      Assert.True(original.Specification.Welding);
      // spacings
      Assert.NotNull(original.CustomSpacing);
      Assert.Equal(2, original.CustomSpacing.Count);
      Assert.NotNull(original.CustomSpacing[0]);
      Assert.Equal(Length.Zero, original.CustomSpacing[0].DistanceFromStart);
      Assert.Equal(2, original.CustomSpacing[0].NumberOfRows);
      Assert.Equal(1, original.CustomSpacing[0].NumberOfLines);
      Assert.Equal(25, original.CustomSpacing[0].Spacing.Centimeters);
      Assert.NotNull(original.CustomSpacing[1]);
      Assert.Equal(Length.Zero, original.CustomSpacing[1].DistanceFromStart);
      Assert.Equal(1, original.CustomSpacing[1].NumberOfRows);
      Assert.Equal(2, original.CustomSpacing[1].NumberOfLines);
      Assert.Equal(35, original.CustomSpacing[1].Spacing.Centimeters);
      //other
      Assert.Equal(StudSpacingType.Custom, original.StudSpacingType);
      Assert.True(original.CheckStudSpacing);
      Assert.Equal(double.NaN, original.Interaction);
      Assert.Equal(double.NaN, original.MinSavingMultipleZones);
    }

    [Fact]
    public void TestStudDuplicate2()
    {
      // 1 create with constructor and duplicate
      Stud original = TestConstructorStudAutomaticOrMinSpacing(StudSpacingType.Automatic, 0.2);
      Stud duplicate = (Stud)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(StudSpacingType.Automatic, duplicate.StudSpacingType);
      Assert.Null(duplicate.CustomSpacing);
      Assert.Equal(0.2, duplicate.MinSavingMultipleZones);
      Assert.Equal(double.NaN, duplicate.Interaction);

      // 3 make some changes to duplicate
      duplicate.MinSavingMultipleZones = 0.3;

      // 4 check that duplicate has set changes
      Assert.Equal(0.3, duplicate.MinSavingMultipleZones);

      // 5 check that original has not been changed
      Assert.Equal(0.2, original.MinSavingMultipleZones);

      // 1 create with new constructor and duplicate
      original = TestConstructorStudPartialSpacing(0.15, 0.90);
      duplicate = (Stud)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(StudSpacingType.Partial_Interaction, duplicate.StudSpacingType);
      Assert.Null(duplicate.CustomSpacing);
      Assert.Equal(0.15, duplicate.MinSavingMultipleZones);
      Assert.Equal(0.90, duplicate.Interaction);

      // 3 make some changes to duplicate
      duplicate.MinSavingMultipleZones = 0.25;
      duplicate.Interaction = 0.97;

      // 4 check that duplicate has set changes
      Assert.Equal(0.25, duplicate.MinSavingMultipleZones);
      Assert.Equal(0.97, duplicate.Interaction);

      // 5 check that original has not been changed
      Assert.Equal(0.15, original.MinSavingMultipleZones);
      Assert.Equal(0.90, original.Interaction);
    }

    [Fact]
    public void TestBS5950ssToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	STANDARD	19mm/100mm	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	AUTO_100	0.200000" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	YES" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D19mmH100mm);
      IStudSpecification specs = new StudSpecification(true, Length.Zero, Length.Zero);
      Stud stud = new Stud(dimensions, specs, 0.2, StudSpacingType.Automatic);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits(), Code.BS5950_3_1_1990_Superseded);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestBS5950ToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	USER_DEFINED	0.0210000	0.105000	95000.0	REDUCED_NO	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	AUTO_PERCENT	0.200000	0.850000" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	1.00000	5.00000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	NO" + '\n';

      IStudDimensions dimensions = new StudDimensions(new Length(21, LengthUnit.Millimeter), new Length(105, LengthUnit.Millimeter), new Force(95000, ForceUnit.Newton));
      IStudSpecification specs = new StudSpecification(false, new Length(1, LengthUnit.Meter), new Length(5, LengthUnit.Meter));
      Stud stud = new Stud(dimensions, specs, 0.2, 0.85);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits(), Code.BS5950_3_1_1990_A1_2010);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestEC4StdGrdToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	STANDARD	19mm/100mm	WELDED_NO" + '\n' +
        "STUD_LAYOUT	MEMBER-1	AUTO_100	0.200000" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	YES" + '\n' +
        "STUD_NCCI_LIMIT_APPLY	MEMBER-1	NO" + '\n' +
        "STUD_EC4_RFT_POS	MEMBER-1	0.0300000" + '\n' +
        "EC4_STUD_GRADE	MEMBER-1	CODE_GRADE_YES	SD2_EN13918" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D19mmH100mm, StandardStudGrade.SD2_EN13918);
      IStudSpecification specs = new StudSpecification(new Length(0, LengthUnit.Meter), new Length(0, LengthUnit.Meter), new Length(30, LengthUnit.Millimeter), false, false);
      Stud stud = new Stud(dimensions, specs, 0.2, StudSpacingType.Automatic);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits(), Code.EN1994_1_1_2004);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestEC4CustomGrdToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	STANDARD	25mm/100mm	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	AUTO_MINIMUM_STUD	0.200000" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	YES" + '\n' +
        "STUD_NCCI_LIMIT_APPLY	MEMBER-1	YES" + '\n' +
        "STUD_EC4_RFT_POS	MEMBER-1	0.0300000" + '\n' +
        "EC4_STUD_GRADE	MEMBER-1	CODE_GRADE_NO	4.40000e+008" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D25mmH100mm, new Pressure(440, PressureUnit.Megapascal));
      IStudSpecification specs = new StudSpecification(new Length(0, LengthUnit.Meter), new Length(0, LengthUnit.Meter), new Length(30, LengthUnit.Millimeter), true, true);
      Stud stud = new Stud(dimensions, specs, 0.2, StudSpacingType.Min_Num_of_Studs);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits(), Code.EN1994_1_1_2004);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestHK05CustomSpacingToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	STANDARD	13mm/65mm	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	1	0.000000	2	1	0.0760000	0.0950000	0.150000	CHECK_SPACE_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	2	4.50000	3	2	0.0760000	0.0950000	0.250000	CHECK_SPACE_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	3	9.00000	4	3	0.0760000	0.0950000	0.350000	CHECK_SPACE_YES" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	YES" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D13mmH65mm);
      IStudSpecification specs = new StudSpecification(new Length(0, LengthUnit.Meter), new Length(0, LengthUnit.Meter), true);
      IStudGroupSpacing spacing1 = new StudGroupSpacing(Length.Zero, 2, 1, new Length(150, LengthUnit.Millimeter));
      IStudGroupSpacing spacing2 = new StudGroupSpacing(new Length(4.5, LengthUnit.Meter), 3, 2, new Length(250, LengthUnit.Millimeter));
      IStudGroupSpacing spacing3 = new StudGroupSpacing(new Length(9, LengthUnit.Meter), 4, 3, new Length(350, LengthUnit.Millimeter));

      List<IStudGroupSpacing> spacing = new List<IStudGroupSpacing>() { spacing1, spacing2, spacing3 };
      Stud stud = new Stud(dimensions, specs, spacing, true);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits(), Code.HKSUOS_2005);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestHK11ToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	STANDARD	19mm/95mm	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	AUTO_100	0.200000" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	YES" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D19mmH95mm);
      IStudSpecification specs = new StudSpecification(Length.Zero, Length.Zero, true);
      Stud stud = new Stud(dimensions, specs, 0.2, StudSpacingType.Automatic);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits(), Code.HKSUOS_2011);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestASNZCustomSpacingToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	STANDARD	19mm/100mm	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	2	1	0.000000	2	1	0.0570000	0.0950000	0.150000	CHECK_SPACE_NO" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	2	2	8.00000	3	2	0.0570000	0.0950000	0.250000	CHECK_SPACE_NO" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	YES" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardStudSize.D19mmH100mm);
      IStudSpecification specs = new StudSpecification(new Length(0, LengthUnit.Meter), new Length(0, LengthUnit.Meter), true);
      IStudGroupSpacing spacing1 = new StudGroupSpacing(Length.Zero, 2, 1, new Length(150, LengthUnit.Millimeter));
      IStudGroupSpacing spacing2 = new StudGroupSpacing(new Length(8, LengthUnit.Meter), 3, 2, new Length(250, LengthUnit.Millimeter));

      List<IStudGroupSpacing> spacing = new List<IStudGroupSpacing>() { spacing1, spacing2 };
      Stud stud = new Stud(dimensions, specs, spacing, false);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits(), Code.AS_NZS2327_2017);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestFileCoaStringForStudParts()
    {
      // Arrange 
      string coaString =
"UNIT_DATA\tFORCE\tN\t1.00000" + '\n' +
"UNIT_DATA\tLENGTH\tm\t1.00000" + '\n' +
"UNIT_DATA\tDISP\tm\t1.00000" + '\n' +
"UNIT_DATA\tSECTION\tmm\t1000.00" + '\n' +
"UNIT_DATA\tSTRESS\tN/m²\t1.00000" + '\n' +
"UNIT_DATA\tMASS\tkg\t1.00000" + '\n' +
"MEMBER_TITLE\tMEMBER-1\t\tB/tf=15\t\tChange in direction > 11 degrees" + '\n' +
"DESIGN_OPTION\tMEMBER-1\tBS5950-3.1:1990 (superseded)\tUNPROPPED\tBEAM_WEIGHT_NO\tSLAB_WEIGHT_NO\tSHEAR_DEFORM_NO\tTHIN_SECTION_NO\t2.00000\t2.00000" + '\n' +
"STUD_DEFINITION\tMEMBER-1\tSTANDARD\t19mm/100mm\tWELDED_YES" + '\n' +
"STUD_LAYOUT\tMEMBER-1\tAUTO_100\t0.200000" + '\n' +
"STUD_NO_STUD_ZONE\tMEMBER-1\t0.000000\t0.000000" + '\n' +
"STUD_EC4_APPLY\tMEMBER-1\tYES" + '\n' +

"MEMBER_TITLE\tMEMBER-2\t\tB/tf=17.5\t\tChange in direction < 10 degrees" + '\n' +
"DESIGN_OPTION\tMEMBER-2\tBS5950-3.1:1990+A1:2010\tUNPROPPED\tBEAM_WEIGHT_NO\tSLAB_WEIGHT_NO\tSHEAR_DEFORM_NO\tTHIN_SECTION_NO\t2.00000\t2.00000" + '\n' +
"STUD_DEFINITION\tMEMBER-2\tUSER_DEFINED\t21.0000\t131.000\t99000.0\tREDUCED_YES\tWELDED_NO" + '\n' +
"STUD_LAYOUT\tMEMBER-2\tAUTO_PERCENT\t0.200000\t0.850000" + '\n' +
"STUD_NO_STUD_ZONE\tMEMBER-2\t0.000000\t0.000000" + '\n' +
"STUD_EC4_APPLY\tMEMBER-2\tNO" + '\n' +

"MEMBER_TITLE\tMEMBER-3\t\tB/tf=17.5\t\tChange in direction > 10 degrees" + '\n' +
"DESIGN_OPTION\tMEMBER-3\tEN1994-1-1:2004\tUNPROPPED\tBEAM_WEIGHT_NO\tSLAB_WEIGHT_NO\tSHEAR_DEFORM_NO\tTHIN_SECTION_NO\t2.00000\t2.00000" + '\n' +
"STUD_DEFINITION\tMEMBER-3\tSTANDARD\t19mm/100mm\tWELDED_YES" + '\n' +
"STUD_LAYOUT\tMEMBER-3\tAUTO_100\t0.200000" + '\n' +
"STUD_NO_STUD_ZONE\tMEMBER-3\t0.000000\t0.000000" + '\n' +
"STUD_EC4_APPLY\tMEMBER-3\tYES" + '\n' +
"STUD_NCCI_LIMIT_APPLY\tMEMBER-3\tYES" + '\n' +
"STUD_EC4_RFT_POS\tMEMBER-3\t0.0300000" + '\n' +
"EC4_STUD_GRADE\tMEMBER-3\tCODE_GRADE_YES\tSD2_EN13918" + '\n' +

"MEMBER_TITLE\tMEMBER-4\t\tB/tf=20\t\tChange in direction < 9 degrees" + '\n' +
"DESIGN_OPTION\tMEMBER-4\tEN1994-1-1:2004\tUNPROPPED\tBEAM_WEIGHT_NO\tSLAB_WEIGHT_NO\tSHEAR_DEFORM_NO\tTHIN_SECTION_NO\t2.00000\t2.00000" + '\n' +
"STUD_DEFINITION\tMEMBER-4\tSTANDARD\t19mm/100mm\tWELDED_YES" + '\n' +
"STUD_LAYOUT\tMEMBER-4\tAUTO_MINIMUM_STUD\t0.200000" + '\n' +
"STUD_NO_STUD_ZONE\tMEMBER-4\t0.000000\t0.000000" + '\n' +
"STUD_EC4_APPLY\tMEMBER-4\tYES" + '\n' +
"STUD_NCCI_LIMIT_APPLY\tMEMBER-4\tNO" + '\n' +
"STUD_EC4_RFT_POS\tMEMBER-4\t0.0300000" + '\n' +
"EC4_STUD_GRADE\tMEMBER-4\tCODE_GRADE_NO\t4.79000e+008" + '\n' +

"MEMBER_TITLE\tMEMBER-5\t\tB/tf=20\t\tChange in direction > 9 degrees" + '\n' +
"DESIGN_OPTION\tMEMBER-5\tHKSUOS:2005\tUNPROPPED\tBEAM_WEIGHT_NO\tSLAB_WEIGHT_NO\tSHEAR_DEFORM_NO\tTHIN_SECTION_NO\t2.00000\t2.00000" + '\n' +
"STUD_DEFINITION\tMEMBER-5\tSTANDARD\t19mm/100mm\tWELDED_YES" + '\n' +
"STUD_LAYOUT\tMEMBER-5\tUSER_DEFINED\t3\t1\t0.000000\t2\t1\t0.0760000\t0.0950000\t0.150000\tCHECK_SPACE_NO" + '\n' +
"STUD_LAYOUT\tMEMBER-5\tUSER_DEFINED\t3\t2\t0.000000\t3\t2\t0.0760000\t0.0950000\t0.250000\tCHECK_SPACE_YES" + '\n' +
"STUD_LAYOUT\tMEMBER-5\tUSER_DEFINED\t3\t3\t0.000000\t4\t3\t0.0760000\t0.0950000\t0.350000\tCHECK_SPACE_NO" + '\n' +
"STUD_NO_STUD_ZONE\tMEMBER-5\t0.000000\t0.000000" + '\n' +
"STUD_EC4_APPLY\tMEMBER-5\tYES" + '\n' +

"MEMBER_TITLE\tMEMBER-6\t\tB/tf=22.5\t\tChange in direction < 8 degrees" + '\n' +
"DESIGN_OPTION\tMEMBER-6\tHKSUOS:2011\tUNPROPPED\tBEAM_WEIGHT_NO\tSLAB_WEIGHT_NO\tSHEAR_DEFORM_NO\tTHIN_SECTION_NO\t2.00000\t2.00000" + '\n' +
"STUD_DEFINITION\tMEMBER-6\tSTANDARD\t19mm/95mm\tWELDED_YES" + '\n' +
"STUD_LAYOUT\tMEMBER-6\tAUTO_100\t0.200000" + '\n' +
"STUD_NO_STUD_ZONE\tMEMBER-6\t0.000000\t0.000000" + '\n' +
"STUD_EC4_APPLY\tMEMBER-6\tYES" + '\n' +

"MEMBER_TITLE\tMEMBER-7\t\tB/tf=22.5\t\tChange in direction > 8 degrees" + '\n' +
"DESIGN_OPTION\tMEMBER-7\tAS/NZS2327:2017\tUNPROPPED\tBEAM_WEIGHT_NO\tSLAB_WEIGHT_NO\tSHEAR_DEFORM_NO\tTHIN_SECTION_NO\t2.00000\t2.00000" + '\n' +
"STUD_DEFINITION\tMEMBER-7\tSTANDARD\t19mm/100mm\tWELDED_YES" + '\n' +
"STUD_LAYOUT\tMEMBER-7\tUSER_DEFINED\t2\t1\t0.000000\t2\t1\t0.0570000\t0.0950000\t0.150000\tCHECK_SPACE_NO" + '\n' +
"STUD_LAYOUT\tMEMBER-7\tUSER_DEFINED\t2\t2\t8.000000\t3\t2\t0.0570000\t0.0950000\t0.250000\tCHECK_SPACE_NO" + '\n' +
"STUD_NO_STUD_ZONE\tMEMBER-7\t0.000000\t0.000000" + '\n' +
"STUD_EC4_APPLY\tMEMBER-7\tYES" + '\n';

      // Act
      ComposFile composFile = ComposFile.FromCoaString(coaString);

      //// Assert
      Assert.Equal(7, composFile.GetMembers().Count);
      int i = 0;
      Assert.Equal(19, composFile.GetMembers()[i].Stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(100, composFile.GetMembers()[i].Stud.Dimensions.Height.Millimeters);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.Welding);
      Assert.Equal(StudSpacingType.Automatic, composFile.GetMembers()[i].Stud.StudSpacingType);
      Assert.Equal(0.2, composFile.GetMembers()[i].Stud.MinSavingMultipleZones);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneEnd);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.EC4_Limit);
      i++;
      Assert.Equal(21, composFile.GetMembers()[i].Stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(131, composFile.GetMembers()[i].Stud.Dimensions.Height.Millimeters);
      Assert.False(composFile.GetMembers()[i].Stud.Specification.Welding);
      Assert.Equal(99, composFile.GetMembers()[i].Stud.Dimensions.CharacterStrength.Kilonewtons);
      Assert.Equal(StudSpacingType.Partial_Interaction, composFile.GetMembers()[i].Stud.StudSpacingType);
      Assert.Equal(0.85, composFile.GetMembers()[i].Stud.Interaction);
      Assert.Equal(0.2, composFile.GetMembers()[i].Stud.MinSavingMultipleZones);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneEnd);
      Assert.False(composFile.GetMembers()[i].Stud.Specification.EC4_Limit);
      i++;
      Assert.Equal(19, composFile.GetMembers()[i].Stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(100, composFile.GetMembers()[i].Stud.Dimensions.Height.Millimeters);
      Assert.Equal(450, composFile.GetMembers()[i].Stud.Dimensions.Fu.Megapascals);
      Assert.Equal(StudSpacingType.Automatic, composFile.GetMembers()[i].Stud.StudSpacingType);
      Assert.Equal(0.2, composFile.GetMembers()[i].Stud.MinSavingMultipleZones);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneEnd);
      Assert.Equal(30, composFile.GetMembers()[i].Stud.Specification.ReinforcementPosition.Millimeters);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.EC4_Limit);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.NCCI);
      i++;
      Assert.Equal(19, composFile.GetMembers()[i].Stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(100, composFile.GetMembers()[i].Stud.Dimensions.Height.Millimeters);
      Assert.Equal(479, composFile.GetMembers()[i].Stud.Dimensions.Fu.Megapascals);
      Assert.Equal(StudSpacingType.Min_Num_of_Studs, composFile.GetMembers()[i].Stud.StudSpacingType);
      Assert.Equal(0.2, composFile.GetMembers()[i].Stud.MinSavingMultipleZones);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneEnd);
      Assert.Equal(30, composFile.GetMembers()[i].Stud.Specification.ReinforcementPosition.Millimeters);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.EC4_Limit);
      Assert.False(composFile.GetMembers()[i].Stud.Specification.NCCI);
      i++;
      Assert.Equal(19, composFile.GetMembers()[i].Stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(100, composFile.GetMembers()[i].Stud.Dimensions.Height.Millimeters);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.Welding);
      Assert.Equal(StudSpacingType.Custom, composFile.GetMembers()[i].Stud.StudSpacingType);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneEnd);
      Assert.Equal(3, composFile.GetMembers()[i].Stud.CustomSpacing.Count);
      Assert.Equal(0, composFile.GetMembers()[i].Stud.CustomSpacing[0].DistanceFromStart.As(LengthUnit.Meter));
      Assert.Equal(2, composFile.GetMembers()[i].Stud.CustomSpacing[0].NumberOfRows);
      Assert.Equal(1, composFile.GetMembers()[i].Stud.CustomSpacing[0].NumberOfLines);
      Assert.Equal(150, composFile.GetMembers()[i].Stud.CustomSpacing[0].Spacing.Millimeters);
      Assert.Equal(0, composFile.GetMembers()[i].Stud.CustomSpacing[1].DistanceFromStart.As(LengthUnit.Meter));
      Assert.Equal(3, composFile.GetMembers()[i].Stud.CustomSpacing[1].NumberOfRows);
      Assert.Equal(2, composFile.GetMembers()[i].Stud.CustomSpacing[1].NumberOfLines);
      Assert.Equal(250, composFile.GetMembers()[i].Stud.CustomSpacing[1].Spacing.Millimeters);
      Assert.Equal(0, composFile.GetMembers()[i].Stud.CustomSpacing[2].DistanceFromStart.As(LengthUnit.Meter));
      Assert.Equal(4, composFile.GetMembers()[i].Stud.CustomSpacing[2].NumberOfRows);
      Assert.Equal(3, composFile.GetMembers()[i].Stud.CustomSpacing[2].NumberOfLines);
      Assert.Equal(350, composFile.GetMembers()[i].Stud.CustomSpacing[2].Spacing.Millimeters, 6);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.EC4_Limit);
      i++;
      Assert.Equal(19, composFile.GetMembers()[i].Stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(95, composFile.GetMembers()[i].Stud.Dimensions.Height.Millimeters);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.Welding);
      Assert.Equal(StudSpacingType.Automatic, composFile.GetMembers()[i].Stud.StudSpacingType);
      Assert.Equal(0.2, composFile.GetMembers()[i].Stud.MinSavingMultipleZones);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneEnd);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.EC4_Limit);
      i++;
      Assert.Equal(19, composFile.GetMembers()[i].Stud.Dimensions.Diameter.Millimeters);
      Assert.Equal(100, composFile.GetMembers()[i].Stud.Dimensions.Height.Millimeters);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.Welding);
      Assert.Equal(StudSpacingType.Custom, composFile.GetMembers()[i].Stud.StudSpacingType);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneStart);
      Assert.Equal(Length.Zero, composFile.GetMembers()[i].Stud.Specification.NoStudZoneEnd);
      Assert.Equal(2, composFile.GetMembers()[i].Stud.CustomSpacing.Count);
      Assert.Equal(0, composFile.GetMembers()[i].Stud.CustomSpacing[0].DistanceFromStart.As(LengthUnit.Meter));
      Assert.Equal(2, composFile.GetMembers()[i].Stud.CustomSpacing[0].NumberOfRows);
      Assert.Equal(1, composFile.GetMembers()[i].Stud.CustomSpacing[0].NumberOfLines);
      Assert.Equal(150, composFile.GetMembers()[i].Stud.CustomSpacing[0].Spacing.Millimeters);
      Assert.Equal(8, composFile.GetMembers()[i].Stud.CustomSpacing[1].DistanceFromStart.As(LengthUnit.Meter));
      Assert.Equal(3, composFile.GetMembers()[i].Stud.CustomSpacing[1].NumberOfRows);
      Assert.Equal(2, composFile.GetMembers()[i].Stud.CustomSpacing[1].NumberOfLines);
      Assert.Equal(250, composFile.GetMembers()[i].Stud.CustomSpacing[1].Spacing.Millimeters);
      Assert.True(composFile.GetMembers()[i].Stud.Specification.EC4_Limit);
    }
  }
}
