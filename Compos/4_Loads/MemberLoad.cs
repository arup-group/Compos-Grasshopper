using System;

using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  public class MemberLoad : Load
  {
    public IQuantity Position { get; set; }
    public string MemberName { get; set; }
    public enum SupportSide { Left, Right };
    public SupportSide Support { get; set; }
    public MemberLoad() { m_type = LoadType.MemberLoad; }
    public MemberLoad(string memberName, SupportSide supportSide, IQuantity position)
    {
      if (position.QuantityInfo.UnitType != typeof(LengthUnit)
          & position.QuantityInfo.UnitType != typeof(RatioUnit))
        throw new ArgumentException("Position must be either Length or Ratio");
      Position = position;
      MemberName = memberName;
      Support = supportSide;
      m_type = LoadType.MemberLoad;
    }
  }
}
