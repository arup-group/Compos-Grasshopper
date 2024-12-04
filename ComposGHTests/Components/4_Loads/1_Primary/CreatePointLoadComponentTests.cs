using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using OasysUnits.Units;
using Xunit;

namespace ComposGHTests.Load {
  [Collection("GrasshopperFixture collection")]
  public class CreatePointLoadComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreatePointLoad();
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
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = ComponentMother();

      comp.SetSelected(0, 1); // change dropdown to kN
      ForceUnit force = ForceUnit.Kilonewton;
      comp.SetSelected(1, 2); // change dropdown to m
      LengthUnit length = LengthUnit.Meter;

      var output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      var load = (PointLoad)output.Value;

      int i = 0;
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.Position.As(length));
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
