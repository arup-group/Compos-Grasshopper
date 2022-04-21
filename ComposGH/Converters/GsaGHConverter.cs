using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GsaGH.Parameters;

namespace ComposGH.Converters
{
  public class GsaGHConverter
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

    public static Type GetTypeFor(Type type)
    {
      throw new NotImplementedException();
    }

    public static object CastToComposBeam(object source)
    {
      throw new NotImplementedException();
    }
  }
}
