using ComposGH.Helpers;
using ComposGHTests.Helpers;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Members.Tests {
  [Collection("ComposAPI Fixture collection")]
  public class DeflectionLimitTests {

    [Fact]
    public void AbsoluteConstructorTest() {
      var deflectionLimit = new DeflectionLimit(0.3, LengthUnit.Meter);

      Assert.Equal(new Length(0.3, LengthUnit.Meter), deflectionLimit.AbsoluteDeflection);
      Assert.Equal(Ratio.Zero, deflectionLimit.SpanOverDeflectionRatio);
    }

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      var original = new DeflectionLimit();
      var duplicate = (DeflectionLimit)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void EmptyConstructorTest() {
      var deflectionLimit = new DeflectionLimit();

      Assert.Equal(Length.Zero, deflectionLimit.AbsoluteDeflection);
      Assert.Equal(Ratio.Zero, deflectionLimit.SpanOverDeflectionRatio);
    }

    [Theory]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	CONSTRUCTION_DEAD_LOAD	ABSOLUTE	30.0000\n", DeflectionLimitLoadType.ConstructionDeadLoad, 30)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	ADDITIONAL_DEAD_LOAD	ABSOLUTE	10.0000\n", DeflectionLimitLoadType.AdditionalDeadLoad, 10)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	FINAL_LIVE_LOAD	ABSOLUTE	20.0000\n", DeflectionLimitLoadType.FinalLiveLoad, 20)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	TOTAL	ABSOLUTE	34.0000\n", DeflectionLimitLoadType.Total, 34)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	POST_CONSTRUCTION	ABSOLUTE	32.0000\n", DeflectionLimitLoadType.PostConstruction, 32)]
    public void FromCoaStringAbsTest(string coaString, DeflectionLimitLoadType expectedType, double expectedAbsolute) {
      // Assemble
      var units = ComposUnits.GetStandardUnits();
      units.Displacement = LengthUnit.Millimeter;
      var expectedLimit = new DeflectionLimit(expectedAbsolute, LengthUnit.Millimeter);

      // Act
      IDeflectionLimit deflectionLimit = DeflectionLimit.FromCoaString(coaString, "MEMBER-1", expectedType, units);

      // Assert
      Duplicates.AreEqual(expectedLimit, deflectionLimit);
    }

    [Theory]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	CONSTRUCTION_DEAD_LOAD	SPAN/DEF_RATIO	360.000\n", DeflectionLimitLoadType.ConstructionDeadLoad, 360)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	ADDITIONAL_DEAD_LOAD	SPAN/DEF_RATIO	340.000\n", DeflectionLimitLoadType.AdditionalDeadLoad, 340)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	FINAL_LIVE_LOAD	SPAN/DEF_RATIO	300.000\n", DeflectionLimitLoadType.FinalLiveLoad, 300)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	TOTAL	SPAN/DEF_RATIO	200.000\n", DeflectionLimitLoadType.Total, 200)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	POST_CONSTRUCTION	SPAN/DEF_RATIO	500.000\n", DeflectionLimitLoadType.PostConstruction, 500)]
    public void FromCoaStringRatioTest(string coaString, DeflectionLimitLoadType expectedType, double expectedRatio) {
      // Assemble
      var units = ComposUnits.GetStandardUnits();
      var expectedLimit = new DeflectionLimit(expectedRatio);

      // Act
      IDeflectionLimit deflectionLimit = DeflectionLimit.FromCoaString(coaString, "MEMBER-1", expectedType, units);

      // Assert
      Duplicates.AreEqual(expectedLimit, deflectionLimit);
    }

    [Fact]
    public void SpanDeflectionConstructorTest() {
      var deflectionLimit = new DeflectionLimit(350);

      Assert.Equal(new Ratio(350, RatioUnit.DecimalFraction), deflectionLimit.SpanOverDeflectionRatio);
      Assert.Equal(Length.Zero, deflectionLimit.AbsoluteDeflection);
    }

    [Theory]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	CONSTRUCTION_DEAD_LOAD	ABSOLUTE	30.0000\n", DeflectionLimitLoadType.ConstructionDeadLoad, 30)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	ADDITIONAL_DEAD_LOAD	ABSOLUTE	10.0000\n", DeflectionLimitLoadType.AdditionalDeadLoad, 10)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	FINAL_LIVE_LOAD	ABSOLUTE	20.0000\n", DeflectionLimitLoadType.FinalLiveLoad, 20)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	TOTAL	ABSOLUTE	34.0000\n", DeflectionLimitLoadType.Total, 34)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	POST_CONSTRUCTION	ABSOLUTE	32.0000\n", DeflectionLimitLoadType.PostConstruction, 32)]
    public void ToCoaStringAbsTest(string expected_CoaString, DeflectionLimitLoadType type, double absolute) {
      // Assemble
      var units = ComposUnits.GetStandardUnits();
      units.Displacement = LengthUnit.Millimeter;
      var deflectionLimit = new DeflectionLimit(absolute, LengthUnit.Millimeter);

      // Act
      string coaString = deflectionLimit.ToCoaString("MEMBER-1", type, units);

      // Assert
      Assert.Equal(expected_CoaString, coaString);
    }

    [Theory]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	CONSTRUCTION_DEAD_LOAD	SPAN/DEF_RATIO	360.000\n", DeflectionLimitLoadType.ConstructionDeadLoad, 360)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	ADDITIONAL_DEAD_LOAD	SPAN/DEF_RATIO	340.000\n", DeflectionLimitLoadType.AdditionalDeadLoad, 340)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	FINAL_LIVE_LOAD	SPAN/DEF_RATIO	300.000\n", DeflectionLimitLoadType.FinalLiveLoad, 300)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	TOTAL	SPAN/DEF_RATIO	200.000\n", DeflectionLimitLoadType.Total, 200)]
    [InlineData("CRITERIA_DEF_LIMIT	MEMBER-1	POST_CONSTRUCTION	SPAN/DEF_RATIO	500.000\n", DeflectionLimitLoadType.PostConstruction, 500)]
    public void ToCoaStringRatioTest(string expected_CoaString, DeflectionLimitLoadType type, double ratio) {
      // Assemble
      var units = ComposUnits.GetStandardUnits();
      var deflectionLimit = new DeflectionLimit(ratio);

      // Act
      string coaString = deflectionLimit.ToCoaString("MEMBER-1", type, units);

      // Assert
      Assert.Equal(expected_CoaString, coaString);
    }
  }
}
