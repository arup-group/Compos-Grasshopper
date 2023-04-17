using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using Xunit;

namespace ComposGHTests.Beam {
  [Collection("GrasshopperFixture collection")]
  public class CreateStandardASNZSteelMaterialComponentTests {

    [Fact]
    public void ChangeDropDownTest() {
      var comp = new CreateStandardASNZSteelMaterial();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentTest() {
      var comp = new CreateStandardASNZSteelMaterial();
      comp.CreateAttributes();

      SteelMaterialGoo output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StandardASNZSteelMaterialGrade.C450_AS1163, ((ASNZSteelMaterial)output.Value).Grade);
    }

    [Fact]
    public void CreateComponentWithInputsTest() {
      var comp = new CreateStandardASNZSteelMaterial();
      comp.CreateAttributes();

      string input1 = "Gr250L15 AS3678";
      ComponentTestHelper.SetInput(comp, input1, 0);

      SteelMaterialGoo output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StandardASNZSteelMaterialGrade.Gr250L15_AS3678, ((ASNZSteelMaterial)output.Value).Grade);
    }

    [Fact]
    public void DeserializeTest() {
      var comp = new CreateStandardASNZSteelMaterial();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
