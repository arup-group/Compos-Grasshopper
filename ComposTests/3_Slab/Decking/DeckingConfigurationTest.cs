using ComposAPI.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH;

namespace ComposAPI.Slabs.Tests
{
  public static class DeckingConfigurationMother
  {
    public static DeckingConfiguration CreateDeckingConfiguration()
    {
      AngleUnit angleUnit = AngleUnit.Radian;
      return new DeckingConfiguration(new Angle(Math.PI / 2, angleUnit), true, true);
    }
  }

  [Collection("ComposAPI Fixture collection")]
  public class DeckingConfigurationTest
  {
    // 1 setup inputs
    [Fact]
    public void EmptyConstructorTest()
    {
      // 2 create object instance with constructor
      DeckingConfiguration configuration = new DeckingConfiguration();

      // 3 check that inputs are set in object's members
      Assert.Equal(90, configuration.Angle.Value);
      Assert.False(configuration.IsDiscontinous);
      Assert.False(configuration.IsWelded);
    }

    [Fact]
    public void DuplicateStdTest()
    {
      // 1 create with constructor and duplicate
      DeckingConfiguration original = DeckingConfigurationMother.CreateDeckingConfiguration();
      DeckingConfiguration duplicate = (DeckingConfiguration)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(0, false, false)]
    [InlineData(45, false, true)]
    [InlineData(60, true, false)]
    [InlineData(90, true, true)]
    public void ConstructorTest(double angle, bool isDiscontinous, bool isWelded)
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
