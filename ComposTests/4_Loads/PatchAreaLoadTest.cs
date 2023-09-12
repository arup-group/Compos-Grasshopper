using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helpers;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Loads.Tests {
  public partial class LoadTest {

    [Fact]
    public void DuplicatePatchAreaTest() {
      // 1 create with constructor and duplicate
      Load original = TestPatchAreaLoadConstructor(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000);
      var duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void PatchAreaLoadFromCoaStringTest() {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      var units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      PressureUnit forcePerAreaUnit = ComposUnitsHelper.GetForcePerAreaUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Patch	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.00000	9.00000	10.0000	11.0000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      var patchAreaLoad = (PatchLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Patch	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.00000	9.00000	10.0000	11.0000
      Assert.Equal(1, patchAreaLoad.LoadW1.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(2, patchAreaLoad.LoadW1.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(3, patchAreaLoad.LoadW1.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(4.5, patchAreaLoad.LoadW1.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(6, patchAreaLoad.LoadW1.Position.As(lengthUnit));
      Assert.Equal(7, patchAreaLoad.LoadW2.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(8, patchAreaLoad.LoadW2.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(9, patchAreaLoad.LoadW2.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(10, patchAreaLoad.LoadW2.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(11, patchAreaLoad.LoadW2.Position.As(lengthUnit));
      Assert.Equal(LoadType.Patch, patchAreaLoad.Type);
      Assert.Equal(LoadDistribution.Area, patchAreaLoad.Distribution);
    }

    [Fact]
    public void PatchAreaLoadFromCoaStringTestPercentage() {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      var units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      PressureUnit forcePerAreaUnit = ComposUnitsHelper.GetForcePerAreaUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Patch	Area	1.00000	2.00000	3.00000	4.50000	6.00000%	7.00000	8.00000	9.00000	10.0000	11.0000%\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      var patchAreaLoad = (PatchLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Patch	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.00000	9.00000	10.0000	11.0000
      Assert.Equal(1, patchAreaLoad.LoadW1.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(2, patchAreaLoad.LoadW1.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(3, patchAreaLoad.LoadW1.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(4.5, patchAreaLoad.LoadW1.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(6, patchAreaLoad.LoadW1.Position.As(RatioUnit.Percent));
      Assert.Equal(7, patchAreaLoad.LoadW2.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(8, patchAreaLoad.LoadW2.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(9, patchAreaLoad.LoadW2.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(10, patchAreaLoad.LoadW2.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(11, patchAreaLoad.LoadW2.Position.As(RatioUnit.Percent));
      Assert.Equal(LoadType.Patch, patchAreaLoad.Type);
      Assert.Equal(LoadDistribution.Area, patchAreaLoad.Distribution);
    }

    [Fact]
    public void PatchAreaLoadToCoaStringTest() {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Patch	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000\n";
      Load load = TestPatchAreaLoadConstructor(0.001, 0.002, 0.003, 0.0045, 6000, 0.007, 0.0089, 0.010, 0.011, 12000); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void PatchAreaLoadToCoaStringTestPercentage() {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Patch	Area	1.00000	2.00000	3.00000	4.50000	6.00000%	7.00000	8.90000	10.0000	11.0000	12.0000%\n";
      Load load = TestPatchAreaLoadConstructorPercentage(0.001, 0.002, 0.003, 0.0045, 6, 0.007, 0.0089, 0.010, 0.011, 12); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000)]
    public Load TestPatchAreaLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1, double positionW1,
      double consDead2, double consLive2, double finalDead2, double finalLive2, double positionW2) {
      LengthUnit length = LengthUnit.Millimeter;
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      // 2 create object instance with constructor
      var load = new PatchLoad(
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
      Assert.Equal(LoadType.Patch, load.Type);
      Assert.Equal(LoadDistribution.Area, load.Distribution);

      return load;
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000)]
    public Load TestPatchAreaLoadConstructorPercentage(double consDead1, double consLive1, double finalDead1, double finalLive1, double positionW1,
      double consDead2, double consLive2, double finalDead2, double finalLive2, double positionW2) {
      RatioUnit ratio = RatioUnit.Percent;
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      // 2 create object instance with constructor
      var load = new PatchLoad(
        new Pressure(consDead1, force), new Pressure(consLive1, force), new Pressure(finalDead1, force), new Pressure(finalLive1, force), new Ratio(positionW1, ratio),
        new Pressure(consDead2, force), new Pressure(consLive2, force), new Pressure(finalDead2, force), new Pressure(finalLive2, force), new Ratio(positionW2, ratio));

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
      Assert.Equal(LoadType.Patch, load.Type);
      Assert.Equal(LoadDistribution.Area, load.Distribution);

      return load;
    }
  }
}
