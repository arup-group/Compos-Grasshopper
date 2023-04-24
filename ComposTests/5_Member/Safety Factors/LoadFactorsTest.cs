using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGHTests.Helpers;
using OasysGH;
using Xunit;

namespace ComposAPI.Members.Tests {
  [Collection("ComposAPI Fixture collection")]
  public partial class LoadFactorsTest {

    [Fact]
    public LoadFactors ConstructorTest() {
      // 1 setup input
      // empty constructor creates default (non-EC4) values

      // 2 create object instance with constructor
      var loadFactors = new LoadFactors();

      // 3 check that inputs are set in object's members
      Assert.Equal(1.4, loadFactors.ConstantDead);
      Assert.Equal(1.4, loadFactors.ConstantLive);
      Assert.Equal(1.6, loadFactors.FinalDead);
      Assert.Equal(1.6, loadFactors.FinalLive);

      // (optionally return object for other tests)
      return loadFactors;
    }

    [Fact]
    public void DuplicateLFTest() {
      // 1 create with constructor and duplicate
      LoadFactors original = ConstructorTest();
      var duplicate = (LoadFactors)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Theory]
    [InlineData("SAFETY_FACTOR_LOAD	MEMBER-1	1.10000	1.20000	1.30000	1.40000\n", 1.1, 1.2, 1.3, 1.4)]
    public void FromCoaStringTest(string coaString, double expected_constantDead, double expected_finalDead, double expected_constantLive, double expected_finalLive) {
      var units = ComposUnits.GetStandardUnits();

      List<string> parameters = CoaHelper.Split(coaString);
      ILoadFactors loadFactors = LoadFactors.FromCoaString(parameters);

      Assert.Equal(expected_constantDead, loadFactors.ConstantDead);
      Assert.Equal(expected_finalDead, loadFactors.FinalDead);
      Assert.Equal(expected_constantLive, loadFactors.ConstantLive);
      Assert.Equal(expected_finalLive, loadFactors.FinalLive);
    }

    [Theory]
    [InlineData(1.1, 1.2, 1.3, 1.4, "SAFETY_FACTOR_LOAD	MEMBER-1	1.10000	1.20000	1.30000	1.40000\n")]
    public void ToCoaStringTest(double constantDead, double finalDead, double constantLive, double finalLive, string expected_coaString) {
      var loadFactors = new LoadFactors {
        ConstantDead = constantDead,
        FinalDead = finalDead,
        ConstantLive = constantLive,
        FinalLive = finalLive
      };
      string coaString = loadFactors.ToCoaString("MEMBER-1");

      Assert.Equal(expected_coaString, coaString);
    }
  }
}
