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
  public class CreateWebOpeningTests
  {
    public GH_OasysDropDownComponent CreateWebOpeningComponentMother()
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
      var comp = CreateWebOpeningComponentMother();

      comp.SetSelected(1, 0); // change the dropdown to mm

      WebOpeningGoo output = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(400, output.Value.Width.Millimeters);
      Assert.Equal(300, output.Value.Height.Millimeters);
      Assert.Equal(0.5, output.Value.CentroidPosFromStart.As(UnitsNet.Units.RatioUnit.DecimalFraction));
      Assert.Equal(150, output.Value.CentroidPosFromTop.As(UnitsNet.Units.LengthUnit.Millimeter));
      Assert.Equal(OpeningType.Rectangular, output.Value.WebOpeningType);
    }

    [Fact]
    public void CreateComponentWithInputsTest2()
    {
      var comp = CreateWebOpeningComponentMother();

      comp.SetSelected(0, 1); // change the dropdown to Circular
      comp.SetSelected(1, 1); // change the dropdown to cm

      WebOpeningGoo output = (WebOpeningGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(400, output.Value.Diameter.Centimeters);
      Assert.Equal(0.5, output.Value.CentroidPosFromStart.As(UnitsNet.Units.RatioUnit.DecimalFraction));
      Assert.Equal(150, output.Value.CentroidPosFromTop.As(UnitsNet.Units.LengthUnit.Centimeter));
      Assert.Equal(OpeningType.Circular, output.Value.WebOpeningType);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateWebOpeningComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateWebOpeningComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
