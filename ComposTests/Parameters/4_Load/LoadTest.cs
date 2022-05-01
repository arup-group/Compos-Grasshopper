using Xunit;
using UnitsNet;
using UnitsNet.Units;
using ComposAPI.Loads;
using static ComposAPI.Loads.Load;

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
  }
}
