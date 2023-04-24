using System;
using System.Collections.Generic;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class CustomTransverseReinforcementLayout : ICustomTransverseReinforcementLayout {
    public Length Cover { get; set; }
    public Length Diameter { get; set; }
    // end x of the reinforcement
    public IQuantity EndPosition {
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
    // diameter of the reinforcement
    public Length Spacing { get; set; }
    public IQuantity StartPosition {
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
    private IQuantity m_EndPosition = new Ratio(100, RatioUnit.Percent);
    private IQuantity m_StartPosition = Length.Zero;
    //	spacing of the reinforcement
    // reinforcement cover

    public CustomTransverseReinforcementLayout() { }

    public CustomTransverseReinforcementLayout(IQuantity distanceFromStart, IQuantity distanceFromEnd, Length diameter, Length spacing, Length cover) {
      StartPosition = distanceFromStart;
      EndPosition = distanceFromEnd;
      Diameter = diameter;
      Spacing = spacing;
      Cover = cover;
    }

    public string ToCoaString(string name, ComposUnits units) {
      var parameters = new List<string> {
        CoaIdentifier.RebarTransverse,
        name,
        "USER_DEFINED"
      };
      if (StartPosition.QuantityInfo.UnitType == typeof(RatioUnit)) {
        // start position in percent
        var p = (Ratio)StartPosition;
        // percentage in coa string for beam section is a negative decimal fraction!
        parameters.Add(CoaHelper.FormatSignificantFigures(p.As(RatioUnit.DecimalFraction) * -1, p.DecimalFractions == 1 ? 5 : 6));
      } else {
        parameters.Add(CoaHelper.FormatSignificantFigures(StartPosition.ToUnit(units.Length).Value, 6));
      }
      if (EndPosition.QuantityInfo.UnitType == typeof(RatioUnit)) {
        // start position in percent
        var p = (Ratio)EndPosition;
        // percentage in coa string for beam section is a negative decimal fraction!
        parameters.Add(CoaHelper.FormatSignificantFigures(p.As(RatioUnit.DecimalFraction) * -1, p.DecimalFractions == 1 ? 5 : 6));
      } else {
        parameters.Add(CoaHelper.FormatSignificantFigures(EndPosition.ToUnit(units.Length).Value, 6));
      }
      parameters.Add(CoaHelper.FormatSignificantFigures(Diameter.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(Spacing.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(Cover.ToUnit(units.Length).Value, 6));

      return CoaHelper.CreateString(parameters);
    }

    public override string ToString() {
      string start = "";
      if (StartPosition.QuantityInfo.UnitType == typeof(LengthUnit)) {
        var l = (Length)StartPosition;
        start = l.ToString("g2").Replace(" ", string.Empty);
      } else {
        var p = (Ratio)StartPosition;
        start = p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
      }

      string end = "";
      if (EndPosition.QuantityInfo.UnitType == typeof(LengthUnit)) {
        var l = (Length)EndPosition;
        end = l.ToString("g2").Replace(" ", string.Empty);
      } else {
        var p = (Ratio)EndPosition;
        end = p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
      }

      string startend = start + "->" + end;

      string dia = "Ø" + Diameter.ToString("f0").Replace(" ", string.Empty);
      string spacing = "/" + Spacing.ToString("f0").Replace(" ", string.Empty);
      string cov = ", c:" + Cover.ToString("f0").Replace(" ", string.Empty);
      string diaspacingcov = dia + spacing + cov;
      string joined = string.Join(", ", new List<string>() { startend, diaspacingcov });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }

    internal static ICustomTransverseReinforcementLayout FromCoaString(List<string> parameters, ComposUnits units) {
      var layout = new CustomTransverseReinforcementLayout {
        StartPosition = CoaHelper.ConvertToLengthOrRatio(parameters[3], units.Length),
        EndPosition = CoaHelper.ConvertToLengthOrRatio(parameters[4], units.Length),
        Diameter = CoaHelper.ConvertToLength(parameters[5], units.Length),
        Spacing = CoaHelper.ConvertToLength(parameters[6], units.Length),
        Cover = CoaHelper.ConvertToLength(parameters[7], units.Length)
      };

      return layout;
    }
  }
}
