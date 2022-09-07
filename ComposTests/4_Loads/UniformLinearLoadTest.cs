using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposAPI.Tests;
using ComposGHTests.Helpers;
using UnitsNet;
using UnitsNet.Units;
using Xunit;
using OasysGH;
using static ComposAPI.Load;

namespace ComposAPI.Loads.Tests
{
  public partial class LoadTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5)]
    [InlineData(3, 4.5, 6, 5)]
    public Load TestUniformLineLoadConstructor(double consDead, double consLive, double finalDead, double finalLive)
    {
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      // 2 create object instance with constructor
      UniformLoad load = new UniformLoad(
        new ForcePerLength(consDead, force), new ForcePerLength(consLive, force), new ForcePerLength(finalDead, force), new ForcePerLength(finalLive, force));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead, load.Load.ConstantDead.As(force));
      Assert.Equal(consLive, load.Load.ConstantLive.As(force));
      Assert.Equal(finalDead, load.Load.FinalDead.As(force));
      Assert.Equal(finalLive, load.Load.FinalLive.As(force));
      Assert.Equal(LoadType.Uniform, load.Type);
      Assert.Equal(LoadDistribution.Line, load.Distribution);

      return load;
    }
    [Fact]
    public void DuplicateUniLineTest()
    {
      // 1 create with constructor and duplicate
      Load original = TestUniformAreaLoadConstructor(1, 1.5, 3, 5);
      Load duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void UniformLineLoadToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Uniform	Line	2.00000	3.00000	4.50000	6.00000\n";
      Load load = TestUniformLineLoadConstructor(0.002, 0.003, 0.0045, 0.006); // input unit in kN/m, coa string in N/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void UniformLineLoadFromCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      ForcePerLengthUnit forcePerLengthUnit = UnitsHelper.GetForcePerLengthUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Uniform	Line	1.00000	2.00000	3.00000	4.50000	Line	1.00000	2.00000	3.00000	4.50000	3.00000	4.50000	6.00000	5.00000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      UniformLoad uniformLineLoad = (UniformLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Uniform	Line	1.00000	2.00000	3.00000	4.50000	Line	1.00000	2.00000	3.00000	4.50000	3.00000	4.50000	6.00000	5.00000
      Assert.Equal(1, uniformLineLoad.Load.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(2, uniformLineLoad.Load.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(3, uniformLineLoad.Load.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(4.5, uniformLineLoad.Load.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(LoadType.Uniform, uniformLineLoad.Type);
      Assert.Equal(LoadDistribution.Line, uniformLineLoad.Distribution);
    }
  }
}
