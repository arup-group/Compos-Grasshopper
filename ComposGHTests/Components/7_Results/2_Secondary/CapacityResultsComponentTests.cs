using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using OasysGH.Parameters;
using Xunit;

namespace ComposGHTests.Result {
  [Collection("GrasshopperFixture collection")]
  public class CapacityResultsComponentTests {

    public static GH_OasysDropDownComponent CapacityResultsComponentMother() {
      var comp = new CapacityResults();
      comp.CreateAttributes();
      MemberGoo input = (MemberGoo)ComponentTestHelper.GetOutput(CompFile.FileComponentsTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input);

      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      var comp = CapacityResultsComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponentWithInput() {
      var comp = CapacityResultsComponentMother();
      comp.ExpireSolution(true);

      int expectedNumberOfResults = 7;

      for (int i = 0; i < comp.Params.Output.Count; i++) {
        comp.Params.Output[i].CollectData();
        for (int j = 0; j < expectedNumberOfResults; j++) {
          GH_UnitNumber output = (GH_UnitNumber)ComponentTestHelper.GetOutput(comp, i, 0, j);
          Assert.NotNull(output);
        }
      }
    }

    [Fact]
    public void DeserializeTest() {
      var comp = CapacityResultsComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
