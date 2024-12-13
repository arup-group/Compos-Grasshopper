using System;
using System.Collections.Generic;
using System.Globalization;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  /// <summary>
  /// Object for setting custom spacing/layout for a <see cref="Stud"/>
  /// </summary>
  public class StudGroupSpacing : IStudGroupSpacing {
    public IQuantity DistanceFromStart {
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
    public int NumberOfLines { get; set; } = 1;
    public int NumberOfRows { get; set; } = 2;
    public Length Spacing { get; set; }
    private IQuantity m_StartPosition = Length.Zero;

    public StudGroupSpacing() {
      // empty constructor
    }

    public StudGroupSpacing(IQuantity distanceFromStart, int numberOfRows, int numberOfLines, Length spacing) {
      DistanceFromStart = distanceFromStart;
      if (numberOfRows < 1) {
        throw new ArgumentException("Number of rows must be bigger or equal to 1");
      }
      NumberOfRows = numberOfRows;
      if (numberOfLines < 1) {
        throw new ArgumentException("Number of lines must be bigger or equal to 1");
      }
      NumberOfLines = numberOfLines;
      Spacing = spacing;
    }

    public override string ToString() {
      string start = "";
      if (DistanceFromStart.QuantityInfo.UnitType == typeof(LengthUnit)) {
        var l = (Length)DistanceFromStart;
        if (!ComposUnitsHelper.IsEqual(l, Length.Zero)) {
          start = "From:" + l.ToString("g2").Replace(" ", string.Empty);
        }
      }
      else {
        var p = (Ratio)DistanceFromStart;
        if (!ComposUnitsHelper.IsEqual(p, Ratio.Zero)) {
          start = "From:" + p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
        }
      }
      string rows = NumberOfRows + "R";
      string lines = NumberOfLines + "L";
      string spacing = "@" + Spacing.ToUnit(ComposUnitsHelper.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { start, rows, lines, spacing });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }

    internal static StudGroupSpacing FromCoaString(List<string> parameters, ComposUnits units) {
      //STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	1	0.000000	2	1	0.0760000	0.0950000	0.150000	CHECK_SPACE_NO
      //STUD_LAYOUT	MEMBER-1	USER_DEFINED	2	1	0.000000	2	1	0.0570000	0.0950000	0.150000	CHECK_SPACE_NO
      //STUD_LAYOUT MEMBER-1 USER_DEFINED 2 2 8.000000 3 2 0.0570000 0.0950000 0.250000 CHECK_SPACE_NO
      var groupSpacing = new StudGroupSpacing();
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      groupSpacing.DistanceFromStart = CoaHelper.ConvertToLengthOrRatio(parameters[5], units.Length);
      groupSpacing.NumberOfRows = Convert.ToInt32(parameters[6], noComma);
      groupSpacing.NumberOfLines = Convert.ToInt32(parameters[7], noComma);
      groupSpacing.Spacing = new Length(Convert.ToDouble(parameters[10], noComma), units.Length);

      return groupSpacing;
    }
  }
}
