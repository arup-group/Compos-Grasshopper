using Xunit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Parameters.Tests
{
  public class WebOpeningStiffenersTest
  {
    [Theory]
    [InlineData(1, 2, 3, 4, 5, false)]
    [InlineData(1, 2, 3, 4, 5, true)]
    public WebOpeningStiffeners TestConstructor1(double distanceFrom, double topStiffenerWidth, double topStiffenerThickness, double bottomStiffenerWidth, double bottomStiffenerThickness, bool isBothSides)
    {
      LengthUnit unit = LengthUnit.Millimeter;

      WebOpeningStiffeners webOpeningStiffeners = new WebOpeningStiffeners(new Length(distanceFrom, unit), new Length(topStiffenerWidth, unit), new Length(topStiffenerThickness, unit), new Length(bottomStiffenerWidth, unit), new Length(bottomStiffenerThickness, unit), isBothSides);

      Assert.Equal(distanceFrom, webOpeningStiffeners.DistanceFrom.Millimeters);
      Assert.Equal(topStiffenerWidth, webOpeningStiffeners.TopStiffenerWidth.Millimeters);
      Assert.Equal(topStiffenerThickness, webOpeningStiffeners.TopStiffenerThickness.Millimeters);
      Assert.Equal(bottomStiffenerWidth, webOpeningStiffeners.BottomStiffenerWidth.Millimeters);
      Assert.Equal(bottomStiffenerThickness, webOpeningStiffeners.BottomStiffenerThickness.Millimeters);
      Assert.Equal(isBothSides, webOpeningStiffeners.isBothSides);
      Assert.False(webOpeningStiffeners.isNotch);

      return webOpeningStiffeners;
    }
  }
}
