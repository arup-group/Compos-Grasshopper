using System;
using System.Collections.Generic;
using System.Globalization;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class CustomDecking : Decking, IDecking {
    public Pressure Strength { get; set; } // decking material characteristic strength

    public CustomDecking() {
      m_type = DeckingType.Custom;
    }

    public CustomDecking(Length distanceB1, Length distanceB2, Length distanceB3, Length distanceB4, Length distanceB5, Length depth, Length thickness, Pressure strength, IDeckingConfiguration configuration) {
      B1 = distanceB1;
      B2 = distanceB2;
      B3 = distanceB3;
      B4 = distanceB4;
      B5 = distanceB5;
      Depth = depth;
      Thickness = thickness;
      Strength = strength;
      DeckingConfiguration = configuration;
      m_type = DeckingType.Custom;
    }

    #region coa interop

    public override string ToCoaString(string name, ComposUnits units) {
      var parameters = new List<string> {
        "DECKING_USER",
        name,

        // NO_DECKING ??
        "USER_DEFINED",
        CoaHelper.FormatSignificantFigures(Strength.ToUnit(units.Stress).Value, 6),
        CoaHelper.FormatSignificantFigures(DeckingConfiguration.Angle.ToUnit(AngleUnit.Degree).Value, 6),
        CoaHelper.FormatSignificantFigures(B1.ToUnit(units.Section).Value, 6),
        CoaHelper.FormatSignificantFigures(B2.ToUnit(units.Section).Value, 6),
        CoaHelper.FormatSignificantFigures(B3.ToUnit(units.Section).Value, 6),
        CoaHelper.FormatSignificantFigures(Depth.ToUnit(units.Section).Value, 6),
        CoaHelper.FormatSignificantFigures(Thickness.ToUnit(units.Section).Value, 6),
        CoaHelper.FormatSignificantFigures(B4.ToUnit(units.Section).Value, 6),
        CoaHelper.FormatSignificantFigures(B5.ToUnit(units.Section).Value, 6)
      };

      if (DeckingConfiguration.IsDiscontinous) {
        parameters.Add("DECKING_JOINTED");
      } else {
        parameters.Add("DECKING_CONTINUED");
      }

      if (DeckingConfiguration.IsWelded) {
        parameters.Add("JOINT_WELDED");
      } else {
        parameters.Add("JOINT_NOT_WELD");
      }

      return CoaHelper.CreateString(parameters);
    }

    public override string ToString() {
      string distanceB1 = (B1.Value == 0) ? "" : "b1:" + B1.ToString().Replace(" ", string.Empty);
      string distanceB2 = (B2.Value == 0) ? "" : "b2:" + B2.ToString().Replace(" ", string.Empty);
      string distanceB3 = (B3.Value == 0) ? "" : "b3:" + B3.ToString().Replace(" ", string.Empty);
      string distanceB4 = (B4.Value == 0) ? "" : "b4:" + B4.ToString().Replace(" ", string.Empty);
      string distanceB5 = (B5.Value == 0) ? "" : "b5:" + B5.ToString().Replace(" ", string.Empty);
      string depth = (Depth.Value == 0) ? "" : "d:" + Depth.ToString().Replace(" ", string.Empty);
      string thickness = (Thickness.Value == 0) ? "" : "th:" + Thickness.ToString().Replace(" ", string.Empty);
      string stress = (Strength.Value == 0) ? "" : "stress:" + Strength.ToString().Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { distanceB1, distanceB2, distanceB3, distanceB4, distanceB5, depth, thickness, stress });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }

    internal static IDecking FromCoaString(List<string> parameters, ComposUnits units) {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      var decking = new CustomDecking {
        Strength = new Pressure(Convert.ToDouble(parameters[3], noComma), units.Stress)
      };
      var deckingConfiguration = new DeckingConfiguration {
        Angle = new Angle(Convert.ToDouble(parameters[4], noComma), units.Angle)
      };
      decking.B1 = new Length(Convert.ToDouble(parameters[5], noComma), units.Length);
      decking.B2 = new Length(Convert.ToDouble(parameters[6], noComma), units.Length);
      decking.B3 = new Length(Convert.ToDouble(parameters[7], noComma), units.Length);
      decking.Depth = new Length(Convert.ToDouble(parameters[8], noComma), units.Length);
      decking.Thickness = new Length(Convert.ToDouble(parameters[9], noComma), units.Length);
      decking.B4 = new Length(Convert.ToDouble(parameters[10], noComma), units.Length);
      decking.B5 = new Length(Convert.ToDouble(parameters[11], noComma), units.Length);

      if (parameters[12] == "DECKING_JOINTED") {
        deckingConfiguration.IsDiscontinous = true;
      } else {
        deckingConfiguration.IsDiscontinous = false;
      }

      if (parameters[13] == "JOINT_WELDED") {
        deckingConfiguration.IsWelded = true;
      } else {
        deckingConfiguration.IsWelded = false;
      }
      decking.DeckingConfiguration = deckingConfiguration;

      return decking;
    }

    #endregion

    #region
    #endregion
  }
}
