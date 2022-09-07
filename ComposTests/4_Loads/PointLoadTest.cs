using System.Collections.Generic;
using Xunit;
using UnitsNet;
using UnitsNet.Units;
using static ComposAPI.Load;
using ComposAPI.Tests;
using ComposGHTests.Helpers;
using OasysGH;

namespace ComposAPI.Loads.Tests
{
  public partial class LoadTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 5000)]
    [InlineData(3, 4.5, 6, 5, 0)]
    public Load TestPointLoadConstructor(double consDead, double consLive, double finalDead, double finalLive, double position)
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForceUnit force = ForceUnit.Kilonewton;

      // 2 create object instance with constructor
      PointLoad load = new PointLoad(
        new Force(consDead, force), new Force(consLive, force), new Force(finalDead, force), new Force(finalLive, force),
        new Length(position, length));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead, load.Load.ConstantDead.As(force));
      Assert.Equal(consLive, load.Load.ConstantLive.As(force));
      Assert.Equal(finalDead, load.Load.FinalDead.As(force));
      Assert.Equal(finalLive, load.Load.FinalLive.As(force));
      Assert.Equal(position, load.Load.Position.As(LengthUnit.Millimeter));
      Assert.Equal(LoadType.Point, load.Type);

      return load;
    }
    [Fact]
    public void DuplicatePointTest()
    {
      // 1 create with constructor and duplicate
      Load original = TestPointLoadConstructor(1, 1.5, 3, 5, 5000);
      Load duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 5000)]
    [InlineData(3, 4.5, 6, 5, 0)]
    public Load TestPointLoadConstructorPercentage(double consDead, double consLive, double finalDead, double finalLive, double position)
    {
      RatioUnit ratio = RatioUnit.Percent;
      ForceUnit force = ForceUnit.Kilonewton;

      // 2 create object instance with constructor
      PointLoad load = new PointLoad(
        new Force(consDead, force), new Force(consLive, force), new Force(finalDead, force), new Force(finalLive, force),
        new Ratio(position, ratio));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead, load.Load.ConstantDead.As(force));
      Assert.Equal(consLive, load.Load.ConstantLive.As(force));
      Assert.Equal(finalDead, load.Load.FinalDead.As(force));
      Assert.Equal(finalLive, load.Load.FinalLive.As(force));
      Assert.Equal(position, load.Load.Position.As(RatioUnit.Percent));
      Assert.Equal(LoadType.Point, load.Type);

      return load;
    }

    [Fact]
    public void PointLoadToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	6.00000\n";
      Load load = TestPointLoadConstructor(0.001, 0.002, 0.003, 0.0045, 6000); // input load in kN, coa string in N - input position length unit in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void PointLoadToCoaStringTestPercentage()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	6.00000%\n";
      Load load = TestPointLoadConstructorPercentage(0.001, 0.002, 0.003, 0.0045, 6); // input load in kN, coa string in N - input position length unit in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void PointLoadFromCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;

      // Arrange
      string coaString = "LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	6.0000000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      PointLoad pointLoad = (PointLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	0.0600000
      Assert.Equal(1, pointLoad.Load.ConstantDead.As(forceUnit));
      Assert.Equal(2, pointLoad.Load.ConstantLive.As(forceUnit));
      Assert.Equal(3, pointLoad.Load.FinalDead.As(forceUnit));
      Assert.Equal(4.5, pointLoad.Load.FinalLive.As(forceUnit));
      Assert.Equal(6, pointLoad.Load.Position.As(lengthUnit));
      Assert.Equal(LoadType.Point, pointLoad.Type);
    }

    [Fact]
    public void PointLoadFromCoaStringTestPercentage()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;

      // Arrange
      string coaString = "LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	6.0000000%\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      PointLoad pointLoad = (PointLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	0.0600000
      Assert.Equal(1, pointLoad.Load.ConstantDead.As(forceUnit));
      Assert.Equal(2, pointLoad.Load.ConstantLive.As(forceUnit));
      Assert.Equal(3, pointLoad.Load.FinalDead.As(forceUnit));
      Assert.Equal(4.5, pointLoad.Load.FinalLive.As(forceUnit));
      Assert.Equal(6, pointLoad.Load.Position.As(RatioUnit.Percent));
      Assert.Equal(LoadType.Point, pointLoad.Type);
    }
  }
}
