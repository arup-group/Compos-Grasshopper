using ComposAPI.Helpers;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Members.Tests
{
  public partial class SafetyFactorsENTest
  {
    [Fact]
    public SafetyFactorsEN ConstructorTest()
    {
      // 1 setup input
      // empty constructor creates default EC4 values

      // 2 create object instance with constructor
      SafetyFactorsEN safetyFactors = new SafetyFactorsEN();

      // 3 check that inputs are set in object's members
      Assert.Null(safetyFactors.MaterialFactors);
      //Assert.Null(safetyFactors.LoadCombinationFactors);
      //Assert.Equal(LoadCombination.Equation6_10, safetyFactors.LoadCombination);

      // (optionally return object for other tests)
      return safetyFactors;
    }

    [Fact]
    public void DuplicateTest()
    {
      // 1 create with constructor and duplicate
      SafetyFactorsEN original = new SafetyFactorsEN();
      SafetyFactorsEN duplicate = (SafetyFactorsEN)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Null(duplicate.MaterialFactors);
      //Assert.Null(duplicate.LoadCombinationFactors);
      Assert.Equal(LoadCombination.Equation6_10, duplicate.LoadCombinationFactors.LoadCombination);

      // 1 create member objects and duplicate again
      original.MaterialFactors = new EC4MaterialPartialFactors();
      original.LoadCombinationFactors = new LoadCombinationFactors();
      duplicate = (SafetyFactorsEN)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Assert.Equal(1.0, duplicate.MaterialFactors.gamma_M0);
      Assert.Equal(1.0, duplicate.MaterialFactors.gamma_M1);
      Assert.Equal(1.25, duplicate.MaterialFactors.gamma_M2);
      Assert.Equal(1.5, duplicate.MaterialFactors.gamma_C);
      Assert.Equal(1.0, duplicate.MaterialFactors.gamma_Deck);
      Assert.Equal(1.25, duplicate.MaterialFactors.gamma_vs);
      Assert.Equal(1.15, duplicate.MaterialFactors.gamma_S);
      Assert.Equal(1.0, duplicate.LoadCombinationFactors.ConstantXi);
      Assert.Equal(1.0, duplicate.LoadCombinationFactors.ConstantPsi);
      //Assert.Equal(1.35, duplicate.LoadCombinationFactors.Constantgamma_G);
      //Assert.Equal(1.5, duplicate.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.0, duplicate.LoadCombinationFactors.FinalXi);
      Assert.Equal(1.0, duplicate.LoadCombinationFactors.FinalPsi);
      //Assert.Equal(1.35, duplicate.LoadCombinationFactors.Finalgamma_G);
      //Assert.Equal(1.5, duplicate.LoadCombinationFactors.Finalgamma_Q);

      // 3 make some changes to duplicate
      EC4MaterialPartialFactors partialFactors = new EC4MaterialPartialFactors();
      partialFactors.gamma_M0 = 1.2;
      partialFactors.gamma_M1 = 1.25;
      partialFactors.gamma_M2 = 1.3;
      partialFactors.gamma_C = 1.35;
      partialFactors.gamma_Deck = 1.4;
      partialFactors.gamma_vs = 1.05;
      partialFactors.gamma_S = 1.5;
      duplicate.MaterialFactors = partialFactors;
      LoadCombinationFactors combinationFactors = new LoadCombinationFactors();
      combinationFactors.ConstantXi = 1.15;
      combinationFactors.ConstantPsi = 1.45;
      //combinationFactors.Constantgamma_G = 1.55;
      //combinationFactors.Constantgamma_Q = 0.95;
      combinationFactors.FinalXi = 1.15;
      combinationFactors.FinalPsi = 1.45;
      //combinationFactors.Finalgamma_G = 1.55;
      //combinationFactors.Finalgamma_Q = 0.95;
      duplicate.LoadCombinationFactors = combinationFactors;

      // 4 check that duplicate has set changes
      Assert.Equal(1.2, duplicate.MaterialFactors.gamma_M0);
      Assert.Equal(1.25, duplicate.MaterialFactors.gamma_M1);
      Assert.Equal(1.3, duplicate.MaterialFactors.gamma_M2);
      Assert.Equal(1.35, duplicate.MaterialFactors.gamma_C);
      Assert.Equal(1.4, duplicate.MaterialFactors.gamma_Deck);
      Assert.Equal(1.05, duplicate.MaterialFactors.gamma_vs);
      Assert.Equal(1.5, duplicate.MaterialFactors.gamma_S);
      Assert.Equal(1.15, duplicate.LoadCombinationFactors.ConstantXi);
      Assert.Equal(1.45, duplicate.LoadCombinationFactors.ConstantPsi);
      //Assert.Equal(1.55, duplicate.LoadCombinationFactors.Constantgamma_G);
      //Assert.Equal(0.95, duplicate.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.15, duplicate.LoadCombinationFactors.FinalXi);
      Assert.Equal(1.45, duplicate.LoadCombinationFactors.FinalPsi);
      //Assert.Equal(1.55, duplicate.LoadCombinationFactors.Finalgamma_G);
      //Assert.Equal(0.95, duplicate.LoadCombinationFactors.Finalgamma_Q);

      // 5 check that original has not been changed
      Assert.Equal(1.0, original.MaterialFactors.gamma_M0);
      Assert.Equal(1.0, original.MaterialFactors.gamma_M1);
      Assert.Equal(1.25, original.MaterialFactors.gamma_M2);
      Assert.Equal(1.5, original.MaterialFactors.gamma_C);
      Assert.Equal(1.0, original.MaterialFactors.gamma_Deck);
      Assert.Equal(1.25, original.MaterialFactors.gamma_vs);
      Assert.Equal(1.15, original.MaterialFactors.gamma_S);
      Assert.Equal(1.0, original.LoadCombinationFactors.ConstantXi);
      Assert.Equal(1.0, original.LoadCombinationFactors.ConstantPsi);
      //Assert.Equal(1.35, original.LoadCombinationFactors.Constantgamma_G);
      //Assert.Equal(1.5, original.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.0, original.LoadCombinationFactors.FinalXi);
      Assert.Equal(1.0, original.LoadCombinationFactors.FinalPsi);
      //Assert.Equal(1.35, original.LoadCombinationFactors.Finalgamma_G);
      //Assert.Equal(1.5, original.LoadCombinationFactors.Finalgamma_Q);
    }
  }
}
