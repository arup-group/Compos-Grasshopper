﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI.Helpers {
  public class CoaHelper {
    internal static NumberFormatInfo NoComma = CultureInfo.InvariantCulture.NumberFormat;

    public static string FormatSignificantFigures(IQuantity lengthOrRatio, LengthUnit lengthUnit, int significantFigures) {
      if (lengthOrRatio.Value == 0) {
        return FormatSignificantFigures(0, 6);
      }

      if (lengthOrRatio.QuantityInfo.UnitType == typeof(LengthUnit)) {
        var l = (Length)lengthOrRatio;
        return FormatSignificantFigures(l.ToUnit(lengthUnit).Value, significantFigures);
      } else if (lengthOrRatio.QuantityInfo.UnitType == typeof(RatioUnit)) {
        var r = (Ratio)lengthOrRatio;
        return FormatSignificantFigures(r.Percent, significantFigures) + "%";
      } else {
        throw new Exception("Unable to format coa string, expected IQuantity of either Length or Ratio");
      }
    }

    public static string FormatSignificantFigures(double value, int significantFigures) {
      // if for instance 6 significant figures and value is above 1,000,000
      // compos coa is shown as 4.50000e+008 which is value.ToString("e6")
      if (value > Math.Pow(10, significantFigures)) {
        return value.ToString("e" + (significantFigures - 1), NoComma);
      }

      int magnitude;
      if (value < 1 && value > -1) {
        magnitude = GetInverseMagnitude(value);
      } else {
        magnitude = GetMagnitude((int)value);
      }
      int decimalPlaces = Math.Max(0, significantFigures - magnitude);
      string format = "{0:0.";
      for (int i = 0; i < decimalPlaces; i++) {
        format += "0";
      }
      format += "}";
      return string.Format(NoComma, format, value);
    }

    public static int GetInverseMagnitude(double num) {
      if (num == 0) {
        return 0;
      }
      int magnitude = 1;
      while (Math.Abs(num) < 1) {
        magnitude--;
        num *= 10;
      }
      return magnitude;
    }

    public static int GetMagnitude(int num) {
      int magnitude = 0;
      while (num > 0) {
        magnitude++;
        num /= 10;
      }
      return magnitude;
    }

    internal static void AddParameter(List<string> parameters, string parameter, bool flag) {
      string str = parameter + "_";
      if (flag) {
        str += "YES";
      } else {
        str += "NO";
      }
      parameters.Add(str);
    }

    internal static Angle ConvertToAngle(string value, AngleUnit unit) {
      return new Angle(Convert.ToDouble(value, NoComma), unit);
    }

    internal static Density ConvertToDensity(string value, DensityUnit unit) {
      return new Density(Convert.ToDouble(value, NoComma), unit);
    }

    internal static double ConvertToDouble(string value) {
      return Convert.ToDouble(value, NoComma);
    }

    internal static Length ConvertToLength(string value, LengthUnit unit) {
      return new Length(Convert.ToDouble(value, NoComma), unit);
    }

    internal static IQuantity ConvertToLengthOrRatio(string parameters, LengthUnit lengthUnit, RatioUnit ratioUnit = RatioUnit.Percent) {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      if (parameters.EndsWith("%")) {
        return new Ratio(Convert.ToDouble(parameters.Replace("%", string.Empty), noComma), ratioUnit);
      } else {
        return new Length(Convert.ToDouble(parameters, noComma), lengthUnit);
      }
    }

    internal static Strain ConvertToStrain(string value, StrainUnit unit) {
      return new Strain(Convert.ToDouble(value, NoComma), unit);
    }

    internal static Pressure ConvertToStress(string value, PressureUnit unit) {
      return new Pressure(Convert.ToDouble(value, NoComma), unit);
    }

    internal static string CreateString(List<string> parameters) {
      string str = "";
      foreach (string param in parameters) {
        str += param + '\t';
      }
      str = str.Remove(str.Length - 1, 1);
      str += '\n';
      return str;
    }

    internal static string RemoveWhitespace(string str) {
      return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
    }

    internal static List<string> Split(string coaString) {
      var parameters = coaString.Split('\t').ToList();
      parameters = parameters.Select(parameter => parameter.Trim()).ToList();

      return parameters;
    }

    internal static List<string> SplitAndStripLines(string coaString) {
      return StripComments(SplitLines(coaString));
    }

    internal static List<string> SplitLines(string coaString) {
      var lines = coaString.Replace("\r", "").Split('\n').ToList();

      // remove last line if empty
      if (lines[lines.Count - 1] == "") {
        lines.RemoveAt(lines.Count - 1);
      }

      return lines;
    }

    internal static List<string> StripComments(List<string> lines) {
      var stripped = new List<string>();
      foreach (string line in lines) {
        if (!line.StartsWith("!")) {
          stripped.Add(line);
        }
      }
      return stripped;
    }
  }
}
