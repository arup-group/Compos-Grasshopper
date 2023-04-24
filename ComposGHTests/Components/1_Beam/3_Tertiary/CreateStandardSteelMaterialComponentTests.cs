using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using Xunit;

namespace ComposGHTests.Beam {
  [Collection("GrasshopperFixture collection")]
  public class CreateStandardSteelMaterialComponentTests {

    [Fact]
    public void ChangeDropDownTest() {
      var comp = new CreateStandardSteelMaterial();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentTest() {
      var comp = new CreateStandardSteelMaterial();
      comp.CreateAttributes();

      var output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StandardSteelGrade.S235, output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputsTest() {
      var comp = new CreateStandardSteelMaterial();
      comp.CreateAttributes();

      string input1 = "S275";
      ComponentTestHelper.SetInput(comp, input1, 0);

      var output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StandardSteelGrade.S275, output.Value.Grade);
    }

    [Fact]
    public void DeserializeTest() {
      var comp = new CreateStandardSteelMaterial();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
