using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Design {
  [Collection("GrasshopperFixture collection")]
  public class CreateDeflectionLimitComponentTests {

    public static GH_OasysDropDownComponent CreateDeflectionLimitMother() {
      var comp = new CreateDeflectionLimit();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, "35 mm", 0);
      ComponentTestHelper.SetInput(comp, 500, 1);
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      var comp = CreateDeflectionLimitMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentWithInputs() {
      var comp = CreateDeflectionLimitMother();

      DeflectionLimitGoo output = (DeflectionLimitGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(35, output.Value.AbsoluteDeflection.Millimeters);
      Assert.Equal(500, output.Value.SpanOverDeflectionRatio.DecimalFractions);
    }

    [Fact]
    public void DeserializeTest() {
      var comp = CreateDeflectionLimitMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
