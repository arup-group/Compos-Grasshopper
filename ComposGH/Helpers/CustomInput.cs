using System;
using ComposGH.Parameters;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;

namespace ComposGH.Helpers
{
  internal class CustomInput
  {
    internal static string BeamSection(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false)
    {
      BeamSectionGoo goo = null;
      string profile = "";
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ))
      {
        if (gh_typ.Value is BeamSectionGoo)
        {
          goo = (BeamSectionGoo)gh_typ.Value;
          return goo.Value.SectionDescription;
        }
        else if (gh_typ.CastTo(ref profile))
          return profile.Trim();
        else
        {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " input (" + gh_typ.Value.GetType().Name + ") to " + typeof(BeamSectionGoo).Name.Replace("Goo", string.Empty) + " or text string");
          return null;
        }
      }
      else if (!isOptional)
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");

      return String.Empty;
    }
  }
}
