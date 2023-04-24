using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using Grasshopper.Kernel.Types;
using Xunit;

namespace ComposGHTests.Result {
  [Collection("GrasshopperFixture collection")]
  public class UtilisationsComponentTests {

    [Fact]
    public void CreateComponentWithInput() {
      var comp = new Utilisations();
      var input = (MemberGoo)ComponentTestHelper.GetOutput(CompFile.FileComponentsTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input);

      int expectedOutputCount = 9;
      Assert.Equal(expectedOutputCount, comp.Params.Output.Count);

      for (int i = 0; i < expectedOutputCount; i++) {
        var output = (GH_Number)ComponentTestHelper.GetOutput(comp, i);
        Assert.NotNull(output);
      }
    }
  }
}
