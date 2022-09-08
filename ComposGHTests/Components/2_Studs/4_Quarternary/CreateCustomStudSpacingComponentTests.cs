using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.Units;
using OasysGH.Components;

namespace ComposGHTests.Stud
{
  [Collection("GrasshopperFixture collection")]
  public class CreateCustomStudSpacingComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
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
    public void CreateComponentWithInputsTest()
    {
      var comp = ComponentMother();

      comp.SetSelected(0, 0); // change the dropdown to mm

      StudGroupSpacingGoo output = (StudGroupSpacingGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(50, output.Value.DistanceFromStart.As(RatioUnit.Percent));
      Assert.Equal(15, output.Value.NumberOfRows);
      Assert.Equal(2, output.Value.NumberOfLines);
      Assert.Equal(150, output.Value.Spacing.Millimeters);
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
