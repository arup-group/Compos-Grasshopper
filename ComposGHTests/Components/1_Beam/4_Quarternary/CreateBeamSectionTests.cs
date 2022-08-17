using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class CreateBeamSectionTests
  {
    [Theory]
    [InlineData("CAT IPE IPE100")]
    [InlineData("STD GI 400 300 250 12 25 20")]
    [InlineData("STD I(cm) 20. 19. 8.5 1.27")]
    public void CreateComponentWithInputsTest(string profile)
    {
      var comp = new CreateBeamSection();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, profile, 0);

      BeamSectionGoo output = (BeamSectionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(profile, output.Value.SectionDescription);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = new CreateBeamSection();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "CAT IPE IPE200", 0);
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = new CreateBeamSection();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, "CAT IPE IPE200", 0);
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
