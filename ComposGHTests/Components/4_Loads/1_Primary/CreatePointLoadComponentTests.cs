using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using UnitsNet.Units;

namespace ComposGHTests.Load
{
  [Collection("GrasshopperFixture collection")]
  public class CreatePointLoadComponentTests
  {
    public static GH_OasysDropDownComponent CreatePointLoadMother()
    {
      var comp = new CreatePointLoad();
      comp.CreateAttributes();

      for (int i = 0; i < comp.Params.Input.Count; i++)
        ComponentTestHelper.SetInput(comp, (i + 1) * 1, i);

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreatePointLoadMother();

      comp.SetSelected(0, 1); // change dropdown to kN
      ForceUnit force = ForceUnit.Kilonewton;
      comp.SetSelected(1, 2); // change dropdown to m
      LengthUnit length = LengthUnit.Meter;

      LoadGoo output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      PointLoad load = (PointLoad)output.Value;
      
      int i = 0;
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.Position.As(length));
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreatePointLoadMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreatePointLoadMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
