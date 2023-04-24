using ComposGH.Parameters;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace ComposGH.Helpers {
  internal class CustomInput {
    internal static string BeamSection(GH_Component owner, IGH_DataAccess DA, int inputid, bool isOptional = false) {
      string profile = "";
      var gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(inputid, ref gh_typ)) {
        if (gh_typ.Value is BeamSectionGoo goo) {
          return goo.Value.SectionDescription;
        } else if (gh_typ.CastTo(ref profile)) {
          return profile.Trim();
        } else {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert " + owner.Params.Input[inputid].NickName + " input (" + gh_typ.Value.GetType().Name + ") to " + typeof(BeamSectionGoo).Name.Replace("Goo", string.Empty) + " or text string");
          return null;
        }
      } else if (!isOptional) {
        owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter " + owner.Params.Input[inputid].NickName + " failed to collect data!");
      }

      return string.Empty;
    }
  }
}
