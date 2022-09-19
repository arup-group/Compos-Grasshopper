using System;
using System.Collections.Generic;
using System.Linq;

using OasysUnitsNet;
using OasysUnitsNet.Units;

namespace ComposAPI
{
  public class MemberLoad : Load
  {
    public IQuantity Position { get; set; }
    public string MemberName { get; set; }
    public enum SupportSide { Left, Right };
    public SupportSide Support { get; set; }
    public MemberLoad() { this.m_type = LoadType.MemberLoad; }
    public MemberLoad(string memberName, SupportSide supportSide, IQuantity position)
    {
      if (position.QuantityInfo.UnitType != typeof(LengthUnit)
          & position.QuantityInfo.UnitType != typeof(RatioUnit))
        throw new ArgumentException("Position must be either Length or Ratio");
      this.Position = position;
      this.MemberName = memberName;
      this.Support = supportSide;
      this.m_type = LoadType.MemberLoad;
    }
  }
}
