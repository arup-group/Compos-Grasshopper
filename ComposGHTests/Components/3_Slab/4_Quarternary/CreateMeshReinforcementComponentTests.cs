using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.Units;
using ComposAPI;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateMeshReinforcementComponentTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new CreateMeshReinforcement();
      comp.CreateAttributes();

      int i = 0;
      ComponentTestHelper.SetInput(comp, "35 mm", i++);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputsTest()
    {
      var comp = ComponentMother();

      MeshReinforcementGoo output = (MeshReinforcementGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(35, output.Value.Cover.As(LengthUnit.Millimeter));
      Assert.False(output.Value.Rotated);
      Assert.Equal(ReinforcementMeshType.A393, output.Value.MeshType);
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
