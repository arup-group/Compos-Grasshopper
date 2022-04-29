using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;

namespace ComposAPI.Loads
{
  public class PointLoad : Load
  {
    public NonConstantLoad Load { get; set; }
    public PointLoad() { this.m_type = LoadType.Point; }

    public PointLoad(Force consDead, Force consLive, Force finalDead, Force finalLive, Length position)
    {
      this.Load = new NonConstantLoad(consDead, consLive, finalDead, finalLive, position);
      this.m_type = LoadType.Point;
    }
    public override Load Duplicate()
    {
      PointLoad dup = (PointLoad)this.MemberwiseClone();
      dup.Load = this.Load.Duplicate();
      return (Load)dup;
    }
  }
  public class UniformLoad : Load
  {
    public LoadValues Load { get; set; }
    public LoadDistribution Distribution { get; }
    public UniformLoad() { this.m_type = LoadType.Uniform; }

    public UniformLoad(ForcePerLength consDead, ForcePerLength consLive, ForcePerLength finalDead, ForcePerLength finalLive)
    {
      this.Load = new LoadValues(consDead, consLive, finalDead, finalLive);
      this.m_type = LoadType.Uniform;
      this.Distribution = LoadDistribution.Line;
    }
    public UniformLoad(Pressure consDead, Pressure consLive, Pressure finalDead, Pressure finalLive)
    {
      this.Load = new LoadValues(consDead, consLive, finalDead, finalLive);
      this.m_type = LoadType.Uniform;
      this.Distribution = LoadDistribution.Area;
    }
    public override Load Duplicate()
    {
      UniformLoad dup = (UniformLoad)this.MemberwiseClone();
      dup.Load = this.Load.Duplicate();
      return (Load)dup;
    }
  }
  public class LinearLoad : Load
  {
    public LoadValues LoadW1 { get; set; }
    public LoadValues LoadW2 { get; set; }
    public LoadDistribution Distribution { get; }
    public LinearLoad() { this.m_type = LoadType.Linear; }

