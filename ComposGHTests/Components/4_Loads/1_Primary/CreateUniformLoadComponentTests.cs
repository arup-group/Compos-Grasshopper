﻿using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using OasysUnits.Units;
using Xunit;

namespace ComposGHTests.Load {
  [Collection("GrasshopperFixture collection")]
  public class CreateUniformLoadComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateUniformLoad();
      comp.CreateAttributes();

      comp.SetSelected(1, 5); // change dropdown to kN/m2

      for (int i = 0; i < comp.Params.Input.Count; i++) {
        ComponentTestHelper.SetInput(comp, (i + 1) * 1, i);
      }

      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponent1() {
      GH_OasysDropDownComponent comp = ComponentMother();

      comp.SetSelected(0, 0); // change dropdown to line
      comp.SetSelected(1, 5); // change dropdown to kN/m
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      var output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (UniformLoad)output.Value;

      int i = 0;
      Assert.Equal(LoadDistribution.Line, load.Distribution);
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalLive.As(force));
    }

    [Fact]
    public void CreateComponent2() {
      GH_OasysDropDownComponent comp = ComponentMother();

      comp.SetSelected(0, 1); // change dropdown to area
      comp.SetSelected(1, 5); // change dropdown to kN/m2
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      var output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (UniformLoad)output.Value;

      int i = 0;
      Assert.Equal(LoadDistribution.Area, load.Distribution);
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalLive.As(force));
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
