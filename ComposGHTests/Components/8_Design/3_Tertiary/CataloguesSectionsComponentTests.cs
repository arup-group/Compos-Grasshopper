using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using Grasshopper.Kernel.Types;

namespace ComposGHTests.Design
{
  [Collection("GrasshopperFixture collection")]
  public class CataloguesSectionsComponentTests
  {
    public static GH_OasysDropDownComponent CataloguesSectionsMother()
    {
      var comp = new CataloguesSections();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CataloguesSectionsMother();

      GH_Integer output = (GH_Integer)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(26, (int)output.Value);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CataloguesSectionsMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CataloguesSectionsMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
