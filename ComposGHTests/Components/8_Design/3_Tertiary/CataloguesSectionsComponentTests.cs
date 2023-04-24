using ComposGH.Components;
using ComposGHTests.Helpers;
using Grasshopper.Kernel.Types;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Design {
  [Collection("GrasshopperFixture collection")]
  public class CataloguesSectionsComponentTests {

    public static GH_OasysDropDownComponent CataloguesSectionsMother() {
      var comp = new CataloguesSections();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      GH_OasysDropDownComponent comp = CataloguesSectionsMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponent() {
      GH_OasysDropDownComponent comp = CataloguesSectionsMother();

      var output = (GH_Integer)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(26, (int)output.Value);
    }

    [Fact]
    public void DeserializeTest() {
      GH_OasysDropDownComponent comp = CataloguesSectionsMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
