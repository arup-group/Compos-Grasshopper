using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using OasysUnitsNet.Units;
using OasysGH.Components;

namespace ComposGHTests.Load
{
  [Collection("GrasshopperFixture collection")]
  public class CreateAxialLoadComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateAxialLoad();
      comp.CreateAttributes();

      comp.SetSelected(0, 1); // change dropdown to kN

      for (int i = 0; i < comp.Params.Input.Count; i++)
        ComponentTestHelper.SetInput(comp, (i + 1) * 1, i);

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = ComponentMother();

      comp.SetSelected(0, 1); // change dropdown to kN
      ForceUnit force = ForceUnit.Kilonewton;
      comp.SetSelected(1, 0); // change dropdown to mm
      LengthUnit length = LengthUnit.Millimeter;

      LoadGoo output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      AxialLoad load = (AxialLoad)output.Value;
      
      int i = 0;
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Depth1.As(length));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Depth2.As(length));
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
