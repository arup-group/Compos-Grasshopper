using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateSlabDimensionTests
  {
    public static GH_OasysDropDownComponent CreateSlabDimensionMother()
    {
      var comp = new CreateSlabDimension();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, 130, 1);
      ComponentTestHelper.SetInput(comp, 1700, 2);
      ComponentTestHelper.SetInput(comp, 1200, 3);

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreateSlabDimensionMother();
      comp.SetSelected(0, 0); // change dropdown to mm
      SlabDimensionGoo output = (SlabDimensionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(0, output.Value.StartPosition.Value);
      Assert.Equal(130, output.Value.OverallDepth.Millimeters);
      Assert.Equal(1.7, output.Value.AvailableWidthLeft.Meters);
      Assert.Equal(1.2, output.Value.AvailableWidthRight.Meters);
      Assert.False(output.Value.TaperedToNext);
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = CreateSlabDimensionMother();

      comp.SetSelected(0, 2); // change dropdown to m

      ComponentTestHelper.SetInput(comp, 1.5, 4);
      ComponentTestHelper.SetInput(comp, 1.0, 5);
      ComponentTestHelper.SetInput(comp, true, 6);

      SlabDimensionGoo output = (SlabDimensionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(1.5, output.Value.EffectiveWidthLeft.Meters);
      Assert.Equal(1.0, output.Value.EffectiveWidthRight.Meters);
      Assert.True(output.Value.TaperedToNext);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateSlabDimensionMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateSlabDimensionMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
