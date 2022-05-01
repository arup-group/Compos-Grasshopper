using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.Member
{
  public class DesignOptions
  {
    public bool ProppedDuringConstruction { get; set; } = true;
    public bool InclSteelBeamWeight { get; set; } = false;
    public bool InclThinFlangeSections { get; set; } = false;
    public bool ConsiderShearDeflection { get; set; } = false;

    public DesignOptions()
    {
      // default initialiser
    }

    public DesignOptions Duplicate()
    {
      if (this == null) { return null; }
      DesignOptions dup = (DesignOptions)this.MemberwiseClone();
      return dup;
    }
  }
}
