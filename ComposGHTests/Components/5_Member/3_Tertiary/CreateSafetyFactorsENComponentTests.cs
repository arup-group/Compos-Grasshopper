using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Member {
  [Collection("GrasshopperFixture collection")]
  public class CreateSafetyFactorsENComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateSafetyFactorsEN();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (SafetyFactorsENGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(1.0, output.Value.LoadCombinationFactors.ConstantXi);
      Assert.Equal(1.0, output.Value.LoadCombinationFactors.ConstantPsi);
      Assert.Equal(1.35, output.Value.LoadCombinationFactors.Constantgamma_G);
      Assert.Equal(1.5, output.Value.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1.0, output.Value.LoadCombinationFactors.FinalXi);
      Assert.Equal(1.0, output.Value.LoadCombinationFactors.FinalPsi);
      Assert.Equal(1.35, output.Value.LoadCombinationFactors.Finalgamma_G);
      Assert.Equal(1.5, output.Value.LoadCombinationFactors.Finalgamma_Q);
      Assert.Equal(1.0, output.Value.MaterialFactors.Gamma_M0);
      Assert.Equal(1.0, output.Value.MaterialFactors.Gamma_M1);
      Assert.Equal(1.25, output.Value.MaterialFactors.Gamma_M2);
      Assert.Equal(1.5, output.Value.MaterialFactors.Gamma_C);
      Assert.Equal(1.0, output.Value.MaterialFactors.Gamma_Deck);
      Assert.Equal(1.25, output.Value.MaterialFactors.Gamma_vs);
      Assert.Equal(1.15, output.Value.MaterialFactors.Gamma_S);
    }

    [Fact]
    public void CreateComponentWithInputs() {
      GH_OasysDropDownComponent comp = ComponentMother();

      for (int i = 0; i < comp.Params.Input.Count; i++) {
        ComponentTestHelper.SetInput(comp, 1 + (1 / (i + 1)), i);
      }

      var output = (SafetyFactorsENGoo)ComponentTestHelper.GetOutput(comp);

      int j = 0;
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadCombinationFactors.ConstantXi);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadCombinationFactors.ConstantPsi);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadCombinationFactors.Constantgamma_G);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadCombinationFactors.Constantgamma_Q);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadCombinationFactors.FinalXi);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadCombinationFactors.FinalPsi);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadCombinationFactors.Finalgamma_G);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadCombinationFactors.Finalgamma_Q);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.Gamma_M0);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.Gamma_M1);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.Gamma_M2);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.Gamma_C);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.Gamma_Deck);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.Gamma_vs);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.Gamma_S);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
