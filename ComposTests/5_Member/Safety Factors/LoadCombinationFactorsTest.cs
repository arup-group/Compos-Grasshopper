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
      Assert.Equal(1.35, loadFactors.Constantgamma_G);
      Assert.Equal(1.5, loadFactors.Constantgamma_Q);
      Assert.Equal(1.0, loadFactors.FinalXi);
      Assert.Equal(1.0, loadFactors.FinalPsi);
      Assert.Equal(1.35, loadFactors.Finalgamma_G);
      Assert.Equal(1.5, loadFactors.Finalgamma_Q);

      // (optionally return object for other tests)
      return loadFactors;
    }
  }
}
