using ComposGH.Components;
using ComposGH.Parameters;
using ComposGHTests.Helper;
using OasysGH.Components;
using Xunit;

namespace ComposGHTests.Slab {
  [Collection("GrasshopperFixture collection")]
  public class CreateSlabComponentTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new CreateSlab();
      comp.CreateAttributes();

      var input1 = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(CreateConcreteMaterialENComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      var input2 = (SlabDimensionGoo)ComponentTestHelper.GetOutput(CreateSlabDimensionComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input2, 1);

      var input3 = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(CreateTransverseReinforcementComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input3, 2);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1() {
      GH_OasysComponent comp = ComponentMother();
      var output = (SlabGoo)ComponentTestHelper.GetOutput(comp);

      var expexted_input1 = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(CreateConcreteMaterialENComponentTests.ComponentMother());
      Duplicates.AreEqual(expexted_input1.Value, output.Value.Material);

      var expexted_input2 = (SlabDimensionGoo)ComponentTestHelper.GetOutput(CreateSlabDimensionComponentTests.ComponentMother());
      Duplicates.AreEqual(expexted_input2.Value, output.Value.Dimensions[0]);

      var expexted_input3 = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(CreateTransverseReinforcementComponentTests.ComponentMother());
      Duplicates.AreEqual(expexted_input3.Value, output.Value.Transverse);
    }

    [Fact]
    public void CreateComponentWithInputs2() {
      GH_OasysComponent comp = ComponentMother();

      var input4 = (MeshReinforcementGoo)ComponentTestHelper.GetOutput(CreateMeshReinforcementComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input4, 3);

      var input5 = (DeckingGoo)ComponentTestHelper.GetOutput(CreateCatalogueDeckComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input5, 4);

      var output = (SlabGoo)ComponentTestHelper.GetOutput(comp);
      Duplicates.AreEqual(input4.Value, output.Value.Mesh);
      Duplicates.AreEqual(input5.Value, output.Value.Decking);
    }

    [Fact]
    public void CreateComponentWithInputs3() {
      GH_OasysComponent comp = ComponentMother();

      var input2_2 = (SlabDimensionGoo)ComponentTestHelper.GetOutput(CreateSlabDimensionComponentTests.ComponentMother());
      ComponentTestHelper.SetInput(comp, input2_2, 1);

      var output = (SlabGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(2, output.Value.Dimensions.Count);
    }
  }
}
