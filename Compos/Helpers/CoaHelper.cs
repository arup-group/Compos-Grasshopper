using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI.Helpers
{
  public class CoaHelper
  {
    internal static NumberFormatInfo NoComma = CultureInfo.InvariantCulture.NumberFormat;

    internal static void AddParameter(List<string> parameters, string parameter, bool flag)
    {
      string str = parameter + "_";
      if (flag)
        str += "YES";
      else
        str += "NO";
      parameters.Add(str);
    }

    internal static string CreateString(List<string> parameters)
    {
      string str = "";
      foreach (string param in parameters)
        str += param + '\t';
      str = str.Remove(str.Length - 1, 1);
      str += '\n';
      return str;
    }

    internal static string RemoveWhitespace(string str)
    {
      return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
    }

    public static int GetMagnitude(int num)
    {
      int magnitude = 0;
      while (num > 0)
      {
        magnitude++;
        num /= 10;
      }
      return magnitude;
    }

    public static int GetInverseMagnitude(double num)
    {
      if (num == 0)
        return 0;
      int magnitude = 1;
      while (Math.Abs(num) < 1)
      {
        magnitude--;
        num *= 10;
      }
      return magnitude;
    }

    public static string FormatSignificantFigures(double value, int significantFigures, bool isExponential = false)
    {
      // if for instance 6 significant figures and value is above 1,000,000
      // compos coa is shown as 4.50000e+008 which is value.ToString("e6")
      if (value > Math.Pow(10, significantFigures))
        return value.ToString("e" + (significantFigures - 1), NoComma);

      int magnitude;
      if (value < 1 && value > -1)
      {
        magnitude = GetInverseMagnitude(value);
      }
      else
      {
        magnitude = GetMagnitude((int)value);
      }
      int decimalPlaces = Math.Max(0, significantFigures - magnitude);
      string format = "{0:0.";
      for (int i = 0; i < decimalPlaces; i++)
        format += "0";
      format += "}";
      return String.Format(NoComma, format, value);
    }

    internal static Angle ConvertToAngle(string value, AngleUnit unit)
    {
      return new Angle(Convert.ToDouble(value, NoComma), unit);
    }

    internal static Density ConvertToDensity(string value, DensityUnit unit)
    {
      return new Density(Convert.ToDouble(value, NoComma), unit);
    }

    internal static double ConvertToDouble(string value)
    {
      return Convert.ToDouble(value, NoComma);
    }

    internal static Length ConvertToLength(string value, LengthUnit unit)
    {
      return new Length(Convert.ToDouble(value, NoComma), unit);
    }

    internal static Strain ConvertToStrain(string value, StrainUnit unit)
    {
      return new Strain(Convert.ToDouble(value, NoComma), unit);
    }

    internal static Pressure ConvertToStress(string value, PressureUnit unit)
    {
      return new Pressure(Convert.ToDouble(value, NoComma), unit);
    }

    internal static List<string> Split(string coaString)
    {
      List<string> parameters = coaString.Split('\t').ToList();
      parameters = parameters.Select(parameter => parameter.Trim()).ToList();

      return parameters;
    }

    internal static List<string> SplitAndStripLines(string coaString)
    {
      return StripComments(SplitLines(coaString));
    }

    internal static List<string> SplitLines(string coaString)
    {
      List<string> lines = coaString.Split('\n').ToList();

      // remove last line if empty
      if (lines[lines.Count - 1] == "")
        lines.RemoveAt(lines.Count - 1);

      return lines;
    }
    internal static List<string> StripComments(List<string> lines)
    {
      List<string> stripped = new List<string>();
      foreach (string line in lines)
      {
        if (!line.StartsWith("!"))
          stripped.Add(line);
      }
      return stripped;
    }
  }
}
