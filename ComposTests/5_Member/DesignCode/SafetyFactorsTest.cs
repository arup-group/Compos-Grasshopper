using Xunit;

namespace ComposAPI.Tests
{
  public partial class DesignCodeTest
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
    public IMaterialPartialFactors TestMaterialPartialFactorsConstructor()
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
    public LoadFactors TestLoadFactorsConstructor()
    {
      // 1 setup input
      // empty constructor creates default (non-EC4) values

      // 2 create object instance with constructor
      LoadFactors loadFactors = new LoadFactors();

      // 3 check that inputs are set in object's members
      Assert.Equal(1.4, loadFactors.ConstantDead);
      Assert.Equal(1.4, loadFactors.ConstantLive);
      Assert.Equal(1.6, loadFactors.FinalDead);
      Assert.Equal(1.6, loadFactors.FinalLive);

      // (optionally return object for other tests)
      return loadFactors;
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
      //duplicate.MaterialFactors.SteelBeam = 1.2;
      //duplicate.MaterialFactors.ConcreteCompression = 1.25;
      //duplicate.MaterialFactors.ConcreteShear = 1.3;
      //duplicate.MaterialFactors.MetalDecking = 1.35;
      //duplicate.MaterialFactors.ShearStud = 1.4;
      //duplicate.MaterialFactors.Reinforcement = 1.05;
      //duplicate.LoadFactors.ConstantDead = 1.15;
      //duplicate.LoadFactors.ConstantLive = 1.45;
      //duplicate.LoadFactors.FinalDead = 1.55;
      //duplicate.LoadFactors.FinalLive = 0.95;

      // 4 check that duplicate has set changes
      //Assert.Equal(1.2, duplicate.MaterialFactors.SteelBeam);
      //Assert.Equal(1.25, duplicate.MaterialFactors.ConcreteCompression);
      //Assert.Equal(1.3, duplicate.MaterialFactors.ConcreteShear);
      //Assert.Equal(1.35, duplicate.MaterialFactors.MetalDecking);
      //Assert.Equal(1.4, duplicate.MaterialFactors.ShearStud);
      //Assert.Equal(1.05, duplicate.MaterialFactors.Reinforcement);
      //Assert.Equal(1.15, duplicate.LoadFactors.ConstantDead);
      //Assert.Equal(1.45, duplicate.LoadFactors.ConstantLive);
      //Assert.Equal(1.55, duplicate.LoadFactors.FinalDead);
      //Assert.Equal(0.95, duplicate.LoadFactors.FinalLive);

      //// 5 check that original has not been changed
      //Assert.Equal(1.0, original.MaterialFactors.SteelBeam);
      //Assert.Equal(1.5, original.MaterialFactors.ConcreteCompression);
      //Assert.Equal(1.25, original.MaterialFactors.ConcreteShear);
      //Assert.Equal(1.0, original.MaterialFactors.MetalDecking);
      //Assert.Equal(1.25, original.MaterialFactors.ShearStud);
      //Assert.Equal(1.15, original.MaterialFactors.Reinforcement);
      //Assert.Equal(1.4, original.LoadFactors.ConstantDead);
      //Assert.Equal(1.4, original.LoadFactors.ConstantLive);
      //Assert.Equal(1.6, original.LoadFactors.FinalDead);
      //Assert.Equal(1.6, original.LoadFactors.FinalLive);
    }

    [Fact]
    public SafetyFactors TestEC4SafetyFactorConstructor()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      EC4SafetyFactors safetyFactors = new EC4SafetyFactors();

      // 3 check that inputs are set in object's members
      Assert.Null(safetyFactors.MaterialFactors);
      Assert.Null(safetyFactors.LoadFactors);
      Assert.Equal(LoadCombination.Equation6_10, safetyFactors.LoadCombination);

