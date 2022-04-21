using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdSecGH.Parameters;

namespace Converters
{
  internal class AdSecGHConverter
  {
    public static bool IsPresent()
    {
      try
      {
        AdSecMaterial material = new AdSecMaterial();
      }
      catch (DllNotFoundException)
      {
        return false;
      }
      return true;
    }
  }
}
