using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Member {
  [Collection("GrasshopperFixture collection")]
  public class CreateSafetyFactorsComponentTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateSafetyFactors();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysComponent comp = ComponentMother();

      var output = (SafetyFactorsGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(1.4, output.Value.LoadFactors.ConstantDead);
      Assert.Equal(1.4, output.Value.LoadFactors.FinalDead);
      Assert.Equal(1.6, output.Value.LoadFactors.ConstantLive);
      Assert.Equal(1.6, output.Value.LoadFactors.FinalLive);
      Assert.Equal(1.0, output.Value.MaterialFactors.SteelBeam);
      Assert.Equal(1.5, output.Value.MaterialFactors.ConcreteCompression);
      Assert.Equal(1.25, output.Value.MaterialFactors.ConcreteShear);
      Assert.Equal(1.0, output.Value.MaterialFactors.MetalDecking);
      Assert.Equal(1.25, output.Value.MaterialFactors.ShearStud);
      Assert.Equal(1.15, output.Value.MaterialFactors.Reinforcement);
    }

    [Fact]
    public void CreateComponentWithInputs() {
      GH_OasysComponent comp = ComponentMother();

      for (int i = 0; i < comp.Params.Input.Count; i++) {
        ComponentTestHelper.SetInput(comp, 1 + (1 / (i + 1)), i);
      }

      var output = (SafetyFactorsGoo)ComponentTestHelper.GetOutput(comp);

      int j = 0;
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadFactors.ConstantDead);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadFactors.FinalDead);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadFactors.ConstantLive);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.LoadFactors.FinalLive);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.SteelBeam);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.ConcreteCompression);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.ConcreteShear);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.MetalDecking);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.ShearStud);
      Assert.Equal(1 + (1 / (j++ + 1)), output.Value.MaterialFactors.Reinforcement);
    }
  }
}
