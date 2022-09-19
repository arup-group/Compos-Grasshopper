using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI.Helpers;
using OasysUnitsNet;

namespace ComposAPI
{
  public class DesignOption : IDesignOption
  {
    public bool ProppedDuringConstruction { get; set; } = true; // construction type
    public bool InclSteelBeamWeight { get; set; } = false; // include beam weight or not in analysis
    public bool InclConcreteSlabWeight { get; set; } = false; //	include slab weight or not in analysis
    public bool ConsiderShearDeflection { get; set; } = false; //	shear deformation
    public bool InclThinFlangeSections { get; set; } = false; // include thin flange section or not in the selection of steel beams in design

    public DesignOption()
    {
      // default initialiser
    }
  }
}
