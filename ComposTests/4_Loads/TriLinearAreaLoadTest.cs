using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGHTests.Helpers;
using OasysGH;
using UnitsNet;
using UnitsNet.Units;
using Xunit;
using static ComposAPI.Load;

namespace ComposAPI.Loads.Tests
{
  public partial class LoadTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000)]
    public Load TestTriLinearAreaLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1, double positionW1, 
      double consDead2, double consLive2, double finalDead2, double finalLive2, double positionW2)
    {
      LengthUnit length = LengthUnit.Millimeter;
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      // 2 create object instance with constructor
      TriLinearLoad load = new TriLinearLoad(
        new Pressure(consDead1, force), new Pressure(consLive1, force), new Pressure(finalDead1, force), new Pressure(finalLive1, force), new Length(positionW1, length),
        new Pressure(consDead2, force), new Pressure(consLive2, force), new Pressure(finalDead2, force), new Pressure(finalLive2, force), new Length(positionW2, length));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal(consLive1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal(finalDead1, load.LoadW1.FinalDead.As(force));
      Assert.Equal(finalLive1, load.LoadW1.FinalLive.As(force));
      Assert.Equal(positionW1, load.LoadW1.Position.As(LengthUnit.Millimeter));
      Assert.Equal(consDead2, load.LoadW2.ConstantDead.As(force));
      Assert.Equal(consLive2, load.LoadW2.ConstantLive.As(force));
      Assert.Equal(finalDead2, load.LoadW2.FinalDead.As(force));
      Assert.Equal(finalLive2, load.LoadW2.FinalLive.As(force));
      Assert.Equal(positionW2, load.LoadW2.Position.As(LengthUnit.Millimeter));
      Assert.Equal(LoadType.TriLinear, load.Type);
      Assert.Equal(LoadDistribution.Area, load.Distribution);

      return load;
    }
    [Fact]
    public void DuplicateTriAreaTest()
    {
      // 1 create with constructor and duplicate
      Load original = TestTriLinearAreaLoadConstructor(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000);
      Load duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000)]
    public Load TestTriLinearAreaLoadConstructorPercentage(double consDead1, double consLive1, double finalDead1, double finalLive1, double positionW1,
      double consDead2, double consLive2, double finalDead2, double finalLive2, double positionW2)
    {
      RatioUnit length = RatioUnit.Percent;
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      // 2 create object instance with constructor
      TriLinearLoad load = new TriLinearLoad(
        new Pressure(consDead1, force), new Pressure(consLive1, force), new Pressure(finalDead1, force), new Pressure(finalLive1, force), new Ratio(positionW1, length),
        new Pressure(consDead2, force), new Pressure(consLive2, force), new Pressure(finalDead2, force), new Pressure(finalLive2, force), new Ratio(positionW2, length));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal(consLive1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal(finalDead1, load.LoadW1.FinalDead.As(force));
      Assert.Equal(finalLive1, load.LoadW1.FinalLive.As(force));
      Assert.Equal(positionW1, load.LoadW1.Position.As(RatioUnit.Percent));
      Assert.Equal(consDead2, load.LoadW2.ConstantDead.As(force));
      Assert.Equal(consLive2, load.LoadW2.ConstantLive.As(force));
      Assert.Equal(finalDead2, load.LoadW2.FinalDead.As(force));
      Assert.Equal(finalLive2, load.LoadW2.FinalLive.As(force));
      Assert.Equal(positionW2, load.LoadW2.Position.As(RatioUnit.Percent));
      Assert.Equal(LoadType.TriLinear, load.Type);
      Assert.Equal(LoadDistribution.Area, load.Distribution);

      return load;
    }

    [Fact]
    public void TriLinearAreaLoadToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.00000	4.50000	6.00000	7.00000	8.90000	10.0000\n";
      Load load = TestTriLinearAreaLoadConstructor(0.003, 0.0045, 0.006, 0.007, 8000, 0.0045, 0.006, 0.007, 0.0089, 10000); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TriLinearAreaLoadToCoaStringTestPercentage()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.00000%	4.50000	6.00000	7.00000	8.90000	10.0000%\n";
      Load load = TestTriLinearAreaLoadConstructorPercentage(0.003, 0.0045, 0.006, 0.007, 8, 0.0045, 0.006, 0.007, 0.0089, 10); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TriLinearAreaLoadFromCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      PressureUnit forcePerAreaUnit = UnitsHelper.GetForcePerAreaUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000	13.0000	14.5000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      TriLinearLoad trilinearAreaLoad = (TriLinearLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000	13.0000	14.5000
      Assert.Equal(3, trilinearAreaLoad.LoadW1.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(4.5, trilinearAreaLoad.LoadW1.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(6, trilinearAreaLoad.LoadW1.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(7, trilinearAreaLoad.LoadW1.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(8.9, trilinearAreaLoad.LoadW1.Position.As(lengthUnit));
      Assert.Equal(10, trilinearAreaLoad.LoadW2.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(11, trilinearAreaLoad.LoadW2.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(12, trilinearAreaLoad.LoadW2.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(13, trilinearAreaLoad.LoadW2.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(14.5, trilinearAreaLoad.LoadW2.Position.As(lengthUnit));
      Assert.Equal(LoadType.TriLinear, trilinearAreaLoad.Type);
      Assert.Equal(LoadDistribution.Area, trilinearAreaLoad.Distribution);
    }

    [Fact]
    public void TriLinearAreaLoadFromCoaStringTestPercentage()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      PressureUnit forcePerAreaUnit = UnitsHelper.GetForcePerAreaUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.90000%	10.0000	11.0000	12.0000	13.0000	14.5000%\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      TriLinearLoad trilinearAreaLoad = (TriLinearLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000	13.0000	14.5000
      Assert.Equal(3, trilinearAreaLoad.LoadW1.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(4.5, trilinearAreaLoad.LoadW1.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(6, trilinearAreaLoad.LoadW1.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(7, trilinearAreaLoad.LoadW1.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(8.9, trilinearAreaLoad.LoadW1.Position.As(RatioUnit.Percent));
      Assert.Equal(10, trilinearAreaLoad.LoadW2.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(11, trilinearAreaLoad.LoadW2.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(12, trilinearAreaLoad.LoadW2.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(13, trilinearAreaLoad.LoadW2.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(14.5, trilinearAreaLoad.LoadW2.Position.As(RatioUnit.Percent));
      Assert.Equal(LoadType.TriLinear, trilinearAreaLoad.Type);
      Assert.Equal(LoadDistribution.Area, trilinearAreaLoad.Distribution);
    }
  }
}
