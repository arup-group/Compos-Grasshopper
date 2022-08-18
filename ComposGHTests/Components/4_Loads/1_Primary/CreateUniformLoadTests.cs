using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using UnitsNet.Units;

namespace ComposGHTests.Load
{
  [Collection("GrasshopperFixture collection")]
  public class CreateUniformLoadTests
  {
    public static GH_OasysDropDownComponent CreateUniformLoadMother()
    {
      var comp = new CreateUniformLoad();
      comp.CreateAttributes();

      for (int i = 0; i < comp.Params.Input.Count; i++)
        ComponentTestHelper.SetInput(comp, (i + 1) * 1, i);

      return comp;
    }

    [Fact]
    public void CreateComponent1()
    {
      var comp = CreateUniformLoadMother();

      comp.SetSelected(0, 0); // change dropdown to line
      comp.SetSelected(1, 5); // change dropdown to kN/m
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      LoadGoo output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      UniformLoad load = (UniformLoad)output.Value;
      
      int i = 0;
      Assert.Equal(LoadDistribution.Line, load.Distribution);
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalLive.As(force));
    }

    [Fact]
    public void CreateComponent2()
    {
      var comp = CreateUniformLoadMother();

      comp.SetSelected(0, 1); // change dropdown to area
      comp.SetSelected(1, 5); // change dropdown to kN/m2
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      LoadGoo output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      UniformLoad load = (UniformLoad)output.Value;

      int i = 0;
      Assert.Equal(LoadDistribution.Area, load.Distribution);
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.ConstantLive.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalDead.As(force));
      Assert.Equal((i++ + 1) * 1, load.Load.FinalLive.As(force));
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateUniformLoadMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateUniformLoadMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
