using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using UnitsNet.Units;

namespace ComposGHTests.Load
{
  [Collection("GrasshopperFixture collection")]
  public class CreateLinearLoadComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateLinearLoad();
      comp.CreateAttributes();

      comp.SetSelected(0, 1); // change dropdown to kN

      for (int i = 0; i < comp.Params.Input.Count; i++)
        ComponentTestHelper.SetInput(comp, (i + 1) * 1, i);

      return comp;
    }

    [Fact]
    public void CreateComponent1()
    {
      var comp = ComponentMother();

      comp.SetSelected(0, 0); // change dropdown to line
      comp.SetSelected(1, 5); // change dropdown to kN/m
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      LoadGoo output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      LinearLoad load = (LinearLoad)output.Value;
      
      int i = 0;
      Assert.Equal(LoadDistribution.Line, load.Distribution);
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalLive.As(force));
    }

    [Fact]
    public void CreateComponent2()
    {
      var comp = ComponentMother();

      comp.SetSelected(0, 1); // change dropdown to area
      comp.SetSelected(1, 5); // change dropdown to kN/m2
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      LoadGoo output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      LinearLoad load = (LinearLoad)output.Value;

      int i = 0;
      Assert.Equal(LoadDistribution.Area, load.Distribution);
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW1.FinalLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.LoadW2.FinalLive.As(force));
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
