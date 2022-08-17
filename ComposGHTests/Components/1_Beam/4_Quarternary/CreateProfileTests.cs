using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests.Beam
{
  [Collection("GrasshopperFixture collection")]
  public class CreateProfileTests
  {
    [Fact]
    public void CreateComponentTest()
    {
      var comp = new CreateProfile();
      comp.CreateAttributes();

      // this is the first profile in the catalogue
      string expectedProfile = "CAT BSI-IPE IPEAA80";

      BeamSectionGoo output = (BeamSectionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(expectedProfile, output.Value.SectionDescription);
    }

    [Theory]
    [InlineData("IPE100", "CAT BSI-IPE IPE100")]
    [InlineData("HE 200 B", "CAT BSI-HE HE200B")]
    [InlineData("UC254", "CAT BSI-UC UC254x254x73")]
    public void CreateComponentWithInputsTest(string profile, string expected)
    {
      var comp = new CreateProfile();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, profile, 0);

      BeamSectionGoo output = (BeamSectionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(expected, output.Value.SectionDescription);
    }

    [Fact]
    public void CreateComponentIBeamAsymmetricalTest()
    {
      var comp = new CreateProfile();
      comp.CreateAttributes();

      comp.SetSelected(0, 1); // change the dropdown to Asymmetrical
      comp.SetSelected(1, 3); // change the dropdown to inches

      int i = 0;
      ComponentTestHelper.SetInput(comp, 400, i++);
      ComponentTestHelper.SetInput(comp, 300, i++);
      ComponentTestHelper.SetInput(comp, 250, i++);
      ComponentTestHelper.SetInput(comp, 12, i++);
      ComponentTestHelper.SetInput(comp, 25, i++);
      ComponentTestHelper.SetInput(comp, 20, i++);

      BeamSectionGoo output = (BeamSectionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("STD GI(in) 400 300 250 12 25 20", output.Value.SectionDescription);

      OasysDropDownComponentTestHelper.TestDeserialize(comp, "Asymmetrical");
    }

    [Fact]
    public void CreateComponentIBeamSymmetricalTest()
    {
      var comp = new CreateProfile();
      comp.CreateAttributes();

      comp.SetSelected(0, 2); // change the dropdown to Symmetrical
      comp.SetSelected(1, 1); // change the dropdown to cm

      int i = 0;
      ComponentTestHelper.SetInput(comp, "4 m", i++);
      ComponentTestHelper.SetInput(comp, "3000 mm", i++);
      ComponentTestHelper.SetInput(comp, 12, i++);
      ComponentTestHelper.SetInput(comp, 20, i++);

      BeamSectionGoo output = (BeamSectionGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("STD I(cm) 400 300 12 20", output.Value.SectionDescription);

      OasysDropDownComponentTestHelper.TestDeserialize(comp, "Symmetrical");
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = new CreateProfile();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = new CreateProfile();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
