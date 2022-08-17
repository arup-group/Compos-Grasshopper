using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateWebOpeningStiffenerTests
  {
    public static GH_OasysDropDownComponent CreateWebOpeningStiffenerComponentMother()
    {
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
    public void CreateComponentWithInputsTest1()
    {
      var comp = CreateWebOpeningStiffenerComponentMother();

      comp.SetSelected(1, 0); // change the dropdown to mm

      WebOpeningStiffenersGoo output = (WebOpeningStiffenersGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(50, output.Value.DistanceFrom.Millimeters);
      Assert.Equal(100, output.Value.TopStiffenerWidth.Millimeters);
      Assert.Equal(10, output.Value.TopStiffenerThickness.Millimeters);
      Assert.Equal(100, output.Value.BottomStiffenerWidth.Millimeters);
      Assert.Equal(10, output.Value.BottomStiffenerThickness.Millimeters);
      Assert.True(output.Value.isBothSides);
      Assert.False(output.Value.isNotch);
    }

    [Fact]
    public void CreateComponentWithInputsTest2()
    {
      var comp = CreateWebOpeningStiffenerComponentMother();

      comp.SetSelected(0, 1); // change the dropdown to notch
      comp.SetSelected(1, 0); // change the dropdown to mm
      
      var input = new Param_Boolean();
      input.CreateAttributes();
      input.PersistentData.Append(new GH_Boolean(false));
      comp.Params.Input[0] = input;

      WebOpeningStiffenersGoo output = (WebOpeningStiffenersGoo)ComponentTestHelper.GetOutput(comp);
      Assert.False(output.Value.isBothSides);
      Assert.True(output.Value.isNotch);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateWebOpeningStiffenerComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateWebOpeningStiffenerComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
