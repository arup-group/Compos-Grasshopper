using ComposAPI.Tests;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Members.Tests
{
  public partial class DesignCriteriaTest
  {
    [Fact]
    public BeamSizeLimits EmptyConstructorTest()
    {
      // 1 setup input
      // empty constructor 

      // 2 create object instance with constructor
      BeamSizeLimits beamSizeLimits = new BeamSizeLimits();

      // 3 check that inputs are set in object's members
      Assert.Equal(new Length(0.2, LengthUnit.Meter), beamSizeLimits.MinDepth);
      Assert.Equal(new Length(1, LengthUnit.Meter), beamSizeLimits.MaxDepth);
      Assert.Equal(new Length(0.1, LengthUnit.Meter), beamSizeLimits.MinWidth);
      Assert.Equal(new Length(0.5, LengthUnit.Meter), beamSizeLimits.MaxWidth);

      // (optionally return object for other tests)
      return beamSizeLimits;
    }

    [Fact]
    public BeamSizeLimits ConstructorTest()
    {
      // 1 setup input
      // empty constructor 

      // 2 create object instance with constructor
      BeamSizeLimits beamSizeLimits = new BeamSizeLimits(0.3, 1.1, 0.2, 0.6, LengthUnit.Meter);

      // 3 check that inputs are set in object's members
      Assert.Equal(new Length(0.3, LengthUnit.Meter), beamSizeLimits.MinDepth);
      Assert.Equal(new Length(1.1, LengthUnit.Meter), beamSizeLimits.MaxDepth);
      Assert.Equal(new Length(0.2, LengthUnit.Meter), beamSizeLimits.MinWidth);
      Assert.Equal(new Length(0.6, LengthUnit.Meter), beamSizeLimits.MaxWidth);

      // (optionally return object for other tests)
      return beamSizeLimits;
    }

    [Theory]
    [InlineData("CRITERIA_BEAM_SIZE_LIMIT	MEMBER-1	20.0000	100.000	10.0000	50.0000\n", 20, 100, 10, 50)]
    public void ToCoaStringTest(string expected_CoaString, double minDepth, double maxDepth, double minWidth, double maxWidth)
    {
      // Assemble
      LengthUnit unit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Section = unit;
      BeamSizeLimits beamSizeLimits = new BeamSizeLimits(minDepth, maxDepth, minWidth, maxWidth, unit);

      // Act
      string coaString = beamSizeLimits.ToCoaString("MEMBER-1", units);

      // Assert
      Assert.Equal(expected_CoaString, coaString);
    }

    [Fact]
    public void DuplicateTest()
    {
      // 1 create with constructor and duplicate
      BeamSizeLimits original = new BeamSizeLimits();
      BeamSizeLimits duplicate = (BeamSizeLimits)original.Duplicate();

      // 2 check that duplicate has duplicated values
      ObjectExtensionTest.IsEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }
  }
}
