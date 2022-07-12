using System.Collections.Generic;
using Xunit;
using UnitsNet;
using UnitsNet.Units;
using static ComposAPI.Load;

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
      Assert.Equal(position, load.Load.Position.Millimeters);
      Assert.Equal(LoadType.Point, load.Type);

      return load;
    }

    [Fact]
    public void PointLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Centimeter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	6.00000\n";
      Load load = TestPointLoadConstructor(0.001, 0.002, 0.003, 0.0045, 6000); // input load in kN, coa string in N - input position length unit in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

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
    public void UniformLineLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Centimeter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Uniform	Line	2.00000	3.00000	4.50000	6.00000\n";
      Load load = TestUniformLineLoadConstructor(0.002, 0.003, 0.0045, 0.006); // input unit in kN/m, coa string in N/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

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
    public void UniformAreaLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Uniform	Area	3.00000	4.50000	6.00000	7.00000\n";
      Load load = TestUniformAreaLoadConstructor(0.003, 0.0045, 0.006, 0.007); // input unit in kN/m, coa string in N/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 3, 4.5, 6, 5)]
    public Load TestLinearLineLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1,
      double consDead2, double consLive2, double finalDead2, double finalLive2)
    {
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      // 2 create object instance with constructor
      LinearLoad load = new LinearLoad(
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
    [Fact]
    public void LinearLineLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;

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
    public void LinearAreaLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Linear	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.00000	9.00000\n";
      Load load = TestLinearAreaLoadConstructor(0.001, 0.002, 0.003, 0.0045, 0.006, 0.007, 0.008, 0.009); //input unit in kN/m, coa string in N/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

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
      Assert.Equal(positionW1, load.LoadW1.Position.Millimeters);
      Assert.Equal(consDead2, load.LoadW2.ConstantDead.As(force));
      Assert.Equal(consLive2, load.LoadW2.ConstantLive.As(force));
      Assert.Equal(finalDead2, load.LoadW2.FinalDead.As(force));
      Assert.Equal(finalLive2, load.LoadW2.FinalLive.As(force));
      Assert.Equal(positionW2, load.LoadW2.Position.Millimeters);
      Assert.Equal(LoadType.TriLinear, load.Type);
      Assert.Equal(LoadDistribution.Line, load.Distribution);

      return load;
    }
    [Fact]
    public void TriLinearLineLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Tri-Linear	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000\n";
      Load load = TestTriLinearLineLoadConstructor(0.002, 0.003, 0.0045, 0.006, 7000, 0.003, 0.0045, 0.006, 0.007, 8900); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

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
      Assert.Equal(positionW1, load.LoadW1.Position.Millimeters);
      Assert.Equal(consDead2, load.LoadW2.ConstantDead.As(force));
      Assert.Equal(consLive2, load.LoadW2.ConstantLive.As(force));
      Assert.Equal(finalDead2, load.LoadW2.FinalDead.As(force));
      Assert.Equal(finalLive2, load.LoadW2.FinalLive.As(force));
      Assert.Equal(positionW2, load.LoadW2.Position.Millimeters);
      Assert.Equal(LoadType.TriLinear, load.Type);
      Assert.Equal(LoadDistribution.Area, load.Distribution);

      return load;
    }
    [Fact]
    public void TriLinearAreaLoadToCoaStringTest()
    {
      // Arrange
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;
      string expected_coaString = "LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.00000	4.50000	6.00000	7.00000	8.90000	10.0000\n";
      Load load = TestTriLinearAreaLoadConstructor(0.003, 0.0045, 0.006, 0.007, 8000, 0.0045, 0.006, 0.007, 0.0089, 10000); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000)]
    public Load TestPatchLineLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1, double positionW1,
      double consDead2, double consLive2, double finalDead2, double finalLive2, double positionW2)
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForcePerLengthUnit force = ForcePerLengthUnit.KilonewtonPerMeter;

      // 2 create object instance with constructor
      PatchLoad load = new PatchLoad(
        new ForcePerLength(consDead1, force), new ForcePerLength(consLive1, force), new ForcePerLength(finalDead1, force), new ForcePerLength(finalLive1, force), new Length(positionW1, length),
        new ForcePerLength(consDead2, force), new ForcePerLength(consLive2, force), new ForcePerLength(finalDead2, force), new ForcePerLength(finalLive2, force), new Length(positionW2, length));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal(consLive1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal(finalDead1, load.LoadW1.FinalDead.As(force));
      Assert.Equal(finalLive1, load.LoadW1.FinalLive.As(force));
      Assert.Equal(positionW1, load.LoadW1.Position.Millimeters);
      Assert.Equal(consDead2, load.LoadW2.ConstantDead.As(force));
      Assert.Equal(consLive2, load.LoadW2.ConstantLive.As(force));
      Assert.Equal(finalDead2, load.LoadW2.FinalDead.As(force));
      Assert.Equal(finalLive2, load.LoadW2.FinalLive.As(force));
      Assert.Equal(positionW2, load.LoadW2.Position.Millimeters);
      Assert.Equal(LoadType.Patch, load.Type);
      Assert.Equal(LoadDistribution.Line, load.Distribution);

      return load;
    }
    [Fact]
    public void PatchLineLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Patch	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000\n";
      Load load = TestPatchLineLoadConstructor(0.002, 0.003, 0.0045, 0.006, 7000, 0.003, 0.0045, 0.006, 0.007, 8900); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(1, 1.5, 3, 5, 4000, 3, 4.5, 6, 5, 6000)]
    public Load TestPatchAreaLoadConstructor(double consDead1, double consLive1, double finalDead1, double finalLive1, double positionW1,
      double consDead2, double consLive2, double finalDead2, double finalLive2, double positionW2)
    {
      LengthUnit length = LengthUnit.Millimeter;
      PressureUnit force = PressureUnit.KilonewtonPerSquareMeter;

      // 2 create object instance with constructor
      PatchLoad load = new PatchLoad(
        new Pressure(consDead1, force), new Pressure(consLive1, force), new Pressure(finalDead1, force), new Pressure(finalLive1, force), new Length(positionW1, length),
        new Pressure(consDead2, force), new Pressure(consLive2, force), new Pressure(finalDead2, force), new Pressure(finalLive2, force), new Length(positionW2, length));

      // 3 check that inputs are set in object's members
      Assert.Equal(consDead1, load.LoadW1.ConstantDead.As(force));
      Assert.Equal(consLive1, load.LoadW1.ConstantLive.As(force));
      Assert.Equal(finalDead1, load.LoadW1.FinalDead.As(force));
      Assert.Equal(finalLive1, load.LoadW1.FinalLive.As(force));
      Assert.Equal(positionW1, load.LoadW1.Position.Millimeters);
      Assert.Equal(consDead2, load.LoadW2.ConstantDead.As(force));
      Assert.Equal(consLive2, load.LoadW2.ConstantLive.As(force));
      Assert.Equal(finalDead2, load.LoadW2.FinalDead.As(force));
      Assert.Equal(finalLive2, load.LoadW2.FinalLive.As(force));
      Assert.Equal(positionW2, load.LoadW2.Position.Millimeters);
      Assert.Equal(LoadType.Patch, load.Type);
      Assert.Equal(LoadDistribution.Area, load.Distribution);

      return load;
    }
    [Fact]
    public void PatchAreaLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Patch	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000\n";
      Load load = TestPatchAreaLoadConstructor(0.001, 0.002, 0.003, 0.0045, 6000, 0.007, 0.0089, 0.010, 0.011, 12000); // input unit in kN/m, coa string in N/m - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

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
    public void AxialLoadToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Axial	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000\n";
      Load load = TestAxialLoadConstructor(0.001, 0.002, 0.003, 0.0045, 6000, 0.007, 0.0089, 0.010, 0.011, 12000); // input force in kn, coa string in n - input pos units in mm, coa string in m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    // 1 setup inputs
    [Theory]
    [InlineData(100, "MEMBER-2", MemberLoad.SupportSide.Right)]
    [InlineData(4000, "MEMBER-1", MemberLoad.SupportSide.Left)]
    public Load TestMemberLoadConstructor(double position, string name, MemberLoad.SupportSide side)
    {
      LengthUnit length = LengthUnit.Millimeter;

      // 2 create object instance with constructor
      MemberLoad load = new MemberLoad(name, side, new Length(position, length));

      // 3 check that inputs are set in object's members
      Assert.Equal(position, load.Position.Millimeters);
      Assert.Equal(name, load.MemberName);
      Assert.Equal(side, load.Support);
      Assert.Equal(LoadType.MemberLoad, load.Type);

      return load;
    }
    [Fact]
    public void MemberLoadLeftToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Left	0.150000\n";
      Load load = TestMemberLoadConstructor(150, "Member-2", MemberLoad.SupportSide.Left); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }
    [Fact]
    public void MemberLoadRightToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Right	0.00250000\n";
      Load load = TestMemberLoadConstructor(2.5, "Member-2", MemberLoad.SupportSide.Right); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", ComposUnits.GetStandardUnits());
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }

    [Fact]
    public void TestFileCoaStringForLoadingParts()
    {
      // Arrange 
      string coaString =
        "UNIT_DATA	FORCE	kN	0.00100000" + '\n' +
        "UNIT_DATA	LENGTH	m	1.00000" + '\n' +
        "UNIT_DATA	DISP	mm	1000.00" + '\n' +
        "UNIT_DATA	SECTION	mm	1000.00" + '\n' +
        "UNIT_DATA	STRESS	N/mm²	1.00000e-006" + '\n' +
        "UNIT_DATA	MASS	t	0.00100000" + '\n' +
        "MEMBER_TITLE	MEMBER-1	Latitude_1  Longitude_1	Note" + '\n' +
        "LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	6.0000000" + '\n' +
        "LOAD	MEMBER-1	Uniform	Line	1.00000	2.00000	3.00000	4.50000	Line	1.00000	2.00000	3.00000	4.50000	3.00000	4.50000	6.00000	5.00000" + '\n' +
        "LOAD	MEMBER-1	Uniform	Area	3.00000	4.50000	6.00000	7.00000	Area	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	5.00000" + '\n' +
        "LOAD	MEMBER-1	Linear	Line	4.50000	6.00000	7.00000	8.00000	8.90000	10.0000	11.0000	12.0000" + '\n' +
        "LOAD	MEMBER-1	Linear	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.00000	9.00000" + '\n' +
        "LOAD	MEMBER-1	Tri-Linear	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000" + '\n' +
        "LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000	13.0000	14.5000" + '\n' +
        "LOAD	MEMBER-1	Patch	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000" + '\n' +
        "LOAD	MEMBER-1	Patch	Area	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.00000	9.00000	10.0000	11.0000" + '\n' +
        "LOAD	MEMBER-1	Member load	MEMBER-2	Left	1.50000" + '\n' +
        "LOAD	MEMBER-1	Member load	MEMBER-2	Right	3.00000" + '\n' +
        "LOAD	MEMBER-1	Axial	1.00000	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000" + '\n' +
        "LOAD	MEMBER-1	Axial	2.00000	3.00000	4.50000	6.00000	7.00000	8.90000	10.0000	11.0000	12.0000	13.0000" + '\n';

      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;
      ForcePerLengthUnit forcePerLengthUnit = Units.GetForcePerLengthUnit(forceUnit, lengthUnit);
      PressureUnit forcePerAreaUnit = Units.GetForcePerAreaUnit(forceUnit, lengthUnit);

      // Act
      ComposFile composFile = ComposFile.FromCoaString(coaString);
      IMember member1 = composFile.Members[0];
      IList<ILoad> loads = member1.Loads;

      // Assert
      Assert.Equal(13, loads.Count);
      int i = 0;

      // point load
      PointLoad pointLoad = (PointLoad)loads[i++];
      //LOAD	MEMBER-1	Point	1.00000	2.00000	3.00000	4.50000	0.0600000
      Assert.Equal(1, pointLoad.Load.ConstantDead.As(forceUnit));
      Assert.Equal(2, pointLoad.Load.ConstantLive.As(forceUnit));
      Assert.Equal(3, pointLoad.Load.FinalDead.As(forceUnit));
      Assert.Equal(4.5, pointLoad.Load.FinalLive.As(forceUnit));
      Assert.Equal(6, pointLoad.Load.Position.As(lengthUnit));
      Assert.Equal(LoadType.Point, pointLoad.Type);

      // uniform line load
      UniformLoad uniformLineLoad = (UniformLoad)loads[i++];
      //LOAD	MEMBER-1	Uniform	Line	1.00000	2.00000	3.00000	4.50000	Line	1.00000	2.00000	3.00000	4.50000	3.00000	4.50000	6.00000	5.00000
      Assert.Equal(1, uniformLineLoad.Load.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(2, uniformLineLoad.Load.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(3, uniformLineLoad.Load.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(4.5, uniformLineLoad.Load.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(LoadType.Uniform, uniformLineLoad.Type);
      Assert.Equal(LoadDistribution.Line, uniformLineLoad.Distribution);

      // uniform area load
      UniformLoad uniformAreaLoad = (UniformLoad)loads[i++];
      //LOAD	MEMBER-1	Uniform	Area	3.00000	4.50000	6.00000	7.00000	Area	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	5.00000
      Assert.Equal(3, uniformAreaLoad.Load.ConstantDead.As(forcePerAreaUnit));
      Assert.Equal(4.5, uniformAreaLoad.Load.ConstantLive.As(forcePerAreaUnit));
      Assert.Equal(6, uniformAreaLoad.Load.FinalDead.As(forcePerAreaUnit));
      Assert.Equal(7, uniformAreaLoad.Load.FinalLive.As(forcePerAreaUnit));
      Assert.Equal(LoadType.Uniform, uniformAreaLoad.Type);
      Assert.Equal(LoadDistribution.Area, uniformAreaLoad.Distribution);

      // linear line load
      LinearLoad linearLineLoad = (LinearLoad)loads[i++];
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

      // linear area load
      LinearLoad linearAreaLoad = (LinearLoad)loads[i++];
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

      // trilinear line load
      TriLinearLoad trilinearLineLoad = (TriLinearLoad)loads[i++];
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

      // trilinear area load
      TriLinearLoad trilinearAreaLoad = (TriLinearLoad)loads[i++];
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

      // patch line load
      PatchLoad patchLineLoad = (PatchLoad)loads[i++];
      //LOAD	MEMBER-1	Patch	Line	2.00000	3.00000	4.50000	6.00000	7.00000	3.00000	4.50000	6.00000	7.00000	8.90000
      Assert.Equal(2, patchLineLoad.LoadW1.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(3, patchLineLoad.LoadW1.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(4.5, patchLineLoad.LoadW1.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(6, patchLineLoad.LoadW1.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(7, patchLineLoad.LoadW1.Position.As(lengthUnit));
      Assert.Equal(3, patchLineLoad.LoadW2.ConstantDead.As(forcePerLengthUnit));
      Assert.Equal(4.5, patchLineLoad.LoadW2.ConstantLive.As(forcePerLengthUnit));
      Assert.Equal(6, patchLineLoad.LoadW2.FinalDead.As(forcePerLengthUnit));
      Assert.Equal(7, patchLineLoad.LoadW2.FinalLive.As(forcePerLengthUnit));
      Assert.Equal(8.9, patchLineLoad.LoadW2.Position.As(lengthUnit));
      Assert.Equal(LoadType.Patch, patchLineLoad.Type);
      Assert.Equal(LoadDistribution.Line, patchLineLoad.Distribution);

      // patch area
      PatchLoad patchAreaLoad = (PatchLoad)loads[i++];
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

      // member load
      MemberLoad memberLoad1 = (MemberLoad)loads[i++];
      //LOAD	MEMBER-1	Member load	MEMBER-2	Left	1.50000
      Assert.Equal(1.5, memberLoad1.Position.As(lengthUnit));
      Assert.Equal("MEMBER-2", memberLoad1.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Left, memberLoad1.Support);
      Assert.Equal(LoadType.MemberLoad, memberLoad1.Type);

      MemberLoad memberLoad2 = (MemberLoad)loads[i++];
      //LOAD	MEMBER-1	Member load	MEMBER-2	Right	3.00000
      Assert.Equal(3, memberLoad2.Position.As(lengthUnit));
      Assert.Equal("MEMBER-2", memberLoad2.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Right, memberLoad2.Support);
      Assert.Equal(LoadType.MemberLoad, memberLoad2.Type);

      // axial load
      AxialLoad axialLoad1 = (AxialLoad)loads[i++];
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

      AxialLoad axialLoad2 = (AxialLoad)loads[i++];
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
