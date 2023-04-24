using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Slab {
  [Collection("GrasshopperFixture collection")]
  public class CreateERatioComponentTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateERatio();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysComponent comp = ComponentMother();
      var output = (ERatioGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(6.24304, output.Value.ShortTerm);
      Assert.Equal(23.5531, output.Value.LongTerm);
      Assert.Equal(5.526, output.Value.Vibration);
      Assert.Equal(22.3517, output.Value.Shrinkage);
    }
  }
}
