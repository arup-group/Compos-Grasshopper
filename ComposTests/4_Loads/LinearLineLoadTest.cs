﻿using System.Collections.Generic;
using ComposAPI.Helpers;
using ComposGH.Helpers;
using ComposGHTests.Helper;
using OasysGH;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace ComposAPI.Loads.Tests {
  public partial class LoadTest {

    [Fact]
    public void DuplicateLinearLineTest() {
      // 1 create with constructor and duplicate
      Load original = TestLinearLineLoadConstructor(1, 1.5, 3, 5, 3, 4.5, 6, 5);
      var duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void LinearLineLoadFromCoaStringTest() {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      var units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;
      ForcePerLengthUnit forcePerLengthUnit = ComposUnitsHelper.GetForcePerLengthUnit(forceUnit, lengthUnit);

      // Arrange
      string coaString = "LOAD	MEMBER-1	Linear	Line	4.50000	6.00000	7.00000	8.00000	8.90000	10.0000	11.0000	12.0000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      var linearLineLoad = (LinearLoad)loads[0];

      // Assert
      //LOAD MEMBER-1 Linear Line 4.50000 6.00000 7.00000 8.00000 8.90000 10.0000 11.0000 12.0000
      Assert.Equal(4.5, linearLineLoad.LoadW1.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(6, linearLineLoad.LoadW1.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(7, linearLineLoad.LoadW1.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(8, linearLineLoad.LoadW1.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(8.9, linearLineLoad.LoadW2.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(10, linearLineLoad.LoadW2.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(11, linearLineLoad.LoadW2.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(12, linearLineLoad.LoadW2.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(LoadType.Linear, linearLineLoad.Type);
      Assert.Equal(LoadDistribution.Line, linearLineLoad.Distribution);
    }

    [Fact]
    public void LinearLineLoadToCoaStringTest() {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Linear	Line	4.50000	6.00000	7.00000	8.00000	8.90000	10.0000	11.0000	12.0000\n";
      Load load = TestLinearLineLoadConstructor(0.0045, 0.006, 0.007, 0.008, 0.0089, 0.010, 0.011, 0.012); // input unit in kN/m, coa string in N/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 3, 4.5, 6, 5)]
    public Load TestLinearLineLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1,
      double consDead2, double consLive2, double finalDead2, double finalLive2) {
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      // 2 create object instance with constructor
      var load = new LinearLoad(
        new ForcePerLength(consDead1, force), new ForcePerLength(consLive1, force), new ForcePerLength(finalDead1, force), new ForcePerLength(finalLive1, force),
        new ForcePerLength(consDead2, force), new ForcePerLength(consLive2, force), new ForcePerLength(finalDead2, force), new ForcePerLength(finalLive2, force));

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
      Assert.Equal(LoadDistribution.Line, load.Distribution);

      return load;
    }
  }
}
