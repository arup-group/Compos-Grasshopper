using Xunit;
using UnitsNet;
using UnitsNet.Units;
using System.Collections.Generic;
using ComposAPI.Helpers;

namespace ComposAPI.Tests
{
  public partial class WebOpeningTest
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
      WebOpening webOpening = new WebOpening(
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
      WebOpening webOpening = new WebOpening(
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
      WebOpening webOpening = new WebOpening(
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
    public WebOpening TestConstructorRectangularWebOpeningWithStiffener(double width, double height,
      double positionCentroidFromStart, double positionCentroidFromTop)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1b create additional input from custom class object
      WebOpeningStiffeners stiffener = TestConstructorStiffenersWebOpening(25, 75, 12, 125, 15, false);

      // 2 create object instance with constructor
      WebOpening webOpening = new WebOpening(
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
    public WebOpening TestConstructorCircularWebOpeningWithStiffener(double diameter,
      double positionCentroidFromStart, double positionCentroidFromTop)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1b create additional input from custom class object
      WebOpeningStiffeners stiffener = TestConstructorStiffenersWebOpening(80, 17, 5, 98, 12, true);

      // 2 create object instance with constructor
      WebOpening webOpening = new WebOpening(
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
    public WebOpening TestConstructorNotchWebOpeningWithStiffener(double width,
      double height, NotchPosition position)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      // 1b create additional input from custom class object
      WebOpeningStiffeners stiffener = TestConstructorStiffenersNotch(15, 90, 7, false);

      // 2 create object instance with constructor
      WebOpening webOpening = new WebOpening(
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

    [Fact]
    public void DuplicateTest()
    {
      // 1 create with constructor and duplicate
      WebOpening original = TestConstructorRectangularWebOpeningWithStiffener(400, 300, 6000, 70);
      WebOpening duplicate = original.Duplicate() as WebOpening;

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
    }

    [Fact]
    public void DuplicateTest2()
    {
      // 1 create with constructor and duplicate
      WebOpening original = TestConstructorCircularWebOpeningWithStiffener(300, 7000, 150);
      WebOpening duplicate = original.Duplicate() as WebOpening;

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

    [Theory]
    [InlineData(400, 300, 7.5, 350, OpeningType.Rectangular, "WEB_OPEN_DIMENSION	MEMBER-1	RECTANGULAR	400.000	300.000	7.50000	350.000	STIFFENER_NO\n")]
    [InlineData(400, 400, 3.5, 190, OpeningType.Circular, "WEB_OPEN_DIMENSION	MEMBER-1	CIRCULAR	400.000	400.000	3.50000	190.000	STIFFENER_NO\n")]
    [InlineData(400, 300, 0, 0, OpeningType.Start_notch, "WEB_OPEN_DIMENSION	MEMBER-1	LEFT_NOTCH	400.000	300.000	50.0000%	50.0000%	STIFFENER_NO\n")]
    public void ToCoaStringNoStiffener(double width, double height, double startPos, double posFromTop, OpeningType openingType, string expected_CoaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();
      WebOpening webOpening = new WebOpening();
      switch (openingType)
      {
        case OpeningType.Rectangular:
          webOpening = new WebOpening(new Length(width, units.Section), new Length(height, units.Section), new Length(startPos, units.Length), new Length(posFromTop, units.Section));
          break;
        case OpeningType.Circular:
          webOpening = new WebOpening(new Length(width, units.Section), new Length(startPos, units.Length), new Length(posFromTop, units.Section));
          break;
        case OpeningType.Start_notch:
          webOpening = new WebOpening(new Length(width, units.Section), new Length(height, units.Section), NotchPosition.Start);
          break;
        case OpeningType.End_notch:
          webOpening = new WebOpening(new Length(width, units.Section), new Length(height, units.Section), NotchPosition.End);
          break;
      }

      string coaString = webOpening.ToCoaString("MEMBER-1", units);
      Assert.Equal(expected_CoaString, coaString);
    }

    [Theory]
    [InlineData(400, 300, 7.5, 350, OpeningType.Rectangular, "WEB_OPEN_DIMENSION	MEMBER-1	RECTANGULAR	400.000	300.000	7.50000	350.000	STIFFENER_NO\n")]
    [InlineData(400, 400, 3.5, 190, OpeningType.Circular, "WEB_OPEN_DIMENSION	MEMBER-1	CIRCULAR	400.000	400.000	3.50000	190.000	STIFFENER_NO\n")]
    [InlineData(400, 300, 0, 0, OpeningType.Start_notch, "WEB_OPEN_DIMENSION	MEMBER-1	LEFT_NOTCH	400.000	300.000	50.0000%	50.0000%	STIFFENER_NO\n")]
    public void FromCoaStringNoStiffener(double expected_width, double expected_height, double expected_startPos, double expected_posFromTop, OpeningType expected_OpeningType, string coaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();
      List<string> parameters = CoaHelper.Split(coaString);
      IWebOpening webOpening = WebOpening.FromCoaString(parameters, units);

      Assert.Equal(expected_OpeningType, webOpening.WebOpeningType);

      switch (webOpening.WebOpeningType)
      {
        case OpeningType.Rectangular:
          Assert.Equal(expected_width, webOpening.Width.As(units.Section));
          Assert.Equal(expected_height, webOpening.Height.As(units.Section));
          Assert.Equal(expected_startPos, webOpening.CentroidPosFromStart.As(units.Length));
          Assert.Equal(expected_posFromTop, webOpening.CentroidPosFromTop.As(units.Section));
          break;

        case OpeningType.End_notch:
        case OpeningType.Start_notch:
          Assert.Equal(expected_width, webOpening.Width.As(units.Section));
          Assert.Equal(expected_height, webOpening.Height.As(units.Section));
          break;

        case OpeningType.Circular:
          Assert.Equal(expected_width, webOpening.Diameter.As(units.Section));
          Assert.Equal(expected_startPos, webOpening.CentroidPosFromStart.As(units.Length));
          Assert.Equal(expected_posFromTop, webOpening.CentroidPosFromTop.As(units.Section));
          break;
      }
    }

    [Theory]
    [InlineData(400, 300, 0, 0, OpeningType.End_notch, false, 50, 100, 10, 0, 0, "WEB_OPEN_DIMENSION	MEMBER-1	RIGHT_NOTCH	400.000	300.000	50.0000%	50.0000%	STIFFENER_YES	ONE_SIDE_STIFFENER	50.0000	100.000	10.0000	100.000	10.0000\n")]
    [InlineData(400, 300, 1.5, 250, OpeningType.Rectangular, true, 60, 100, 10, 50, 5, "WEB_OPEN_DIMENSION	MEMBER-1	RECTANGULAR	400.000	300.000	1.50000	250.000	STIFFENER_YES	BOTH_SIDE_STIFFENER	60.0000	100.000	10.0000	50.0000	5.00000\n")]
    [InlineData(400, 400, 9.5, 150, OpeningType.Circular, true, 10, 120, 12, 70, 7, "WEB_OPEN_DIMENSION	MEMBER-1	CIRCULAR	400.000	400.000	9.50000	150.000	STIFFENER_YES	BOTH_SIDE_STIFFENER	10.0000	120.000	12.0000	70.0000	7.00000\n")]
    public void ToCoaStringWithStiffener(double width, double height, double startPos, double posFromTop, OpeningType openingType, bool bothSides, double distFrom, double topWidth, double topThk, double bottomWidth, double bottomThk, string expected_CoaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();
      WebOpening webOpening = new WebOpening();
      WebOpeningStiffeners stiffeners = new WebOpeningStiffeners();
      switch (openingType)
      {
        case OpeningType.Rectangular:
          webOpening = new WebOpening(new Length(width, units.Section), new Length(height, units.Section), new Length(startPos, units.Length), new Length(posFromTop, units.Section));
          stiffeners = new WebOpeningStiffeners(new Length(distFrom, units.Section), new Length(topWidth, units.Section), new Length(topThk, units.Section), new Length(bottomWidth, units.Section), new Length(bottomThk, units.Section), bothSides);
          break;
        case OpeningType.Circular:
          webOpening = new WebOpening(new Length(width, units.Section), new Length(startPos, units.Length), new Length(posFromTop, units.Section));
          stiffeners = new WebOpeningStiffeners(new Length(distFrom, units.Section), new Length(topWidth, units.Section), new Length(topThk, units.Section), new Length(bottomWidth, units.Section), new Length(bottomThk, units.Section), bothSides);
          break;
        case OpeningType.Start_notch:
          webOpening = new WebOpening(new Length(width, units.Section), new Length(height, units.Section), NotchPosition.Start);
          stiffeners = new WebOpeningStiffeners(new Length(distFrom, units.Section), new Length(topWidth, units.Section), new Length(topThk, units.Section), bothSides);
          break;
        case OpeningType.End_notch:
          webOpening = new WebOpening(new Length(width, units.Section), new Length(height, units.Section), NotchPosition.End);
          stiffeners = new WebOpeningStiffeners(new Length(distFrom, units.Section), new Length(topWidth, units.Section), new Length(topThk, units.Section), bothSides);
          break;
      }
      webOpening.OpeningStiffeners = stiffeners;

      string coaString = webOpening.ToCoaString("MEMBER-1", units);
      Assert.Equal(expected_CoaString, coaString);
    }

    [Theory]
    [InlineData(400, 300, 0, 0, OpeningType.End_notch, false, 50, 100, 10, 0, 0, "WEB_OPEN_DIMENSION	MEMBER-1	RIGHT_NOTCH	400.000	300.000	50.0000%	50.0000%	STIFFENER_YES	ONE_SIDE_STIFFENER	50.0000	100.000	10.0000	100.000	10.0000\n")]
    [InlineData(400, 300, 1.5, 250, OpeningType.Rectangular, true, 60, 100, 10, 50, 5, "WEB_OPEN_DIMENSION	MEMBER-1	RECTANGULAR	400.000	300.000	1.50000	250.000	STIFFENER_YES	BOTH_SIDE_STIFFENER	60.0000	100.000	10.0000	50.0000	5.00000\n")]
    [InlineData(400, 400, 9.5, 150, OpeningType.Circular, true, 10, 120, 12, 70, 7, "WEB_OPEN_DIMENSION	MEMBER-1	CIRCULAR	400.000	400.000	9.50000	150.000	STIFFENER_YES	BOTH_SIDE_STIFFENER	10.0000	120.000	12.0000	70.0000	7.00000\n")]
    public void FromCoaStringWithStiffener(double expected_width, double expected_height, double expected_startPos, double expected_posFromTop, OpeningType expected_OpeningType, bool expected_bothSides, double expected_distFrom, double expected_topWidth, double expected_topThk, double expected_bottomWidth, double expected_bottomThk, string coaString)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();
      List<string> parameters = CoaHelper.Split(coaString);
      IWebOpening webOpening = WebOpening.FromCoaString(parameters, units);

      Assert.Equal(expected_OpeningType, webOpening.WebOpeningType);

      switch (webOpening.WebOpeningType)
      {
        case OpeningType.Rectangular:
          Assert.Equal(expected_width, webOpening.Width.As(units.Section));
          Assert.Equal(expected_height, webOpening.Height.As(units.Section));
          Assert.Equal(expected_startPos, webOpening.CentroidPosFromStart.As(units.Length));
          Assert.Equal(expected_posFromTop, webOpening.CentroidPosFromTop.As(units.Section));
          break;

        case OpeningType.End_notch:
        case OpeningType.Start_notch:
          Assert.Equal(expected_width, webOpening.Width.As(units.Section));
          Assert.Equal(expected_height, webOpening.Height.As(units.Section));
          break;

        case OpeningType.Circular:
          Assert.Equal(expected_width, webOpening.Diameter.As(units.Section));
          Assert.Equal(expected_startPos, webOpening.CentroidPosFromStart.As(units.Length));
          Assert.Equal(expected_posFromTop, webOpening.CentroidPosFromTop.As(units.Section));
          break;
      }

      Assert.NotNull(webOpening.OpeningStiffeners);

      Assert.Equal(expected_bothSides, webOpening.OpeningStiffeners.isBothSides);
      Assert.Equal(expected_distFrom, webOpening.OpeningStiffeners.DistanceFrom.As(units.Section));
      Assert.Equal(expected_topWidth, webOpening.OpeningStiffeners.TopStiffenerWidth.As(units.Section));
      Assert.Equal(expected_topThk, webOpening.OpeningStiffeners.TopStiffenerThickness.As(units.Section));

      if (!webOpening.OpeningStiffeners.isNotch)
      {
        Assert.Equal(expected_bottomWidth, webOpening.OpeningStiffeners.BottomStiffenerWidth.As(units.Section));
        Assert.Equal(expected_bottomThk, webOpening.OpeningStiffeners.BottomStiffenerThickness.As(units.Section));
      }
    }
  }
}
