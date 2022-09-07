using ComposAPI.Tests;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH;

namespace ComposAPI.Members.Tests
{
  public partial class EC4MaterialPartialFactorsTest
  {
    [Fact]
    public MaterialPartialFactors ConstructorTest()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      MaterialPartialFactors partialFactors = new MaterialPartialFactors();

      // 3 check that inputs are set in object's members
      Assert.Equal(1.0, partialFactors.gamma_M0);
      Assert.Equal(1.0, partialFactors.gamma_M1);
      Assert.Equal(1.25, partialFactors.gamma_M2);
      Assert.Equal(1.5, partialFactors.gamma_C);
      Assert.Equal(1.0, partialFactors.gamma_Deck);
      Assert.Equal(1.25, partialFactors.gamma_vs);
      Assert.Equal(1.15, partialFactors.gamma_S);

      // (optionally return object for other tests)
      return partialFactors;
    }
    [Fact]
    public void DuplicateMatEC4Test()
    {
      // 1 create with constructor and duplicate
      MaterialPartialFactors original = ConstructorTest();
      MaterialPartialFactors duplicate = (MaterialPartialFactors)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }
  }
}