      // (optionally return object for other tests)
      return safetyFactors;
    }

    [Fact]
    public EC4MaterialPartialFactors TestEC4MaterialPartialFactorsConstructor()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      EC4MaterialPartialFactors partialFactors = new EC4MaterialPartialFactors();

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
    public LoadCombinationFactors TestLoadCombinationFactorsConstructor()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      LoadCombinationFactors loadFactors = new LoadCombinationFactors();

      // 3 check that inputs are set in object's members
      Assert.Equal(1.0, loadFactors.xi);
      Assert.Equal(1.0, loadFactors.psi_0);
      Assert.Equal(1.35, loadFactors.gamma_G);
      Assert.Equal(1.5, loadFactors.gamma_Q);

      // (optionally return object for other tests)
      return loadFactors;
    }

    [Fact]
    public void TestECSafetyFactorDuplicate()
    {
      // 1 create with constructor and duplicate
      EC4SafetyFactors original = new EC4SafetyFactors();
      EC4SafetyFactors duplicate = original.Duplicate() as EC4SafetyFactors;

      // 2 check that duplicate has duplicated values
      Assert.Null(duplicate.MaterialFactors);
      Assert.Null(duplicate.LoadFactors);
      Assert.Equal(LoadCombination.Equation6_10, duplicate.LoadCombination);

      // 1 create member objects and duplicate again
      original.MaterialFactors = new EC4MaterialPartialFactors();
      original.LoadFactors = new LoadCombinationFactors();
      duplicate = original.Duplicate() as EC4SafetyFactors;

      // 2 check that duplicate has duplicated values
      Assert.Equal(1.0, duplicate.MaterialFactors.gamma_M0);
      Assert.Equal(1.0, duplicate.MaterialFactors.gamma_M1);
      Assert.Equal(1.25, duplicate.MaterialFactors.gamma_M2);
      Assert.Equal(1.5, duplicate.MaterialFactors.gamma_C);
      Assert.Equal(1.0, duplicate.MaterialFactors.gamma_Deck);
      Assert.Equal(1.25, duplicate.MaterialFactors.gamma_vs);
      Assert.Equal(1.15, duplicate.MaterialFactors.gamma_S);
      Assert.Equal(1.0, duplicate.LoadFactors.xi);
      Assert.Equal(1.0, duplicate.LoadFactors.psi_0);
      Assert.Equal(1.35, duplicate.LoadFactors.gamma_G);
      Assert.Equal(1.5, duplicate.LoadFactors.gamma_Q);

      // 3 make some changes to duplicate
      duplicate.MaterialFactors.gamma_M0 = 1.2;
      duplicate.MaterialFactors.gamma_M1 = 1.25;
      duplicate.MaterialFactors.gamma_M2 = 1.3;
      duplicate.MaterialFactors.gamma_C = 1.35;
      duplicate.MaterialFactors.gamma_Deck = 1.4;
      duplicate.MaterialFactors.gamma_vs = 1.05;
      duplicate.MaterialFactors.gamma_S = 1.5;
      duplicate.LoadFactors.xi = 1.15;
      duplicate.LoadFactors.psi_0 = 1.45;
      duplicate.LoadFactors.gamma_G = 1.55;
      duplicate.LoadFactors.gamma_Q = 0.95;

      // 4 check that duplicate has set changes
      Assert.Equal(1.2, duplicate.MaterialFactors.gamma_M0);
      Assert.Equal(1.25, duplicate.MaterialFactors.gamma_M1);
      Assert.Equal(1.3, duplicate.MaterialFactors.gamma_M2);
      Assert.Equal(1.35, duplicate.MaterialFactors.gamma_C);
      Assert.Equal(1.4, duplicate.MaterialFactors.gamma_Deck);
      Assert.Equal(1.05, duplicate.MaterialFactors.gamma_vs);
      Assert.Equal(1.5, duplicate.MaterialFactors.gamma_S);
      Assert.Equal(1.15, duplicate.LoadFactors.xi);
      Assert.Equal(1.45, duplicate.LoadFactors.psi_0);
      Assert.Equal(1.55, duplicate.LoadFactors.gamma_G);
      Assert.Equal(0.95, duplicate.LoadFactors.gamma_Q);

      // 5 check that original has not been changed
      Assert.Equal(1.0, original.MaterialFactors.gamma_M0);
      Assert.Equal(1.0, original.MaterialFactors.gamma_M1);
      Assert.Equal(1.25, original.MaterialFactors.gamma_M2);
      Assert.Equal(1.5, original.MaterialFactors.gamma_C);
      Assert.Equal(1.0, original.MaterialFactors.gamma_Deck);
      Assert.Equal(1.25, original.MaterialFactors.gamma_vs);
      Assert.Equal(1.15, original.MaterialFactors.gamma_S);
      Assert.Equal(1.0, original.LoadFactors.xi);
      Assert.Equal(1.0, original.LoadFactors.psi_0);
      Assert.Equal(1.35, original.LoadFactors.gamma_G);
      Assert.Equal(1.5, original.LoadFactors.gamma_Q);
    }
  }
}
