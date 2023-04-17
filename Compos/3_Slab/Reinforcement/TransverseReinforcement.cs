using ComposAPI.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ComposAPI {
  public enum LayoutMethod {
    Automatic,
    Custom
  }

  public class TransverseReinforcement : ITransverseReinforcement, ICoaObject {
    public IList<ICustomTransverseReinforcementLayout> CustomReinforcementLayouts { get; set; }
    public LayoutMethod LayoutMethod { get; set; }
    public IReinforcementMaterial Material { get; set; } // reinforcement material grade

    public TransverseReinforcement() {
      LayoutMethod = LayoutMethod.Automatic;
    }

    public TransverseReinforcement(IReinforcementMaterial material) {
      Material = material;
      LayoutMethod = LayoutMethod.Automatic;
    }

    public TransverseReinforcement(IReinforcementMaterial material, List<ICustomTransverseReinforcementLayout> transverseReinforcmentLayout) {
      Material = material;
      LayoutMethod = LayoutMethod.Custom;
      CustomReinforcementLayouts = transverseReinforcmentLayout;
    }

    public string ToCoaString(string name, ComposUnits units) {
      string str = Material.ToCoaString(name);

      str += "REBAR_LONGITUDINAL" + '\t' + name + '\t' + "PROGRAM_DESIGNED" + '\n';

      if (LayoutMethod == LayoutMethod.Automatic) {
        List<string> parameters = new List<string>();
        parameters.Add(CoaIdentifier.RebarTransverse);
        parameters.Add(name);
        parameters.Add("PROGRAM_DESIGNED");
        str += CoaHelper.CreateString(parameters);
      }
      else {
        foreach (ICustomTransverseReinforcementLayout layout in CustomReinforcementLayouts) {
          str += layout.ToCoaString(name, units);
        }
      }
      return str;
    }

    public override string ToString() {
      if (Material == null) { return "Invalid material"; }
      string mat = Material.ToString();
      if (LayoutMethod == LayoutMethod.Automatic) {
        return mat + ", Automatic layout";
      }
      else {
        string rebar = string.Join(":", CustomReinforcementLayouts.Select(x => x.ToString()).ToList());
        return mat + ", " + rebar;
      }
    }

    internal static ITransverseReinforcement FromCoaString(string coaString, string name, Code code, ComposUnits units) {
      TransverseReinforcement reinforcement = new TransverseReinforcement();
      reinforcement.CustomReinforcementLayouts = new List<ICustomTransverseReinforcementLayout>();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines) {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return reinforcement;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        switch (parameters[0]) {
          case (CoaIdentifier.RebarMaterial):
            reinforcement.Material = ReinforcementMaterial.FromCoaString(parameters, code);
            break;

          case (CoaIdentifier.RebarTransverse):
            if (parameters[2] == "PROGRAM_DESIGNED") {
              reinforcement.LayoutMethod = LayoutMethod.Automatic;
            }
            else {
              reinforcement.LayoutMethod = LayoutMethod.Custom;
              reinforcement.CustomReinforcementLayouts.Add(CustomTransverseReinforcementLayout.FromCoaString(parameters, units));
            }
            break;

          default:
            // continue;
            break;
        }
      }
      return reinforcement;
    }
  }
}
