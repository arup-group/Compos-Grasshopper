using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateRebarMaterialComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateRebarMaterial();
      comp.CreateAttributes();
      
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = ComponentMother();

      ReinforcementMaterialGoo output = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(RebarGrade.EN_500B, output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputsTest()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, "498 MPa", 0);

      ReinforcementMaterialGoo output = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(498, output.Value.Fy.Megapascals);
      Assert.True(output.Value.UserDefined);
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
