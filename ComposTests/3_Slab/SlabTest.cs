using ComposGHTests.Helpers;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using System.Collections.Generic;
using Xunit;

namespace ComposAPI.Slabs.Tests {
  public static class SlabMother {

    public static string Example1CoaString() {
      return
        "SLAB_CONCRETE_MATERIAL	MEMBER-1	C40	NORMAL	CODE_DENSITY	2400.00	NOT_APPLY	0.330000	CODE_E_RATIO	CODE_STRAIN" + '\n' +
        "SLAB_DIMENSION	MEMBER-1	3	1	0.000000	0.130000	1.50000	1.50000	TAPERED_YES	EFFECTIVE_WIDTH_NO" + '\n' +
        "SLAB_DIMENSION	MEMBER-1	3	2	15.0000%	0.150000	1.00000	1.00000	TAPERED_NO	EFFECTIVE_WIDTH_NO" + '\n' +
        "SLAB_DIMENSION	MEMBER-1	3	3	9.00000	0.150000	1.00000	1.00000	TAPERED_YES	EFFECTIVE_WIDTH_YES	1.00000	1.00000" + '\n' +
        "REBAR_WESH	MEMBER-1	A142	0.0250000	PARALLEL" + '\n' +
        "REBAR_MATERIAL	MEMBER-1	STANDARD	460T" + '\n' +
        "REBAR_LONGITUDINAL	MEMBER-1	PROGRAM_DESIGNED" + '\n' +
        "REBAR_TRANSVERSE	MEMBER-1	PROGRAM_DESIGNED" + '\n' +
        "DECKING_CATALOGUE	MEMBER-1	Kingspan	Multideck 50 (0.85)	S280	90.0000	DECKING_JOINTED	JOINT_NOT_WELD" + '\n';
    }

    public static Slab Example1Slab() {
      ConcreteMaterial material = (ConcreteMaterial)ConcreteMaterialMother.CreateConcreteMaterial();
      material.Grade = ConcreteGrade.C40.ToString();

      ISlabDimension slabDimension1 = new SlabDimension(Length.Zero, new Length(0.13, LengthUnit.Meter), new Length(1.5, LengthUnit.Meter), new Length(1.5, LengthUnit.Meter), true);
      ISlabDimension slabDimension2 = new SlabDimension(new Ratio(15, RatioUnit.Percent), new Length(0.15, LengthUnit.Meter), new Length(1.0, LengthUnit.Meter), new Length(1.0, LengthUnit.Meter), false);
      ISlabDimension slabDimension3 = new SlabDimension(new Length(9, LengthUnit.Meter), new Length(0.15, LengthUnit.Meter), new Length(1.0, LengthUnit.Meter), new Length(1.0, LengthUnit.Meter), new Length(1, LengthUnit.Meter), new Length(1, LengthUnit.Meter), true);
      List<ISlabDimension> slabDimensions = new List<ISlabDimension>() { slabDimension1, slabDimension2, slabDimension3 };

      IReinforcementMaterial rebarMat = new ReinforcementMaterial(RebarGrade.BS_460T);
      ITransverseReinforcement transverseReinforcement = new TransverseReinforcement(rebarMat);

      IMeshReinforcement mesh = new MeshReinforcement(new Length(0.025, LengthUnit.Meter), ReinforcementMeshType.A142, false);

      IDeckingConfiguration deckingConfiguration = new DeckingConfiguration(new Angle(90, AngleUnit.Degree), true, false);
      IDecking decking = new CatalogueDecking("Kingspan", "Multideck 50 (0.85)", DeckingSteelGrade.S280, deckingConfiguration);

      return new Slab(material, slabDimensions, transverseReinforcement, mesh, decking);
    }
  }

  [Collection("ComposAPI Fixture collection")]
  public class SlabTest {

    // 1 setup inputs
    [Fact]
    public void ConstructorTest1() {
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
    public void ConstructorTest2() {
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

    [Fact]
    public void DuplicateTest() {
      // 1 create with constructor and duplicate
      Slab original = SlabMother.Example1Slab();
      Slab duplicate = (Slab)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void FromCoaStringTest() {
      // Assemble
      ComposUnits units = ComposUnits.GetStandardUnits();
      Slab expectedSlab = SlabMother.Example1Slab();

      // Act
      Slab slab = (Slab)Slab.FromCoaString(SlabMother.Example1CoaString(), "MEMBER-1", Code.BS5950_3_1_1990_A1_2010, units);

      // Assert
      ObjectExtension.Equals(expectedSlab, slab);
    }

    [Fact]
    public void ToCoaStringTest() {
      // Assemble
      ComposUnits units = ComposUnits.GetStandardUnits();
      string expectedCoaString = SlabMother.Example1CoaString();

      // Act
      string coaString = SlabMother.Example1Slab().ToCoaString("MEMBER-1", units);

      // Assert
      Assert.Equal(expectedCoaString, coaString);
    }
  }
}
