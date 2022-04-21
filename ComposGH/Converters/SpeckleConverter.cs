using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speckle.Core.Kits;
using Speckle.Core.Models;

namespace ComposGH.Converters
{
  public class SpeckleConverter //: ISpeckleConverter
  {
    public static bool IsPresent()
    {
      try
      {
        // Get a list of all available kits
        IEnumerable<ISpeckleKit> kits = KitManager.Kits;

        // Load the default Objects Kit and the included Revit converter
        var kit = Speckle.Core.Kits.KitManager.GetDefaultKit();
      }
      catch (DllNotFoundException)
      {
        return false;
      }
      return true;
    }

  }
}
