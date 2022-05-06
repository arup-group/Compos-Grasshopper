using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;

namespace ComposAPI
{
  public class DesignOptions : IDesignOptions
  {
    public bool ProppedDuringConstruction { get; set; } = true; // construction type
    public bool InclSteelBeamWeight { get; set; } = false; // include beam weight or not in analysis
    public bool InclConcreteSlabWeight { get; set; } = false; //	include slab weight or not in analysis
    public bool ConsiderShearDeflection { get; set; } = false; //	shear deformation
    public bool InclThinFlangeSections { get; set; } = false; // include thin flange section or not in the selection of steel beams in design

    public DesignOptions()
    {
      // default initialiser
    }

    public string ToCoaString(string name, Code code)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignOption);
      parameters.Add(name);
      parameters.Add(code.ToString());
      
      if (this.ProppedDuringConstruction)
        parameters.Add("PROPPED");
      else
        parameters.Add("UNPROPPED");

      CoaHelper.AddParameter(parameters, "BEAM_WEIGHT", this.InclSteelBeamWeight);
      CoaHelper.AddParameter(parameters, "SLAB_WEIGHT", this.InclConcreteSlabWeight);
      CoaHelper.AddParameter(parameters, "SHEAR_DEFORM", this.ConsiderShearDeflection);
      CoaHelper.AddParameter(parameters, "THIN_SECTION", this.InclThinFlangeSections);

      return CoaHelper.CreateString(parameters);
    }
  }
}
