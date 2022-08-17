using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.Units;
using System;
using Grasshopper.Kernel.Parameters;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateDeckingConfigurationTests
  {
    public static GH_OasysComponent CreateDeckingConfigurationMother()
    {
      var comp = new CreateDeckingConfiguration();
      comp.CreateAttributes();

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreateDeckingConfigurationMother();

      DeckingConfigurationGoo output = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Math.PI / 2, output.Value.Angle.Radians);
      Assert.True(output.Value.IsDiscontinous);
      Assert.False(output.Value.IsWelded);
    }

    [Fact]
    public void CreateComponentWithInputsTest1()
    {
      var comp = CreateDeckingConfigurationMother();
      int i = 0;
      ComponentTestHelper.SetInput(comp, 0, i++);
      ComponentTestHelper.SetInput(comp, false, i++);
      ComponentTestHelper.SetInput(comp, true, i++);

      DeckingConfigurationGoo output = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.Angle.Radians);
      Assert.False(output.Value.IsDiscontinous);
      Assert.True(output.Value.IsWelded);
    }

    [Fact]
    public void CreateComponentWithInputsTest2()
    {
      var comp = CreateDeckingConfigurationMother();
      Param_Number angleParameter = (Param_Number)comp.Params.Input[0];
      angleParameter.UseDegrees = true;

      int i = 0;
      ComponentTestHelper.SetInput(comp, 45, i++);
      ComponentTestHelper.SetInput(comp, false, i++);
      ComponentTestHelper.SetInput(comp, true, i++);

      DeckingConfigurationGoo output = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(45, output.Value.Angle.Degrees);
      Assert.False(output.Value.IsDiscontinous);
      Assert.True(output.Value.IsWelded);
    }
  }
}
