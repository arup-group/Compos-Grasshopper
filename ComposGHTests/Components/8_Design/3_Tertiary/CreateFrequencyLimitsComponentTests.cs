using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Design {
  [Collection("GrasshopperFixture collection")]
  public class CreateFrequencyLimitsComponentTests {

    public static GH_OasysComponent CreateFrequencyLimitsMother() {
      var comp = new CreateFrequencyLimits();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "4 Hz", 0);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1() {
      GH_OasysComponent comp = CreateFrequencyLimitsMother();

      var output = (FrequencyLimitsGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(4, output.Value.MinimumRequired.Hertz);
      Assert.Equal(1, output.Value.DeadLoadIncl.DecimalFractions);
      Assert.Equal(0.1, output.Value.LiveLoadIncl.DecimalFractions);
    }

    [Fact]
    public void CreateComponentWithInputs2() {
      GH_OasysComponent comp = CreateFrequencyLimitsMother();

      ComponentTestHelper.SetInput(comp, 0.9, 1);
      ComponentTestHelper.SetInput(comp, 0.2, 2);

      var output = (FrequencyLimitsGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(4, output.Value.MinimumRequired.Hertz);
      Assert.Equal(0.9, output.Value.DeadLoadIncl.DecimalFractions);
      Assert.Equal(0.2, output.Value.LiveLoadIncl.DecimalFractions);
    }
  }
}
