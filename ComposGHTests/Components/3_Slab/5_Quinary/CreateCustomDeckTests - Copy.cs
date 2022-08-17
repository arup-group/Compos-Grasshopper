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
    public void CreateComponentWithInputs1()
    {
      var comp = CreateCatalogueDeckMother();

      comp.SetSelected(0, 0); // set dropdown to mm
      comp.SetSelected(1, 2); // set dropdown to MPa

      DeckingGoo output = (DeckingGoo)ComponentTestHelper.GetOutput(comp);
      CatalogueDecking catDecking = (CustomDecking)output.Value;
      Assert.Equal(11, catDecking.b1.Millimeters);
      Assert.Equal(12, catDecking.b2.Millimeters);
      Assert.Equal(13, catDecking.b3.Millimeters);
      Assert.Equal(14.5, catDecking.b4.Millimeters);
      Assert.Equal(9, catDecking.b5.Millimeters);
      Assert.Equal(8, catDecking.Depth.Millimeters);
      Assert.Equal(7, catDecking.Thickness.Millimeters);
      Assert.Equal(600, catDecking.Strength.Megapascals);
    }

    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = CreateCustomDeckMother();

      DeckingConfigurationGoo input9 = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(CreateDeckingConfigurationTests.CreateDeckingConfigurationMother());
      ComponentTestHelper.SetInput(comp, input9, 8);

      comp.SetSelected(0, 1); // set dropdown to cm
      comp.SetSelected(1, 0); // set dropdown to Pa

      DeckingGoo output = (DeckingGoo)ComponentTestHelper.GetOutput(comp);
      CustomDecking customDecking = (CustomDecking)output.Value;
      Assert.Equal(11, customDecking.b1.Centimeters);
      Assert.Equal(12, customDecking.b2.Centimeters);
      Assert.Equal(13, customDecking.b3.Centimeters);
      Assert.Equal(14.5, customDecking.b4.Centimeters);
      Assert.Equal(9, customDecking.b5.Centimeters);
      Assert.Equal(8, customDecking.Depth.Centimeters);
      Assert.Equal(7, customDecking.Thickness.Centimeters);
      Assert.Equal(600, customDecking.Strength.Pascals);
      Duplicates.AreEqual(input9.Value, customDecking.DeckingConfiguration);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateCustomDeckMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateCustomDeckMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
