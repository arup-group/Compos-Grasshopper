using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using Grasshopper.Kernel.Types;

namespace ComposGHTests.Design
{
  [Collection("GrasshopperFixture collection")]
  public class CreateBeamSizeLimitsComponentTests
  {
    public static GH_OasysDropDownComponent CreateBeamSizeLimitsMother()
    {
      var comp = new CreateBeamSizeLimits();
      comp.CreateAttributes();
      
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreateBeamSizeLimitsMother();

      BeamSizeLimitsGoo output = (BeamSizeLimitsGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(20, output.Value.MinDepth.Centimeters);
      Assert.Equal(100, output.Value.MaxDepth.Centimeters);
      Assert.Equal(10, output.Value.MinWidth.Centimeters);
      Assert.Equal(50, output.Value.MaxWidth.Centimeters);
    }

    [Fact]
    public void CreateComponentWithInputs()
    {
      var comp = CreateBeamSizeLimitsMother();

      comp.SetSelected(0, 0); // change dropdown to mm

      ComponentTestHelper.SetInput(comp, 250, 0);
      ComponentTestHelper.SetInput(comp, 1200, 1);
      ComponentTestHelper.SetInput(comp, 150, 2);
      ComponentTestHelper.SetInput(comp, 700, 3);

      BeamSizeLimitsGoo output = (BeamSizeLimitsGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(250, output.Value.MinDepth.Millimeters);
      Assert.Equal(1200, output.Value.MaxDepth.Millimeters);
      Assert.Equal(150, output.Value.MinWidth.Millimeters);
      Assert.Equal(700, output.Value.MaxWidth.Millimeters);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateBeamSizeLimitsMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateBeamSizeLimitsMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
