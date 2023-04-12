using System.Collections.Generic;
using Xunit;
using OasysUnits;
using OasysUnits.Units;
using ComposGHTests.Helpers;
using ComposAPI.Helpers;
using OasysGH;

namespace ComposAPI.Loads.Tests
{
    public partial class LoadTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000)]
    public Load TestTriLinearLineLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1, double positionW1,
      double consDead2, double consLive2, double finalDead2, double finalLive2, double positionW2)
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      // 2 create object instance with constructor
      TriLinearLoad load = new TriLinearLoad(
        new ForcePerLength(consDead1, force), new ForcePerLength(consLive1, force), new ForcePerLength(finalDead1, force), new ForcePerLength(finalLive1, force), new Length(positionW1, length),
        new ForcePerLength(consDead2, force), new ForcePerLength(consLive2, force), new ForcePerLength(finalDead2, force), new ForcePerLength(finalLive2, force), new Length(positionW2, length));

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
      Assert.Equal(LoadDistribution.Line, load.Distribution);

      return load;
    }
    [Fact]
    public void DuplicateTriLineTest()
    {
      // 1 create with constructor and duplicate
      Load original = TestTriLinearLineLoadConstructor(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000);
      Load duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000)]
    public Load TestTriLinearLineLoadConstructorPercentage(double consDead1, double consLive1, double finalDead1, double finalLive1, double positionW1,
      double consDead2, double consLive2, double finalDead2, double finalLive2, double positionW2)
    {
      RatioUnit ratio = RatioUnit.Percent;
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      // 2 create object instance with constructor
      TriLinearLoad load = new TriLinearLoad(
        new ForcePerLength(consDead1, force), new ForcePerLength(consLive1, force), new ForcePerLength(finalDead1, force), new ForcePerLength(finalLive1, force), new Ratio(positionW1, ratio),
        new ForcePerLength(consDead2, force), new ForcePerLength(consLive2, force), new ForcePerLength(finalDead2, force), new ForcePerLength(finalLive2, force), new Ratio(positionW2, ratio));

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
      Assert.Equal(LoadDistribution.Line, load.Distribution);

      return load;
    }

    [Fact]
    public void TriLinearLineLoadToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Tri-Linear	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000\n";
      Load load = TestTriLinearLineLoadConstructor(0.002, 0.003, 0.0045, 0.006, 7000, 0.003, 0.0045, 0.006, 0.007, 8900); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TriLinearLineLoadToCoaStringTestPercentage()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Tri-Linear	Line	2.00000	3.00000	4.50000	6.00000	7.00000%	3.00000	4.50000	6.00000	7.00000	8.90000%\n";
      Load load = TestTriLinearLineLoadConstructorPercentage(0.002, 0.003, 0.0045, 0.006, 7, 0.003, 0.0045, 0.006, 0.007, 8.9); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TriLinearLineLoadFromCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      ForcePerLengthUnit forcePerLengthUnit = ComposUnitsHelper.GetForcePerLengthUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Tri-Linear	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      TriLinearLoad trilinearLineLoad = (TriLinearLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Tri-Linear	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000
      Assert.Equal(2, trilinearLineLoad.LoadW1.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(3, trilinearLineLoad.LoadW1.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(4.5, trilinearLineLoad.LoadW1.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(6, trilinearLineLoad.LoadW1.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(7, trilinearLineLoad.LoadW1.Position.As(lengthUnit));
      Assert.Equal(3, trilinearLineLoad.LoadW2.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(4.5, trilinearLineLoad.LoadW2.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(6, trilinearLineLoad.LoadW2.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(7, trilinearLineLoad.LoadW2.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(8.9, trilinearLineLoad.LoadW2.Position.As(lengthUnit));
      Assert.Equal(LoadType.TriLinear, trilinearLineLoad.Type);
      Assert.Equal(LoadDistribution.Line, trilinearLineLoad.Distribution);
    }

    [Fact]
    public void TriLinearLineLoadFromCoaStringTestPercentage()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      ForcePerLengthUnit forcePerLengthUnit = ComposUnitsHelper.GetForcePerLengthUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Tri-Linear	Line	2.00000	3.00000	4.50000	6.00000	7.00000%	3.00000	4.50000	6.00000	7.00000	8.90000%\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      TriLinearLoad trilinearLineLoad = (TriLinearLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Tri-Linear	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000
      Assert.Equal(2, trilinearLineLoad.LoadW1.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(3, trilinearLineLoad.LoadW1.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(4.5, trilinearLineLoad.LoadW1.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(6, trilinearLineLoad.LoadW1.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(7, trilinearLineLoad.LoadW1.Position.As(RatioUnit.Percent));
      Assert.Equal(3, trilinearLineLoad.LoadW2.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(4.5, trilinearLineLoad.LoadW2.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(6, trilinearLineLoad.LoadW2.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(7, trilinearLineLoad.LoadW2.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(8.9, trilinearLineLoad.LoadW2.Position.As(RatioUnit.Percent));
      Assert.Equal(LoadType.TriLinear, trilinearLineLoad.Type);
      Assert.Equal(LoadDistribution.Line, trilinearLineLoad.Distribution);
    }
  }
}
