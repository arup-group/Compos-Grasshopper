﻿using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Slab {
  [Collection("GrasshopperFixture collection")]
  public class CreateSlabDimensionComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateSlabDimension();
      comp.CreateAttributes();

      comp.SetSelected(0, 0); // change dropdown to mm

      ComponentTestHelper.SetInput(comp, 130, 1);
      ComponentTestHelper.SetInput(comp, 1700, 2);
      ComponentTestHelper.SetInput(comp, 1200, 3);

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
      comp.SetSelected(0, 0); // change dropdown to mm
      var output = (SlabDimensionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.StartPosition.Value);
      Assert.Equal(130, output.Value.OverallDepth.Millimeters);
      Assert.Equal(1.7, output.Value.AvailableWidthLeft.Meters);
      Assert.Equal(1.2, output.Value.AvailableWidthRight.Meters);
      Assert.False(output.Value.TaperedToNext);
    }

    [Fact]
    public void CreateComponentWithInputs1() {
      GH_OasysDropDownComponent comp = ComponentMother();

      comp.SetSelected(0, 2); // change dropdown to m

      ComponentTestHelper.SetInput(comp, 1.5, 4);
      ComponentTestHelper.SetInput(comp, 1.0, 5);
      ComponentTestHelper.SetInput(comp, true, 6);

      var output = (SlabDimensionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(1.5, output.Value.EffectiveWidthLeft.Meters);
      Assert.Equal(1.0, output.Value.EffectiveWidthRight.Meters);
      Assert.True(output.Value.TaperedToNext);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
