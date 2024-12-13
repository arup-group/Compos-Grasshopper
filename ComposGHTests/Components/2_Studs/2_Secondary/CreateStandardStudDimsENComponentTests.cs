using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Stud {
  [Collection("GrasshopperFixture collection")]
  public class CreateStandardStudDimsENComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateStandardStudDimensionsEN();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      var output = (StudDimensionsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(19, output.Value.Diameter.Millimeters);
      Assert.Equal(100, output.Value.Height.Millimeters);
    }

    [Fact]
    public void CreateComponentWithInputsTest1() {
      GH_OasysDropDownComponent comp = ComponentMother();

      int i = 0;
      ComponentTestHelper.SetInput(comp, 450, i++);

      comp.SetSelected(2, 2); // change the dropdown to MPa

      var output = (StudDimensionsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(450, output.Value.Fu.Megapascals);
    }

    [Fact]
    public void CreateComponentWithInputsTest2() {
      GH_OasysDropDownComponent comp = ComponentMother();
      Assert.Single(comp.Params.Input);
      comp.SetSelected(0, 0); // change the dropdown to Custom, adding two new inputs
      Assert.Equal(3, comp.Params.Input.Count);
      comp.SetSelected(3, 0); // change the dropdown to mm

      int i = 0;
      ComponentTestHelper.SetInput(comp, 21, i++);
      ComponentTestHelper.SetInput(comp, 110, i++);
      ComponentTestHelper.SetInput(comp, 500, i++);

      var output = (StudDimensionsGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(21, output.Value.Diameter.Millimeters);
      Assert.Equal(110, output.Value.Height.Millimeters);
      Assert.Equal(500, output.Value.Fu.Megapascals);

      comp.SetSelected(0, 1); // change the dropdown to not Custom, removing two inputs
      Assert.Single(comp.Params.Input);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
