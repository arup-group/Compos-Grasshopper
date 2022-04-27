using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using UnitsNet;
using UnitsNet.Units;

namespace ComposGH.Parameters.Tests
{
  public class SteelConcreteModularRatioTest
  {
    [Theory]
    publicSteelConcreteModularRatio TestEmptyConstructor()
    {
      // 2 create object instance with constructor
      SteelConcreteModularRatio steelConcreteModularRatio = new SteelConcreteModularRatio();

      // 3 check that inputs are set in object's members
      Assert.Equal(shortTerm, steelConcreteModularRatio.ShortTerm);
      Assert.Equal(longTerm, steelConcreteModularRatio.LongTerm);
      Assert.Equal(vibration, steelConcreteModularRatio.Vibration);
      Assert.Equal(0, steelConcreteModularRatio.Shrinkage);

      // (optionally return object for other tests)
      return steelConcreteModularRatio;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(6, 18, 5.39)]
    public SteelConcreteModularRatio TestConstructor1(double shortTerm, double longTerm, double vibration)
    {
      // 2 create object instance with constructor
      SteelConcreteModularRatio steelConcreteModularRatio = new SteelConcreteModularRatio(shortTerm, longTerm, vibration);

      // 3 check that inputs are set in object's members
      Assert.Equal(shortTerm, steelConcreteModularRatio.ShortTerm);
      Assert.Equal(longTerm, steelConcreteModularRatio.LongTerm);
      Assert.Equal(vibration, steelConcreteModularRatio.Vibration);
      Assert.Equal(0, steelConcreteModularRatio.Shrinkage);

      // (optionally return object for other tests)
      return steelConcreteModularRatio;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(9.87, 28.72, 9.55, 27.55)]
    public SteelConcreteModularRatio TestConstructor2(double shortTerm, double longTerm, double vibration, double shrinkage)
    {
      // 2 create object instance with constructor
      SteelConcreteModularRatio steelConcreteModularRatio = new SteelConcreteModularRatio(shortTerm, longTerm, vibration, shrinkage);
      
      // 3 check that inputs are set in object's members
      Assert.Equal(shortTerm, steelConcreteModularRatio.ShortTerm);
      Assert.Equal(longTerm, steelConcreteModularRatio.LongTerm);
      Assert.Equal(vibration, steelConcreteModularRatio.Vibration);
      Assert.Equal(shrinkage, steelConcreteModularRatio.Shrinkage);

      // (optionally return object for other tests)
      return steelConcreteModularRatio;
    }


  }
}
