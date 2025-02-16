using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helper;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Members.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class FrequencyLimitsTests {

    [Fact]
    public void ConstructorTest() {
      var frequencyLimits = new FrequencyLimits(4, 90, 20);

      Assert.Equal(new Frequency(4, FrequencyUnit.Hertz), frequencyLimits.MinimumRequired);
      Assert.Equal(new Ratio(90, RatioUnit.Percent), frequencyLimits.DeadLoadIncl);
      Assert.Equal(new Ratio(20, RatioUnit.Percent), frequencyLimits.LiveLoadIncl);
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      var original = new FrequencyLimits();
      var duplicate = (FrequencyLimits)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var frequencyLimits = new FrequencyLimits();

      Assert.Equal(Frequency.Zero, frequencyLimits.MinimumRequired);
      Assert.Equal(new Ratio(1, RatioUnit.DecimalFraction), frequencyLimits.DeadLoadIncl);
      Assert.Equal(new Ratio(0.1, RatioUnit.DecimalFraction), frequencyLimits.LiveLoadIncl);
    }

    [Theory]
    [InlineData("CRITERIA_FREQUENCY	MEMBER-1	CHECK_NATURAL_FREQUENCY	4.00000	1.00000	0.100000\n", 4, 100, 10)]
    public void FromCoaStringTest(string coaString, double frequency, double deadload, double liveload) {
      // Assemble
      var expectedfrequencyLimits = new FrequencyLimits(frequency, deadload, liveload);
      List<string> parameters = CoaHelper.Split(coaString);

      // Act
      IFrequencyLimits frequencyLimits = FrequencyLimits.FromCoaString(parameters);

      // Assert
      Duplicates.AreEqual(expectedfrequencyLimits, frequencyLimits);
    }

    [Theory]
    [InlineData("CRITERIA_FREQUENCY	MEMBER-1	CHECK_NATURAL_FREQUENCY	4.00000	1.00000	0.100000\n", 4, 100, 10)]
    public void ToCoaStringTest(string expected_CoaString, double frequency, double deadload, double liveload) {
      // Assemble
      var frequencyLimits = new FrequencyLimits(frequency, deadload, liveload);

      // Act
      string coaString = frequencyLimits.ToCoaString("MEMBER-1");

      // Assert
      Assert.Equal(expected_CoaString, coaString);
    }
  }
}
