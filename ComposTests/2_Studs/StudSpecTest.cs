using Xunit;
using OasysUnitsNet;
using OasysUnitsNet.Units;
using ComposGHTests.Helpers;
using OasysGH;

namespace ComposAPI.Studs.Tests
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
      Assert.Equal(noStudZoneStart, studSpec.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(noStudZoneEnd, studSpec.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.Equal(reinforcementPosition, studSpec.ReinforcementPosition.Millimeters);
      Assert.Equal(welding, studSpec.Welding);
      Assert.Equal(ncci, studSpec.NCCI);
      Assert.Equal(StudSpecType.EC4, studSpec.SpecType);

      return studSpec;
    }

    [Fact]
    public void TestConstructorStudSpecEC4Ratio()
    {
      LengthUnit unit = LengthUnit.Millimeter;
      RatioUnit percent = RatioUnit.Percent;

      // 2 create object instance with constructor
      StudSpecification studSpec = new StudSpecification(
        new Ratio(20, percent), new Ratio(5, percent),
        new Length(20, unit), true, false);

      // 3 check that inputs are set in object's members
      Assert.Equal(20, studSpec.NoStudZoneStart.As(percent));
      Assert.Equal(5, studSpec.NoStudZoneEnd.As(percent));
    }

    [Fact]
    public void DuplicateEC4Test()
    {
      // 1 create with constructor and duplicate
      StudSpecification original = TestConstructorStudSpecEC4(50, 100, 10, true, false);
      StudSpecification duplicate = (StudSpecification)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
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
      Assert.Equal(noStudZoneStart, studSpec.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(noStudZoneEnd, studSpec.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.Equal(useEC4Limit, studSpec.EC4_Limit);
      Assert.Equal(StudSpecType.BS5950, studSpec.SpecType);

      return studSpec;
    }

    [Fact]
    public void DuplicateBS5950Test()
    {
      // 1 create with constructor and duplicate
      StudSpecification original = TestConstructorStudSpecBS5950(true, 100, 10);
      StudSpecification duplicate = (StudSpecification)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
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
      Assert.Equal(noStudZoneStart, studSpec.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(noStudZoneEnd, studSpec.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.Equal(welding, studSpec.Welding);
      Assert.Equal(StudSpecType.Other, studSpec.SpecType);

      return studSpec;
    }
    [Fact]
    public void DuplicateStudSpecTest()
    {
      // 1 create with constructor and duplicate
      StudSpecification original = TestConstructorStudSpec(50, 100, true);
      StudSpecification duplicate = (StudSpecification)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void TestStudSpecDuplicate1()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      StudSpecification original = TestConstructorStudSpecEC4(25, 75, 15, false, true);
      StudSpecification duplicate = (StudSpecification)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(25, duplicate.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(75, duplicate.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.Equal(15, duplicate.ReinforcementPosition.Millimeters);
      Assert.False(duplicate.Welding);
      Assert.True(duplicate.NCCI);
      Assert.Equal(StudSpecType.EC4, duplicate.SpecType);

      // 3 make some changes to duplicate
      duplicate.NoStudZoneStart = new Length(26, unit);
      duplicate.NoStudZoneEnd = new Length(76, unit);
      duplicate.ReinforcementPosition = new Length(13, unit);
      duplicate.Welding = true;
      duplicate.NCCI = false;

      // 4 check that duplicate has set changes
      Assert.Equal(26, duplicate.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(76, duplicate.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.Equal(13, duplicate.ReinforcementPosition.Millimeters);
      Assert.True(duplicate.Welding);
      Assert.False(duplicate.NCCI);
      Assert.Equal(StudSpecType.EC4, duplicate.SpecType);

      // 5 check that original has not been changed
      Assert.Equal(25, original.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(75, original.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.Equal(15, original.ReinforcementPosition.Millimeters);
      Assert.False(original.Welding);
      Assert.True(original.NCCI);
      Assert.Equal(StudSpecType.EC4, original.SpecType);
    }

    [Fact]
    public void TestStudSpecDuplicate2()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with new constructor and duplicate
      StudSpecification original = TestConstructorStudSpecBS5950(false, 25, 75);
      StudSpecification duplicate = (StudSpecification)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(25, duplicate.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(75, duplicate.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.False(duplicate.EC4_Limit);
      Assert.Equal(StudSpecType.BS5950, duplicate.SpecType);

      // 3 make some changes to duplicate
      duplicate.NoStudZoneStart = new Length(26, unit);
      duplicate.NoStudZoneEnd = new Length(77, unit);
      duplicate.EC4_Limit = true;

      // 4 check that duplicate has set changes
      Assert.Equal(26, duplicate.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(77, duplicate.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.Equal(Length.Zero, duplicate.ReinforcementPosition);
      Assert.True(duplicate.EC4_Limit);
      Assert.Equal(StudSpecType.BS5950, duplicate.SpecType);

      // 5 check that original has not been changed
      Assert.Equal(25, original.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(75, original.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.False(original.EC4_Limit);
      Assert.Equal(StudSpecType.BS5950, original.SpecType);
    }

    [Fact]
    public void TestStudSpecDuplicate3()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with new constructor and duplicate
      StudSpecification original = TestConstructorStudSpec(19, 20, true);
      StudSpecification duplicate = (StudSpecification)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(19, duplicate.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(20, duplicate.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.True(duplicate.Welding);
      Assert.Equal(StudSpecType.Other, duplicate.SpecType);

      // 3 make some changes to duplicate
      duplicate.NoStudZoneStart = new Length(18, unit);
      duplicate.NoStudZoneEnd = new Length(21, unit);
      duplicate.Welding = false;

      // 4 check that duplicate has set changes
      Assert.Equal(18, duplicate.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(21, duplicate.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.Equal(Length.Zero, duplicate.ReinforcementPosition);
      Assert.False(duplicate.Welding);
      Assert.Equal(StudSpecType.Other, duplicate.SpecType);

      // 5 check that original has not been changed
      Assert.Equal(19, original.NoStudZoneStart.As(LengthUnit.Millimeter));
      Assert.Equal(20, original.NoStudZoneEnd.As(LengthUnit.Millimeter));
      Assert.True(original.Welding);
      Assert.Equal(StudSpecType.Other, original.SpecType);
    }
  }
}
