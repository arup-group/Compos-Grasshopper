using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateCustomStudSpacingTests
  {
    public static GH_OasysDropDownComponent CreateCustomStudSpacingMother()
    {
      var comp = new CreateCustomStudSpacing();
      comp.CreateAttributes();

      int i = 0;
      ComponentTestHelper.SetInput(comp, "50 %", i++);
      ComponentTestHelper.SetInput(comp, 15, i++);
      ComponentTestHelper.SetInput(comp, 2, i++);
      ComponentTestHelper.SetInput(comp, 150, i++);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputsTest1()
    {
      var comp = CreateCustomStudSpacingMother();

      comp.SetSelected(0, 0); // change the dropdown to mm

      StudGroupSpacingGoo output = (StudGroupSpacingGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(50, output.Value.DistanceFromStart.As(UnitsNet.Units.RatioUnit.Percent));
      Assert.Equal(15, output.Value.NumberOfRows);
      Assert.Equal(2, output.Value.NumberOfLines);
      Assert.Equal(150, output.Value.Spacing.Millimeters);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateCustomStudSpacingMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateCustomStudSpacingMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
