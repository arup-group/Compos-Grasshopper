using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ComposAPI.Tests
{
  public static class SlabMother
  {
    //public static Slab CreateSlab()
    //{
    //ConcreteMaterial material = ConcreteMaterialMother.CreateConcreteMaterial();
    //List<SlabDimension> dimensions = new List<SlabDimension>() { SlabDimensionMother.CreateSlabDimension() };
    //TransverseReinforcement transverseReinforcement = new TransverseReinforcementMother.CreateTransverseReinforcement() };
    //MeshReinforcement meshReinforcement = new MeshReinforcementMother.CreateMeshReinforcement() };
    //Decking decking = new DeckingMother.CreateDecking() };
    //return new Slab(material, dimensions, transverseReinforcement, meshReinforcement, decking);
    //}
  }

  public class SlabTest
  {
    // 1 setup inputs
    [Fact]
    public void TestConstructor1()
    {
      ConcreteMaterial material = ConcreteMaterialMother.CreateConcreteMaterial();
      List<SlabDimension> dimensions = new List<SlabDimension>() { SlabDimensionMother.CreateSlabDimension() };
      //TransverseReinforcement transverseReinforcement = new TransverseReinforcementMother.CreateTransverseReinforcement() };
      //Slab slab = new Slab(material, dimensions, transverseReinforcement, meshReinforcement, decking);

      // 3 check that inputs are set in object's members
      //Assert.Equal(material, slab.Material);
      //Assert.Equal(dimensions, slab.Dimensions);
      //Assert.Equal(transverseReinforcement, slab.TransverseReinforcement);
      //Assert.Null(slab.MeshReinforcement);
      //Assert.Null(slab.Decking);
    }

    // 1 setup inputs
    [Fact]
    public void TestConstructor2()
    {
      // 2 create object instance with constructor
      ConcreteMaterial material = ConcreteMaterialMother.CreateConcreteMaterial();
      List<SlabDimension> dimensions = new List<SlabDimension>() { SlabDimensionMother.CreateSlabDimension() };
      //TransverseReinforcement transverseReinforcement = new TransverseReinforcementMother.CreateTransverseReinforcement() };
      //MeshReinforcement meshReinforcement = new MeshReinforcementMother.CreateMeshReinforcement() };
      //Decking decking = new DeckingMother.CreateDecking() };
      //Slab slab = new Slab(material, dimensions, transverseReinforcement, meshReinforcement, decking);

      // 3 check that inputs are set in object's members
      //Assert.Equal(material, slab.Material);
      //Assert.Equal(dimensions, slab.Dimensions);
      //Assert.Equal(transverseReinforcement, slab.TransverseReinforcement);
      //Assert.Equal(meshReinforcement, slab.MeshReinforcement);
      //Assert.Equal(decking, slab.Decking);
    }

    [Fact]
    public void TestDuplicate()
    {
      // 1 create with constructor and duplicate
      //Slab original = SlabMother.CreateSlab();
      //Slab duplicate = original.Duplicate();

      //// 2 check that duplicate has duplicated values
      //Assert.Equal(shortTerm, duplicate.ShortTerm);
      //Assert.Equal(longTerm, duplicate.LongTerm);
      //Assert.Equal(vibration, duplicate.Vibration);
      //Assert.Equal(shrinkage, duplicate.Shrinkage);
      //Assert.True(duplicate.UserDefined);

      //// 3 make some changes to duplicate
      //duplicate.ShortTerm = 6;
      //duplicate.LongTerm = 18;
      //duplicate.Vibration = 5.39;
      //duplicate.Shrinkage = 0;

      //// 4 check that duplicate has set changes
      //Assert.Equal(6, duplicate.ShortTerm);
      //Assert.Equal(18, duplicate.LongTerm);
      //Assert.Equal(5.39, duplicate.Vibration);
      //Assert.Equal(0, duplicate.Shrinkage);

      //// 5 check that original has not been changed
      //Assert.Equal(shortTerm, original.ShortTerm);
      //Assert.Equal(longTerm, original.LongTerm);
      //Assert.Equal(vibration, original.Vibration);
      //Assert.Equal(shrinkage, original.Shrinkage);
      //Assert.True(original.UserDefined);
    }
  }
}
