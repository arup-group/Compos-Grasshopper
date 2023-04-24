using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using Xunit;

namespace ComposGHTests.Beam {
  [Collection("GrasshopperFixture collection")]
  public class CreateStandardENSteelMaterialComponentTests {

    [Fact]
    public void ChangeDropDownTest() {
      var comp = new CreateStandardENSteelMaterial();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentTest() {
      var comp = new CreateStandardENSteelMaterial();
      comp.CreateAttributes();

      var output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StandardSteelGrade.S235, output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputsTest() {
      var comp = new CreateStandardENSteelMaterial();
      comp.CreateAttributes();

      string input1 = "355";
      ComponentTestHelper.SetInput(comp, input1, 0);

      var output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StandardSteelGrade.S355, output.Value.Grade);
    }

    [Fact]
    public void DeserializeTest() {
      var comp = new CreateStandardENSteelMaterial();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
