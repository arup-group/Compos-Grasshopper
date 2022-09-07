using ComposGHTests.Helpers;
using OasysGH;
using Xunit;

namespace ComposAPI.Members.Tests
{
  public partial class SafetyFactorsTest
  {
    [Fact]
    public SafetyFactors ConstructorTest()
    {
      // 1 setup input
      // empty constructor creates default (non-EC4) values

      // 2 create object instance with constructor
      SafetyFactors safetyFactors = new SafetyFactors();

      // 3 check that inputs are set in object's members
      Assert.Null(safetyFactors.MaterialFactors);
      Assert.Null(safetyFactors.LoadFactors);

      // (optionally return object for other tests)
      return safetyFactors;
    }
    [Fact]
    public void DuplicateSFTest()
    {
      // 1 create with constructor and duplicate
      SafetyFactors original = ConstructorTest();
      SafetyFactors duplicate = (SafetyFactors)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Theory]
    [InlineData("SAFETY_FACTOR_LOAD	MEMBER-1	2.00000	3.00000	4.00000	5.00000\nSAFETY_FACTOR_MATERIAL	MEMBER-1	6.00000	1.00000	1.00000	7.00000	8.00000	9.00000	10.0000	11.0000\n", true, 6, 7, 8, 9, 10, 11, 2, 3, 4, 5)]
    [InlineData("SAFETY_FACTOR_LOAD	MEMBER-1	1.10000	1.20000	1.30000	1.40000\n", false, 0, 0, 0, 0, 0, 0, 1.1, 1.2, 1.3, 1.4)]
    public void ToCoaStringTest(string expected_CoaString, bool materialFactorsSet, double steelBeam, double concreteCompression, double concreteShear, double metalDecking, double shearStud, double reinforcement,double constantDead, double finalDead, double constantLive, double finalLive)
    {
      // Assemble
      SafetyFactors safetyFactors = new SafetyFactors();

      if (materialFactorsSet)
      {
        MaterialFactors mat = new MaterialFactors();
        mat.SteelBeam = steelBeam;
        mat.ConcreteCompression = concreteCompression;
        mat.ConcreteShear = concreteShear;
        mat.MetalDecking = metalDecking;
        mat.ShearStud = shearStud;
        mat.Reinforcement = reinforcement;
        safetyFactors.MaterialFactors = mat;
      }

      LoadFactors loadFactors = new LoadFactors();
      loadFactors.ConstantDead = constantDead;
      loadFactors.ConstantLive = constantLive;
      loadFactors.FinalDead = finalDead;
      loadFactors.FinalLive = finalLive;
      safetyFactors.LoadFactors = loadFactors;

      // Act
      string coaString = safetyFactors.ToCoaString("MEMBER-1");

      // Assert
      Assert.Equal(expected_CoaString, coaString);
    }

    [Fact]
    public void DuplicateTest()
    {
      // 1 create with constructor and duplicate
      SafetyFactors original = new SafetyFactors();
      SafetyFactors duplicate = (SafetyFactors)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Null(duplicate.MaterialFactors);
      Assert.Null(duplicate.LoadFactors);

      // 1 create member objects and duplicate again
      original.MaterialFactors = new MaterialFactors();
      original.LoadFactors = new LoadFactors();
      duplicate = (SafetyFactors)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(1.0, duplicate.MaterialFactors.SteelBeam);
      Assert.Equal(1.5, duplicate.MaterialFactors.ConcreteCompression);
      Assert.Equal(1.25, duplicate.MaterialFactors.ConcreteShear);
      Assert.Equal(1.0, duplicate.MaterialFactors.MetalDecking);
      Assert.Equal(1.25, duplicate.MaterialFactors.ShearStud);
      Assert.Equal(1.15, duplicate.MaterialFactors.Reinforcement);
      Assert.Equal(1.4, duplicate.LoadFactors.ConstantDead);
      Assert.Equal(1.4, duplicate.LoadFactors.ConstantLive);
      Assert.Equal(1.6, duplicate.LoadFactors.FinalDead);
      Assert.Equal(1.6, duplicate.LoadFactors.FinalLive);

      // 3 make some changes to duplicate
      MaterialFactors materialPartialFactors = (MaterialFactors)duplicate.MaterialFactors;
      materialPartialFactors.SteelBeam = 1.2;
      materialPartialFactors.ConcreteCompression = 1.25;
      materialPartialFactors.ConcreteShear = 1.3;
      materialPartialFactors.MetalDecking = 1.35;
      materialPartialFactors.ShearStud = 1.4;
      materialPartialFactors.Reinforcement = 1.05;
      LoadFactors loadFactors = (LoadFactors)duplicate.LoadFactors;
      loadFactors.ConstantDead = 1.15;
      loadFactors.ConstantLive = 1.45;
      loadFactors.FinalDead = 1.55;
      loadFactors.FinalLive = 0.95;

      //4 check that duplicate has set changes
      Assert.Equal(1.2, duplicate.MaterialFactors.SteelBeam);
      Assert.Equal(1.25, duplicate.MaterialFactors.ConcreteCompression);
      Assert.Equal(1.3, duplicate.MaterialFactors.ConcreteShear);
      Assert.Equal(1.35, duplicate.MaterialFactors.MetalDecking);
      Assert.Equal(1.4, duplicate.MaterialFactors.ShearStud);
      Assert.Equal(1.05, duplicate.MaterialFactors.Reinforcement);
      Assert.Equal(1.15, duplicate.LoadFactors.ConstantDead);
      Assert.Equal(1.45, duplicate.LoadFactors.ConstantLive);
      Assert.Equal(1.55, duplicate.LoadFactors.FinalDead);
      Assert.Equal(0.95, duplicate.LoadFactors.FinalLive);

      // 5 check that original has not been changed
      Assert.Equal(1.0, original.MaterialFactors.SteelBeam);
      Assert.Equal(1.5, original.MaterialFactors.ConcreteCompression);
      Assert.Equal(1.25, original.MaterialFactors.ConcreteShear);
      Assert.Equal(1.0, original.MaterialFactors.MetalDecking);
      Assert.Equal(1.25, original.MaterialFactors.ShearStud);
      Assert.Equal(1.15, original.MaterialFactors.Reinforcement);
      Assert.Equal(1.4, original.LoadFactors.ConstantDead);
      Assert.Equal(1.4, original.LoadFactors.ConstantLive);
      Assert.Equal(1.6, original.LoadFactors.FinalDead);
      Assert.Equal(1.6, original.LoadFactors.FinalLive);
    }
  }
}
