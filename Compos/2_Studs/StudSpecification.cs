using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum StudSpecType
  {
    EC4,
    BS5950,
    Other
  }

  /// <summary>
  /// Object for setting various (code dependent) specifications for a <see cref="ComposGH.Stud.Stud"/>
  /// </summary>
  public class StudSpecification : IStudSpecification
  {
    // Stud Specifications
    public bool Welding { get; set; }
    public bool NCCI { get; set; }
    public bool EC4_Limit { get; set; }
    public IQuantity NoStudZoneStart
    {
      get { return this.m_StartPosition; }
      set
      {
        if (value == null) return;
        if (value.QuantityInfo.UnitType != typeof(LengthUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("Start Position must be either Length or Ratio");
        else
          this.m_StartPosition = value;
      }
    }
    private IQuantity m_StartPosition = Length.Zero;
    public IQuantity NoStudZoneEnd
    {
      get { return this.m_EndPosition; }
      set
      {
        if (value == null) return;
        if (value.QuantityInfo.UnitType != typeof(LengthUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("Start Position must be either Length or Ratio");
        else
          this.m_EndPosition = value;
      }
    }
    private IQuantity m_EndPosition = Length.Zero;
    public Length ReinforcementPosition { get; set; }
    public StudSpecType SpecType { get; set; }

    #region constructors
    public StudSpecification()
    {
      // empty constructor
    }

    /// <summary>
    /// for EC4 code
    /// </summary>
    /// <param name="noStudZoneStart"></param>
    /// <param name="noStudZoneEnd"></param>
    /// <param name="reinforcementPosition"></param>
    /// <param name="welding"></param>
    /// <param name="ncci"></param>
    public StudSpecification(IQuantity noStudZoneStart, IQuantity noStudZoneEnd, Length reinforcementPosition, bool welding, bool ncci)
    {
      this.NoStudZoneStart = noStudZoneStart;
      this.NoStudZoneEnd = noStudZoneEnd;
      this.ReinforcementPosition = reinforcementPosition;
      this.Welding = welding;
      this.NCCI = ncci;
      this.SpecType = StudSpecType.EC4;
    }

    /// <summary>
    /// for BS5950 code
    /// </summary>
    /// <param name="useEC4Limit"></param>
    /// <param name="noStudZoneStart"></param>
    /// <param name="noStudZoneEnd"></param>
    /// <param name=""></param>
    public StudSpecification(bool useEC4Limit, IQuantity noStudZoneStart, IQuantity noStudZoneEnd)
    {
      this.NoStudZoneStart = noStudZoneStart;
      this.NoStudZoneEnd = noStudZoneEnd;
      this.EC4_Limit = useEC4Limit;
      this.SpecType = StudSpecType.BS5950;
      this.Welding = true;
    }

    /// <summary>
    /// for codes: AS/NZ, HK, 
    /// </summary>
    /// <param name="noStudZoneStart"></param>
    /// <param name="noStudZoneEnd"></param>
    /// <param name="welding"></param>
    public StudSpecification(IQuantity noStudZoneStart, IQuantity noStudZoneEnd, bool welding)
    {
      this.NoStudZoneStart = noStudZoneStart;
      this.NoStudZoneEnd = noStudZoneEnd;
      this.Welding = welding;
      this.SpecType = StudSpecType.Other;
    }

    #endregion

    #region methods
    public override string ToString()
    {
      string noStudStart = "";
      if (this.NoStudZoneStart.QuantityInfo.UnitType == typeof(LengthUnit))
      {
        Length l = (Length)this.NoStudZoneStart;
        if (l != Length.Zero)
          noStudStart = "NoStudStart:" + l.ToString("g2").Replace(" ", string.Empty);
      }
      else
      {
        Ratio p = (Ratio)this.NoStudZoneStart;
        if (p != Ratio.Zero)
          noStudStart = "NoStudStart:" + p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
      }
      string noStudEnd = "";
      if (this.NoStudZoneEnd.QuantityInfo.UnitType == typeof(LengthUnit))
      {
        Length l = (Length)this.NoStudZoneEnd;
        if (l != Length.Zero)
          noStudEnd = "NoStudEnd:" + l.ToString("g2").Replace(" ", string.Empty);
      }
      else
      {
        Ratio p = (Ratio)this.NoStudZoneEnd;
        if (p != Ratio.Zero)
          noStudEnd = "NoStudEnd:" + p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
      }
      string rebarPos = (this.ReinforcementPosition.Value == 0) ? "" : "RbP:" + this.ReinforcementPosition.ToString("g4").Replace(" ", string.Empty);
      string welding = (this.Welding == true) ? "Welded" : "";
      string ncci = (this.NCCI == true) ? "NCCI Limit" : "";
      string ec4 = (this.EC4_Limit == true) ? "EC4 Limit" : "";
      string joined = string.Join(" ", new List<string>() { noStudStart, noStudEnd, rebarPos, welding, ncci, ec4 });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
