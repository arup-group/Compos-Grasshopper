using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Slab {
  [Collection("GrasshopperFixture collection")]
  public class CreateRebarMaterialComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateRebarMaterial();
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

      var output = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(RebarGrade.EN_500B, output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputsTest() {
      GH_OasysDropDownComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, "498 MPa", 0);

      var output = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(498, output.Value.Fy.Megapascals);
      Assert.True(output.Value.UserDefined);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
