using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateCatalogueDeckTests
  {
    public static GH_OasysDropDownComponent CreateCatalogueDeckMother()
    {
      var comp = new CreateCatalogueDeck();
      comp.CreateAttributes();

      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = CreateCatalogueDeckMother();

      comp.SetSelected(0, 0); // set dropdown to mm
      comp.SetSelected(1, 2); // set dropdown to MPa

      DeckingGoo output = (DeckingGoo)ComponentTestHelper.GetOutput(comp);
      CatalogueDecking catDecking = (CatalogueDecking)output.Value;
      Assert.Equal("RLD", catDecking.Catalogue);
      Assert.Equal("Holorib S280/S350 (0.9)", catDecking.Profile);
    }

    [Fact]
    public void CreateComponentWithInputs()
    {
      var comp = CreateCatalogueDeckMother();

      DeckingConfigurationGoo input1 = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(CreateDeckingConfigurationTests.CreateDeckingConfigurationMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      DeckingGoo output = (DeckingGoo)ComponentTestHelper.GetOutput(comp);
      CatalogueDecking catDecking = (CatalogueDecking)output.Value;
      Duplicates.AreEqual(input1.Value, catDecking.DeckingConfiguration);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateCatalogueDeckMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateCatalogueDeckMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
