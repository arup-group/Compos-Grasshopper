using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Beam {
  [Collection("GrasshopperFixture collection")]
  public class CreateWebOpeningStiffenerComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateWebOpeningStiffener();
      comp.CreateAttributes();

      int i = 0;
      ComponentTestHelper.SetInput(comp, true, i++);
      ComponentTestHelper.SetInput(comp, 50, i++);
      ComponentTestHelper.SetInput(comp, 100, i++);
      ComponentTestHelper.SetInput(comp, 10, i++);
      ComponentTestHelper.SetInput(comp, 100, i++);
      ComponentTestHelper.SetInput(comp, 10, i++);
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentWithInputsTest1() {
      GH_OasysDropDownComponent comp = ComponentMother();

      comp.SetSelected(1, 0); // change the dropdown to mm

      var output = (WebOpeningStiffenersGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(50, output.Value.DistanceFrom.Millimeters);
      Assert.Equal(100, output.Value.TopStiffenerWidth.Millimeters);
      Assert.Equal(10, output.Value.TopStiffenerThickness.Millimeters);
      Assert.Equal(100, output.Value.BottomStiffenerWidth.Millimeters);
      Assert.Equal(10, output.Value.BottomStiffenerThickness.Millimeters);
      Assert.True(output.Value.IsBothSides);
      Assert.False(output.Value.IsNotch);
    }

    [Fact]
    public void CreateComponentWithInputsTest2() {
      GH_OasysDropDownComponent comp = ComponentMother();

      comp.SetSelected(0, 1); // change the dropdown to notch
      comp.SetSelected(1, 0); // change the dropdown to mm

      var input = new Param_Boolean();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Boolean(false));
      comp.Params.Input[0] = input;

      var output = (WebOpeningStiffenersGoo)ComponentTestHelper.GetOutput(comp);
      Assert.False(output.Value.IsBothSides);
      Assert.True(output.Value.IsNotch);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
