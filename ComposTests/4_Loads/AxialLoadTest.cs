using System.Collections.Generic;
using ComposGHTests.Helpers;
using OasysGH;
using UnitsNet;
using UnitsNet.Units;
using Xunit;

namespace ComposAPI.Loads.Tests
{
  public partial class LoadTest
  {
    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 150, 3, 4.5, 6, 5, 200)]
    public Load TestAxialLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1, double depth1,
      double consDead2, double consLive2, double finalDead2, double finalLive2, double depth2)
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForceUnit force = ForceUnit.Kilonewton;

      // 2 create object instance with constructor
      AxialLoad load = new AxialLoad(
        new Force(consDead1, force), new Force(consLive1, force), new Force(finalDead1, force), new Force(finalLive1, force), new Length(depth1, length),
        new Force(consDead2, force), new Force(consLive2, force), new Force(finalDead2, force), new Force(finalLive2, force), new Length(depth2, length));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal(consLive1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal(finalDead1, load.LoadW1.FinalDead.As(force));
      Assert.Equal(finalLive1, load.LoadW1.FinalLive.As(force));
      Assert.Equal(depth1, load.Depth1.Millimeters);
      Assert.Equal(consDead2, load.LoadW2.ConstantDead.As(force));
      Assert.Equal(consLive2, load.LoadW2.ConstantLive.As(force));
      Assert.Equal(finalDead2, load.LoadW2.FinalDead.As(force));
      Assert.Equal(finalLive2, load.LoadW2.FinalLive.As(force));
      Assert.Equal(depth2, load.Depth2.Millimeters);
      Assert.Equal(LoadType.Axial, load.Type);

      return load;
    }
    [Fact]
    public void DuplicateAxialTest()
    {
      // 1 create with constructor and duplicate
      Load original = TestAxialLoadConstructor(1, 1.5, 3, 5, 150, 3, 4.5, 6, 5, 200);
      Load duplicate = (Load)original.Duplicate();

      // 2 check that duplicate has duplicated values
      Duplicates.AreEqual(original, duplicate);

      // 3 check that the memory pointer is not the same
      Assert.NotSame(original, duplicate);
    }

    [Fact]
    public void AxialLoadToCoaStringTest()
    {
      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Axial	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000\n";
      Load load = TestAxialLoadConstructor(0.001, 0.002, 0.003, 0.0045, 6000, 0.007, 0.0089, 0.010, 0.011, 12000); // input force in kn, coa string in n - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void AxialLoadFromCoaStringTest1()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;

      // Arrange
      string coaString = "LOAD	MEMBER-1	Axial	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m
      
      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      AxialLoad axialLoad1 = (AxialLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Axial	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000
      Assert.Equal(1, axialLoad1.LoadW1.ConstantDead.As(forceUnit));
      Assert.Equal(2, axialLoad1.LoadW1.ConstantLive.As(forceUnit));
      Assert.Equal(3, axialLoad1.LoadW1.FinalDead.As(forceUnit));
      Assert.Equal(4.5, axialLoad1.LoadW1.FinalLive.As(forceUnit));
      Assert.Equal(6, axialLoad1.Depth1.As(lengthUnit));
      Assert.Equal(7, axialLoad1.LoadW2.ConstantDead.As(forceUnit));
      Assert.Equal(8.9, axialLoad1.LoadW2.ConstantLive.As(forceUnit));
      Assert.Equal(10, axialLoad1.LoadW2.FinalDead.As(forceUnit));
      Assert.Equal(11, axialLoad1.LoadW2.FinalLive.As(forceUnit));
      Assert.Equal(12, axialLoad1.Depth2.As(lengthUnit));
      Assert.Equal(LoadType.Axial, axialLoad1.Type);
    }

    [Fact]
    public void AxialLoadFromCoaStringTest2()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;
      ComposUnits units = ComposUnits.GetStandardUnits();
      units.Force = forceUnit;
      units.Length = lengthUnit;

      // Arrange
      string coaString = "LOAD	MEMBER-1	Axial	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000	13.0000\n";
      // input force in kn, coa string in n - input pos units in mm, coa string in m

      // Act
      IList<ILoad> loads = Load.FromCoaString(coaString, "MEMBER-1", units);
      AxialLoad axialLoad2 = (AxialLoad)loads[0];

      // Assert
      //LOAD	MEMBER-1	Axial	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000	13.0000
      Assert.Equal(2, axialLoad2.LoadW1.ConstantDead.As(forceUnit));
      Assert.Equal(3, axialLoad2.LoadW1.ConstantLive.As(forceUnit));
      Assert.Equal(4.5, axialLoad2.LoadW1.FinalDead.As(forceUnit));
      Assert.Equal(6, axialLoad2.LoadW1.FinalLive.As(forceUnit));
      Assert.Equal(7, axialLoad2.Depth1.As(lengthUnit));
      Assert.Equal(8.9, axialLoad2.LoadW2.ConstantDead.As(forceUnit));
      Assert.Equal(10, axialLoad2.LoadW2.ConstantLive.As(forceUnit));
      Assert.Equal(11, axialLoad2.LoadW2.FinalDead.As(forceUnit));
      Assert.Equal(12, axialLoad2.LoadW2.FinalLive.As(forceUnit));
      Assert.Equal(13, axialLoad2.Depth2.As(lengthUnit));
      Assert.Equal(LoadType.Axial, axialLoad2.Type);
    }
  }
}
