using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ComposAPI.Tests
{
  public class SlabTest
  {
    // 1 setup inputs
    [Fact]
    public void ConstructorTest1()
    {
      // 2 create object instance with constructor
      IConcreteMaterial material = ConcreteMaterialMother.CreateConcreteMaterial();
      List<ISlabDimension> dimensions = new List<ISlabDimension>() { SlabDimensionMother.CreateSlabDimension() };
      TransverseReinforcement transverse = new TransverseReinforcement(new ReinforcementMaterial(RebarGrade.EN_500B));
      MeshReinforcement mesh = new MeshReinforcement();
      Decking decking = new Decking();

      Slab slab = new Slab(material, dimensions, transverse, mesh, decking);

      // 3 check that inputs are set in object's members
      Assert.Equal(material, slab.Material);
      Assert.Equal(dimensions, slab.Dimensions);
      Assert.Equal(transverse, slab.Transverse);
      Assert.Equal(mesh, slab.Mesh);
      Assert.Equal(decking, slab.Decking);
    }

    // 1 setup inputs
    [Fact]
    public void ConstructorTest2()
    {
      // 2 create object instance with constructor
      IConcreteMaterial material = ConcreteMaterialMother.CreateConcreteMaterial();
      List<ISlabDimension> dimensions = new List<ISlabDimension>() { SlabDimensionMother.CreateSlabDimension() };
      TransverseReinforcement transverse = new TransverseReinforcement(new ReinforcementMaterial(RebarGrade.EN_500B));

      Slab slab = new Slab(material, dimensions, transverse);

      // 3 check that inputs are set in object's members
      Assert.Equal(material, slab.Material);
      Assert.Equal(dimensions, slab.Dimensions);
      Assert.Equal(transverse, slab.Transverse);
      Assert.Null(slab.Mesh);
      Assert.Null(slab.Decking);
    }
  }
}
