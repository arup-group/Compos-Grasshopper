using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using OasysUnits.Units;
using Xunit;

namespace ComposGHTests.Load {
  [Collection("GrasshopperFixture collection")]
  public class CreateTriLinearLoadComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateTriLinearLoad();
      comp.CreateAttributes();

      comp.SetSelected(0, 1); // change dropdown to kN

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
      comp.SetSelected(2, 2); // change dropdown to m
      LengthUnit length = LengthUnit.Meter;

      var output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (TriLinearLoad)output.Value;

      int i = 0;
      Assert.Equal(LoadDistribution.Line, load.Distribution);
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.Position.As(length));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.Position.As(length));
    }

    [Fact]
    public void CreateComponent2() {
      GH_OasysDropDownComponent comp = ComponentMother();

      comp.SetSelected(0, 1); // change dropdown to area
      comp.SetSelected(1, 5); // change dropdown to kN/m2
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;
      comp.SetSelected(2, 2); // change dropdown to m
      LengthUnit length = LengthUnit.Meter;

      var output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (TriLinearLoad)output.Value;

      int i = 0;
      Assert.Equal(LoadDistribution.Area, load.Distribution);
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.Position.As(length));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.Position.As(length));
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
