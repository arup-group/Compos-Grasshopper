using ComposAPI.Helpers;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Members.Tests
{
  public partial class MaterialPartialFactorsTest
  {
    [Fact]
    public IMaterialPartialFactors ConstructorTest()
    {
      // 1 setup input
      // empty constructor creates default (non-EC4) values

      // 2 create object instance with constructor
      IMaterialPartialFactors partialFactors = new MaterialPartialFactors();

      // 3 check that inputs are set in object's members
      Assert.Equal(1.0, partialFactors.SteelBeam);
      Assert.Equal(1.5, partialFactors.ConcreteCompression);
      Assert.Equal(1.25, partialFactors.ConcreteShear);
      Assert.Equal(1.0, partialFactors.MetalDecking);
      Assert.Equal(1.25, partialFactors.ShearStud);
      Assert.Equal(1.15, partialFactors.Reinforcement);

      // (optionally return object for other tests)
      return partialFactors;
    }

    [Fact]
    public void ToCoaStringTest()
    {
      // Arrange
      MaterialPartialFactors materialPartialFactors = new MaterialPartialFactors();
      materialPartialFactors.SteelBeam = 1.1;
      materialPartialFactors.ConcreteCompression = 1.2;
      materialPartialFactors.ConcreteShear = 1.3;
      materialPartialFactors.MetalDecking = 1.4;
      materialPartialFactors.ShearStud = 1.5;
      materialPartialFactors.Reinforcement = 1.6;
      
      // Act
      string coaString = materialPartialFactors.ToCoaString("MEMBER-1");

      string expected_coaString = "SAFETY_FACTOR_MATERIAL	MEMBER-1	1.10000	1.00000	1.00000	1.20000	1.30000	1.40000	1.50000	1.60000\n";

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void FromCoaStringTest()
    {
      // Arrange
      string coaString = "SAFETY_FACTOR_MATERIAL	MEMBER-1	1.10000	1.00000	1.00000	1.20000	1.30000	1.40000	1.50000	1.60000\n";
      List<string> parameters = CoaHelper.Split(coaString);

      // Act
      IMaterialPartialFactors materialPartialFactors = MaterialPartialFactors.FromCoaString(parameters);

      double expected_steelBeam = 1.1;
      double expected_concreteCompression = 1.2;
      double expected_concreteShear = 1.3;
      double expected_metalDecking = 1.4;
      double expected_shearStud = 1.5;
      double expected_reinforcement = 1.6;

      // Assert
      Assert.Equal(expected_steelBeam, materialPartialFactors.SteelBeam);
      Assert.Equal(expected_concreteCompression, materialPartialFactors.ConcreteCompression);
      Assert.Equal(expected_concreteShear, materialPartialFactors.ConcreteShear);
      Assert.Equal(expected_metalDecking, materialPartialFactors.MetalDecking);
      Assert.Equal(expected_shearStud, materialPartialFactors.ShearStud);
      Assert.Equal(expected_reinforcement, materialPartialFactors.Reinforcement);
    }
  }
}