    public LinearLoad(
      ForcePerLength consDeadW1, ForcePerLength consLiveW1, ForcePerLength finalDeadW1, ForcePerLength finalLiveW1,
      ForcePerLength consDeadW2, ForcePerLength consLiveW2, ForcePerLength finalDeadW2, ForcePerLength finalLiveW2)
    {
      this.LoadW1 = new LoadValues(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new LoadValues(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.m_type = LoadType.Linear;
      this.Distribution = LoadDistribution.Line;
    }
    public LinearLoad(
      Pressure consDeadW1, Pressure consLiveW1, Pressure finalDeadW1, Pressure finalLiveW1,
      Pressure consDeadW2, Pressure consLiveW2, Pressure finalDeadW2, Pressure finalLiveW2)
    {
      this.LoadW1 = new LoadValues(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new LoadValues(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.m_type = LoadType.Linear;
      this.Distribution = LoadDistribution.Area;
    }
    public override Load Duplicate()
    {
      LinearLoad dup = (LinearLoad)this.MemberwiseClone();
      dup.LoadW1 = this.LoadW1.Duplicate();
      dup.LoadW2 = this.LoadW2.Duplicate();
      return (Load)dup;
    }
  }
  public class TriLinearLoad : Load
  {
    public NonConstantLoad LoadW1 { get; set; }
    public NonConstantLoad LoadW2 { get; set; }
    public LoadDistribution Distribution { get; }
    public TriLinearLoad() { this.m_type = LoadType.TriLinear; }

    public TriLinearLoad(
      ForcePerLength consDeadW1, ForcePerLength consLiveW1, ForcePerLength finalDeadW1, ForcePerLength finalLiveW1, Length positionW1,
      ForcePerLength consDeadW2, ForcePerLength consLiveW2, ForcePerLength finalDeadW2, ForcePerLength finalLiveW2, Length positionW2)
    {
      this.LoadW1 = new NonConstantLoad(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1, positionW1);
      this.LoadW2 = new NonConstantLoad(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2, positionW2);
      this.m_type = LoadType.TriLinear;
      this.Distribution = LoadDistribution.Line;
    }
    public TriLinearLoad(
      Pressure consDeadW1, Pressure consLiveW1, Pressure finalDeadW1, Pressure finalLiveW1, Length positionW1,
      Pressure consDeadW2, Pressure consLiveW2, Pressure finalDeadW2, Pressure finalLiveW2, Length positionW2)
    {
      this.LoadW1 = new NonConstantLoad(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1, positionW1);
      this.LoadW2 = new NonConstantLoad(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2, positionW2);
      this.m_type = LoadType.TriLinear;
      this.Distribution = LoadDistribution.Area;
    }
    public override Load Duplicate()
    {
      TriLinearLoad dup = (TriLinearLoad)this.MemberwiseClone();
      dup.LoadW1 = this.LoadW1.Duplicate();
      dup.LoadW2 = this.LoadW2.Duplicate();
      return (Load)dup;
    }
  }
  public class PatchLoad : Load
  {
    public NonConstantLoad LoadW1 { get; set; }
    public NonConstantLoad LoadW2 { get; set; }
    public LoadDistribution Distribution { get; }
    public PatchLoad() { this.m_type = LoadType.Patch; }

    public PatchLoad(
      ForcePerLength consDeadW1, ForcePerLength consLiveW1, ForcePerLength finalDeadW1, ForcePerLength finalLiveW1, Length positionW1,
      ForcePerLength consDeadW2, ForcePerLength consLiveW2, ForcePerLength finalDeadW2, ForcePerLength finalLiveW2, Length positionW2)
    {
      this.LoadW1 = new NonConstantLoad(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1, positionW1);
      this.LoadW2 = new NonConstantLoad(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2, positionW2);
      this.m_type = LoadType.Patch;
      this.Distribution = LoadDistribution.Line;
    }
    public PatchLoad(
      Pressure consDeadW1, Pressure consLiveW1, Pressure finalDeadW1, Pressure finalLiveW1, Length positionW1,
      Pressure consDeadW2, Pressure consLiveW2, Pressure finalDeadW2, Pressure finalLiveW2, Length positionW2)
    {
      this.LoadW1 = new NonConstantLoad(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1, positionW1);
      this.LoadW2 = new NonConstantLoad(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2, positionW2);
      this.m_type = LoadType.Patch;
      this.Distribution = LoadDistribution.Area;
    }
    public override Load Duplicate()
    {
      PatchLoad dup = (PatchLoad)this.MemberwiseClone();
      dup.LoadW1 = this.LoadW1.Duplicate();
      dup.LoadW2 = this.LoadW2.Duplicate();
      return (Load)dup;
    }
  }
  public class MemberLoad : Load
  {
    public Length Position { get; set; }
    public string MemberName { get; set; }
    public enum SupportSide { Left, Right };
    public SupportSide Support { get; set; }
    public MemberLoad() { this.m_type = LoadType.MemberLoad; }
    public MemberLoad(Length position, string memberName, SupportSide supportSide)
    {
      this.Position = position;
      this.MemberName = memberName;
      this.Support = supportSide;
      this.m_type = LoadType.MemberLoad;
    }
    public override Load Duplicate()
    {
      MemberLoad dup = (MemberLoad)this.MemberwiseClone();
      return (Load)dup;
    }
  }
  public class AxialLoad : Load
  {
    public LoadValues LoadW1 { get; set; }
    public LoadValues LoadW2 { get; set; }
    public AxialLoad() { this.m_type = LoadType.Axial; }
    public Length Depth1 { get; set; }
    public Length Depth2 { get; set; }

    public AxialLoad(
      Force consDeadW1, Force consLiveW1, Force finalDeadW1, Force finalLiveW1, Length depth1,
      Force consDeadW2, Force consLiveW2, Force finalDeadW2, Force finalLiveW2, Length depth2)
    {
      this.LoadW1 = new LoadValues(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new LoadValues(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.Depth1 = depth1;
      this.Depth2 = depth2;
      this.m_type = LoadType.Axial;
    }
    public override Load Duplicate()
    {
      AxialLoad dup = (AxialLoad)this.MemberwiseClone();
      dup.LoadW1 = this.LoadW1.Duplicate();
      dup.LoadW2 = this.LoadW2.Duplicate();
      dup.Depth1 = this.Depth1.ToUnit(this.Depth1.Unit);
      dup.Depth2 = this.Depth2.ToUnit(this.Depth2.Unit);
      return (Load)dup;
    }
  }
}
