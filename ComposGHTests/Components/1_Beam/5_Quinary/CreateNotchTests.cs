using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.GH;
using Grasshopper.Kernel.Types;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateNotchTests
  {
    public GH_OasysDropDownComponent CreateNotchComponentMother()
    {
      var comp = new CreateNotch();
      comp.CreateAttributes();

      int i = 0;
      ComponentTestHelper.SetInput(comp, 400, i++);
      ComponentTestHelper.SetInput(comp, 300, i++);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputsTest1()
    {
      var comp = CreateNotchComponentMother();
      comp.CreateAttributes();

      comp.SetSelected(1, 0); // change the dropdown to mm

      WebOpeningGoo output1 = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp, 0, 0, 0);
      Assert.Equal(400, output1.Value.Width.Millimeters);
      Assert.Equal(300, output1.Value.Height.Millimeters);
      Assert.Equal(OpeningType.Start_notch, output1.Value.WebOpeningType);
      WebOpeningGoo output2 = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp, 0, 0, 1);
      Assert.Equal(400, output2.Value.Width.Millimeters);
      Assert.Equal(300, output2.Value.Height.Millimeters);
      Assert.Equal(OpeningType.End_notch, output2.Value.WebOpeningType);
    }

    [Fact]
    public void CreateComponentWithInputsTest2()
    {
      var comp = CreateNotchComponentMother();
      comp.CreateAttributes();

      comp.SetSelected(0, 1); // change the dropdown to Start
      comp.SetSelected(1, 1); // change the dropdown to cm

      WebOpeningGoo output = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(400, output.Value.Width.Centimeters);
      Assert.Equal(300, output.Value.Height.Centimeters);
      Assert.Equal(OpeningType.Start_notch, output.Value.WebOpeningType);
    }

    [Fact]
    public void CreateComponentWithInputsTest3()
    {
      var comp = CreateNotchComponentMother();
      comp.CreateAttributes();

      comp.SetSelected(0, 2); // change the dropdown to End
      comp.SetSelected(1, 2); // change the dropdown to m

      WebOpeningGoo output = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(400, output.Value.Width.Meters);
      Assert.Equal(300, output.Value.Height.Meters);
      Assert.Equal(OpeningType.End_notch, output.Value.WebOpeningType);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateNotchComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateNotchComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
