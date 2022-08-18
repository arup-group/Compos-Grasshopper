using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateSlabComponentTests
  {
    public static GH_OasysComponent CreateSlabMother()
    {
      var comp = new CreateSlab();
      comp.CreateAttributes();

      ConcreteMaterialGoo input1 = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(CreateConcreteMaterialENComponentTests.CreateConcreteMaterialENMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      SlabDimensionGoo input2 = (SlabDimensionGoo)ComponentTestHelper.GetOutput(CreateSlabDimensionComponentTests.CreateSlabDimensionMother());
      ComponentTestHelper.SetInput(comp, input2, 1);

      TransverseReinforcementGoo input3 = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(CreateTransverseReinforcementComponentTests.CreateTransverseReinforcementMother());
      ComponentTestHelper.SetInput(comp, input3, 2);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = CreateSlabMother();
      SlabGoo output = (SlabGoo)ComponentTestHelper.GetOutput(comp);

      ConcreteMaterialGoo expexted_input1 = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(CreateConcreteMaterialENComponentTests.CreateConcreteMaterialENMother());
      Duplicates.AreEqual(expexted_input1.Value, output.Value.Material);

      SlabDimensionGoo expexted_input2 = (SlabDimensionGoo)ComponentTestHelper.GetOutput(CreateSlabDimensionComponentTests.CreateSlabDimensionMother());
      Duplicates.AreEqual(expexted_input2.Value, output.Value.Dimensions[0]);

      TransverseReinforcementGoo expexted_input3 = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(CreateTransverseReinforcementComponentTests.CreateTransverseReinforcementMother());
      Duplicates.AreEqual(expexted_input3.Value, output.Value.Transverse);
    }

    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = CreateSlabMother();

      MeshReinforcementGoo input4 = (MeshReinforcementGoo)ComponentTestHelper.GetOutput(CreateMeshReinforcementComponentTests.CreateMeshReinforcementMother());
      ComponentTestHelper.SetInput(comp, input4, 3);

      DeckingGoo input5 = (DeckingGoo)ComponentTestHelper.GetOutput(CreateCatalogueDeckComponentTests.CreateCatalogueDeckMother());
      ComponentTestHelper.SetInput(comp, input5, 4);

      SlabGoo output = (SlabGoo)ComponentTestHelper.GetOutput(comp);
      Duplicates.AreEqual(input4.Value, output.Value.Mesh);
      Duplicates.AreEqual(input5.Value, output.Value.Decking);
    }


    [Fact]
    public void CreateComponentWithInputs3()
    {
      var comp = CreateSlabMother();

      SlabDimensionGoo input2_2 = (SlabDimensionGoo)ComponentTestHelper.GetOutput(CreateSlabDimensionComponentTests.CreateSlabDimensionMother());
      ComponentTestHelper.SetInput(comp, input2_2, 1);

      SlabGoo output = (SlabGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(2, output.Value.Dimensions.Count);
    }
  }
}
