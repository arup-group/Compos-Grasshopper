using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;

namespace ComposAPI
{
  public class CustomTransverseReinforcementLayout : ICustomTransverseReinforcementLayout
  {
    public Length DistanceFromStart { get; set; } // start x of the reinforcement
    public Length DistanceFromEnd { get; set; } // end x of the reinforcement
    public Length Diameter { get; set; } // diameter of the reinforcement
    public Length Spacing { get; set; } //	spacing of the reinforcement
    public Length Cover { get; set; } // reinforcement cover

    public CustomTransverseReinforcementLayout() { }

    public CustomTransverseReinforcementLayout(Length distanceFromStart, Length distanceFromEnd, Length diameter, Length spacing, Length cover)
    {
      this.DistanceFromStart = distanceFromStart;
      this.DistanceFromEnd = distanceFromEnd;
      this.Diameter = diameter;
      this.Spacing = spacing;
      this.Cover = cover;
    }

    #region coa interop
    internal CustomTransverseReinforcementLayout(List<string> parameters, ComposUnits units)
    {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      this.DistanceFromStart = new Length(Convert.ToDouble(parameters[3], noComma), units.Length);
      this.DistanceFromEnd = new Length(Convert.ToDouble(parameters[4], noComma), units.Length);
      this.Diameter = new Length(Convert.ToDouble(parameters[5], noComma), units.Length);
      this.Spacing = new Length(Convert.ToDouble(parameters[6], noComma), units.Length);
      this.Cover = new Length(Convert.ToDouble(parameters[7], noComma), units.Length);
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.RebarTransverse);
      parameters.Add(name);
      parameters.Add("USER_DEFINED");
      parameters.Add(CoaHelper.FormatSignificantFigures(this.DistanceFromStart.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.DistanceFromEnd.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.Diameter.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.Spacing.ToUnit(units.Length).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.Cover.ToUnit(units.Length).Value, 6));

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string start = (this.DistanceFromStart.Value == 0) ? "" : this.DistanceFromStart.ToUnit(Units.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty) + "<-";
      string end = (this.DistanceFromEnd.Value == 0) ? "" : "->" + this.DistanceFromEnd.ToUnit(Units.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty);
      string startend = start + end;
      startend = startend.Replace("--", "-").Replace(",", string.Empty);
      string dia = "Ø" + this.Diameter.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string spacing = "/" + this.Spacing.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string cov = ", c:" + this.Cover.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string diaspacingcov = dia + spacing + cov;
      string joined = string.Join(" ", new List<string>() { startend, diaspacingcov });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
