using Xunit;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI.Tests
{
  public partial class StudTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(50, 1, 2, 150, true)]
    public StudGroupSpacing TestConstructorStudSpacing(double distanceFromStart, int numberOfRows, 
      int numberOfLines, double spacing, bool check)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      StudGroupSpacing studSpacing = new StudGroupSpacing(
        new Length(distanceFromStart, unit), numberOfRows, numberOfLines, new Length(spacing, unit), check);

      // 3 check that inputs are set in object's members
      Assert.Equal(distanceFromStart, studSpacing.DistanceFromStart.Millimeters);
      Assert.Equal(numberOfRows, studSpacing.NumberOfRows);
      Assert.Equal(numberOfLines, studSpacing.NumberOfLines);
      Assert.Equal(spacing, studSpacing.Spacing.Millimeters);
      Assert.Equal(check, studSpacing.CheckSpacing);

      // 4 return object as input for overaching class test
      return studSpacing;
    }

    [Fact]
    public void TestContructorStudSpacingExceptions()
    {
      // check that exceptions are thrown if inputs does not comply with allowed
      Assert.Throws<System.ArgumentException>(() => TestConstructorStudSpacing(150, 0, 1, 250, false));
      Assert.Throws<System.ArgumentException>(() => TestConstructorStudSpacing(150, 1, 0, 250, true));
    }

    [Fact]
    public void TestStudSpacingDuplicate()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      StudGroupSpacing original = TestConstructorStudSpacing(25, 1, 2, 250, false);
      StudGroupSpacing duplicate = original.Duplicate() as StudGroupSpacing;

      // 2 check that duplicate has duplicated values
      Assert.Equal(25, duplicate.DistanceFromStart.Millimeters);
      Assert.Equal(1, duplicate.NumberOfRows);
      Assert.Equal(2, duplicate.NumberOfLines);
      Assert.Equal(250, duplicate.Spacing.Millimeters);
      Assert.False(duplicate.CheckSpacing);

      // 3 make some changes to duplicate
      duplicate.DistanceFromStart = new Length(26, unit);
      duplicate.NumberOfRows = 2;
      duplicate.NumberOfLines = 3;
      duplicate.Spacing = new Length(199.99, unit);
      duplicate.CheckSpacing = true;

      // 4 check that duplicate has set changes
      Assert.Equal(26, duplicate.DistanceFromStart.Millimeters);
      Assert.Equal(2, duplicate.NumberOfRows);
      Assert.Equal(3, duplicate.NumberOfLines);
      Assert.Equal(199.99, duplicate.Spacing.Millimeters);
      Assert.True(duplicate.CheckSpacing);

      // 5 check that original has not been changed
      Assert.Equal(25, original.DistanceFromStart.Millimeters);
      Assert.Equal(1, original.NumberOfRows);
      Assert.Equal(2, original.NumberOfLines);
      Assert.Equal(250, original.Spacing.Millimeters);
      Assert.False(original.CheckSpacing);
    }
  }
}
