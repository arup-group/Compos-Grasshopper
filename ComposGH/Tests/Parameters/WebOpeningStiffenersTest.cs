using Xunit;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Parameters.Tests
{
  public partial class ComposWebOpeningTest
  {
    [Theory]
    [InlineData(50, 100, 10, 100, 10, false)]
    [InlineData(20, 120, 5, 80, 12, true)]
    public WebOpeningStiffeners TestConstructorStiffenersWebOpening(double startPos, double topWidth, 
      double topTHK, double bottomWidth, double bottomTHK, bool bothSides)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      WebOpeningStiffeners webOpeningStiffeners = new WebOpeningStiffeners(
        new Length(startPos, unit), new Length(topWidth, unit), 
        new Length(topTHK, unit), new Length(bottomWidth, unit), 
        new Length(bottomTHK, unit), bothSides);

      Assert.Equal(startPos, webOpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(topWidth, webOpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(topTHK, webOpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(bottomWidth, webOpeningStiffeners.BottomStiffenerWidth.Millimeters);
      Assert.Equal(bottomTHK, webOpeningStiffeners.BottomStiffenerThickness.Millimeters);
      Assert.Equal(bothSides, webOpeningStiffeners.isBothSides);
      Assert.False(webOpeningStiffeners.isNotch);

      return webOpeningStiffeners;
    }
    [Theory]
    [InlineData(50, 100, 10, false)]
    [InlineData(20, 120, 5, true)]
    public WebOpeningStiffeners TestConstructorStiffenersNotch(double distance, double topWidth,
      double topTHK, bool bothSides)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      WebOpeningStiffeners webOpeningStiffeners = new WebOpeningStiffeners(
        new Length(distance, unit), new Length(topWidth, unit),
        new Length(topTHK, unit), bothSides);

      Assert.Equal(distance, webOpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(topWidth, webOpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(topTHK, webOpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(Length.Zero, webOpeningStiffeners.BottomStiffenerWidth);
      Assert.Equal(Length.Zero, webOpeningStiffeners.BottomStiffenerThickness);
      Assert.Equal(bothSides, webOpeningStiffeners.isBothSides);
      Assert.True(webOpeningStiffeners.isNotch);

      return webOpeningStiffeners;
    }
    [Fact]
    public void TestStiffenerDuplicate()
    {
      LengthUnit unit = LengthUnit.Millimeter;

      WebOpeningStiffeners original = TestConstructorStiffenersWebOpening(25, 75, 12, 125, 15, false);
      WebOpeningStiffeners duplicate = original.Duplicate();

      Assert.Equal(25, duplicate.DistanceFrom.Millimeters);
      Assert.Equal(75, duplicate.TopStiffenerWidth.Millimeters);
      Assert.Equal(12, duplicate.TopStiffenerThickness.Millimeters);
      Assert.Equal(125, duplicate.BottomStiffenerWidth.Millimeters);
      Assert.Equal(15, duplicate.BottomStiffenerThickness.Millimeters);
      Assert.False(duplicate.isBothSides);
      Assert.False(duplicate.isNotch);

      duplicate.DistanceFrom = new Length(26, unit);
      duplicate.TopStiffenerWidth = new Length(76, unit);
      duplicate.TopStiffenerThickness = new Length(13, unit);
      duplicate.BottomStiffenerWidth = new Length(126, unit);
      duplicate.BottomStiffenerThickness = new Length(16, unit);
      duplicate.isBothSides = true;

      Assert.Equal(25, original.DistanceFrom.Millimeters);
      Assert.Equal(75, original.TopStiffenerWidth.Millimeters);
      Assert.Equal(12, original.TopStiffenerThickness.Millimeters);
      Assert.Equal(125, original.BottomStiffenerWidth.Millimeters);
      Assert.Equal(15, original.BottomStiffenerThickness.Millimeters);
      Assert.False(original.isBothSides);
      Assert.False(original.isNotch);

      Assert.Equal(26, duplicate.DistanceFrom.Millimeters);
      Assert.Equal(76, duplicate.TopStiffenerWidth.Millimeters);
      Assert.Equal(13, duplicate.TopStiffenerThickness.Millimeters);
      Assert.Equal(126, duplicate.BottomStiffenerWidth.Millimeters);
      Assert.Equal(16, duplicate.BottomStiffenerThickness.Millimeters);
      Assert.True(duplicate.isBothSides);
      Assert.False(duplicate.isNotch);

      original = TestConstructorStiffenersNotch(27, 77, 14, true);
      duplicate = original.Duplicate();
      duplicate.DistanceFrom = new Length(28, unit);
      duplicate.TopStiffenerWidth = new Length(78, unit);
      duplicate.TopStiffenerThickness = new Length(15, unit);
      duplicate.isBothSides = false;

      Assert.Equal(27, original.DistanceFrom.Millimeters);
      Assert.Equal(77, original.TopStiffenerWidth.Millimeters);
      Assert.Equal(14, original.TopStiffenerThickness.Millimeters);
      Assert.Equal(Length.Zero, original.BottomStiffenerWidth);
      Assert.Equal(Length.Zero, original.BottomStiffenerThickness);
      Assert.True(original.isBothSides);
      Assert.True(original.isNotch);

      Assert.Equal(28, duplicate.DistanceFrom.Millimeters);
      Assert.Equal(78, duplicate.TopStiffenerWidth.Millimeters);
      Assert.Equal(15, duplicate.TopStiffenerThickness.Millimeters);
      Assert.Equal(Length.Zero, duplicate.BottomStiffenerWidth);
      Assert.Equal(Length.Zero, duplicate.BottomStiffenerThickness);
      Assert.False(duplicate.isBothSides);
      Assert.True(duplicate.isNotch);
    }
  }
}
