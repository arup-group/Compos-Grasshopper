using Xunit;
using UnitsNet;
using UnitsNet.Units;
using System.Collections.Generic;

namespace ComposGH.Parameters.Tests
{
  public partial class ComposStudTest
  {
    [Fact]
    public ComposStud TestConstructorStudCustomSpacing()
    {
      // 1 setup inputs
      StudDimensions dimensions = new StudDimensions(StudDimensions.StandardSize.D13mmH65mm, StudDimensions.StandardGrade.SD1_EN13918);
      StudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);
      List<StudGroupSpacing> studSpacings = new List<StudGroupSpacing>();
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 2, 1, new Length(25, LengthUnit.Centimeter)));
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 1, 2, new Length(35, LengthUnit.Centimeter)));

      // 2 create object instance with constructor
      ComposStud stud = new ComposStud(dimensions, specification, studSpacings, true);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.StudDimension);
      Assert.Equal(13, stud.StudDimension.Diameter.Millimeters);
      Assert.Equal(65, stud.StudDimension.Height.Millimeters);
      Assert.Equal(400, stud.StudDimension.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.StudDimension.CharacterStrength);
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
      Assert.Equal(StudGroupSpacing.StudSpacingType.Custom, stud.StudSpacingType);
      Assert.True(stud.CheckStudSpacing);
      Assert.Equal(double.NaN, stud.Interaction);
      Assert.Equal(double.NaN, stud.MinSavingMultipleZones);

      return stud;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(StudGroupSpacing.StudSpacingType.Min_Num_of_Studs, 0.2)]
    [InlineData(StudGroupSpacing.StudSpacingType.Automatic, 0.3)]
    public ComposStud TestConstructorStudAutomaticOrMinSpacing(StudGroupSpacing.StudSpacingType type, double minSaving)
    {
      // 1b setup inputs
      StudDimensions dimensions = new StudDimensions(StudDimensions.StandardSize.D13mmH65mm, StudDimensions.StandardGrade.SD1_EN13918);
      StudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);

      // 2 create object instance with constructor
      ComposStud stud = new ComposStud(dimensions, specification, minSaving, type);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.StudDimension);
      Assert.Equal(13, stud.StudDimension.Diameter.Millimeters);
      Assert.Equal(65, stud.StudDimension.Height.Millimeters);
      Assert.Equal(400, stud.StudDimension.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.StudDimension.CharacterStrength);
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
    [InlineData(StudGroupSpacing.StudSpacingType.Custom)]
    [InlineData(StudGroupSpacing.StudSpacingType.Partial_Interaction)]
    public void TestConstructorStudAutomaticOrMinSpacingExceptions(StudGroupSpacing.StudSpacingType type)
    {
      // check that exceptions are thrown if inputs does not comply with allowed
      Assert.Throws<System.ArgumentException>(() => TestConstructorStudAutomaticOrMinSpacing(type, 0.2));
    }

    // 1 setup inputs
    [Theory]
    [InlineData(0.2, 0.95)]
    [InlineData(0.3, 0.85)]
    public ComposStud TestConstructorStudPartialSpacing(double minSaving, double interaction)
    {
      // 1b setup inputs
      StudDimensions dimensions = new StudDimensions(StudDimensions.StandardSize.D13mmH65mm, StudDimensions.StandardGrade.SD1_EN13918);
      StudSpecification specification = new StudSpecification(Length.Zero, Length.Zero, true);

      // 2 create object instance with constructor
      ComposStud stud = new ComposStud(dimensions, specification, minSaving, interaction);

      // 3 check that inputs are set in object's members
      // dimensions
      Assert.NotNull(stud.StudDimension);
      Assert.Equal(13, stud.StudDimension.Diameter.Millimeters);
      Assert.Equal(65, stud.StudDimension.Height.Millimeters);
      Assert.Equal(400, stud.StudDimension.Fu.Megapascals);
      Assert.Equal(Force.Zero, stud.StudDimension.CharacterStrength);
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
      Assert.Equal(StudGroupSpacing.StudSpacingType.Partial_Interaction, stud.StudSpacingType);

      return stud;
    }

    [Fact]
    public void TestComposStudDuplicate()
    {
      // 1 create with constructor and duplicate
      ComposStud original = TestConstructorStudCustomSpacing();
      ComposStud duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      // dimensions
      Assert.NotNull(duplicate.StudDimension);
      Assert.Equal(13, duplicate.StudDimension.Diameter.Millimeters);
      Assert.Equal(65, duplicate.StudDimension.Height.Millimeters);
      Assert.Equal(400, duplicate.StudDimension.Fu.Megapascals);
      Assert.Equal(Force.Zero, duplicate.StudDimension.CharacterStrength);
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
      Assert.Equal(StudGroupSpacing.StudSpacingType.Custom, duplicate.StudSpacingType);
      Assert.True(duplicate.CheckStudSpacing);
      Assert.Equal(double.NaN, duplicate.Interaction);
      Assert.Equal(double.NaN, duplicate.MinSavingMultipleZones);

      // 3 make some changes to duplicate
      StudDimensions dimensions = new StudDimensions(StudDimensions.StandardSize.D25mmH100mm, StudDimensions.StandardGrade.SD3_EN13918);
      StudSpecification specification = new StudSpecification(new Length(25, LengthUnit.Centimeter), new Length(35, LengthUnit.Centimeter), false);
      List<StudGroupSpacing> studSpacings = new List<StudGroupSpacing>();
      studSpacings.Add(new StudGroupSpacing(Length.Zero, 3, 2, new Length(10, LengthUnit.Centimeter)));

      duplicate.StudDimension = dimensions;
      duplicate.StudSpecification = specification;
      duplicate.CustomSpacing = studSpacings;
      duplicate.CheckStudSpacing = false;

      // 4 check that duplicate has set changes
      // dimensions
      Assert.NotNull(duplicate.StudDimension);
      Assert.Equal(25, duplicate.StudDimension.Diameter.Millimeters);
      Assert.Equal(100, duplicate.StudDimension.Height.Millimeters);
      Assert.Equal(500, duplicate.StudDimension.Fu.Megapascals);
      Assert.Equal(Force.Zero, duplicate.StudDimension.CharacterStrength);
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
      Assert.Equal(StudGroupSpacing.StudSpacingType.Custom, duplicate.StudSpacingType);
      Assert.False(duplicate.CheckStudSpacing);
      Assert.Equal(double.NaN, duplicate.Interaction);
      Assert.Equal(double.NaN, duplicate.MinSavingMultipleZones);

      // 5 check that original has not been changed
      // dimensions
      Assert.NotNull(original.StudDimension);
      Assert.Equal(13, original.StudDimension.Diameter.Millimeters);
      Assert.Equal(65, original.StudDimension.Height.Millimeters);
      Assert.Equal(400, original.StudDimension.Fu.Megapascals);
      Assert.Equal(Force.Zero, original.StudDimension.CharacterStrength);
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
      Assert.Equal(StudGroupSpacing.StudSpacingType.Custom, original.StudSpacingType);
      Assert.True(original.CheckStudSpacing);
      Assert.Equal(double.NaN, original.Interaction);
      Assert.Equal(double.NaN, original.MinSavingMultipleZones);

      // 1 create with new constructor and duplicate
      original = TestConstructorStudAutomaticOrMinSpacing(StudGroupSpacing.StudSpacingType.Automatic, 0.2);
      duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(StudGroupSpacing.StudSpacingType.Automatic, duplicate.StudSpacingType);
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
      duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(StudGroupSpacing.StudSpacingType.Partial_Interaction, duplicate.StudSpacingType);
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
  }
}
