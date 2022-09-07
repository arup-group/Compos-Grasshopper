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
    [InlineData(1, 1.5, 3, 5, 3, 4.5, 6, 5)]
    public Load TestLinearAreaLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1,
      double consDead2, double consLive2, double finalDead2, double finalLive2)
    {
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      // 2 create object instance with constructor
      LinearLoad load = new LinearLoad(
        new Pressure(consDead1, force), new Pressure(consLive1, force), new Pressure(finalDead1, force), new Pressure(finalLive1, force),
        new Pressure(consDead2, force), new Pressure(consLive2, force), new Pressure(finalDead2, force), new Pressure(finalLive2, force));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal(consLive1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal(finalDead1, load.LoadW1.FinalDead.As(force));
      Assert.Equal(finalLive1, load.LoadW1.FinalLive.As(force));
      Assert.Equal(consDead2, load.LoadW2.ConstantDead.As(force));
      Assert.Equal(consLive2, load.LoadW2.ConstantLive.As(force));
      Assert.Equal(finalDead2, load.LoadW2.FinalDead.As(force));
      Assert.Equal(finalLive2, load.LoadW2.FinalLive.As(force));
      Assert.Equal(LoadType.Linear, load.Type);
      Assert.Equal(LoadDistribution.Area, load.Distribution);

      return load;
    }
    [Fact]
    public void DuplicateLinearAreaTest()
    {
      // 1 create with constructor and duplicate
      Load original = TestLinearAreaLoadConstructor(1, 1.5, 3, 5, 3, 4.5, 6, 5);
      Load duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void LinearAreaLoadToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Linear	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.00000	9.00000\n";
      Load load = TestLinearAreaLoadConstructor(0.001, 0.002, 0.003, 0.0045, 0.006, 0.007, 0.008, 0.009); //input unit in kN/m, coa string in N/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void LinearAreaLoadFromCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      PressureUnit forcePerAreaUnit = UnitsHelper.GetForcePerAreaUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Linear	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.00000	9.00000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      LinearLoad linearAreaLoad = (LinearLoad)loads[0];

      // Assert
      //LOAD MEMBER - 1 Linear Area 1.00000 2.00000 3.00000 4.50000 6.00000 7.00000 8.00000 9.00000
      Assert.Equal(1, linearAreaLoad.LoadW1.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(2, linearAreaLoad.LoadW1.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(3, linearAreaLoad.LoadW1.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(4.5, linearAreaLoad.LoadW1.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(6, linearAreaLoad.LoadW2.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(7, linearAreaLoad.LoadW2.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(8, linearAreaLoad.LoadW2.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(9, linearAreaLoad.LoadW2.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(LoadType.Linear, linearAreaLoad.Type);
      Assert.Equal(LoadDistribution.Area, linearAreaLoad.Distribution);
    }
  }
}
