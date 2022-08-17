using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateStandardStudDimsTests
  {
    public static GH_OasysDropDownComponent CreateStandardStudDimsMother()
    {
      var comp = new CreateStandardStudDimensions();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = CreateStandardStudDimsMother();
      
      StudDimensionsGoo output = (StudDimensionsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(19, output.Value.Diameter.Millimeters);
      Assert.Equal(100, output.Value.Height.Millimeters);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateStandardStudDimsMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateStandardStudDimsMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
