using System;
using System.Collections.Generic;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  public class CustomTransverseReinforcementLayout : ICustomTransverseReinforcementLayout
  {
    public IQuantity StartPosition
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
    // end x of the reinforcement
    public IQuantity EndPosition
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
    private IQuantity m_EndPosition = new Ratio(100, RatioUnit.Percent);
    public Length Diameter { get; set; } // diameter of the reinforcement
    public Length Spacing { get; set; } //	spacing of the reinforcement
    public Length Cover { get; set; } // reinforcement cover

    public CustomTransverseReinforcementLayout() { }

    public CustomTransverseReinforcementLayout(IQuantity distanceFromStart, IQuantity distanceFromEnd, Length diameter, Length spacing, Length cover)
    {
      this.StartPosition = distanceFromStart;
      this.EndPosition = distanceFromEnd;
      this.Diameter = diameter;
      this.Spacing = spacing;
      this.Cover = cover;
    }

    #region coa interop
    internal static ICustomTransverseReinforcementLayout FromCoaString(List<string> parameters, ComposUnits units)
    {
      CustomTransverseReinforcementLayout layout = new CustomTransverseReinforcementLayout();

      layout.StartPosition = CoaHelper.ConvertToLengthOrRatio(parameters[3], units.Length);
      layout.EndPosition = CoaHelper.ConvertToLengthOrRatio(parameters[4], units.Length);
      layout.Diameter = CoaHelper.ConvertToLength(parameters[5], units.Length);
      layout.Spacing = CoaHelper.ConvertToLength(parameters[6], units.Length);
      layout.Cover = CoaHelper.ConvertToLength(parameters[7], units.Length);

      return layout;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.RebarTransverse);
      parameters.Add(name);
      parameters.Add("USER_DEFINED");
      if (this.StartPosition.QuantityInfo.UnitType == typeof(RatioUnit))
      {
        // start position in percent
        Ratio p = (Ratio)this.StartPosition;
        // percentage in coa string for beam section is a negative decimal fraction!
        parameters.Add(CoaHelper.FormatSignificantFigures(p.As(RatioUnit.DecimalFraction) * -1, p.DecimalFractions == 1 ? 5 : 6));
      }
      else
        parameters.Add(CoaHelper.FormatSignificantFigures(this.StartPosition.ToUnit(units.Length).Value, 6));
      if (this.EndPosition.QuantityInfo.UnitType == typeof(RatioUnit))
      {
        // start position in percent
        Ratio p = (Ratio)this.EndPosition;
        // percentage in coa string for beam section is a negative decimal fraction!
        parameters.Add(CoaHelper.FormatSignificantFigures(p.As(RatioUnit.DecimalFraction) * -1, p.DecimalFractions == 1 ? 5 : 6));
      }
      else
        parameters.Add(CoaHelper.FormatSignificantFigures(this.EndPosition.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.Diameter.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.Spacing.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.Cover.ToUnit(units.Length).Value, 6));

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string start = "";
      if (this.StartPosition.QuantityInfo.UnitType == typeof(LengthUnit))
      {
        Length l = (Length)this.StartPosition;
        start = l.ToString("g2").Replace(" ", string.Empty);
      }
      else
      {
        Ratio p = (Ratio)this.StartPosition;
        start = p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
      }

      string end = "";
      if (this.EndPosition.QuantityInfo.UnitType == typeof(LengthUnit))
      {
        Length l = (Length)this.EndPosition;
        end = l.ToString("g2").Replace(" ", string.Empty);
      }
      else
      {
        Ratio p = (Ratio)this.EndPosition;
        end = p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
      }

      string startend = start + "->" + end;

      string dia = "Ø" + this.Diameter.ToString("f0").Replace(" ", string.Empty);
      string spacing = "/" + this.Spacing.ToString("f0").Replace(" ", string.Empty);
      string cov = ", c:" + this.Cover.ToString("f0").Replace(" ", string.Empty);
      string diaspacingcov = dia + spacing + cov;
      string joined = string.Join(", ", new List<string>() { startend, diaspacingcov });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
