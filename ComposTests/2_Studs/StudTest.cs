using Xunit;
using UnitsNet;
using UnitsNet.Units;
using System.Collections.Generic;

namespace ComposAPI.Tests
{
  public partial class StudTest
  {
    [Fact]
    public Stud TestConstructorStudCustomSpacing()
    {
      // 1 setup inputs
      IStudDimensions dimensions = new StudDimensions(StandardSize.D13mmH65mm, StandardGrade.SD1_EN13918);
      IStudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);
      List<IStudGroupSpacing> studSpacings = new List<IStudGroupSpacing>();
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 2, 1, new Length(25, LengthUnit.Centimeter)));
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 1, 2, new Length(35, LengthUnit.Centimeter)));

      // 2 create object instance with constructor
      Stud stud = new Stud(dimensions, specification, studSpacings, true);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.StudDimensions);
      Assert.Equal(13, stud.StudDimensions.Diameter.Millimeters);
      Assert.Equal(65, stud.StudDimensions.Height.Millimeters);
      Assert.Equal(400, stud.StudDimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.StudDimensions.CharacterStrength);
      // specification
      Assert.NotNull(stud.StudSpecification);
      Assert.Equal(Length.Zero, stud.StudSpecification.NoStudZoneStart);
      Assert.Equal(Length.Zero, stud.StudSpecification.NoStudZoneEnd);
      Assert.True(stud.StudSpecification.Welding);
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

    // 1 setup inputs
    [Theory]
    [InlineData(StudSpacingType.Min_Num_of_Studs, 0.2)]
    [InlineData(StudSpacingType.Automatic, 0.3)]
    public Stud TestConstructorStudAutomaticOrMinSpacing(StudSpacingType type, double minSaving)
    {
      // 1b setup inputs
      IStudDimensions dimensions = new StudDimensions(StandardSize.D13mmH65mm, StandardGrade.SD1_EN13918);
      IStudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);

      // 2 create object instance with constructor
      Stud stud = new Stud(dimensions, specification, minSaving, type);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.StudDimensions);
      Assert.Equal(13, stud.StudDimensions.Diameter.Millimeters);
      Assert.Equal(65, stud.StudDimensions.Height.Millimeters);
      Assert.Equal(400, stud.StudDimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.StudDimensions.CharacterStrength);
      // specification
      Assert.NotNull(stud.StudSpecification);
      Assert.Equal(Length.Zero, stud.StudSpecification.NoStudZoneStart);
      Assert.Equal(Length.Zero, stud.StudSpecification.NoStudZoneEnd);
      Assert.True(stud.StudSpecification.Welding);
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
      IStudDimensions dimensions = new StudDimensions(StandardSize.D13mmH65mm, StandardGrade.SD1_EN13918);
      IStudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);

      // 2 create object instance with constructor
      Stud stud = new Stud(dimensions, specification, minSaving, interaction);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.StudDimensions);
      Assert.Equal(13, stud.StudDimensions.Diameter.Millimeters);
      Assert.Equal(65, stud.StudDimensions.Height.Millimeters);
      Assert.Equal(400, stud.StudDimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.StudDimensions.CharacterStrength);
      // specification
      Assert.NotNull(stud.StudSpecification);
      Assert.Equal(Length.Zero, stud.StudSpecification.NoStudZoneStart);
      Assert.Equal(Length.Zero, stud.StudSpecification.NoStudZoneEnd);
      Assert.True(stud.StudSpecification.Welding);
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
      Stud duplicate = original.Duplicate() as Stud;

      // 2 check that duplicate has duplicated values
      // dimensions
      Assert.NotNull(duplicate.StudDimensions);
      Assert.Equal(13, duplicate.StudDimensions.Diameter.Millimeters);
      Assert.Equal(65, duplicate.StudDimensions.Height.Millimeters);
      Assert.Equal(400, duplicate.StudDimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, duplicate.StudDimensions.CharacterStrength);
      // specification
      Assert.NotNull(duplicate.StudSpecification);
      Assert.Equal(Length.Zero, duplicate.StudSpecification.NoStudZoneStart);
      Assert.Equal(Length.Zero, duplicate.StudSpecification.NoStudZoneEnd);
      Assert.True(duplicate.StudSpecification.Welding);
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

      // 3 make some changes to duplicate
      IStudDimensions dimensions = new StudDimensions(StandardSize.D25mmH100mm, StandardGrade.SD3_EN13918);
      IStudSpecification specification = new StudSpecification(new Length(25, LengthUnit.Centimeter), new Length(35, LengthUnit.Centimeter), false);
      List<IStudGroupSpacing> studSpacings = new List<IStudGroupSpacing>();
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 3, 2, new Length(10, LengthUnit.Centimeter)));

      duplicate.StudDimensions = dimensions;
      duplicate.StudSpecification = specification;
      duplicate.CustomSpacing = studSpacings;
      duplicate.CheckStudSpacing = false;

      // 4 check that duplicate has set changes
      // dimensions
      Assert.NotNull(duplicate.StudDimensions);
      Assert.Equal(25, duplicate.StudDimensions.Diameter.Millimeters);
      Assert.Equal(100, duplicate.StudDimensions.Height.Millimeters);
      Assert.Equal(500, duplicate.StudDimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, duplicate.StudDimensions.CharacterStrength);
      // specification
      Assert.NotNull(duplicate.StudSpecification);
      Assert.Equal(25, duplicate.StudSpecification.NoStudZoneStart.Centimeters);
      Assert.Equal(35, duplicate.StudSpecification.NoStudZoneEnd.Centimeters);
      Assert.False(duplicate.StudSpecification.Welding);
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
      Assert.NotNull(original.StudDimensions);
      Assert.Equal(13, original.StudDimensions.Diameter.Millimeters);
      Assert.Equal(65, original.StudDimensions.Height.Millimeters);
      Assert.Equal(400, original.StudDimensions.Fu.Megapascals);
      Assert.Equal(Force.Zero, original.StudDimensions.CharacterStrength);
      // specification
      Assert.NotNull(original.StudSpecification);
      Assert.Equal(Length.Zero, original.StudSpecification.NoStudZoneStart);
      Assert.Equal(Length.Zero, original.StudSpecification.NoStudZoneEnd);
      Assert.True(original.StudSpecification.Welding);
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
      Stud duplicate = original.Duplicate() as Stud;

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
      duplicate = original.Duplicate() as Stud;

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

      IStudDimensions dimensions = new StudDimensions(StandardSize.D19mmH100mm, new Force(1, ForceUnit.Newton));
      IStudSpecification specs = new StudSpecification(true, Length.Zero, Length.Zero);
      Stud stud = new Stud(dimensions, specs, 0.2, StudSpacingType.Automatic);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ForceUnit.Newton, PressureUnit.NewtonPerSquareMeter, LengthUnit.Meter, LengthUnit.Millimeter, Code.BS5950_3_1_1990_Superseded);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestBS5950ToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	USER_DEFINED	21.0000	105.000	95000.0	REDUCED_NO	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	AUTO_PERCENT	0.200000	0.850000" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	1.00000	5.00000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	NO" + '\n';

      IStudDimensions dimensions = new StudDimensions(new Length(21, LengthUnit.Millimeter), new Length(105, LengthUnit.Millimeter), new Force(95000, ForceUnit.Newton));
      IStudSpecification specs = new StudSpecification(false, new Length(1, LengthUnit.Meter), new Length(5, LengthUnit.Meter));
      Stud stud = new Stud(dimensions, specs, 0.2, 0.85);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ForceUnit.Newton, PressureUnit.NewtonPerSquareMeter, LengthUnit.Meter, LengthUnit.Millimeter, Code.BS5950_3_1_1990_A1_2010);

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
        "STUD_EC4_RFT_POS	MEMBER-1	0.030000" + '\n' +
        "EC4_STUD_GRADE	MEMBER-1	CODE_GRADE_YES	SD2_EN13918" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardSize.D19mmH100mm, StandardGrade.SD2_EN13918);
      IStudSpecification specs = new StudSpecification(new Length(0, LengthUnit.Meter), new Length(0, LengthUnit.Meter), new Length(30, LengthUnit.Millimeter), false, false);
      Stud stud = new Stud(dimensions, specs, 0.2, StudSpacingType.Automatic);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ForceUnit.Newton, PressureUnit.NewtonPerSquareMeter, LengthUnit.Meter, LengthUnit.Millimeter, Code.EN1994_1_1_2004);

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
        "STUD_EC4_RFT_POS	MEMBER-1	0.030000" + '\n' +
        "EC4_STUD_GRADE	MEMBER-1	CODE_GRADE_NO	4.40000e+008" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardSize.D25mmH100mm, new Pressure(440, PressureUnit.Megapascal));
      IStudSpecification specs = new StudSpecification(new Length(0, LengthUnit.Meter), new Length(0, LengthUnit.Meter), new Length(30, LengthUnit.Millimeter), true, true);
      Stud stud = new Stud(dimensions, specs, 0.2, StudSpacingType.Min_Num_of_Studs);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ForceUnit.Newton, PressureUnit.NewtonPerSquareMeter, LengthUnit.Meter, LengthUnit.Millimeter, Code.EN1994_1_1_2004);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestHK05CustomSpacingToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	STANDARD	13mm/65mm	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	1	0.000000	2	1	0.076000	0.095000	0.150000	CHECK_SPACE_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	2	4.50000	3	2	0.076000	0.095000	0.250000	CHECK_SPACE_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	3	9.00000	4	3	0.076000	0.095000	0.350000	CHECK_SPACE_YES" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	YES" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardSize.D13mmH65mm, new Force(1, ForceUnit.Newton));
      IStudSpecification specs = new StudSpecification(new Length(0, LengthUnit.Meter), new Length(0, LengthUnit.Meter), true);
      IStudGroupSpacing spacing1 = new StudGroupSpacing(Length.Zero, 2, 1, new Length(150, LengthUnit.Millimeter));
      IStudGroupSpacing spacing2 = new StudGroupSpacing(new Length(4.5, LengthUnit.Meter), 3, 2, new Length(250, LengthUnit.Millimeter));
      IStudGroupSpacing spacing3 = new StudGroupSpacing(new Length(9, LengthUnit.Meter), 4, 3, new Length(350, LengthUnit.Millimeter));

      List<IStudGroupSpacing> spacing = new List<IStudGroupSpacing>() { spacing1, spacing2, spacing3 };
      Stud stud = new Stud(dimensions, specs, spacing, true);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ForceUnit.Newton, PressureUnit.NewtonPerSquareMeter, LengthUnit.Meter, LengthUnit.Millimeter, Code.HKSUOS_2005);

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

      IStudDimensions dimensions = new StudDimensions(StandardSize.D19mmH95mm, new Force(1, ForceUnit.Newton));
      IStudSpecification specs = new StudSpecification(Length.Zero, Length.Zero, true);
      Stud stud = new Stud(dimensions, specs, 0.2, StudSpacingType.Automatic);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ForceUnit.Newton, PressureUnit.NewtonPerSquareMeter, LengthUnit.Meter, LengthUnit.Millimeter, Code.HKSUOS_2011);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestASNZCustomSpacingToCoa()
    {
      // Arrange
      string expected_coaString =
        "STUD_DEFINITION	MEMBER-1	STANDARD	19mm/100mm	WELDED_YES" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	2	1	0.000000	2	1	0.057000	0.095000	0.150000	CHECK_SPACE_NO" + '\n' +
        "STUD_LAYOUT	MEMBER-1	USER_DEFINED	2	2	8.00000	3	2	0.057000	0.095000	0.250000	CHECK_SPACE_NO" + '\n' +
        "STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000" + '\n' +
        "STUD_EC4_APPLY	MEMBER-1	YES" + '\n';

      IStudDimensions dimensions = new StudDimensions(StandardSize.D19mmH100mm, new Force(1, ForceUnit.Newton));
      IStudSpecification specs = new StudSpecification(new Length(0, LengthUnit.Meter), new Length(0, LengthUnit.Meter), true);
      IStudGroupSpacing spacing1 = new StudGroupSpacing(Length.Zero, 2, 1, new Length(150, LengthUnit.Millimeter));
      IStudGroupSpacing spacing2 = new StudGroupSpacing(new Length(8, LengthUnit.Meter), 3, 2, new Length(250, LengthUnit.Millimeter));

      List<IStudGroupSpacing> spacing = new List<IStudGroupSpacing>() { spacing1, spacing2 };
      Stud stud = new Stud(dimensions, specs, spacing, false);

      // Act
      string coaString = stud.ToCoaString("MEMBER-1", ForceUnit.Newton, PressureUnit.NewtonPerSquareMeter, LengthUnit.Meter, LengthUnit.Millimeter, Code.AS_NZS2327_2017);

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }
  }
}
