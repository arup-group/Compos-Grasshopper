using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposAPI.Helpers
{
  public class CoaHelper
  {
    internal static void AddParameter(List<string> parameters, string parameter, bool b)
    {
      string str = parameter + "_";
      if (b)
        str += "YES";
      else
        str += "NO";
      parameters.Add(str);
    }

    public static string CreateString(List<string> parameters)
    {
      string str = "";
      foreach (string param in parameters)
        str += param + "\\t";
      str += "\\n";
      return str;
    }


  }
}
