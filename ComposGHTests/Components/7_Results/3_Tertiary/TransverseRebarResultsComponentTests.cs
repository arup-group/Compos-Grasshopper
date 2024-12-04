using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using Grasshopper.Kernel.Types;
using OasysGH.Components;
using OasysGH.Parameters;
using Xunit;

namespace ComposGHTests.Result {
  [Collection("GrasshopperFixture collection")]
  public class TransverseRebarResultsComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new TransverseRebarResults();
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

      int expectedNumberOfResults = 1;

      for (int i = 0; i < comp.Params.Output.Count; i++) {
        if (i == 1) // natural freq is single item results
        {
          var output = (GH_String)ComponentTestHelper.GetOutput(comp, i);
          Assert.NotNull(output);
          Assert.True(output.Value.Length > 0);
        } else {
          comp.Params.Output[i].CollectData();
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
