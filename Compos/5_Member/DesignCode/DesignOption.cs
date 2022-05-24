using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;

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

    //#region coainterop
    //internal IDesignOptions FromCoaString(List<string> parameters)
    //{
    //  DesignOptions designOptions = new DesignOptions();

    //  return designOptions;
    //}

    //public string ToCoaString(string name, Code code)
    //{
    //  List<string> parameters = new List<string>();
    //  parameters.Add(CoaIdentifier.DesignOption);
    //  parameters.Add(name);

    //  switch (code)
    //  {
    //    case (Code.AS_NZS2327_2017):
    //      parameters.Add(CoaIdentifier.DesignCode.ASNZ);
    //      break;

    //    case (Code.BS5950_3_1_1990_Superseded):
    //      parameters.Add(CoaIdentifier.DesignCode.BS_Superseded);
    //      break;

    //    case (Code.EN1994_1_1_2004):
    //      parameters.Add(CoaIdentifier.DesignCode.EN);
    //      break;

    //    case (Code.HKSUOS_2005):
    //      parameters.Add(CoaIdentifier.DesignCode.HKSUOS2005);
    //      break;

    //    case (Code.HKSUOS_2011):
    //      parameters.Add(CoaIdentifier.DesignCode.HKSUOS2011);
    //      break;

    //    case (Code.BS5950_3_1_1990_A1_2010):
    //    default:
    //      parameters.Add(CoaIdentifier.DesignCode.BS);
    //      break;
    //  }

    //  if (this.ProppedDuringConstruction)
    //    parameters.Add("PROPPED");
    //  else
    //    parameters.Add("UNPROPPED");

    //  CoaHelper.AddParameter(parameters, "BEAM_WEIGHT", this.InclSteelBeamWeight);
    //  CoaHelper.AddParameter(parameters, "SLAB_WEIGHT", this.InclConcreteSlabWeight);
    //  CoaHelper.AddParameter(parameters, "SHEAR_DEFORM", this.ConsiderShearDeflection);
    //  CoaHelper.AddParameter(parameters, "THIN_SECTION", this.InclThinFlangeSections);

    //  return CoaHelper.CreateString(parameters);
    //}
    //#endregion
  }
}
