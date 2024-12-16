using System;
using System.Collections.Generic;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  /// <summary>
  /// Object for setting various (code dependent) specifications for a <see cref="ComposGH.Stud.Stud"/>
  /// </summary>
  public class StudSpecification : IStudSpecification {
    public bool EC4_Limit { get; set; }
    public bool NCCI { get; set; }
    public IQuantity NoStudZoneEnd {
      get => m_EndPosition;
      set {
        if (value == null) {
          return;
        }
        if (value.QuantityInfo.UnitType != typeof(LengthUnit) & value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("Start Position must be either Length or Ratio");
        } else {
          m_EndPosition = value;
        }
      }
    }
    public IQuantity NoStudZoneStart {
      get => m_StartPosition;
      set {
        if (value == null) {
          return;
        }
        if (value.QuantityInfo.UnitType != typeof(LengthUnit) & value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("Start Position must be either Length or Ratio");
        } else {
          m_StartPosition = value;
        }
      }
    }
    public Length ReinforcementPosition { get; set; }
    public StudSpecType SpecType { get; set; }
    // Stud Specifications
    public bool Welding { get; set; }
    private IQuantity m_EndPosition = Length.Zero;
    private IQuantity m_StartPosition = Length.Zero;

    public StudSpecification() {
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
    public StudSpecification(IQuantity noStudZoneStart, IQuantity noStudZoneEnd, Length reinforcementPosition, bool welding, bool ncci) {
      NoStudZoneStart = noStudZoneStart;
      NoStudZoneEnd = noStudZoneEnd;
      ReinforcementPosition = reinforcementPosition;
      Welding = welding;
      NCCI = ncci;
      SpecType = StudSpecType.EC4;
    }

    /// <summary>
    /// for BS5950 code
    /// </summary>
    /// <param name="useEC4Limit"></param>
    /// <param name="noStudZoneStart"></param>
    /// <param name="noStudZoneEnd"></param>
    /// <param name=""></param>
    public StudSpecification(bool useEC4Limit, IQuantity noStudZoneStart, IQuantity noStudZoneEnd) {
      NoStudZoneStart = noStudZoneStart;
      NoStudZoneEnd = noStudZoneEnd;
      EC4_Limit = useEC4Limit;
      SpecType = StudSpecType.BS5950;
      Welding = true;
    }

    /// <summary>
    /// for codes: AS/NZ, HK,
    /// </summary>
    /// <param name="noStudZoneStart"></param>
    /// <param name="noStudZoneEnd"></param>
    /// <param name="welding"></param>
    public StudSpecification(IQuantity noStudZoneStart, IQuantity noStudZoneEnd, bool welding) {
      NoStudZoneStart = noStudZoneStart;
      NoStudZoneEnd = noStudZoneEnd;
      Welding = welding;
      SpecType = StudSpecType.Other;
    }

    public override string ToString() {
      string noStudStart = "";
      if (NoStudZoneStart.QuantityInfo.UnitType == typeof(LengthUnit)) {
        var l = (Length)NoStudZoneStart;
        if (!ComposUnitsHelper.IsEqual(l, Length.Zero)) {
          noStudStart = "NoStudStart:" + l.ToString("g2").Replace(" ", string.Empty);
        }
      } else {
        var p = (Ratio)NoStudZoneStart;
        if (!ComposUnitsHelper.IsEqual(p, Ratio.Zero)) {
          noStudStart = "NoStudStart:" + p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
        }
      }
      string noStudEnd = "";
      if (NoStudZoneEnd.QuantityInfo.UnitType == typeof(LengthUnit)) {
        var l = (Length)NoStudZoneEnd;
        if (!ComposUnitsHelper.IsEqual(l, Length.Zero)) {
          noStudEnd = "NoStudEnd:" + l.ToString("g2").Replace(" ", string.Empty);
        }
      } else {
        var p = (Ratio)NoStudZoneEnd;
        if (!ComposUnitsHelper.IsEqual(p, Ratio.Zero)) {
          noStudEnd = "NoStudEnd:" + p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
        }
      }
      string rebarPos = (ReinforcementPosition.Value == 0) ? "" : "RbP:" + ReinforcementPosition.ToString("g4").Replace(" ", string.Empty);
      string welding = (Welding == true) ? "Welded" : "";
      string ncci = (NCCI == true) ? "NCCI Limit" : "";
      string ec4 = (EC4_Limit == true) ? "EC4 Limit" : "";
      string joined = string.Join(" ", new List<string>() { noStudStart, noStudEnd, rebarPos, welding, ncci, ec4 });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
  }

  public enum StudSpecType {
    EC4,
    BS5950,
    Other
  }
}
