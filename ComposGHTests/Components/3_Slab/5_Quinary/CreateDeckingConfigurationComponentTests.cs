using System;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using Grasshopper.Kernel.Parameters;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Slab {
  [Collection("GrasshopperFixture collection")]
  public class CreateDeckingConfigurationComponentTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateDeckingConfiguration();
      comp.CreateAttributes();

      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysComponent comp = ComponentMother();

      var output = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Math.PI / 2, output.Value.Angle.Radians);
      Assert.True(output.Value.IsDiscontinous);
      Assert.False(output.Value.IsWelded);
    }

    [Fact]
    public void CreateComponentWithInputsTest1() {
      GH_OasysComponent comp = ComponentMother();
      int i = 0;
      ComponentTestHelper.SetInput(comp, 0, i++);
      ComponentTestHelper.SetInput(comp, false, i++);
      ComponentTestHelper.SetInput(comp, true, i++);

      var output = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.Angle.Radians);
      Assert.False(output.Value.IsDiscontinous);
      Assert.True(output.Value.IsWelded);
    }

    [Fact]
    public void CreateComponentWithInputsTest2() {
      GH_OasysComponent comp = ComponentMother();
      var angleParameter = (Param_Number)comp.Params.Input[0];
      angleParameter.UseDegrees = true;

      int i = 0;
      ComponentTestHelper.SetInput(comp, 45, i++);
      ComponentTestHelper.SetInput(comp, false, i++);
      ComponentTestHelper.SetInput(comp, true, i++);

      var output = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(45, output.Value.Angle.Degrees);
      Assert.False(output.Value.IsDiscontinous);
      Assert.True(output.Value.IsWelded);
    }
  }
}
