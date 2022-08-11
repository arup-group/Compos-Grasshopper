using System.Collections.Generic;
using Xunit;
using UnitsNet;
using UnitsNet.Units;
using static ComposAPI.Load;
using ComposAPI.Tests;
using ComposGHTests.Helpers;

namespace ComposAPI.Loads.Tests
{
  public partial class LoadTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5)]
    [InlineData(3, 4.5, 6, 5)]
    public Load TestUniformAreaLoadConstructor(double consDead, double consLive, double finalDead, double finalLive)
    {
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      // 2 create object instance with constructor
      UniformLoad load = new UniformLoad(
        new Pressure(consDead, force), new Pressure(consLive, force), new Pressure(finalDead, force), new Pressure(finalLive, force));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead, load.Load.ConstantDead.As(force));
      Assert.Equal(consLive, load.Load.ConstantLive.As(force));
      Assert.Equal(finalDead, load.Load.FinalDead.As(force));
      Assert.Equal(finalLive, load.Load.FinalLive.As(force));
      Assert.Equal(LoadType.Uniform, load.Type);
      Assert.Equal(LoadDistribution.Area, load.Distribution);

      return load;
    }
    [Fact]
    public void DuplicateUniAreaTest()
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
    public void UniformAreaLoadToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Uniform	Area	3.00000	4.50000	6.00000	7.00000\n";
      Load load = TestUniformAreaLoadConstructor(0.003, 0.0045, 0.006, 0.007); // input unit in kN/m, coa string in N/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void UniformAreaLoadFromCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      PressureUnit forcePerAreaUnit = Units.GetForcePerAreaUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Uniform	Area	3.00000	4.50000	6.00000	7.00000	Area	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	5.00000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      UniformLoad uniformAreaLoad = (UniformLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Uniform	Area	3.00000	4.50000	6.00000	7.00000	Area	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	5.00000
      Assert.Equal(3, uniformAreaLoad.Load.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(4.5, uniformAreaLoad.Load.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(6, uniformAreaLoad.Load.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(7, uniformAreaLoad.Load.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(LoadType.Uniform, uniformAreaLoad.Type);
      Assert.Equal(LoadDistribution.Area, uniformAreaLoad.Distribution);
    }
  }
}
