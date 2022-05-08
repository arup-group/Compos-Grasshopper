using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposAPI.Helpers
{
  public class CoaHelper
  {
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

    internal static List<string> Split(string coaString)
    {
      List<string> parameters = coaString.Split('\t').ToList();
      foreach (string param in parameters)
        RemoveWhitespace(param);
      return parameters;
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

    public static string FormatSignificantFigures(double value, int significantFigures)
    {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      // if for instance 6 significant figures and value is above 1,000,000
      // compos coa is shown as 4.50000e+008 which is value.ToString("e6")
      if (value > Math.Pow(10, significantFigures))
        return value.ToString("e" + (significantFigures-1), noComma);

      int decimalPlaces = Math.Max(0, significantFigures - GetMagnitude((int)value));
      string format = "{0:0.";
      for (int i = 0; i < decimalPlaces; i++)
        format += "0";
      format += "}";
      return String.Format(noComma, format, value);
    }

    internal static List<string> SplitLines(string coaString)
    {
      List<string> lines = coaString.Split('\n').ToList();
      return lines;
    }

  }
}
