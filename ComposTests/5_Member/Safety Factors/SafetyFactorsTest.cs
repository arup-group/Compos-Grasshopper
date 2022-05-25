using Xunit;

namespace ComposAPI.Members.Tests
{
  public partial class SafetyFactorsTest
  {
    [Fact]
    public SafetyFactors TestSafetyFactorConstructor()
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
    public void DefaultToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "";
      SafetyFactors safetyFactors = new SafetyFactors();
      // Act
      string coaString = safetyFactors.ToCoaString("MEMBER-5");
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestSafetyFactorDuplicate()
    {
      // 1 create with constructor and duplicate
      SafetyFactors original = new SafetyFactors();
      SafetyFactors duplicate = original.Duplicate() as SafetyFactors;

      // 2 check that duplicate has duplicated values
      Assert.Null(duplicate.MaterialFactors);
      Assert.Null(duplicate.LoadFactors);

      // 1 create member objects and duplicate again
      original.MaterialFactors = new MaterialPartialFactors();
      original.LoadFactors = new LoadFactors();
      duplicate = original.Duplicate() as SafetyFactors;

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
      MaterialPartialFactors materialPartialFactors = duplicate.MaterialFactors as MaterialPartialFactors;
      materialPartialFactors.SteelBeam = 1.2;
      materialPartialFactors.ConcreteCompression = 1.25;
      materialPartialFactors.ConcreteShear = 1.3;
      materialPartialFactors.MetalDecking = 1.35;
      materialPartialFactors.ShearStud = 1.4;
      materialPartialFactors.Reinforcement = 1.05;
      LoadFactors loadFactors = duplicate.LoadFactors as LoadFactors;
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

    [Fact]
    public LoadCombinationFactors TestLoadCombinationFactorsConstructor()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      LoadCombinationFactors loadFactors = new LoadCombinationFactors();

      // 3 check that inputs are set in object's members
      Assert.Equal(1.0, loadFactors.Constantxi);
      Assert.Equal(1.0, loadFactors.Constantpsi_0);
      Assert.Equal(1.35, loadFactors.Constantgamma_G);
      Assert.Equal(1.5, loadFactors.Constantgamma_Q);
      Assert.Equal(1.0, loadFactors.Finalxi);
      Assert.Equal(1.0, loadFactors.Finalpsi_0);
      Assert.Equal(1.35, loadFactors.Finalgamma_G);
      Assert.Equal(1.5, loadFactors.Finalgamma_Q);

      // (optionally return object for other tests)
      return loadFactors;
    }
  }
}
