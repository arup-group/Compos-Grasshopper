using ComposGHTests.Helpers;
using OasysGH;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Beams.Tests
{
  public partial class WebOpeningTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(50, 100, 10, 100, 10, false)]
    [InlineData(20, 120, 5, 80, 12, true)]
    public WebOpeningStiffeners TestConstructorStiffenersWebOpening(double startPos, double topWidth,
      double topTHK, double bottomWidth, double bottomTHK, bool bothSides)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      WebOpeningStiffeners webOpeningStiffeners = new WebOpeningStiffeners(
        new Length(startPos, unit), new Length(topWidth, unit),
        new Length(topTHK, unit), new Length(bottomWidth, unit),
        new Length(bottomTHK, unit), bothSides);

      // 3 check that inputs are set in object's members
      Assert.Equal(startPos, webOpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(topWidth, webOpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(topTHK, webOpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(bottomWidth, webOpeningStiffeners.BottomStiffenerWidth.Millimeters);
      Assert.Equal(bottomTHK, webOpeningStiffeners.BottomStiffenerThickness.Millimeters);
      Assert.Equal(bothSides, webOpeningStiffeners.isBothSides);
      Assert.False(webOpeningStiffeners.isNotch);

      // (optionally return object for other tests)
      return webOpeningStiffeners;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(50, 100, 10, false)]
    [InlineData(20, 120, 5, true)]
    public WebOpeningStiffeners TestConstructorStiffenersNotch(double distance, double topWidth,
      double topTHK, bool bothSides)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      WebOpeningStiffeners webOpeningStiffeners = new WebOpeningStiffeners(
        new Length(distance, unit), new Length(topWidth, unit),
        new Length(topTHK, unit), bothSides);

      // 3 check that inputs are set in object's members
      Assert.Equal(distance, webOpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(topWidth, webOpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(topTHK, webOpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(Length.Zero, webOpeningStiffeners.BottomStiffenerWidth);
      Assert.Equal(Length.Zero, webOpeningStiffeners.BottomStiffenerThickness);
      Assert.Equal(bothSides, webOpeningStiffeners.isBothSides);
      Assert.True(webOpeningStiffeners.isNotch);

      // (optionally return object for other tests)
      return webOpeningStiffeners;
    }

    [Fact]
    public void DuplicateStiffenerTest()
    {
      // 1 create with constructor and duplicate
      WebOpeningStiffeners original = TestConstructorStiffenersWebOpening(25, 75, 12, 125, 15, false);
      WebOpeningStiffeners duplicate = (WebOpeningStiffeners)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void TestStiffenerDuplicate()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      WebOpeningStiffeners original = TestConstructorStiffenersWebOpening(25, 75, 12, 125, 15, false);
      WebOpeningStiffeners duplicate = (WebOpeningStiffeners)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(25, duplicate.DistanceFrom.Millimeters);
      Assert.Equal(75, duplicate.TopStiffenerWidth.Millimeters);
      Assert.Equal(12, duplicate.TopStiffenerThickness.Millimeters);
      Assert.Equal(125, duplicate.BottomStiffenerWidth.Millimeters);
      Assert.Equal(15, duplicate.BottomStiffenerThickness.Millimeters);
      Assert.False(duplicate.isBothSides);
      Assert.False(duplicate.isNotch);

      // 3 make some changes to duplicate
      duplicate.DistanceFrom = new Length(26, unit);
      duplicate.TopStiffenerWidth = new Length(76, unit);
      duplicate.TopStiffenerThickness = new Length(13, unit);
      duplicate.BottomStiffenerWidth = new Length(126, unit);
      duplicate.BottomStiffenerThickness = new Length(16, unit);
      duplicate.isBothSides = true;

      // 4 check that duplicate has set changes
      Assert.Equal(26, duplicate.DistanceFrom.Millimeters);
      Assert.Equal(76, duplicate.TopStiffenerWidth.Millimeters);
      Assert.Equal(13, duplicate.TopStiffenerThickness.Millimeters);
      Assert.Equal(126, duplicate.BottomStiffenerWidth.Millimeters);
      Assert.Equal(16, duplicate.BottomStiffenerThickness.Millimeters);
      Assert.True(duplicate.isBothSides);
      Assert.False(duplicate.isNotch);

      // 5 check that original has not been changed
      Assert.Equal(25, original.DistanceFrom.Millimeters);
      Assert.Equal(75, original.TopStiffenerWidth.Millimeters);
      Assert.Equal(12, original.TopStiffenerThickness.Millimeters);
      Assert.Equal(125, original.BottomStiffenerWidth.Millimeters);
      Assert.Equal(15, original.BottomStiffenerThickness.Millimeters);
      Assert.False(original.isBothSides);
      Assert.False(original.isNotch);
    }

    [Fact]
    public void TestStiffenerDuplicate2()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      WebOpeningStiffeners original = TestConstructorStiffenersNotch(27, 77, 14, true);
      WebOpeningStiffeners duplicate = (WebOpeningStiffeners)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(27, duplicate.DistanceFrom.Millimeters);
      Assert.Equal(77, duplicate.TopStiffenerWidth.Millimeters);
      Assert.Equal(14, duplicate.TopStiffenerThickness.Millimeters);
      Assert.Equal(Length.Zero, duplicate.BottomStiffenerWidth);
      Assert.Equal(Length.Zero, duplicate.BottomStiffenerThickness);
      Assert.True(duplicate.isBothSides);
      Assert.True(duplicate.isNotch);

      // 3 make some changes to duplicate
      duplicate.DistanceFrom = new Length(28, unit);
      duplicate.TopStiffenerWidth = new Length(78, unit);
      duplicate.TopStiffenerThickness = new Length(15, unit);
      duplicate.isBothSides = false;

      // 4 check that duplicate has set changes
      Assert.Equal(28, duplicate.DistanceFrom.Millimeters);
      Assert.Equal(78, duplicate.TopStiffenerWidth.Millimeters);
      Assert.Equal(15, duplicate.TopStiffenerThickness.Millimeters);
      Assert.Equal(Length.Zero, duplicate.BottomStiffenerWidth);
      Assert.Equal(Length.Zero, duplicate.BottomStiffenerThickness);
      Assert.False(duplicate.isBothSides);
      Assert.True(duplicate.isNotch);

      // 5 check that original has not been changed
      Assert.Equal(27, original.DistanceFrom.Millimeters);
      Assert.Equal(77, original.TopStiffenerWidth.Millimeters);
      Assert.Equal(14, original.TopStiffenerThickness.Millimeters);
      Assert.Equal(Length.Zero, original.BottomStiffenerWidth);
      Assert.Equal(Length.Zero, original.BottomStiffenerThickness);
      Assert.True(original.isBothSides);
      Assert.True(original.isNotch);
    }
  }
}
