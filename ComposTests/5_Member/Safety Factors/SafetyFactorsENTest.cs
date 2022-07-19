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
      
      Assert.Equal(LoadCombination.Equation6_10, safetyFactors.LoadCombinationFactors.LoadCombination);
      Assert.Equal(1.35, safetyFactors.LoadCombinationFactors.Constantgamma_G);
      Assert.Equal(1.35, safetyFactors.LoadCombinationFactors.Finalgamma_G);
      Assert.Equal(1.5, safetyFactors.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.5, safetyFactors.LoadCombinationFactors.Finalgamma_Q);
      Assert.Equal(1, safetyFactors.LoadCombinationFactors.ConstantXi);
      Assert.Equal(1, safetyFactors.LoadCombinationFactors.FinalXi);
      Assert.Equal(1, safetyFactors.LoadCombinationFactors.ConstantPsi);
      Assert.Equal(1, safetyFactors.LoadCombinationFactors.FinalPsi);

      // (optionally return object for other tests)
      return safetyFactors;
    }

    [Theory]
    [InlineData("SAFETY_FACTOR_LOAD	MEMBER-1	22.0000	23.0000	24.0000	25.0000\nSAFETY_FACTOR_MATERIAL	MEMBER-1	11.0000	12.0000	13.0000	14.0000	1.25000	15.0000	16.0000	17.0000\nEC4_LOAD_COMB_FACTORS	MEMBER-1	USER_DEFINED	1.10000	1.20000	1.30000	1.40000", true, 11, 12, 13, 14, 15, 16, 17, LoadCombination.Custom, 22, 23, 24, 25, 1.1, 1.2, 1.3, 1.4)]
    [InlineData("EC4_LOAD_COMB_FACTORS	MEMBER-1	EC0_6_10\n", false, 0, 0, 0, 0, 0, 0, 0, LoadCombination.Equation6_10, 1.35, 1.5, 1.35, 1.5, 1.0, 1.0, 1.0, 1.0)]
    [InlineData("EC4_LOAD_COMB_FACTORS	MEMBER-1	EC0_WORST_6_10A_10B\n", false, 0, 0, 0, 0, 0, 0, 0, LoadCombination.Equation6_10a__6_10b, 1.35, 1.5, 1.35, 1.5, 0.85, 0.85, 1.0, 0.7)]
    [InlineData("SAFETY_FACTOR_LOAD	MEMBER-1	12.0000	13.0000	14.0000	15.0000\nEC4_LOAD_COMB_FACTORS	MEMBER-1	USER_DEFINED	1.10000	1.20000	1.30000	1.40000\n", false, 0, 0, 0, 0, 0, 0, 0, LoadCombination.Custom, 12, 13, 14, 15, 1.1, 1.2, 1.3, 1.4)]
    public void FromCoaStringTest(string coaString, bool materialFactorsSet,double expected_gamma_M0, double expected_gamma_M1, double expected_gamma_M2, double expected_gamma_C, double expected_gamma_Deck, double expected_gamma_vs, double expected_gamma_S, LoadCombination expected_LoadCombination, double expected_Constantgamma_G, double expected_Constantgamma_Q, double expected_Finalgamma_G, double expected_Finalgamma_Q, double expected_Constantxi, double expected_FinalXi, double expected_ConstantPsi, double expected_FinalPsi)
    {
      // Act
      ISafetyFactorsEN safetyFactorsEN = SafetyFactorsEN.FromCoaString(coaString, "MEMBER-1");

      // Assert
      if (materialFactorsSet)
      {
        IMaterialPartialFactorsEN mat = safetyFactorsEN.MaterialFactors;
        Assert.Equal(expected_gamma_M0, mat.gamma_M0);
        Assert.Equal(expected_gamma_M1, mat.gamma_M1);
        Assert.Equal(expected_gamma_M2, mat.gamma_M2);
        Assert.Equal(expected_gamma_C, mat.gamma_C);
        Assert.Equal(expected_gamma_Deck, mat.gamma_Deck);
        Assert.Equal(expected_gamma_vs, mat.gamma_vs);
        Assert.Equal(expected_gamma_S, mat.gamma_S);
      }
      else
        Assert.Null(safetyFactorsEN.MaterialFactors);

      ILoadCombinationFactors combinationFactors = safetyFactorsEN.LoadCombinationFactors;
      Assert.Equal(expected_LoadCombination, combinationFactors.LoadCombination);
      Assert.Equal(expected_Constantxi, combinationFactors.ConstantXi);
      Assert.Equal(expected_FinalXi, combinationFactors.FinalXi);
      Assert.Equal(expected_ConstantPsi, combinationFactors.ConstantPsi);
      Assert.Equal(expected_FinalPsi, combinationFactors.FinalPsi);
      Assert.Equal(expected_Constantgamma_G, combinationFactors.Constantgamma_G);
      Assert.Equal(expected_Constantgamma_Q, combinationFactors.Constantgamma_Q);
      Assert.Equal(expected_Finalgamma_G, combinationFactors.Finalgamma_G);
      Assert.Equal(expected_Finalgamma_Q, combinationFactors.Finalgamma_Q);
    }

    [Theory]
    [InlineData("SAFETY_FACTOR_MATERIAL	MEMBER-1	11.0000	12.0000	13.0000	14.0000	1.25000	15.0000	16.0000	17.0000\nSAFETY_FACTOR_LOAD	MEMBER-1	22.0000	23.0000	24.0000	25.0000\nEC4_LOAD_COMB_FACTORS	MEMBER-1	USER_DEFINED	1.10000	1.20000	1.30000	1.40000\n", true, 11, 12, 13, 14, 15, 16, 17, LoadCombination.Custom, 22, 23, 24, 25, 1.1, 1.2, 1.3, 1.4)]
    [InlineData("EC4_LOAD_COMB_FACTORS	MEMBER-1	EC0_6_10	1.00000	1.00000	1.00000	1.00000\n", false, 0, 0, 0, 0, 0, 0, 0, LoadCombination.Equation6_10, 1.35, 1.35, 1.5, 1.5, 1.0, 1.0, 1.0, 1.0)]
    [InlineData("EC4_LOAD_COMB_FACTORS	MEMBER-1	EC0_WORST_6_10A_10B	0.850000	0.850000	1.00000	0.700000\n", false, 0, 0, 0, 0, 0, 0, 0, LoadCombination.Equation6_10a__6_10b, 1.35, 1.35, 1.5, 1.5, 0.85, 0.85, 1.0, 0.7)]
    [InlineData("SAFETY_FACTOR_LOAD	MEMBER-1	12.0000	13.0000	14.0000	15.0000\nEC4_LOAD_COMB_FACTORS	MEMBER-1	USER_DEFINED	1.10000	1.20000	1.30000	1.40000\n", false, 0, 0, 0, 0, 0, 0, 0, LoadCombination.Custom, 12, 13, 14, 15, 1.1, 1.2, 1.3, 1.4)]
    public void ToCoaStringTest(string expected_CoaString, bool materialFactorsSet, double gamma_M0, double gamma_M1, double gamma_M2, double gamma_C, double gamma_Deck, double gamma_vs, double gamma_S, LoadCombination loadCombination, double constantgamma_G, double finalgamma_G, double constantgamma_Q, double finalgamma_Q, double constantxi, double finalXi, double constantPsi, double finalPsi)
    {
      // Assemble
      SafetyFactorsEN safetyFactorsEN = new SafetyFactorsEN();
      
      if (materialFactorsSet)
      {
        MaterialPartialFactorsEN mat = new MaterialPartialFactorsEN();
        mat.gamma_M0 = gamma_M0;
        mat.gamma_M1 = gamma_M1;
        mat.gamma_M2 = gamma_M2;
        mat.gamma_C = gamma_C;
        mat.gamma_Deck = gamma_Deck;
        mat.gamma_vs = gamma_vs;
        mat.gamma_S = gamma_S;
        safetyFactorsEN.MaterialFactors = mat;
      }

      LoadCombinationFactors combinationFactors = new LoadCombinationFactors();
      combinationFactors.LoadCombination = loadCombination;
      combinationFactors.ConstantXi = constantxi;
      combinationFactors.FinalXi = finalXi;
      combinationFactors.ConstantPsi = constantPsi;
      combinationFactors.FinalPsi = finalPsi;
      combinationFactors.Constantgamma_G = constantgamma_G;
      combinationFactors.Constantgamma_Q = constantgamma_Q;
      combinationFactors.Finalgamma_G = finalgamma_G;
      combinationFactors.Finalgamma_Q = finalgamma_Q;
      safetyFactorsEN.LoadCombinationFactors = combinationFactors;

      // Act
      string coaString = safetyFactorsEN.ToCoaString("MEMBER-1");

      // Assert
      Assert.Equal(expected_CoaString, coaString);
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
      original.MaterialFactors = new MaterialPartialFactorsEN();
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
      Assert.Equal(1.35, duplicate.LoadCombinationFactors.Constantgamma_G);
      Assert.Equal(1.5, duplicate.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.0, duplicate.LoadCombinationFactors.FinalXi);
      Assert.Equal(1.0, duplicate.LoadCombinationFactors.FinalPsi);
      Assert.Equal(1.35, duplicate.LoadCombinationFactors.Finalgamma_G);
      Assert.Equal(1.5, duplicate.LoadCombinationFactors.Finalgamma_Q);

      // 3 make some changes to duplicate
      MaterialPartialFactorsEN partialFactors = new MaterialPartialFactorsEN();
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
      combinationFactors.Constantgamma_G = 1.55;
      combinationFactors.Constantgamma_Q = 0.95;
      combinationFactors.FinalXi = 1.15;
      combinationFactors.FinalPsi = 1.45;
      combinationFactors.Finalgamma_G = 1.55;
      combinationFactors.Finalgamma_Q = 0.95;
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
      Assert.Equal(1.55, duplicate.LoadCombinationFactors.Constantgamma_G);
      Assert.Equal(0.95, duplicate.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.15, duplicate.LoadCombinationFactors.FinalXi);
      Assert.Equal(1.45, duplicate.LoadCombinationFactors.FinalPsi);
      Assert.Equal(1.55, duplicate.LoadCombinationFactors.Finalgamma_G);
      Assert.Equal(0.95, duplicate.LoadCombinationFactors.Finalgamma_Q);

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
      Assert.Equal(1.35, original.LoadCombinationFactors.Constantgamma_G);
      Assert.Equal(1.5, original.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.0, original.LoadCombinationFactors.FinalXi);
      Assert.Equal(1.0, original.LoadCombinationFactors.FinalPsi);
      Assert.Equal(1.35, original.LoadCombinationFactors.Finalgamma_G);
      Assert.Equal(1.5, original.LoadCombinationFactors.Finalgamma_Q);
    }
  }
}
