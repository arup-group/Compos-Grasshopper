using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Tests
{
  public static class DeckingConfigurationMother
  {
    public static DeckingConfiguration CreateDeckingConfiguration()
    {
      AngleUnit angleUnit = AngleUnit.Radian;
      return new DeckingConfiguration(new Angle(Math.PI / 2, angleUnit), true, true);
    }
  }

  public class DeckingConfigurationTest
  {
    // 1 setup inputs
    [Fact]
    public void TestEmptyConstructor()
    {
      // 2 create object instance with constructor
      DeckingConfiguration configuration = new DeckingConfiguration();

      // 3 check that inputs are set in object's members
      Assert.Equal(90, configuration.Angle.Value);
      Assert.False(configuration.IsDiscontinous);
      Assert.False(configuration.IsWelded);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(0, false, false)]
    [InlineData(45, false, true)]
    [InlineData(60, true, false)]
    [InlineData(90, true, true)]
    public void TestConstructor(double angle, bool isDiscontinous, bool isWelded)
    {
      // 2 create object instance with constructor
      DeckingConfiguration configuration = new DeckingConfiguration(new Angle(angle, ComposUnits.GetStandardUnits().Angle), isDiscontinous, isWelded);

      // 3 check that inputs are set in object's members
      Assert.Equal(angle, configuration.Angle.Value);
      Assert.Equal(isDiscontinous, configuration.IsDiscontinous);
      Assert.Equal(isWelded, configuration.IsWelded);
    }
  }
}
