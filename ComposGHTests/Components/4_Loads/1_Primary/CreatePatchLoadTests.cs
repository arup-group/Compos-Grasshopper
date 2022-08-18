using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using System;
using Grasshopper.Kernel.Parameters;
using ComposAPI;
using UnitsNet.Units;
using UnitsNet;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreatePatchLoadTests
  {
    public static GH_OasysDropDownComponent CreatePatchLoadMother()
    {
      var comp = new CreatePatchLoad();
      comp.CreateAttributes();

      for (int i = 0; i < comp.Params.Input.Count; i++)
        ComponentTestHelper.SetInput(comp, (i + 1) * 1, i);

      return comp;
    }

    [Fact]
    public void CreateComponent1()
    {
      var comp = CreatePatchLoadMother();

      comp.SetSelected(0, 0); // change dropdown to line
      comp.SetSelected(1, 5); // change dropdown to kN/m
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;
      comp.SetSelected(2, 2); // change dropdown to m
      LengthUnit length = LengthUnit.Meter;

      LoadGoo output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      PatchLoad load = (PatchLoad)output.Value;
      
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
    public void CreateComponent2()
    {
      var comp = CreatePatchLoadMother();

      comp.SetSelected(0, 1); // change dropdown to area
      comp.SetSelected(1, 5); // change dropdown to kN/m2
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;
      comp.SetSelected(2, 2); // change dropdown to m
      LengthUnit length = LengthUnit.Meter;

      LoadGoo output = (LoadGoo)ComponentTestHelper.GetOutput(comp);
      PatchLoad load = (PatchLoad)output.Value;

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
    public void DeserializeTest()
    {
      var comp = CreatePatchLoadMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreatePatchLoadMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
