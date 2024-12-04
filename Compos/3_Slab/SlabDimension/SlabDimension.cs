using System;
using System.Collections.Generic;
using System.Globalization;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  /// <summary>
  /// Slab dimensions such as Depth, Width, Effective Width, starting position and if the section is tapered to next section.
  /// </summary>
  public class SlabDimension : ISlabDimension, ICoaObject {
    public Length AvailableWidthLeft { get; set; }
    // left hand side width of slab
    public Length AvailableWidthRight { get; set; }
    public Length EffectiveWidthLeft { get; set; }
    //	left hand side effective width of slab 
    public Length EffectiveWidthRight { get; set; }
    // Dimensions
    public Length OverallDepth { get; set; }
    public IQuantity StartPosition { get; set; } = Length.Zero;

    // Settings
    public bool TaperedToNext { get; set; }
    //	depth of slab
    //	right hand side width of slab
    public bool UserEffectiveWidth { get; set; } // override effective width
                                                 //	right hand side effective width of slab

    // Tapered to next section flag

    public SlabDimension() {
      // empty constructor
    }

    public SlabDimension(IQuantity startPosition, Length overallDepth, Length availableWidthLeft, Length availableWidthRight, bool taperedToNext = false) {
      if (startPosition.QuantityInfo.UnitType != typeof(LengthUnit) && startPosition.QuantityInfo.UnitType != typeof(RatioUnit)) {
        throw new Exception("Start Position must be either Length or Ratio");
      }
      StartPosition = startPosition;
      OverallDepth = overallDepth;
      AvailableWidthLeft = availableWidthLeft;
      AvailableWidthRight = availableWidthRight;
      UserEffectiveWidth = false;
      TaperedToNext = taperedToNext;
    }

    public SlabDimension(IQuantity startPosition, Length overallDepth, Length availableWidthLeft, Length availableWidthRight,
      Length effectiveWidthLeft, Length effectiveWidthRight, bool taperedToNext = false) {
      if (startPosition.QuantityInfo.UnitType != typeof(LengthUnit) && startPosition.QuantityInfo.UnitType != typeof(RatioUnit)) {
        throw new Exception("Start Position must be either Length or Ratio");
      }
      StartPosition = startPosition;
      OverallDepth = overallDepth;
      AvailableWidthLeft = availableWidthLeft;
      AvailableWidthRight = availableWidthRight;
      UserEffectiveWidth = true;
      EffectiveWidthLeft = effectiveWidthLeft;
      EffectiveWidthRight = effectiveWidthRight;
      TaperedToNext = taperedToNext;
    }

    /// <summary>
    /// SLAB_DIMENSION | name | num | index | x | depth | width_l | width_r | tapered | override | eff_width_l | eff_width_r
    /// SLAB_DIMENSION | name | num | index | x | depth | width_l | width_r | tapered | override
    /// </summary>
    /// <param name="name"></param>
    /// <param name="num"></param>
    /// <param name="index"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    public string ToCoaString(string name, int num, int index, ComposUnits units) {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      var parameters = new List<string> {
        CoaIdentifier.SlabDimension,
        name,
        Convert.ToString(num),
        Convert.ToString(index),
        CoaHelper.FormatSignificantFigures(StartPosition, units.Length, 6),
        string.Format(noComma, "{0:0.000000}", OverallDepth.ToUnit(units.Length).Value),
        string.Format(noComma, "{0:0.00000}", AvailableWidthLeft.ToUnit(units.Length).Value),
        string.Format(noComma, "{0:0.00000}", AvailableWidthRight.ToUnit(units.Length).Value)
      };
      CoaHelper.AddParameter(parameters, "TAPERED", TaperedToNext);
      CoaHelper.AddParameter(parameters, "EFFECTIVE_WIDTH", UserEffectiveWidth);
      if (UserEffectiveWidth) {
        parameters.Add(string.Format(noComma, "{0:0.00000}", EffectiveWidthLeft.ToUnit(units.Length).Value));
        parameters.Add(string.Format(noComma, "{0:0.00000}", EffectiveWidthRight.ToUnit(units.Length).Value));
      }
      return CoaHelper.CreateString(parameters);
    }

    public override string ToString() {
      string start = "";
      if (StartPosition.QuantityInfo.UnitType == typeof(LengthUnit)) {
        var l = (Length)StartPosition;
        if (!ComposUnitsHelper.IsEqual(l, Length.Zero)) {
          start = ", s:" + l.ToUnit(ComposUnitsHelper.LengthUnitGeometry).ToString("g2").Replace(" ", string.Empty);
        }
      } else {
        var p = (Ratio)StartPosition;
        if (!ComposUnitsHelper.IsEqual(p, Length.Zero)) {
          start = ", s:" + p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
        }
      }

      string tapered = "";
      if (TaperedToNext) {
        tapered = ", Tapered";
      }

      string d = "d:" + OverallDepth.ToUnit(ComposUnitsHelper.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string w = ", w:" + new Length(AvailableWidthLeft.As(ComposUnitsHelper.LengthUnitGeometry) + AvailableWidthRight.As(ComposUnitsHelper.LengthUnitGeometry), ComposUnitsHelper.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      if (UserEffectiveWidth) {
        w = ", weff:" + new Length(EffectiveWidthLeft.As(ComposUnitsHelper.LengthUnitGeometry) + EffectiveWidthRight.As(ComposUnitsHelper.LengthUnitGeometry), ComposUnitsHelper.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      }
      return d + w + start + tapered;
    }

    internal static ISlabDimension FromCoaString(List<string> parameters, ComposUnits units) {
      if (parameters.Count < 10) {
        throw new Exception("Unable to convert " + parameters + " to Compos Slab Dimension.");
      }

      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      var dimension = new SlabDimension();

      if (parameters[4].EndsWith("%")) {
        dimension.StartPosition = new Ratio(Convert.ToDouble(parameters[4].Replace("%", string.Empty), noComma), RatioUnit.Percent);
      } else {
        dimension.StartPosition = new Length(Convert.ToDouble(parameters[4], noComma), units.Length);
      }

      dimension.OverallDepth = CoaHelper.ConvertToLength(parameters[5], units.Length);
      dimension.AvailableWidthLeft = CoaHelper.ConvertToLength(parameters[6], units.Length);
      dimension.AvailableWidthRight = CoaHelper.ConvertToLength(parameters[7], units.Length);

      if (parameters[8] == "TAPERED_YES") {
        dimension.TaperedToNext = true;
      } else {
        dimension.TaperedToNext = false;
      }

      if (parameters[9] == "EFFECTIVE_WIDTH_YES") {
        dimension.UserEffectiveWidth = true;
        if (parameters.Count != 12) {
          throw new Exception("Unable to convert " + parameters + " to Compos Slab Dimension.");
        }
        dimension.EffectiveWidthLeft = CoaHelper.ConvertToLength(parameters[10], units.Length);
        dimension.EffectiveWidthRight = CoaHelper.ConvertToLength(parameters[11], units.Length);
      }
      return dimension;
    }
  }
}
