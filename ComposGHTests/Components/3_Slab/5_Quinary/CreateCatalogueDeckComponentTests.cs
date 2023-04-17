using ComposAPI;
using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helpers;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Slab {
  [Collection("GrasshopperFixture collection")]
  public class CreateCatalogueDeckComponentTests {

    public static GH_OasysDropDownComponent ComponentMother() {
      var comp = new CreateCatalogueDeck();
      comp.CreateAttributes();

      return comp;
    }

    [Fact]
    public void ChangeDropDownTest() {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }

    [Fact]
    public void CreateComponent() {
      var comp = ComponentMother();

      comp.SetSelected(0, 0); // set dropdown to mm
      comp.SetSelected(1, 2); // set dropdown to MPa

      DeckingGoo output = (DeckingGoo)ComponentTestHelper.GetOutput(comp);
      CatalogueDecking catDecking = (CatalogueDecking)output.Value;
      Assert.Equal("RLD", catDecking.Catalogue);
      Assert.Equal("Holorib S280/S350 (0.9)", catDecking.Profile);
    }

    [Fact]
    public void CreateComponentWithInputs() {
      var comp = ComponentMother();

      DeckingConfigurationGoo input1 = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(CreateDeckingConfigurationComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      DeckingGoo output = (DeckingGoo)ComponentTestHelper.GetOutput(comp);
      CatalogueDecking catDecking = (CatalogueDecking)output.Value;
      Duplicates.AreEqual(input1.Value, catDecking.DeckingConfiguration);
    }

    [Fact]
    public void DeserializeTest() {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }
  }
}
