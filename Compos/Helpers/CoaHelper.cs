using System;
using System.Collections.Generic;
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
        str += param + "\\t";
      str += "\\n";
      return str;
    }

    internal static string RemoveWhitespace(string str)
    {
      return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
    }

    internal static List<string> Split(string coaString)
    {
      List<string> parameters = coaString.Split('\t').ToList();
      foreach(string param in parameters)
        RemoveWhitespace(param);
      return parameters;
    }

    internal static List<string> SplitLines(string coaString)
    {
      List<string> lines = coaString.Split('\n').ToList();
      return lines;
    }

  }
}
