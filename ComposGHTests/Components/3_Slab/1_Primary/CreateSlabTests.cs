using ComposAPI;
using ComposGH.Parameters;
using ComposGH.Components;
using Xunit;
using ComposGHTests.Helpers;
using UnitsNet.Units;
using static ComposAPI.ConcreteMaterial;

namespace ComposGHTests.Slab
{
  [Collection("GrasshopperFixture collection")]
  public class CreateSlabTests
  {
    public static GH_OasysComponent CreateSlabMother()
    {
      var comp = new CreateSlab();
      comp.CreateAttributes();

      ConcreteMaterialGoo input1 = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(CreateConcreteMaterialENTests.CreateConcreteMaterialENMother());
      ComponentTestHelper.SetInput(comp, input1, 0);

      SlabDimensionGoo input2 = (SlabDimensionGoo)ComponentTestHelper.GetOutput(CreateSlabDimensionTests.CreateSlabDimensionMother());
      ComponentTestHelper.SetInput(comp, input2, 1);

      TransverseReinforcementGoo input3 = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(CreateTransverseReinforcementTests.CreateTransverseReinforcementMother());
      ComponentTestHelper.SetInput(comp, input3, 2);

      return comp;
    }

    [Fact]
    public void CreateComponentWithInputs1()
    {
      var comp = CreateSlabMother();
      SlabGoo output = (SlabGoo)ComponentTestHelper.GetOutput(comp);

      ConcreteMaterialGoo input1 = (ConcreteMaterialGoo)ComponentTestHelper.GetOutput(CreateConcreteMaterialENTests.CreateConcreteMaterialENMother());
      Duplicates.AreEqual(input1.Value, output.Value.Material);

      SlabDimensionGoo input2 = (SlabDimensionGoo)ComponentTestHelper.GetOutput(CreateSlabDimensionTests.CreateSlabDimensionMother());
      ComponentTestHelper.SetInput(comp, input2, 1);
      Duplicates.AreEqual(input2.Value, output.Value.Dimensions[0]);

      TransverseReinforcementGoo input3 = (TransverseReinforcementGoo)ComponentTestHelper.GetOutput(CreateTransverseReinforcementTests.CreateTransverseReinforcementMother());
      Duplicates.AreEqual(input3.Value, output.Value.Transverse);
    }

    [Fact]
    public void CreateComponentWithInputs2()
    {
      var comp = CreateSlabMother();

      MeshReinforcementGoo input4 = (MeshReinforcementGoo)ComponentTestHelper.GetOutput(CreateMeshReinforcementTests.CreateMeshReinforcementMother());
      ComponentTestHelper.SetInput(comp, input4, 3);

      DeckingGoo input5 = (DeckingGoo)ComponentTestHelper.GetOutput(CreateCatalogueDeckTests.CreateCatalogueDeckMother());
      ComponentTestHelper.SetInput(comp, input5, 4);

      SlabGoo output = (SlabGoo)ComponentTestHelper.GetOutput(comp);
      Duplicates.AreEqual(input4.Value, output.Value.Mesh);
      Duplicates.AreEqual(input5.Value, output.Value.Decking);
    }
  }
}
