using ComposAPI.Helpers;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Members.Tests
{
  public partial class LoadCombinationFactorsTest
  {
    [Fact]
    public LoadCombinationFactors ConstructorTest()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      LoadCombinationFactors loadFactors = new LoadCombinationFactors();

      // 3 check that inputs are set in object's members
      Assert.Equal(1.0, loadFactors.ConstantXi);
      Assert.Equal(1.0, loadFactors.ConstantPsi);
      //Assert.Equal(1.35, loadFactors.Constantgamma_G);
      //Assert.Equal(1.5, loadFactors.Constantgamma_Q);
      Assert.Equal(1.0, loadFactors.FinalXi);
      Assert.Equal(1.0, loadFactors.FinalPsi);
      //Assert.Equal(1.35, loadFactors.Finalgamma_G);
      //Assert.Equal(1.5, loadFactors.Finalgamma_Q);

      // (optionally return object for other tests)
      return loadFactors;
    }

    [Theory]
    [InlineData("EC4_LOAD_COMB_FACTORS	MEMBER-1	EC0_6_10\n", LoadCombination.Equation6_10, 1.0, 1.0, 1.0, 1.0)]
    [InlineData("EC4_LOAD_COMB_FACTORS	MEMBER-1	EC0_WORST_6_10A_10B\n", LoadCombination.Equation6_10a__6_10b, 1.0, 1.0, 1.0, 1.0)]
    [InlineData("EC4_LOAD_COMB_FACTORS	MEMBER-1	USER_DEFINED	1.10000	1.20000	1.30000	1.40000\n", LoadCombination.Custom, 1.1, 1.2, 1.3, 1.4)]
    public void FromCoaStringTest(string coaString, LoadCombination expected_LoadCombination, double expected_Constantxi, double expected_FinalXi, double expected_ConstantPsi, double expected_FinalPsi)
    {
      ComposUnits units = ComposUnits.GetStandardUnits();

      List<string> parameters = CoaHelper.Split(coaString);
      ILoadCombinationFactors combinationFactors = LoadCombinationFactors.FromCoaString(parameters);

      Assert.Equal(expected_LoadCombination, combinationFactors.LoadCombination);
      Assert.Equal(expected_Constantxi, combinationFactors.ConstantXi);
      Assert.Equal(expected_FinalXi, combinationFactors.FinalXi);
      Assert.Equal(expected_ConstantPsi, combinationFactors.ConstantPsi);
      Assert.Equal(expected_FinalPsi, combinationFactors.FinalPsi);
    }
  }
}
