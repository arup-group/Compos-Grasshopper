﻿using Xunit;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI.Tests
{
  public partial class StudTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(50, 100, 10, true, false)]
    [InlineData(20, 120, 5, false, true)]
    [InlineData(0, 10.5, 0.15, true, true)]
    [InlineData(-2.0, 9999, -250, false, false)]
    public StudSpecification TestConstructorStudSpecEC4(double noStudZoneStart, double noStudZoneEnd, 
      double reinforcementPosition, bool welding, bool ncci)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      StudSpecification studSpec = new StudSpecification(
        new Length(noStudZoneStart, unit), new Length(noStudZoneEnd, unit), 
        new Length(reinforcementPosition, unit), welding, ncci);

      // 3 check that inputs are set in object's members
      Assert.Equal(noStudZoneStart, studSpec.NoStudZoneStart.Millimeters);
      Assert.Equal(noStudZoneEnd, studSpec.NoStudZoneEnd.Millimeters);
      Assert.Equal(reinforcementPosition, studSpec.ReinforcementPosition.Millimeters);
      Assert.Equal(welding, studSpec.Welding);
      Assert.Equal(ncci, studSpec.NCCI);
      Assert.Equal(StudSpecification.StudSpecType.EC4, studSpec.SpecType);

      return studSpec;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(true, 100, 10)]
    [InlineData(false, 120, 5)]
    public StudSpecification TestConstructorStudSpecBS5950(bool useEC4Limit, double noStudZoneStart, double noStudZoneEnd)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      StudSpecification studSpec = new StudSpecification(useEC4Limit,
        new Length(noStudZoneStart, unit), new Length(noStudZoneEnd, unit));

      // 3 check that inputs are set in object's members
      Assert.Equal(noStudZoneStart, studSpec.NoStudZoneStart.Millimeters);
      Assert.Equal(noStudZoneEnd, studSpec.NoStudZoneEnd.Millimeters);
      Assert.Equal(useEC4Limit, studSpec.EC4_Limit);
      Assert.Equal(StudSpecification.StudSpecType.BS5950, studSpec.SpecType);

      return studSpec;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(50, 100, true)]
    [InlineData(20, 120, false)]
    public StudSpecification TestConstructorStudSpec(double noStudZoneStart, double noStudZoneEnd, bool welding)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      StudSpecification studSpec = new StudSpecification(
        new Length(noStudZoneStart, unit), new Length(noStudZoneEnd, unit), welding);

      // 3 check that inputs are set in object's members
      Assert.Equal(noStudZoneStart, studSpec.NoStudZoneStart.Millimeters);
      Assert.Equal(noStudZoneEnd, studSpec.NoStudZoneEnd.Millimeters);
      Assert.Equal(welding, studSpec.Welding);
      Assert.Equal(StudSpecification.StudSpecType.Other, studSpec.SpecType);

      return studSpec;
    }

    [Fact]
    public void TestStudSpecDuplicate()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      StudSpecification original = TestConstructorStudSpecEC4(25, 75, 15, false, true);
      StudSpecification duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(25, duplicate.NoStudZoneStart.Millimeters);
      Assert.Equal(75, duplicate.NoStudZoneEnd.Millimeters);
      Assert.Equal(15, duplicate.ReinforcementPosition.Millimeters);
      Assert.False(duplicate.Welding);
      Assert.True(duplicate.NCCI);
      Assert.Equal(StudSpecification.StudSpecType.EC4, duplicate.SpecType);

      // 3 make some changes to duplicate
      duplicate.NoStudZoneStart = new Length(26, unit);
      duplicate.NoStudZoneEnd = new Length(76, unit);
      duplicate.ReinforcementPosition = new Length(13, unit);
      duplicate.Welding = true;
      duplicate.NCCI = false;

      // 4 check that duplicate has set changes
      Assert.Equal(26, duplicate.NoStudZoneStart.Millimeters);
      Assert.Equal(76, duplicate.NoStudZoneEnd.Millimeters);
      Assert.Equal(13, duplicate.ReinforcementPosition.Millimeters);
      Assert.True(duplicate.Welding);
      Assert.False(duplicate.NCCI);
      Assert.Equal(StudSpecification.StudSpecType.EC4, duplicate.SpecType);

      // 5 check that original has not been changed
      Assert.Equal(25, original.NoStudZoneStart.Millimeters);
      Assert.Equal(75, original.NoStudZoneEnd.Millimeters);
      Assert.Equal(15, original.ReinforcementPosition.Millimeters);
      Assert.False(original.Welding);
      Assert.True(original.NCCI);
      Assert.Equal(StudSpecification.StudSpecType.EC4, original.SpecType);

      // 1 create with new constructor and duplicate
      original = TestConstructorStudSpecBS5950(false, 25, 75);
      duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(25, duplicate.NoStudZoneStart.Millimeters);
      Assert.Equal(75, duplicate.NoStudZoneEnd.Millimeters);
      Assert.False(duplicate.EC4_Limit);
      Assert.Equal(StudSpecification.StudSpecType.BS5950, duplicate.SpecType);

      // 3 make some changes to duplicate
      duplicate.NoStudZoneStart = new Length(26, unit);
      duplicate.NoStudZoneEnd = new Length(77, unit);
      duplicate.EC4_Limit = true;

      // 4 check that duplicate has set changes
      Assert.Equal(26, duplicate.NoStudZoneStart.Millimeters);
      Assert.Equal(77, duplicate.NoStudZoneEnd.Millimeters);
      Assert.Equal(Length.Zero, duplicate.ReinforcementPosition);
      Assert.True(duplicate.EC4_Limit);
      Assert.Equal(StudSpecification.StudSpecType.BS5950, duplicate.SpecType);

      // 5 check that original has not been changed
      Assert.Equal(25, original.NoStudZoneStart.Millimeters);
      Assert.Equal(75, original.NoStudZoneEnd.Millimeters);
      Assert.False(original.EC4_Limit);
      Assert.Equal(StudSpecification.StudSpecType.BS5950, original.SpecType);

      // 1 create with new constructor and duplicate
      original = TestConstructorStudSpec(19, 20, true);
      duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(19, duplicate.NoStudZoneStart.Millimeters);
      Assert.Equal(20, duplicate.NoStudZoneEnd.Millimeters);
      Assert.True(duplicate.Welding);
      Assert.Equal(StudSpecification.StudSpecType.Other, duplicate.SpecType);

      // 3 make some changes to duplicate
      duplicate.NoStudZoneStart = new Length(18, unit);
      duplicate.NoStudZoneEnd = new Length(21, unit);
      duplicate.Welding = false;

      // 4 check that duplicate has set changes
      Assert.Equal(18, duplicate.NoStudZoneStart.Millimeters);
      Assert.Equal(21, duplicate.NoStudZoneEnd.Millimeters);
      Assert.Equal(Length.Zero, duplicate.ReinforcementPosition);
      Assert.False(duplicate.Welding);
      Assert.Equal(StudSpecification.StudSpecType.Other, duplicate.SpecType);

      // 5 check that original has not been changed
      Assert.Equal(19, original.NoStudZoneStart.Millimeters);
      Assert.Equal(20, original.NoStudZoneEnd.Millimeters);
      Assert.True(original.Welding);
      Assert.Equal(StudSpecification.StudSpecType.Other, original.SpecType);
    }
  }
}