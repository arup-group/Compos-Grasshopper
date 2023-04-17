using ComposGHTests.Helpers;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Studs.Tests {
  public partial class StudTest {

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      StudGroupSpacing original = TestConstructorStudSpacing(50, 1, 2, 150);
      StudGroupSpacing duplicate = (StudGroupSpacing)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(50, 1, 2, 150)]
    public StudGroupSpacing TestConstructorStudSpacing(double distanceFromStart, int numberOfRows,
      int numberOfLines, double spacing) {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      StudGroupSpacing studSpacing = new StudGroupSpacing(
        new Length(distanceFromStart, unit), numberOfRows, numberOfLines, new Length(spacing, unit));

      // 3 check that inputs are set in object's members
      Assert.Equal(distanceFromStart, studSpacing.DistanceFromStart.As(LengthUnit.Millimeter));
      Assert.Equal(numberOfRows, studSpacing.NumberOfRows);
      Assert.Equal(numberOfLines, studSpacing.NumberOfLines);
      Assert.Equal(spacing, studSpacing.Spacing.Millimeters);

      // 4 return object as input for overaching class test
      return studSpacing;
    }

    [Fact]
    public void TestContructorStudSpacingExceptions() {
      // check that exceptions are thrown if inputs does not comply with allowed
      Assert.Throws<System.ArgumentException>(() => TestConstructorStudSpacing(150, 0, 1, 250));
      Assert.Throws<System.ArgumentException>(() => TestConstructorStudSpacing(150, 1, 0, 250));
    }

    [Fact]
    public void TestStudSpacingDuplicate() {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      StudGroupSpacing original = TestConstructorStudSpacing(25, 1, 2, 250);
      StudGroupSpacing duplicate = (StudGroupSpacing)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(25, duplicate.DistanceFromStart.As(LengthUnit.Millimeter));
      Assert.Equal(1, duplicate.NumberOfRows);
      Assert.Equal(2, duplicate.NumberOfLines);
      Assert.Equal(250, duplicate.Spacing.Millimeters);

      // 3 make some changes to duplicate
      duplicate.DistanceFromStart = new Length(26, unit);
      duplicate.NumberOfRows = 2;
      duplicate.NumberOfLines = 3;
      duplicate.Spacing = new Length(199.99, unit);

      // 4 check that duplicate has set changes
      Assert.Equal(26, duplicate.DistanceFromStart.As(LengthUnit.Millimeter));
      Assert.Equal(2, duplicate.NumberOfRows);
      Assert.Equal(3, duplicate.NumberOfLines);
      Assert.Equal(199.99, duplicate.Spacing.Millimeters);

      // 5 check that original has not been changed
      Assert.Equal(25, original.DistanceFromStart.As(LengthUnit.Millimeter));
      Assert.Equal(1, original.NumberOfRows);
      Assert.Equal(2, original.NumberOfLines);
      Assert.Equal(250, original.Spacing.Millimeters);
    }
  }
}
