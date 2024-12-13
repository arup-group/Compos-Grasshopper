using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using Grasshopper.Kernel.Types;
using Xunit;

namespace ComposGHTests.Result {
  [Collection("GrasshopperFixture collection")]
  public class CodeSatisfiedComponentTests {

    [Fact]
    public void CreateComponentWithInput() {
      var comp = new CodeSatisfied();
      var input = (MemberGoo)ComponentTestHelper.GetOutput(CompFile.FileComponentsTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input);

      var output = (GH_String)ComponentTestHelper.GetOutput(comp);

      Assert.Equal("One or more code requirements are not met", output.Value);
    }
  }
}
