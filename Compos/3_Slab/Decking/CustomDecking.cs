using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  public class CustomDecking : Decking, IDecking
  {
    public Pressure Strength { get; set; } // decking material characteristic strength

    public CustomDecking()
    {
      m_type = DeckingType.Custom;
    }

    public CustomDecking(Length distanceB1, Length distanceB2, Length distanceB3, Length distanceB4, Length distanceB5, Length depth, Length thickness, Pressure strength, IDeckingConfiguration configuration)
    {
      b1 = distanceB1;
      b2 = distanceB2;
      b3 = distanceB3;
      b4 = distanceB4;
      b5 = distanceB5;
      Depth = depth;
      Thickness = thickness;
      Strength = strength;
      DeckingConfiguration = configuration;
      m_type = DeckingType.Custom;
    }

    #region coa interop
    internal static IDecking FromCoaString(List<string> parameters, ComposUnits units)
    {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      CustomDecking decking = new CustomDecking();

      decking.Strength = new Pressure(Convert.ToDouble(parameters[3], noComma), units.Stress);
      DeckingConfiguration deckingConfiguration = new DeckingConfiguration();
      deckingConfiguration.Angle = new Angle(Convert.ToDouble(parameters[4], noComma), units.Angle);
      decking.b1 = new Length(Convert.ToDouble(parameters[5], noComma), units.Length);
      decking.b2 = new Length(Convert.ToDouble(parameters[6], noComma), units.Length);
      decking.b3 = new Length(Convert.ToDouble(parameters[7], noComma), units.Length);
      decking.Depth = new Length(Convert.ToDouble(parameters[8], noComma), units.Length);
      decking.Thickness = new Length(Convert.ToDouble(parameters[9], noComma), units.Length);
      decking.b4 = new Length(Convert.ToDouble(parameters[10], noComma), units.Length);
      decking.b5 = new Length(Convert.ToDouble(parameters[11], noComma), units.Length);

      if (parameters[12] == "DECKING_JOINTED")
        deckingConfiguration.IsDiscontinous = true;
      else
        deckingConfiguration.IsDiscontinous = false;

      if (parameters[13] == "JOINT_WELDED")
        deckingConfiguration.IsWelded = true;
      else
        deckingConfiguration.IsWelded = false;
      decking.DeckingConfiguration = deckingConfiguration;

      return decking;
    }

    public override string ToCoaString(string name, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add("DECKING_USER");
      parameters.Add(name);

      // NO_DECKING ??
      parameters.Add("USER_DEFINED");
      parameters.Add(CoaHelper.FormatSignificantFigures(Strength.ToUnit(units.Stress).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(DeckingConfiguration.Angle.ToUnit(AngleUnit.Degree).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(b1.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(b2.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(b3.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(Depth.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(Thickness.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(b4.ToUnit(units.Section).Value, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(b5.ToUnit(units.Section).Value, 6));

      if (DeckingConfiguration.IsDiscontinous)
        parameters.Add("DECKING_JOINTED");
      else
        parameters.Add("DECKING_CONTINUED");

      if (DeckingConfiguration.IsWelded)
        parameters.Add("JOINT_WELDED");
      else
        parameters.Add("JOINT_NOT_WELD");

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region
    public override string ToString()
    {
      string distanceB1 = (b1.Value == 0) ? "" : "b1:" + b1.ToString().Replace(" ", string.Empty);
      string distanceB2 = (b2.Value == 0) ? "" : "b2:" + b2.ToString().Replace(" ", string.Empty);
      string distanceB3 = (b3.Value == 0) ? "" : "b3:" + b3.ToString().Replace(" ", string.Empty);
      string distanceB4 = (b4.Value == 0) ? "" : "b4:" + b4.ToString().Replace(" ", string.Empty);
      string distanceB5 = (b5.Value == 0) ? "" : "b5:" + b5.ToString().Replace(" ", string.Empty);
      string depth = (Depth.Value == 0) ? "" : "d:" + Depth.ToString().Replace(" ", string.Empty);
      string thickness = (Thickness.Value == 0) ? "" : "th:" + Thickness.ToString().Replace(" ", string.Empty);
      string stress = (Strength.Value == 0) ? "" : "stress:" + Strength.ToString().Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { distanceB1, distanceB2, distanceB3, distanceB4, distanceB5, depth, thickness, stress });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
