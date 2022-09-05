using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using ComposAPI;
using OasysGH.Components;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateRebarMaterialComponentTests
  {
    public static GH_OasysDropDownComponent CreateRebarMaterialMother()
    {
      var comp = new CreateRebarMaterial();
      comp.CreateAttributes();
      
      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      var comp = CreateRebarMaterialMother();

      ReinforcementMaterialGoo output = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(RebarGrade.EN_500B, output.Value.Grade);
    }

    [Fact]
    public void CreateComponentWithInputsTest()
    {
      var comp = CreateRebarMaterialMother();

      ComponentTestHelper.SetInput(comp, "498 MPa", 0);

      ReinforcementMaterialGoo output = (ReinforcementMaterialGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(498, output.Value.Fy.Megapascals);
      Assert.True(output.Value.UserDefined);
    }

    [Fact]
    public void DeserializeTest()
    {
      var comp = CreateRebarMaterialMother();
      OasysDropDownComponentTestHelper.TestDeserialize(comp);
    }

    [Fact]
    public void ChangeDropDownTest()
    {
      var comp = CreateRebarMaterialMother();
      OasysDropDownComponentTestHelper.ChangeDropDownTest(comp);
    }
  }
}
