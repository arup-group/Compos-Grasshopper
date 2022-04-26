using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GsaGH.Parameters;

namespace ComposGH.Converters
{
  public static class GsaGHConverter
  {
    public static object CastToComposBeam(object source)
    {
      return null;
    }

    public static Type GetTypeFor(Type type)
    {
      return typeof(GsaElement1d);
    }

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
