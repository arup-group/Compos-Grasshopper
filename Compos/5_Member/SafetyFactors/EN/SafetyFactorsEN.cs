﻿using System.Collections.Generic;
using ComposAPI.Helpers;

namespace ComposAPI {
  public class SafetyFactorsEN : ISafetyFactorsEN {
    public ILoadCombinationFactors LoadCombinationFactors { get; set; } = new LoadCombinationFactors();
    public IMaterialPartialFactors MaterialFactors { get; set; } = null;

    public SafetyFactorsEN() {
      // empty constructor
    }

    public string ToCoaString(string name) {
      string str = "";
      if (MaterialFactors != null) {
        str = MaterialFactors.ToCoaString(name);
      }

      if (LoadCombinationFactors != null) {
        str += LoadCombinationFactors.ToCoaString(name);
      }

      return str;
    }

    internal static ISafetyFactorsEN FromCoaString(string coaString, string name) {
      var safetyFactors = new SafetyFactorsEN();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines) {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END") {
          return safetyFactors;
        }

        if (parameters[1] != name) {
          continue;
        }

        switch (parameters[0]) {
          case CoaIdentifier.SafetyFactorLoad:
            safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Custom;
            var loadFactors = (LoadCombinationFactors)safetyFactors.LoadCombinationFactors;
            loadFactors.Constantgamma_G = CoaHelper.ConvertToDouble(parameters[2]);
            loadFactors.Constantgamma_Q = CoaHelper.ConvertToDouble(parameters[3]);
            loadFactors.Finalgamma_G = CoaHelper.ConvertToDouble(parameters[4]);
            loadFactors.Finalgamma_Q = CoaHelper.ConvertToDouble(parameters[5]);
            safetyFactors.LoadCombinationFactors = loadFactors;
            break;

          case CoaIdentifier.EC4LoadCombinationFactors:
            switch (parameters[2]) {
              case "EC0_WORST_6_10A_10B":
                safetyFactors.LoadCombinationFactors = new LoadCombinationFactors(LoadCombination.Equation6_10a__6_10b);
                break;

              case "USER_DEFINED":
                safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Custom;
                var combinationFactors = (LoadCombinationFactors)safetyFactors.LoadCombinationFactors;
                combinationFactors.ConstantXi = CoaHelper.ConvertToDouble(parameters[3]);
                combinationFactors.FinalXi = CoaHelper.ConvertToDouble(parameters[4]);
                combinationFactors.ConstantPsi = CoaHelper.ConvertToDouble(parameters[5]);
                combinationFactors.FinalPsi = CoaHelper.ConvertToDouble(parameters[6]);
                safetyFactors.LoadCombinationFactors = combinationFactors;
                break;

              case "EC0_6_10":
              default:
                safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Equation6_10;
                break;
            }
            break;

          case CoaIdentifier.SafetyFactorMaterial:
            safetyFactors.MaterialFactors = MaterialPartialFactors.FromCoaString(parameters);
            break;
        }
      }
      return safetyFactors;
    }
  }
}
