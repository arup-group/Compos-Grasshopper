using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests.Beam
{
  [Collection("GrasshopperFixture collection")]
  public class CreateStandardSteelMaterialComponentTests
  {
    [Fact]
    public void CreateComponentTest()
    {
      var comp = new CreateStandardSteelMaterial();
      comp.CreateAttributes();

      SteelMaterialGoo output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StandardSteelGrade.S235, output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputsTest()
    {
      var comp = new CreateStandardSteelMaterial();
      comp.CreateAttributes();

      string input1 = "S275";
      ComponentTestHelper.SetInput(comp, input1, 0);
      
      SteelMaterialGoo output = (SteelMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(StandardSteelGrade.S275, output.Value.Grade);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = new CreateStandardSteelMaterial();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = new CreateStandardSteelMaterial();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
