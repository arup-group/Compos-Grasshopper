using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using OasysGH.Components;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateCustomDeckComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateCustomDeck();
      comp.CreateAttributes();

      int i = 0;
      ComponentTestHelper.SetInput(comp, 11, i++);
      ComponentTestHelper.SetInput(comp, 12, i++);
      ComponentTestHelper.SetInput(comp, 13, i++);
      ComponentTestHelper.SetInput(comp, 14.5, i++);
      ComponentTestHelper.SetInput(comp, 9, i++);
      ComponentTestHelper.SetInput(comp, 8, i++);
      ComponentTestHelper.SetInput(comp, 7, i++);
      ComponentTestHelper.SetInput(comp, 600, i++);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = ComponentMother();

      comp.SetSelected(0, 0); // set dropdown to mm
      comp.SetSelected(1, 2); // set dropdown to MPa

      DeckingGoo output = (DeckingGoo)ComponentTestHelper.GetOutput(comp);
      CustomDecking customDecking = (CustomDecking)output.Value;
      Assert.Equal(11, customDecking.b1.Millimeters);
      Assert.Equal(12, customDecking.b2.Millimeters);
      Assert.Equal(13, customDecking.b3.Millimeters);
      Assert.Equal(14.5, customDecking.b4.Millimeters);
      Assert.Equal(9, customDecking.b5.Millimeters);
      Assert.Equal(8, customDecking.Depth.Millimeters);
      Assert.Equal(7, customDecking.Thickness.Millimeters);
      Assert.Equal(600, customDecking.Strength.Megapascals);
    }

    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = ComponentMother();

      DeckingConfigurationGoo input9 = (DeckingConfigurationGoo)ComponentTestHelper.GetOutput(CreateDeckingConfigurationComponentTests.ComponentMother());
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
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = ComponentMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
