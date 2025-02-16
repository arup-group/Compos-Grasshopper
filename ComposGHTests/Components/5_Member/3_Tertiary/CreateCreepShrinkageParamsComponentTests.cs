﻿using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Member {
  [Collection("GrasshopperFixture collection")]
  public class CreateCreepShrinkageParamsComponentTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateCreepShrinkageParams();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, 1.1, 0);
      ComponentTestHelper.SetInput(comp, 28, 1);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1() {
      GH_OasysComponent comp = ComponentMother();

      var output = (CreepShrinkageParametersGoo)ComponentTestHelper.GetOutput(comp);
      var cs = (CreepShrinkageParametersEN)output.Value;

      Assert.Equal(1.1, cs.CreepCoefficient);
      Assert.Equal(28, cs.ConcreteAgeAtLoad);
      Assert.Equal(36500, cs.FinalConcreteAgeCreep);
      Assert.Equal(50, cs.RelativeHumidity.Percent);
    }

    [Fact]
    public void CreateComponentWithInputs2() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 40000, 2);
      ComponentTestHelper.SetInput(comp, "60 %", 3);

      var output = (CreepShrinkageParametersGoo)ComponentTestHelper.GetOutput(comp);
      var cs = (CreepShrinkageParametersEN)output.Value;

      Assert.Equal(40000, cs.FinalConcreteAgeCreep);
      Assert.Equal(60, cs.RelativeHumidity.Percent);
    }
  }
}
