using UnitsNet;
using UnitsNet.Units;
using Xunit;
using static ComposAPI.Load;

namespace ComposAPI.Tests
{
  public partial class LoadTest
  {
    [Fact]
    public void TestPointLoadDuplicate()
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForceUnit force = ForceUnit.Kilonewton;

      // 1 create with constructor and duplicate
      Load originalParent = TestPointLoadConstructor(0, 0, 1, 1, 0.5);
      Load duplicateParent = originalParent.Duplicate();

      // 1b create child
      PointLoad duplicateChild = (PointLoad)duplicateParent;
      PointLoad originalChild = (PointLoad)originalParent;


      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChild.Load.ConstantDead.As(force));
      Assert.Equal(0, duplicateChild.Load.ConstantLive.As(force));
      Assert.Equal(1, duplicateChild.Load.FinalDead.As(force));
      Assert.Equal(1, duplicateChild.Load.FinalLive.As(force));
      Assert.Equal(0.5, duplicateChild.Load.Position.As(length));
      Assert.Equal(LoadType.Point, duplicateChild.Type);

      // 3 make some changes to duplicate
      duplicateChild.Load.ConstantDead = new Force(15, force);
      duplicateChild.Load.ConstantLive = new Force(20, force);
      duplicateChild.Load.FinalDead = new Force(-10, force);
      duplicateChild.Load.FinalLive = new Force(-5, force);
      duplicateChild.Load.Position = new Length(5000, length);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChild.Load.ConstantDead.As(force));
      Assert.Equal(20, duplicateChild.Load.ConstantLive.As(force));
      Assert.Equal(-10, duplicateChild.Load.FinalDead.As(force));
      Assert.Equal(-5, duplicateChild.Load.FinalLive.As(force));
      Assert.Equal(5000, duplicateChild.Load.Position.As(length));
      Assert.Equal(LoadType.Point, duplicateChild.Type);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChild.Load.ConstantDead.As(force));
      Assert.Equal(0, originalChild.Load.ConstantLive.As(force));
      Assert.Equal(1, originalChild.Load.FinalDead.As(force));
      Assert.Equal(1, originalChild.Load.FinalLive.As(force));
      Assert.Equal(0.5, originalChild.Load.Position.As(length));
      Assert.Equal(LoadType.Point, originalChild.Type);
    }

    [Fact]
    public void TestUniformLoadDuplicate()
    {
      ForcePerLengthUnit length = ForcePerLengthUnit.KilonewtonPerMeter;
      PressureUnit area = PressureUnit.KilonewtonPerSquareMeter;

      // 1 create with constructor and duplicate
      Load originalParent = TestUniformLineLoadConstructor(0, 0, 1, 1);
      Load duplicateParent = originalParent.Duplicate();

      // 1b create child
      UniformLoad duplicateChildLine = (UniformLoad)duplicateParent;
      UniformLoad originalChildLine = (UniformLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildLine.Load.ConstantDead.As(length));
      Assert.Equal(0, duplicateChildLine.Load.ConstantLive.As(length));
      Assert.Equal(1, duplicateChildLine.Load.FinalDead.As(length));
      Assert.Equal(1, duplicateChildLine.Load.FinalLive.As(length));
      Assert.Equal(LoadType.Uniform, duplicateChildLine.Type);
      Assert.Equal(LoadDistribution.Line, duplicateChildLine.Distribution);

      // 3 make some changes to duplicate
      duplicateChildLine.Load.ConstantDead = new ForcePerLength(15, length);
      duplicateChildLine.Load.ConstantLive = new ForcePerLength(20, length);
      duplicateChildLine.Load.FinalDead = new ForcePerLength(-10, length);
      duplicateChildLine.Load.FinalLive = new ForcePerLength(-5, length);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildLine.Load.ConstantDead.As(length));
      Assert.Equal(20, duplicateChildLine.Load.ConstantLive.As(length));
      Assert.Equal(-10, duplicateChildLine.Load.FinalDead.As(length));
      Assert.Equal(-5, duplicateChildLine.Load.FinalLive.As(length));
      Assert.Equal(LoadType.Uniform, duplicateChildLine.Type);
      Assert.Equal(LoadDistribution.Line, duplicateChildLine.Distribution);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildLine.Load.ConstantDead.As(length));
      Assert.Equal(0, originalChildLine.Load.ConstantLive.As(length));
      Assert.Equal(1, originalChildLine.Load.FinalDead.As(length));
      Assert.Equal(1, originalChildLine.Load.FinalLive.As(length));
      Assert.Equal(LoadType.Uniform, originalChildLine.Type);
      Assert.Equal(LoadDistribution.Line, originalChildLine.Distribution);


      // 1 create with constructor and duplicate
      originalParent = TestUniformAreaLoadConstructor(0, 0, 1, 1);
      duplicateParent = originalParent.Duplicate();

      // 1b create child
      UniformLoad duplicateChildArea = (UniformLoad)duplicateParent;
      UniformLoad originalChildArea = (UniformLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildArea.Load.ConstantDead.As(area));
      Assert.Equal(0, duplicateChildArea.Load.ConstantLive.As(area));
      Assert.Equal(1, duplicateChildArea.Load.FinalDead.As(area));
      Assert.Equal(1, duplicateChildArea.Load.FinalLive.As(area));
      Assert.Equal(LoadType.Uniform, duplicateChildArea.Type);
      Assert.Equal(LoadDistribution.Area, duplicateChildArea.Distribution);

      // 3 make some changes to duplicate
      duplicateChildArea.Load.ConstantDead = new Pressure(15, area);
      duplicateChildArea.Load.ConstantLive = new Pressure(20, area);
      duplicateChildArea.Load.FinalDead = new Pressure(-10, area);
      duplicateChildArea.Load.FinalLive = new Pressure(-5, area);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildArea.Load.ConstantDead.As(area));
      Assert.Equal(20, duplicateChildArea.Load.ConstantLive.As(area));
      Assert.Equal(-10, duplicateChildArea.Load.FinalDead.As(area));
      Assert.Equal(-5, duplicateChildArea.Load.FinalLive.As(area));
      Assert.Equal(LoadType.Uniform, duplicateChildArea.Type);
      Assert.Equal(LoadDistribution.Area, duplicateChildArea.Distribution);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildArea.Load.ConstantDead.As(area));
      Assert.Equal(0, originalChildArea.Load.ConstantLive.As(area));
      Assert.Equal(1, originalChildArea.Load.FinalDead.As(area));
      Assert.Equal(1, originalChildArea.Load.FinalLive.As(area));
      Assert.Equal(LoadType.Uniform, originalChildArea.Type);
      Assert.Equal(LoadDistribution.Area, originalChildArea.Distribution);
    }

    [Fact]
    public void TestLinearLoadDuplicate()
    {
      ForcePerLengthUnit length = ForcePerLengthUnit.KilonewtonPerMeter;
      PressureUnit area = PressureUnit.KilonewtonPerSquareMeter;

      // 1 create with constructor and duplicate
      Load originalParent = TestLinearLineLoadConstructor(0, 0, 1, 1, 2, 2, 3, 3);
      Load duplicateParent = originalParent.Duplicate();

      // 1b create child
      LinearLoad duplicateChildLine = (LinearLoad)duplicateParent;
      LinearLoad originalChildLine = (LinearLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildLine.LoadW1.ConstantDead.As(length));
      Assert.Equal(0, duplicateChildLine.LoadW1.ConstantLive.As(length));
      Assert.Equal(1, duplicateChildLine.LoadW1.FinalDead.As(length));
      Assert.Equal(1, duplicateChildLine.LoadW1.FinalLive.As(length));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantDead.As(length));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantLive.As(length));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalDead.As(length));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalLive.As(length));
      Assert.Equal(LoadType.Linear, duplicateChildLine.Type);
      Assert.Equal(LoadDistribution.Line, duplicateChildLine.Distribution);

      // 3 make some changes to duplicate
      duplicateChildLine.LoadW1.ConstantDead = new ForcePerLength(15, length);
      duplicateChildLine.LoadW1.ConstantLive = new ForcePerLength(20, length);
      duplicateChildLine.LoadW1.FinalDead = new ForcePerLength(-10, length);
      duplicateChildLine.LoadW1.FinalLive = new ForcePerLength(-5, length);
      duplicateChildLine.LoadW2.ConstantDead = new ForcePerLength(1, length);
      duplicateChildLine.LoadW2.ConstantLive = new ForcePerLength(2, length);
      duplicateChildLine.LoadW2.FinalDead = new ForcePerLength(3, length);
      duplicateChildLine.LoadW2.FinalLive = new ForcePerLength(4, length);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildLine.LoadW1.ConstantDead.As(length));
      Assert.Equal(20, duplicateChildLine.LoadW1.ConstantLive.As(length));
      Assert.Equal(-10, duplicateChildLine.LoadW1.FinalDead.As(length));
      Assert.Equal(-5, duplicateChildLine.LoadW1.FinalLive.As(length));
      Assert.Equal(1, duplicateChildLine.LoadW2.ConstantDead.As(length));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantLive.As(length));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalDead.As(length));
      Assert.Equal(4, duplicateChildLine.LoadW2.FinalLive.As(length));
      Assert.Equal(LoadType.Linear, duplicateChildLine.Type);
      Assert.Equal(LoadDistribution.Line, duplicateChildLine.Distribution);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildLine.LoadW1.ConstantDead.As(length));
      Assert.Equal(0, originalChildLine.LoadW1.ConstantLive.As(length));
      Assert.Equal(1, originalChildLine.LoadW1.FinalDead.As(length));
      Assert.Equal(1, originalChildLine.LoadW1.FinalLive.As(length));
      Assert.Equal(2, originalChildLine.LoadW2.ConstantDead.As(length));
      Assert.Equal(2, originalChildLine.LoadW2.ConstantLive.As(length));
      Assert.Equal(3, originalChildLine.LoadW2.FinalDead.As(length));
      Assert.Equal(3, originalChildLine.LoadW2.FinalLive.As(length));
      Assert.Equal(LoadType.Linear, originalChildLine.Type);
      Assert.Equal(LoadDistribution.Line, originalChildLine.Distribution);


      // 1 create with constructor and duplicate
      originalParent = TestLinearAreaLoadConstructor(0, 0, 1, 1, 2, 2, 3, 3);
      duplicateParent = originalParent.Duplicate();

      // 1b create child
      LinearLoad duplicateChildArea = (LinearLoad)duplicateParent;
      LinearLoad originalChildArea = (LinearLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildArea.LoadW1.ConstantDead.As(area));
      Assert.Equal(0, duplicateChildArea.LoadW1.ConstantLive.As(area));
      Assert.Equal(1, duplicateChildArea.LoadW1.FinalDead.As(area));
      Assert.Equal(1, duplicateChildArea.LoadW1.FinalLive.As(area));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantDead.As(area));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantLive.As(area));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalDead.As(area));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalLive.As(area));
      Assert.Equal(LoadType.Linear, duplicateChildArea.Type);
      Assert.Equal(LoadDistribution.Area, duplicateChildArea.Distribution);

      // 3 make some changes to duplicate
      duplicateChildArea.LoadW1.ConstantDead = new Pressure(15, area);
      duplicateChildArea.LoadW1.ConstantLive = new Pressure(20, area);
      duplicateChildArea.LoadW1.FinalDead = new Pressure(-10, area);
      duplicateChildArea.LoadW1.FinalLive = new Pressure(-5, area);
      duplicateChildArea.LoadW2.ConstantDead = new Pressure(1, area);
      duplicateChildArea.LoadW2.ConstantLive = new Pressure(2, area);
      duplicateChildArea.LoadW2.FinalDead = new Pressure(3, area);
      duplicateChildArea.LoadW2.FinalLive = new Pressure(4, area);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildArea.LoadW1.ConstantDead.As(area));
      Assert.Equal(20, duplicateChildArea.LoadW1.ConstantLive.As(area));
      Assert.Equal(-10, duplicateChildArea.LoadW1.FinalDead.As(area));
      Assert.Equal(-5, duplicateChildArea.LoadW1.FinalLive.As(area));
      Assert.Equal(1, duplicateChildArea.LoadW2.ConstantDead.As(area));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantLive.As(area));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalDead.As(area));
      Assert.Equal(4, duplicateChildArea.LoadW2.FinalLive.As(area));
      Assert.Equal(LoadType.Linear, duplicateChildArea.Type);
      Assert.Equal(LoadDistribution.Area, duplicateChildArea.Distribution);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildArea.LoadW1.ConstantDead.As(area));
      Assert.Equal(0, originalChildArea.LoadW1.ConstantLive.As(area));
      Assert.Equal(1, originalChildArea.LoadW1.FinalDead.As(area));
      Assert.Equal(1, originalChildArea.LoadW1.FinalLive.As(area));
      Assert.Equal(2, originalChildArea.LoadW2.ConstantDead.As(area));
      Assert.Equal(2, originalChildArea.LoadW2.ConstantLive.As(area));
      Assert.Equal(3, originalChildArea.LoadW2.FinalDead.As(area));
      Assert.Equal(3, originalChildArea.LoadW2.FinalLive.As(area));
      Assert.Equal(LoadType.Linear, originalChildArea.Type);
      Assert.Equal(LoadDistribution.Area, originalChildArea.Distribution);
    }

    [Fact]
    public void TestTriLinearLoadDuplicate()
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForcePerLengthUnit flength = ForcePerLengthUnit.KilonewtonPerMeter;
      PressureUnit farea = PressureUnit.KilonewtonPerSquareMeter;

      // 1 create with constructor and duplicate
      Load originalParent = TestTriLinearLineLoadConstructor(0, 0, 1, 1, 0.5, 2, 2, 3, 3, 1.5);
      Load duplicateParent = originalParent.Duplicate();

      // 1b create child
      TriLinearLoad duplicateChildLine = (TriLinearLoad)duplicateParent;
      TriLinearLoad originalChildLine = (TriLinearLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildLine.LoadW1.ConstantDead.As(flength));
      Assert.Equal(0, duplicateChildLine.LoadW1.ConstantLive.As(flength));
      Assert.Equal(1, duplicateChildLine.LoadW1.FinalDead.As(flength));
      Assert.Equal(1, duplicateChildLine.LoadW1.FinalLive.As(flength));
      Assert.Equal(0.5, duplicateChildLine.LoadW1.Position.As(length));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantDead.As(flength));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantLive.As(flength));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalDead.As(flength));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalLive.As(flength));
      Assert.Equal(1.5, duplicateChildLine.LoadW2.Position.As(length));
      Assert.Equal(LoadType.TriLinear, duplicateChildLine.Type);
      Assert.Equal(LoadDistribution.Line, duplicateChildLine.Distribution);

      // 3 make some changes to duplicate
      duplicateChildLine.LoadW1.ConstantDead = new ForcePerLength(15, flength);
      duplicateChildLine.LoadW1.ConstantLive = new ForcePerLength(20, flength);
      duplicateChildLine.LoadW1.FinalDead = new ForcePerLength(-10, flength);
      duplicateChildLine.LoadW1.FinalLive = new ForcePerLength(-5, flength);
      duplicateChildLine.LoadW1.Position = new Length(700, length);
      duplicateChildLine.LoadW2.ConstantDead = new ForcePerLength(1, flength);
      duplicateChildLine.LoadW2.ConstantLive = new ForcePerLength(2, flength);
      duplicateChildLine.LoadW2.FinalDead = new ForcePerLength(3, flength);
      duplicateChildLine.LoadW2.FinalLive = new ForcePerLength(4, flength);
      duplicateChildLine.LoadW2.Position = new Length(9000, length);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildLine.LoadW1.ConstantDead.As(flength));
      Assert.Equal(20, duplicateChildLine.LoadW1.ConstantLive.As(flength));
      Assert.Equal(-10, duplicateChildLine.LoadW1.FinalDead.As(flength));
      Assert.Equal(-5, duplicateChildLine.LoadW1.FinalLive.As(flength));
      Assert.Equal(700, duplicateChildLine.LoadW1.Position.As(length));
      Assert.Equal(1, duplicateChildLine.LoadW2.ConstantDead.As(flength));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantLive.As(flength));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalDead.As(flength));
      Assert.Equal(4, duplicateChildLine.LoadW2.FinalLive.As(flength));
      Assert.Equal(9000, duplicateChildLine.LoadW2.Position.As(length));
      Assert.Equal(LoadType.TriLinear, duplicateChildLine.Type);
      Assert.Equal(LoadDistribution.Line, duplicateChildLine.Distribution);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildLine.LoadW1.ConstantDead.As(flength));
      Assert.Equal(0, originalChildLine.LoadW1.ConstantLive.As(flength));
      Assert.Equal(1, originalChildLine.LoadW1.FinalDead.As(flength));
      Assert.Equal(1, originalChildLine.LoadW1.FinalLive.As(flength));
      Assert.Equal(0.5, originalChildLine.LoadW1.Position.As(length));
      Assert.Equal(2, originalChildLine.LoadW2.ConstantDead.As(flength));
      Assert.Equal(2, originalChildLine.LoadW2.ConstantLive.As(flength));
      Assert.Equal(3, originalChildLine.LoadW2.FinalDead.As(flength));
      Assert.Equal(3, originalChildLine.LoadW2.FinalLive.As(flength));
      Assert.Equal(1.5, originalChildLine.LoadW2.Position.As(length));
      Assert.Equal(LoadType.TriLinear, originalChildLine.Type);
      Assert.Equal(LoadDistribution.Line, originalChildLine.Distribution);


      // 1 create with constructor and duplicate
      originalParent = TestTriLinearAreaLoadConstructor(0, 0, 1, 1, 0.5, 2, 2, 3, 3, 1.5);
      duplicateParent = originalParent.Duplicate();

      // 1b create child
      TriLinearLoad duplicateChildArea = (TriLinearLoad)duplicateParent;
      TriLinearLoad originalChildArea = (TriLinearLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildArea.LoadW1.ConstantDead.As(farea));
      Assert.Equal(0, duplicateChildArea.LoadW1.ConstantLive.As(farea));
      Assert.Equal(1, duplicateChildArea.LoadW1.FinalDead.As(farea));
      Assert.Equal(1, duplicateChildArea.LoadW1.FinalLive.As(farea));
      Assert.Equal(0.5, duplicateChildArea.LoadW1.Position.As(length));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantDead.As(farea));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantLive.As(farea));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalDead.As(farea));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalLive.As(farea));
      Assert.Equal(1.5, duplicateChildArea.LoadW2.Position.As(length));
      Assert.Equal(LoadType.TriLinear, duplicateChildArea.Type);
      Assert.Equal(LoadDistribution.Area, duplicateChildArea.Distribution);

      // 3 make some changes to duplicate
      duplicateChildArea.LoadW1.ConstantDead = new Pressure(15, farea);
      duplicateChildArea.LoadW1.ConstantLive = new Pressure(20, farea);
      duplicateChildArea.LoadW1.FinalDead = new Pressure(-10, farea);
      duplicateChildArea.LoadW1.FinalLive = new Pressure(-5, farea);
      duplicateChildArea.LoadW1.Position = new Length(700, length);
      duplicateChildArea.LoadW2.ConstantDead = new Pressure(1, farea);
      duplicateChildArea.LoadW2.ConstantLive = new Pressure(2, farea);
      duplicateChildArea.LoadW2.FinalDead = new Pressure(3, farea);
      duplicateChildArea.LoadW2.FinalLive = new Pressure(4, farea);
      duplicateChildArea.LoadW2.Position = new Length(9000, length);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildArea.LoadW1.ConstantDead.As(farea));
      Assert.Equal(20, duplicateChildArea.LoadW1.ConstantLive.As(farea));
      Assert.Equal(-10, duplicateChildArea.LoadW1.FinalDead.As(farea));
      Assert.Equal(-5, duplicateChildArea.LoadW1.FinalLive.As(farea));
      Assert.Equal(700, duplicateChildArea.LoadW1.Position.As(length));
      Assert.Equal(1, duplicateChildArea.LoadW2.ConstantDead.As(farea));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantLive.As(farea));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalDead.As(farea));
      Assert.Equal(4, duplicateChildArea.LoadW2.FinalLive.As(farea));
      Assert.Equal(LoadType.TriLinear, duplicateChildArea.Type);
      Assert.Equal(LoadDistribution.Area, duplicateChildArea.Distribution);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildArea.LoadW1.ConstantDead.As(farea));
      Assert.Equal(0, originalChildArea.LoadW1.ConstantLive.As(farea));
      Assert.Equal(1, originalChildArea.LoadW1.FinalDead.As(farea));
      Assert.Equal(1, originalChildArea.LoadW1.FinalLive.As(farea));
      Assert.Equal(0.5, originalChildArea.LoadW1.Position.As(length));
      Assert.Equal(2, originalChildArea.LoadW2.ConstantDead.As(farea));
      Assert.Equal(2, originalChildArea.LoadW2.ConstantLive.As(farea));
      Assert.Equal(3, originalChildArea.LoadW2.FinalDead.As(farea));
      Assert.Equal(3, originalChildArea.LoadW2.FinalLive.As(farea));
      Assert.Equal(1.5, originalChildArea.LoadW2.Position.As(length));
      Assert.Equal(LoadType.TriLinear, originalChildArea.Type);
      Assert.Equal(LoadDistribution.Area, originalChildArea.Distribution);
    }

    [Fact]
    public void TestPatchLoadDuplicate()
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForcePerLengthUnit flength = ForcePerLengthUnit.KilonewtonPerMeter;
      PressureUnit farea = PressureUnit.KilonewtonPerSquareMeter;

      // 1 create with constructor and duplicate
      Load originalParent = TestPatchLineLoadConstructor(0, 0, 1, 1, 0.5, 2, 2, 3, 3, 1.5);
      Load duplicateParent = originalParent.Duplicate();

      // 1b create child
      PatchLoad duplicateChildLine = (PatchLoad)duplicateParent;
      PatchLoad originalChildLine = (PatchLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildLine.LoadW1.ConstantDead.As(flength));
      Assert.Equal(0, duplicateChildLine.LoadW1.ConstantLive.As(flength));
      Assert.Equal(1, duplicateChildLine.LoadW1.FinalDead.As(flength));
      Assert.Equal(1, duplicateChildLine.LoadW1.FinalLive.As(flength));
      Assert.Equal(0.5, duplicateChildLine.LoadW1.Position.As(length));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantDead.As(flength));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantLive.As(flength));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalDead.As(flength));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalLive.As(flength));
      Assert.Equal(1.5, duplicateChildLine.LoadW2.Position.As(length));
      Assert.Equal(LoadType.Patch, duplicateChildLine.Type);
      Assert.Equal(LoadDistribution.Line, duplicateChildLine.Distribution);

      // 3 make some changes to duplicate
      duplicateChildLine.LoadW1.ConstantDead = new ForcePerLength(15, flength);
      duplicateChildLine.LoadW1.ConstantLive = new ForcePerLength(20, flength);
      duplicateChildLine.LoadW1.FinalDead = new ForcePerLength(-10, flength);
      duplicateChildLine.LoadW1.FinalLive = new ForcePerLength(-5, flength);
      duplicateChildLine.LoadW1.Position = new Length(700, length);
      duplicateChildLine.LoadW2.ConstantDead = new ForcePerLength(1, flength);
      duplicateChildLine.LoadW2.ConstantLive = new ForcePerLength(2, flength);
      duplicateChildLine.LoadW2.FinalDead = new ForcePerLength(3, flength);
      duplicateChildLine.LoadW2.FinalLive = new ForcePerLength(4, flength);
      duplicateChildLine.LoadW2.Position = new Length(9000, length);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildLine.LoadW1.ConstantDead.As(flength));
      Assert.Equal(20, duplicateChildLine.LoadW1.ConstantLive.As(flength));
      Assert.Equal(-10, duplicateChildLine.LoadW1.FinalDead.As(flength));
      Assert.Equal(-5, duplicateChildLine.LoadW1.FinalLive.As(flength));
      Assert.Equal(700, duplicateChildLine.LoadW1.Position.As(length));
      Assert.Equal(1, duplicateChildLine.LoadW2.ConstantDead.As(flength));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantLive.As(flength));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalDead.As(flength));
      Assert.Equal(4, duplicateChildLine.LoadW2.FinalLive.As(flength));
      Assert.Equal(9000, duplicateChildLine.LoadW2.Position.As(length));
      Assert.Equal(LoadType.Patch, duplicateChildLine.Type);
      Assert.Equal(LoadDistribution.Line, duplicateChildLine.Distribution);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildLine.LoadW1.ConstantDead.As(flength));
      Assert.Equal(0, originalChildLine.LoadW1.ConstantLive.As(flength));
      Assert.Equal(1, originalChildLine.LoadW1.FinalDead.As(flength));
      Assert.Equal(1, originalChildLine.LoadW1.FinalLive.As(flength));
      Assert.Equal(0.5, originalChildLine.LoadW1.Position.As(length));
      Assert.Equal(2, originalChildLine.LoadW2.ConstantDead.As(flength));
      Assert.Equal(2, originalChildLine.LoadW2.ConstantLive.As(flength));
      Assert.Equal(3, originalChildLine.LoadW2.FinalDead.As(flength));
      Assert.Equal(3, originalChildLine.LoadW2.FinalLive.As(flength));
      Assert.Equal(1.5, originalChildLine.LoadW2.Position.As(length));
      Assert.Equal(LoadType.Patch, originalChildLine.Type);
      Assert.Equal(LoadDistribution.Line, originalChildLine.Distribution);


      // 1 create with constructor and duplicate
      originalParent = TestPatchAreaLoadConstructor(0, 0, 1, 1, 0.5, 2, 2, 3, 3, 1.5);
      duplicateParent = originalParent.Duplicate();

      // 1b create child
      PatchLoad duplicateChildArea = (PatchLoad)duplicateParent;
      PatchLoad originalChildArea = (PatchLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildArea.LoadW1.ConstantDead.As(farea));
      Assert.Equal(0, duplicateChildArea.LoadW1.ConstantLive.As(farea));
      Assert.Equal(1, duplicateChildArea.LoadW1.FinalDead.As(farea));
      Assert.Equal(1, duplicateChildArea.LoadW1.FinalLive.As(farea));
      Assert.Equal(0.5, duplicateChildArea.LoadW1.Position.As(length));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantDead.As(farea));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantLive.As(farea));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalDead.As(farea));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalLive.As(farea));
      Assert.Equal(1.5, duplicateChildArea.LoadW2.Position.As(length));
      Assert.Equal(LoadType.Patch, duplicateChildArea.Type);
      Assert.Equal(LoadDistribution.Area, duplicateChildArea.Distribution);

      // 3 make some changes to duplicate
      duplicateChildArea.LoadW1.ConstantDead = new Pressure(15, farea);
      duplicateChildArea.LoadW1.ConstantLive = new Pressure(20, farea);
      duplicateChildArea.LoadW1.FinalDead = new Pressure(-10, farea);
      duplicateChildArea.LoadW1.FinalLive = new Pressure(-5, farea);
      duplicateChildArea.LoadW1.Position = new Length(700, length);
      duplicateChildArea.LoadW2.ConstantDead = new Pressure(1, farea);
      duplicateChildArea.LoadW2.ConstantLive = new Pressure(2, farea);
      duplicateChildArea.LoadW2.FinalDead = new Pressure(3, farea);
      duplicateChildArea.LoadW2.FinalLive = new Pressure(4, farea);
      duplicateChildArea.LoadW2.Position = new Length(9000, length);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildArea.LoadW1.ConstantDead.As(farea));
      Assert.Equal(20, duplicateChildArea.LoadW1.ConstantLive.As(farea));
      Assert.Equal(-10, duplicateChildArea.LoadW1.FinalDead.As(farea));
      Assert.Equal(-5, duplicateChildArea.LoadW1.FinalLive.As(farea));
      Assert.Equal(700, duplicateChildArea.LoadW1.Position.As(length));
      Assert.Equal(1, duplicateChildArea.LoadW2.ConstantDead.As(farea));
      Assert.Equal(2, duplicateChildArea.LoadW2.ConstantLive.As(farea));
      Assert.Equal(3, duplicateChildArea.LoadW2.FinalDead.As(farea));
      Assert.Equal(4, duplicateChildArea.LoadW2.FinalLive.As(farea));
      Assert.Equal(LoadType.Patch, duplicateChildArea.Type);
      Assert.Equal(LoadDistribution.Area, duplicateChildArea.Distribution);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildArea.LoadW1.ConstantDead.As(farea));
      Assert.Equal(0, originalChildArea.LoadW1.ConstantLive.As(farea));
      Assert.Equal(1, originalChildArea.LoadW1.FinalDead.As(farea));
      Assert.Equal(1, originalChildArea.LoadW1.FinalLive.As(farea));
      Assert.Equal(0.5, originalChildArea.LoadW1.Position.As(length));
      Assert.Equal(2, originalChildArea.LoadW2.ConstantDead.As(farea));
      Assert.Equal(2, originalChildArea.LoadW2.ConstantLive.As(farea));
      Assert.Equal(3, originalChildArea.LoadW2.FinalDead.As(farea));
      Assert.Equal(3, originalChildArea.LoadW2.FinalLive.As(farea));
      Assert.Equal(1.5, originalChildArea.LoadW2.Position.As(length));
      Assert.Equal(LoadType.Patch, originalChildArea.Type);
      Assert.Equal(LoadDistribution.Area, originalChildArea.Distribution);
    }

    [Fact]
    public void TestMemberLoadDuplicate()
    {
      LengthUnit length = LengthUnit.Millimeter;

      // 1 create with constructor and duplicate
      Load originalParent = TestMemberLoadConstructor(150, "Original", MemberLoad.SupportSide.Left);
      Load duplicateParent = originalParent.Duplicate();

      // 1b create child
      MemberLoad duplicateChild = (MemberLoad)duplicateParent;
      MemberLoad originalChild = (MemberLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(150, duplicateChild.Position.As(length));
      Assert.Equal("Original", duplicateChild.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Left, duplicateChild.Support);
      Assert.Equal(LoadType.MemberLoad, duplicateChild.Type);

      // 3 make some changes to duplicate
      duplicateChild.Position = new Length(9000, length);
      duplicateChild.MemberName = "Duplicate";
      duplicateChild.Support = MemberLoad.SupportSide.Right;

      // 4 check that duplicate has set changes
      Assert.Equal(9000, duplicateChild.Position.As(length));
      Assert.Equal("Duplicate", duplicateChild.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Right, duplicateChild.Support);
      Assert.Equal(LoadType.MemberLoad, duplicateChild.Type);

      // 5 check that original has not been changed
      Assert.Equal(150, originalChild.Position.As(length));
      Assert.Equal("Original", originalChild.MemberName);
      Assert.Equal(MemberLoad.SupportSide.Left, originalChild.Support);
      Assert.Equal(LoadType.MemberLoad, originalChild.Type);
    }

    [Fact]
    public void TestAxialLoadDuplicate()
    {
      LengthUnit length = LengthUnit.Millimeter;
      ForceUnit force = ForceUnit.Kilonewton;

      // 1 create with constructor and duplicate
      Load originalParent = TestAxialLoadConstructor(0, 0, 1, 1, 0.5, 2, 2, 3, 3, 1.5);
      Load duplicateParent = originalParent.Duplicate();

      // 1b create child
      AxialLoad duplicateChildLine = (AxialLoad)duplicateParent;
      AxialLoad originalChildLine = (AxialLoad)originalParent;

      // 2 check that duplicate has duplicated values
      Assert.Equal(0, duplicateChildLine.LoadW1.ConstantDead.As(force));
      Assert.Equal(0, duplicateChildLine.LoadW1.ConstantLive.As(force));
      Assert.Equal(1, duplicateChildLine.LoadW1.FinalDead.As(force));
      Assert.Equal(1, duplicateChildLine.LoadW1.FinalLive.As(force));
      Assert.Equal(0.5, duplicateChildLine.Depth1.As(length));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantDead.As(force));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantLive.As(force));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalDead.As(force));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalLive.As(force));
      Assert.Equal(1.5, duplicateChildLine.Depth2.As(length));
      Assert.Equal(LoadType.Axial, duplicateChildLine.Type);

      // 3 make some changes to duplicate
      duplicateChildLine.LoadW1.ConstantDead = new Force(15, force);
      duplicateChildLine.LoadW1.ConstantLive = new Force(20, force);
      duplicateChildLine.LoadW1.FinalDead = new Force(-10, force);
      duplicateChildLine.LoadW1.FinalLive = new Force(-5, force);
      duplicateChildLine.Depth1 = new Length(700, length);
      duplicateChildLine.LoadW2.ConstantDead = new Force(1, force);
      duplicateChildLine.LoadW2.ConstantLive = new Force(2, force);
      duplicateChildLine.LoadW2.FinalDead = new Force(3, force);
      duplicateChildLine.LoadW2.FinalLive = new Force(4, force);
      duplicateChildLine.Depth2 = new Length(9000, length);

      // 4 check that duplicate has set changes
      Assert.Equal(15, duplicateChildLine.LoadW1.ConstantDead.As(force));
      Assert.Equal(20, duplicateChildLine.LoadW1.ConstantLive.As(force));
      Assert.Equal(-10, duplicateChildLine.LoadW1.FinalDead.As(force));
      Assert.Equal(-5, duplicateChildLine.LoadW1.FinalLive.As(force));
      Assert.Equal(700, duplicateChildLine.Depth1.As(length));
      Assert.Equal(1, duplicateChildLine.LoadW2.ConstantDead.As(force));
      Assert.Equal(2, duplicateChildLine.LoadW2.ConstantLive.As(force));
      Assert.Equal(3, duplicateChildLine.LoadW2.FinalDead.As(force));
      Assert.Equal(4, duplicateChildLine.LoadW2.FinalLive.As(force));
      Assert.Equal(9000, duplicateChildLine.Depth2.As(length));
      Assert.Equal(LoadType.Axial, duplicateChildLine.Type);

      // 5 check that original has not been changed
      Assert.Equal(0, originalChildLine.LoadW1.ConstantDead.As(force));
      Assert.Equal(0, originalChildLine.LoadW1.ConstantLive.As(force));
      Assert.Equal(1, originalChildLine.LoadW1.FinalDead.As(force));
      Assert.Equal(1, originalChildLine.LoadW1.FinalLive.As(force));
      Assert.Equal(0.5, originalChildLine.Depth1.As(length));
      Assert.Equal(2, originalChildLine.LoadW2.ConstantDead.As(force));
      Assert.Equal(2, originalChildLine.LoadW2.ConstantLive.As(force));
      Assert.Equal(3, originalChildLine.LoadW2.FinalDead.As(force));
      Assert.Equal(3, originalChildLine.LoadW2.FinalLive.As(force));
      Assert.Equal(1.5, originalChildLine.Depth2.As(length));
      Assert.Equal(LoadType.Axial, originalChildLine.Type);
    }
  }
}
