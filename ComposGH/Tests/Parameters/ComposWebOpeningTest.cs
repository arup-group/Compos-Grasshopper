using Xunit;
using UnitsNet;
using UnitsNet.Units;
using static ComposGH.Parameters.ComposWebOpening;

namespace ComposGH.Parameters.Tests
{
  public partial class ComposWebOpeningTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(400, 300, 6000, 150)]
    [InlineData(250.123, 423.0013, 1240.12, 214)]
    public void TestConstructorRectangularWebOpening(double width, double height,
      double positionCentroidFromStart, double positionCentroidFromTop)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      ComposWebOpening webOpening = new ComposWebOpening(
        new Length(width, unit), new Length(height, unit),
        new Length(positionCentroidFromStart, unit), new Length(positionCentroidFromTop, unit));

      // 3 check that inputs are set in object's members
      Assert.Equal(OpeningType.Rectangular, webOpening.WebOpeningType);
      Assert.Equal(width, webOpening.Width.Millimeters);
      Assert.Equal(height, webOpening.Height.Millimeters);
      Assert.Equal(positionCentroidFromStart, webOpening.CentroidPosFromStart.Millimeters);
      Assert.Equal(positionCentroidFromTop, webOpening.CentroidPosFromTop.Millimeters);
      Assert.Equal(Length.Zero, webOpening.Diameter);
      Assert.Null(webOpening.OpeningStiffeners);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(200, 400, 9000)]
    [InlineData(123.456, 0.0123, 500.111)]
    public void TestConstructorCircularWebOpening(double diameter,
      double positionCentroidFromStart, double positionCentroidFromTop)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      ComposWebOpening webOpening = new ComposWebOpening(
        new Length(diameter, unit),
        new Length(positionCentroidFromStart, unit), new Length(positionCentroidFromTop, unit));

      // 3 check that inputs are set in object's members
      Assert.Equal(OpeningType.Circular, webOpening.WebOpeningType);
      Assert.Equal(diameter, webOpening.Diameter.Millimeters);
      Assert.Equal(positionCentroidFromStart, webOpening.CentroidPosFromStart.Millimeters);
      Assert.Equal(positionCentroidFromTop, webOpening.CentroidPosFromTop.Millimeters);
      Assert.Equal(Length.Zero, webOpening.Width);
      Assert.Equal(Length.Zero, webOpening.Height);
      Assert.Null(webOpening.OpeningStiffeners);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(200, 400, NotchPosition.Start)]
    [InlineData(123.456, 0.0123, NotchPosition.End)]
    public void TestConstructorNotchWebOpening(double width,
      double height, NotchPosition position)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      ComposWebOpening webOpening = new ComposWebOpening(
        new Length(width, unit), new Length(height, unit), position);

      // 3 check that inputs are set in object's members
      if (position == NotchPosition.Start)
        Assert.Equal(OpeningType.Start_notch, webOpening.WebOpeningType);
      if (position == NotchPosition.End)
        Assert.Equal(OpeningType.End_notch, webOpening.WebOpeningType);
      Assert.Equal(width, webOpening.Width.Millimeters);
      Assert.Equal(height, webOpening.Height.Millimeters);
      Assert.Equal(Length.Zero, webOpening.CentroidPosFromStart);
      Assert.Equal(Length.Zero, webOpening.CentroidPosFromTop);
      Assert.Equal(Length.Zero, webOpening.Diameter);
      Assert.Null(webOpening.OpeningStiffeners);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(900, 450, 2000, 350)]
    [InlineData(87.98, 42.34, 90000, 19.2)]
    public ComposWebOpening TestConstructorRectangularWebOpeningWithStiffener(double width, double height,
      double positionCentroidFromStart, double positionCentroidFromTop)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1b create additional input from custom class object
      WebOpeningStiffeners stiffener = TestConstructorStiffenersWebOpening(25, 75, 12, 125, 15, false);

      // 2 create object instance with constructor
      ComposWebOpening webOpening = new ComposWebOpening(
        new Length(width, unit), new Length(height, unit),
        new Length(positionCentroidFromStart, unit), new Length(positionCentroidFromTop, unit), stiffener);

      // 3 check that inputs are set in object's members
      Assert.Equal(OpeningType.Rectangular, webOpening.WebOpeningType);
      Assert.Equal(width, webOpening.Width.Millimeters);
      Assert.Equal(height, webOpening.Height.Millimeters);
      Assert.Equal(positionCentroidFromStart, webOpening.CentroidPosFromStart.Millimeters);
      Assert.Equal(positionCentroidFromTop, webOpening.CentroidPosFromTop.Millimeters);
      Assert.Equal(Length.Zero, webOpening.Diameter);
      Assert.NotNull(webOpening.OpeningStiffeners);

      // (optionally return object for other tests)
      return webOpening;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(200, 400, 9000)]
    [InlineData(15, 4.0123, 59.211)]
    public ComposWebOpening TestConstructorCircularWebOpeningWithStiffener(double diameter,
      double positionCentroidFromStart, double positionCentroidFromTop)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1b create additional input from custom class object
      WebOpeningStiffeners stiffener = TestConstructorStiffenersWebOpening(80, 17, 5, 98, 12, true);

      // 2 create object instance with constructor
      ComposWebOpening webOpening = new ComposWebOpening(
        new Length(diameter, unit),
        new Length(positionCentroidFromStart, unit), new Length(positionCentroidFromTop, unit), stiffener);

      // 3 check that inputs are set in object's members
      Assert.Equal(OpeningType.Circular, webOpening.WebOpeningType);
      Assert.Equal(diameter, webOpening.Diameter.Millimeters);
      Assert.Equal(positionCentroidFromStart, webOpening.CentroidPosFromStart.Millimeters);
      Assert.Equal(positionCentroidFromTop, webOpening.CentroidPosFromTop.Millimeters);
      Assert.Equal(Length.Zero, webOpening.Width);
      Assert.Equal(Length.Zero, webOpening.Height);
      Assert.NotNull(webOpening.OpeningStiffeners);

      // (optionally return object for other tests)
      return webOpening;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(600, 70, NotchPosition.Start)]
    [InlineData(14.3, 78.123, NotchPosition.End)]
    public ComposWebOpening TestConstructorNotchWebOpeningWithStiffener(double width,
      double height, NotchPosition position)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1b create additional input from custom class object
      WebOpeningStiffeners stiffener = TestConstructorStiffenersNotch(15, 90, 7, false);

      // 2 create object instance with constructor
      ComposWebOpening webOpening = new ComposWebOpening(
        new Length(width, unit), new Length(height, unit),
        position, stiffener);

      // 3 check that inputs are set in object's members
      if (position == NotchPosition.Start)
        Assert.Equal(OpeningType.Start_notch, webOpening.WebOpeningType);
      if (position == NotchPosition.End)
        Assert.Equal(OpeningType.End_notch, webOpening.WebOpeningType);
      Assert.Equal(width, webOpening.Width.Millimeters);
      Assert.Equal(height, webOpening.Height.Millimeters);
      Assert.Equal(Length.Zero, webOpening.CentroidPosFromStart);
      Assert.Equal(Length.Zero, webOpening.CentroidPosFromTop);
      Assert.Equal(Length.Zero, webOpening.Diameter);
      Assert.NotNull(webOpening.OpeningStiffeners);

      // (optionally return object for other tests)
      return webOpening;
    }

    // 1 setup inputs
    [Fact]
    public void TestDuplicate()
    {
      // 1 create with constructor and duplicate
      ComposWebOpening original = TestConstructorRectangularWebOpeningWithStiffener(400, 300, 6000, 70);
      ComposWebOpening duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(OpeningType.Rectangular, duplicate.WebOpeningType);
      Assert.Equal(400, duplicate.Width.Millimeters);
      Assert.Equal(300, duplicate.Height.Millimeters);
      Assert.Equal(6000, duplicate.CentroidPosFromStart.Millimeters);
      Assert.Equal(70, duplicate.CentroidPosFromTop.Millimeters);
      Assert.Equal(Length.Zero, duplicate.Diameter);
      Assert.NotNull(duplicate.OpeningStiffeners);
      Assert.Equal(25, duplicate.OpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(75, duplicate.OpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(12, duplicate.OpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(125, duplicate.OpeningStiffeners.BottomStiffenerWidth.Millimeters);
      Assert.Equal(15, duplicate.OpeningStiffeners.BottomStiffenerThickness.Millimeters);
      Assert.False(duplicate.OpeningStiffeners.isBothSides);
      Assert.False(duplicate.OpeningStiffeners.isNotch);

      // 3 make some changes to duplicate
      duplicate.WebOpeningType = OpeningType.Circular;
      duplicate.Diameter = new Length(150, LengthUnit.Millimeter);
      duplicate.CentroidPosFromStart = new Length(4500, LengthUnit.Millimeter);
      duplicate.CentroidPosFromTop = new Length(250, LengthUnit.Millimeter);
      duplicate.OpeningStiffeners = TestConstructorStiffenersWebOpening(15, 125, 10, 135, 7, true);

      // 4 check that duplicate has set changes
      Assert.Equal(OpeningType.Circular, duplicate.WebOpeningType);
      Assert.Equal(150, duplicate.Diameter.Millimeters);
      Assert.Equal(4500, duplicate.CentroidPosFromStart.Millimeters);
      Assert.Equal(250, duplicate.CentroidPosFromTop.Millimeters);
      Assert.Equal(Length.Zero, duplicate.Width);
      Assert.Equal(Length.Zero, duplicate.Height);
      Assert.NotNull(duplicate.OpeningStiffeners);
      Assert.Equal(15, duplicate.OpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(125, duplicate.OpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(10, duplicate.OpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(135, duplicate.OpeningStiffeners.BottomStiffenerWidth.Millimeters);
      Assert.Equal(7, duplicate.OpeningStiffeners.BottomStiffenerThickness.Millimeters);
      Assert.True(duplicate.OpeningStiffeners.isBothSides);
      Assert.False(duplicate.OpeningStiffeners.isNotch);

      // 5 check that original has not been changed
      Assert.Equal(OpeningType.Rectangular, original.WebOpeningType);
      Assert.Equal(400, original.Width.Millimeters);
      Assert.Equal(300, original.Height.Millimeters);
      Assert.Equal(6000, original.CentroidPosFromStart.Millimeters);
      Assert.Equal(70, original.CentroidPosFromTop.Millimeters);
      Assert.Equal(Length.Zero, original.Diameter);
      Assert.NotNull(original.OpeningStiffeners);
      Assert.Equal(25, original.OpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(75, original.OpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(12, original.OpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(125, original.OpeningStiffeners.BottomStiffenerWidth.Millimeters);
      Assert.Equal(15, original.OpeningStiffeners.BottomStiffenerThickness.Millimeters);
      Assert.False(original.OpeningStiffeners.isBothSides);
      Assert.False(original.OpeningStiffeners.isNotch);

      // 1 create with new constructor and duplicate
      original = TestConstructorCircularWebOpeningWithStiffener(300, 7000, 150);
      duplicate = original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(OpeningType.Circular, duplicate.WebOpeningType);
      Assert.Equal(300, duplicate.Diameter.Millimeters);
      Assert.Equal(Length.Zero, duplicate.Width);
      Assert.Equal(Length.Zero, duplicate.Height);
      Assert.Equal(7000, duplicate.CentroidPosFromStart.Millimeters);
      Assert.Equal(150, duplicate.CentroidPosFromTop.Millimeters);
      Assert.NotNull(duplicate.OpeningStiffeners);
      Assert.Equal(80, duplicate.OpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(17, duplicate.OpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(5, duplicate.OpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(98, duplicate.OpeningStiffeners.BottomStiffenerWidth.Millimeters);
      Assert.Equal(12, duplicate.OpeningStiffeners.BottomStiffenerThickness.Millimeters);
      Assert.True(duplicate.OpeningStiffeners.isBothSides);
      Assert.False(duplicate.OpeningStiffeners.isNotch);

      // 3 make some changes to duplicate
      duplicate.WebOpeningType = OpeningType.Start_notch;
      duplicate.Width = new Length(150, LengthUnit.Millimeter);
      duplicate.Height = new Length(250, LengthUnit.Millimeter);
      duplicate.OpeningStiffeners = TestConstructorStiffenersNotch(15, 125, 10, false);

      // 4 check that duplicate has set changes
      Assert.Equal(OpeningType.Start_notch, duplicate.WebOpeningType);
      Assert.Equal(150, duplicate.Width.Millimeters);
      Assert.Equal(250, duplicate.Height.Millimeters);
      Assert.Equal(Length.Zero, duplicate.CentroidPosFromStart);
      Assert.Equal(Length.Zero, duplicate.CentroidPosFromTop);
      Assert.Equal(Length.Zero, duplicate.Diameter);
      Assert.NotNull(duplicate.OpeningStiffeners);
      Assert.Equal(15, duplicate.OpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(125, duplicate.OpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(10, duplicate.OpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(Length.Zero, duplicate.OpeningStiffeners.BottomStiffenerWidth);
      Assert.Equal(Length.Zero, duplicate.OpeningStiffeners.BottomStiffenerThickness);
      Assert.False(duplicate.OpeningStiffeners.isBothSides);
      Assert.True(duplicate.OpeningStiffeners.isNotch);

      // 5 check that original has not been changed
      Assert.Equal(OpeningType.Circular, original.WebOpeningType);
      Assert.Equal(300, original.Diameter.Millimeters);
      Assert.Equal(Length.Zero, original.Width);
      Assert.Equal(Length.Zero, original.Height);
      Assert.Equal(7000, original.CentroidPosFromStart.Millimeters);
      Assert.Equal(150, original.CentroidPosFromTop.Millimeters);
      Assert.NotNull(original.OpeningStiffeners);
      Assert.Equal(80, original.OpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(17, original.OpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(5, original.OpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(98, original.OpeningStiffeners.BottomStiffenerWidth.Millimeters);
      Assert.Equal(12, original.OpeningStiffeners.BottomStiffenerThickness.Millimeters);
      Assert.True(original.OpeningStiffeners.isBothSides);
      Assert.False(original.OpeningStiffeners.isNotch);
    }
  }
}
