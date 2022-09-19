using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH.Components;
using OasysUnitsNet.Units;

namespace ComposGHTests.Beam
{
  [Collection("GrasshopperFixture collection")]
  public class CreateWebOpeningComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateWebOpening();
      comp.CreateAttributes();

      int i = 0;
      ComponentTestHelper.SetInput(comp, 400, i++);
      ComponentTestHelper.SetInput(comp, 300, i++);
      ComponentTestHelper.SetInput(comp, -0.5, i++);
      ComponentTestHelper.SetInput(comp, 150, i++);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputsTest1()
    {
      var comp = ComponentMother();

      comp.SetSelected(1, 0); // change the dropdown to mm

      WebOpeningGoo output = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(400, output.Value.Width.Millimeters);
      Assert.Equal(300, output.Value.Height.Millimeters);
      Assert.Equal(0.5, output.Value.CentroidPosFromStart.As(RatioUnit.DecimalFraction));
      Assert.Equal(150, output.Value.CentroidPosFromTop.As(LengthUnit.Millimeter));
      Assert.Equal(OpeningType.Rectangular, output.Value.WebOpeningType);
    }

    [Fact]
    public void CreateComponentWithInputsTest2()
    {
      var comp = ComponentMother();

      comp.SetSelected(0, 1); // change the dropdown to Circular
      comp.SetSelected(1, 1); // change the dropdown to cm

      WebOpeningGoo output = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(400, output.Value.Diameter.Centimeters);
      Assert.Equal(0.5, output.Value.CentroidPosFromStart.As(RatioUnit.DecimalFraction));
      Assert.Equal(150, output.Value.CentroidPosFromTop.As(LengthUnit.Centimeter));
      Assert.Equal(OpeningType.Circular, output.Value.WebOpeningType);
    }

    [Fact]
    public void CreateComponentWithInputsTest3()
    {
      var comp = ComponentMother();

      comp.SetSelected(1, 4); // change the dropdown to ft
      WebOpeningStiffenersGoo input5 = (WebOpeningStiffenersGoo)ComponentTestHelper.GetOutput(CreateWebOpeningStiffenerComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input5, 4);

      WebOpeningGoo output = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(400, output.Value.Width.Feet);
      Assert.Equal(300, output.Value.Height.Feet);
      Assert.Equal(0.5, output.Value.CentroidPosFromStart.As(RatioUnit.DecimalFraction));
      Assert.Equal(150, output.Value.CentroidPosFromTop.As(LengthUnit.Foot));
      Assert.Equal(OpeningType.Rectangular, output.Value.WebOpeningType);
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
