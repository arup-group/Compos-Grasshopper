using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Documentation;
using Rhino.Collections;
using UnitsNet;

namespace ComposGH.Parameters
{
  #region inheritance classes
  public class Load
  {
    public IQuantity ConstantDead { get; set; }
    public IQuantity ConstantLive { get; set; }
    public IQuantity FinalDead { get; set; }
    public IQuantity FinalLive { get; set; }

    public Load(IQuantity consDead, IQuantity consLive, IQuantity finalDead, IQuantity finalLive)
    {
      this.ConstantDead = consDead;
      this.ConstantLive = consLive;
      this.FinalDead = finalDead;
      this.FinalLive = finalLive;
    }
  }

  public class NonConstantLoad : Load
  {
    public Length Position { get; set; }
    public NonConstantLoad(IQuantity consDead, IQuantity consLive, IQuantity finalDead, IQuantity finalLive, Length position)
      : base(consDead, consLive, finalDead, finalLive)
    {
      this.Position = position;
    }
  }
  #endregion

  public class PointLoad : ComposLoad
  {
    public NonConstantLoad Load { get; set; }
    public PointLoad() { this.m_type = LoadType.Point; }

    public PointLoad(Force consDead, Force consLive, Force finalDead, Force finalLive, Length position)
    {
      this.Load = new NonConstantLoad(consDead, consLive, finalDead, finalLive, position);
      this.m_type = LoadType.Point;
    }
  }
  public class UniformLoad : ComposLoad
  {
    public Load Load { get; set; }
    public LoadDistribution Distribution { get; }
    public UniformLoad() { this.m_type = LoadType.Uniform; }

    public UniformLoad(ForcePerLength consDead, ForcePerLength consLive, ForcePerLength finalDead, ForcePerLength finalLive)
    {
      this.Load = new Load(consDead, consLive, finalDead, finalLive);
      this.m_type = LoadType.Uniform;
      this.Distribution = LoadDistribution.Line;
    }
    public UniformLoad(Pressure consDead, Pressure consLive, Pressure finalDead, Pressure finalLive)
    {
      this.Load = new Load(consDead, consLive, finalDead, finalLive);
      this.m_type = LoadType.Uniform;
      this.Distribution = LoadDistribution.Area;
    }
  }
  public class LinearLoad : ComposLoad
  {
    public Load LoadW1 { get; set; }
    public Load LoadW2 { get; set; }
    public LoadDistribution Distribution { get; }
    public LinearLoad() { this.m_type = LoadType.Linear; }

    public LinearLoad(
      ForcePerLength consDeadW1, ForcePerLength consLiveW1, ForcePerLength finalDeadW1, ForcePerLength finalLiveW1,
      ForcePerLength consDeadW2, ForcePerLength consLiveW2, ForcePerLength finalDeadW2, ForcePerLength finalLiveW2)
    {
      this.LoadW1 = new Load(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new Load(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.m_type = LoadType.Linear;
      this.Distribution = LoadDistribution.Line;
    }
    public LinearLoad(
      Pressure consDeadW1, Pressure consLiveW1, Pressure finalDeadW1, Pressure finalLiveW1,
      Pressure consDeadW2, Pressure consLiveW2, Pressure finalDeadW2, Pressure finalLiveW2)
    {
      this.LoadW1 = new Load(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new Load(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.m_type = LoadType.Linear;
      this.Distribution = LoadDistribution.Area;
    }
  }
  public class TriLinearLoad : ComposLoad
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
  }
  public class PatchLoad : ComposLoad
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
  }
  public class MemberLoad : ComposLoad
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
  }
  public class AxialLoad : ComposLoad
  {
    public Load LoadW1 { get; set; }
    public Load LoadW2 { get; set; }
    public AxialLoad() { this.m_type = LoadType.Axial; }
    public Length Depth1 { get; set; }
    public Length Depth2 { get; set; }

    public AxialLoad(
      Force consDeadW1, Force consLiveW1, Force finalDeadW1, Force finalLiveW1, Length depth1,
      Force consDeadW2, Force consLiveW2, Force finalDeadW2, Force finalLiveW2, Length depth2)
    {
      this.LoadW1 = new Load(consDeadW1, consLiveW1, finalDeadW1, finalLiveW1);
      this.LoadW2 = new Load(consDeadW2, consLiveW2, finalDeadW2, finalLiveW2);
      this.Depth1 = depth1;
      this.Depth2 = depth2;
      this.m_type = LoadType.Axial;
    }
  }
}
