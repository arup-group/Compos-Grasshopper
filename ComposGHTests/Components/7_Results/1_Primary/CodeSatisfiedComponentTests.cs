using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using Grasshopper.Kernel.Types;
using Xunit;

namespace ComposGHTests.Result {
  [Collection("GrasshopperFixture collection")]
  public class CodeSatisfiedComponentTests {

    [Fact]
    public void CreateComponentWithInput() {
      var comp = new CodeSatisfied();
      MemberGoo input = (MemberGoo)ComponentTestHelper.GetOutput(CompFile.FileComponentsTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input);

      GH_String output = (GH_String)ComponentTestHelper.GetOutput(comp);

      Assert.Equal("One or more code requirements are not met", output.Value);
    }
  }
}
