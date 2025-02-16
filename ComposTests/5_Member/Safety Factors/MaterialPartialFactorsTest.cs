using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helper;
using OasysGH;
using Xunit;

namespace ComposAPI.Members.Tests {
  [Collection("ComposAPI Fixture collection")]
  public partial class MaterialPartialFactorsTest {

    [Fact]
    public IMaterialFactors ConstructorTest() {
      // 1 setup input
      // empty constructor creates default (non-EC4) values

      // 2 create object instance with constructor
      IMaterialFactors partialFactors = new MaterialFactors();

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
    public void DuplicatePFTest() {
      // 1 create with constructor and duplicate
      var original = (MaterialFactors)ConstructorTest();
      var duplicate = (MaterialFactors)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void FromCoaStringTest() {
      // Arrange
      string coaString = "SAFETY_FACTOR_MATERIAL	MEMBER-1	1.10000	1.00000	1.00000	1.20000	1.30000	1.40000	1.50000	1.60000\n";
      List<string> parameters = CoaHelper.Split(coaString);

      // Act
      IMaterialFactors materialPartialFactors = MaterialFactors.FromCoaString(parameters);

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

    [Fact]
    public void ToCoaStringTest() {
      // Arrange
      var materialPartialFactors = new MaterialFactors {
        SteelBeam = 1.1,
        ConcreteCompression = 1.2,
        ConcreteShear = 1.3,
        MetalDecking = 1.4,
        ShearStud = 1.5,
        Reinforcement = 1.6
      };

      // Act
      string coaString = materialPartialFactors.ToCoaString("MEMBER-1");

      string expected_coaString = "SAFETY_FACTOR_MATERIAL	MEMBER-1	1.10000	1.00000	1.00000	1.20000	1.30000	1.40000	1.50000	1.60000\n";

      // Assert
      Assert.Equal(expected_coaString, coaString);
    }
  }
}
