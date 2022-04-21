using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GsaGH.Parameters;

namespace ComposGH.Converters
{
  internal class GsaGHConverter
  {
    public static bool IsPresent()
    {
      try
      {
        GsaMaterial material = new GsaMaterial();
      }
      catch (DllNotFoundException)
      {
        return false;
      }
      return true;
    }
  }
}
