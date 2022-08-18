using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using System;
using Grasshopper.Kernel.Parameters;
using ComposAPI;
using UnitsNet.Units;
using UnitsNet;

namespace ComposGHTests.Member
{
  [Collection("GrasshopperFixture collection")]
  public class CreateSafetyFactorsENTests
  {
    public static GH_OasysDropDownComponent CreateSafetyFactorsENMother()
    {
      var comp = new CreateSafetyFactorsEN();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreateSafetyFactorsENMother();

      SafetyFactorsENGoo output = (SafetyFactorsENGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(1.0, output.Value.LoadCombinationFactors.ConstantXi);
      Assert.Equal(1.0, output.Value.LoadCombinationFactors.ConstantPsi);
      Assert.Equal(1.35, output.Value.LoadCombinationFactors.Constantgamma_G);
      Assert.Equal(1.5, output.Value.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.0, output.Value.LoadCombinationFactors.FinalXi);
      Assert.Equal(1.0, output.Value.LoadCombinationFactors.FinalPsi);
      Assert.Equal(1.35, output.Value.LoadCombinationFactors.Finalgamma_G);
      Assert.Equal(1.5, output.Value.LoadCombinationFactors.Finalgamma_Q);
      Assert.Equal(1.0, output.Value.MaterialFactors.gamma_M0);
      Assert.Equal(1.0, output.Value.MaterialFactors.gamma_M1);
      Assert.Equal(1.25, output.Value.MaterialFactors.gamma_M2);
      Assert.Equal(1.5, output.Value.MaterialFactors.gamma_C);
      Assert.Equal(1.0, output.Value.MaterialFactors.gamma_Deck);
      Assert.Equal(1.25, output.Value.MaterialFactors.gamma_vs);
      Assert.Equal(1.15, output.Value.MaterialFactors.gamma_S);
    }

    [Fact]
    public void CreateComponentWithInputs()
    {
      var comp = CreateSafetyFactorsENMother();
      
      for (int i = 0; i < comp.Params.Input.Count; i++)
        ComponentTestHelper.SetInput(comp, 1 + 1/(i + 1), i);

      SafetyFactorsENGoo output = (SafetyFactorsENGoo)ComponentTestHelper.GetOutput(comp);

      int j = 0;
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.LoadCombinationFactors.ConstantXi);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.LoadCombinationFactors.ConstantPsi);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.LoadCombinationFactors.Constantgamma_G);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.LoadCombinationFactors.FinalXi);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.LoadCombinationFactors.FinalPsi);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.LoadCombinationFactors.Finalgamma_G);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.LoadCombinationFactors.Finalgamma_Q);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.MaterialFactors.gamma_M0);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.MaterialFactors.gamma_M1);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.MaterialFactors.gamma_M2);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.MaterialFactors.gamma_C);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.MaterialFactors.gamma_Deck);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.MaterialFactors.gamma_vs);
      Assert.Equal(1 + 1 / (j++ + 1), output.Value.MaterialFactors.gamma_S);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateSafetyFactorsENMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateSafetyFactorsENMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
