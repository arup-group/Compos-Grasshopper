using Xunit;
using UnitsNet;
using UnitsNet.Units;
using static ComposAPI.Load;

namespace ComposAPI.Tests
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
      Load load = TestPointLoadConstructor(1, 2, 3, 4.5, 60); // position length unit in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      Load load = TestUniformLineLoadConstructor(200, 300, 450, 600); // unit in kN/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      Load load = TestUniformAreaLoadConstructor(3, 4.5, 6, 7); // unit in kN/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      Load load = TestLinearLineLoadConstructor(4.5, 6, 7, 8, 8.9, 10, 11, 12); // unit in kN/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      Load load = TestLinearAreaLoadConstructor(1, 2, 3, 4.5, 6, 7, 8, 9); // unit in kN/m
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      Load load = TestTriLinearLineLoadConstructor(2, 3, 4.5, 6, 7000, 3, 4.5, 6, 7, 8900); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Meter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Tri-Linear	Area	3.00000	4.50000	6.00000	7.00000	8.00000	4.50000	6.00000	7.00000	8.90000	10.0000\n";
      Load load = TestTriLinearAreaLoadConstructor(3, 4.5, 6, 7, 8000, 4.5, 6, 7, 8.9, 10000); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      Load load = TestPatchLineLoadConstructor(2, 3, 4.5, 6, 7000, 3, 4.5, 6, 7, 8900); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      Load load = TestPatchAreaLoadConstructor(1, 2, 3, 4.5, 6000, 7, 8.9, 10, 11, 12000); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      Load load = TestAxialLoadConstructor(1, 2, 3, 4.5, 6, 7, 8.9, 10, 11, 12); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
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
      MemberLoad load = new MemberLoad(new Length(position, length), name, side);

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
      string expected_coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Left	150.000\n";
      Load load = TestMemberLoadConstructor(150, "Member-2", MemberLoad.SupportSide.Left); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }
    [Fact]
    public void MemberLoadRightToCoaStringTest()
    {
      ForceUnit forceUnit = ForceUnit.Kilonewton;
      LengthUnit lengthUnit = LengthUnit.Millimeter;

      // Arrange
      string expected_coaString = "LOAD	MEMBER-1	Member load	MEMBER-2	Right	2.50000\n";
      Load load = TestMemberLoadConstructor(2.5, "Member-2", MemberLoad.SupportSide.Right); // pos units in mm
      // Act
      string coaString = load.ToCoaString("MEMBER-1", forceUnit, lengthUnit);
      // Assert
      Assert.Equal(expected_coaString, coaString);
    }
  }
}
