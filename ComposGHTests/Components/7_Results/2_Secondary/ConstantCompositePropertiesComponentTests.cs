using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using Xunit;

namespace ComposGHTests.Result {
  [Collection("GrasshopperFixture collection")]
  public class ConstantCompositePropertiesComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new ConstantCompositeProperties();
      comp.CreateAttributes();
      var input = (MemberGoo)ComponentTestHelper.GetOutput(CompFile.FileComponentsTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input);

      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentWithInput() {
      GH_OasysDropDownComponent comp = ComponentMother();
      comp.ExpireSolution(true);

      int expectedNumberOfResults = 7;

      for (int i = 0; i < comp.Params.Output.Count; i++) {
        comp.Params.Output[i].CollectData();

        if (i == 4) // natural freq is single item results
        {
          var output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i);
          Assert.NotNull(output);
        } else {
          for (int j = 0; j < expectedNumberOfResults; j++) {
            var output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i, 0, j);
            Assert.NotNull(output);
          }
        }
      }
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
