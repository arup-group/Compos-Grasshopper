﻿using ComposAPI.Helpers;
using System.Collections.Generic;

namespace ComposAPI {
  /// <summary>
  /// Custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public class LoadFactors : ILoadFactors {
    public double ConstantDead { get; set; } = 1.4;
    public double ConstantLive { get; set; } = 1.4;
    public double FinalDead { get; set; } = 1.6;
    public double FinalLive { get; set; } = 1.6;

    public LoadFactors() { }

    public string ToCoaString(string name) {
      string str = CoaIdentifier.SafetyFactorLoad + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(ConstantDead, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(FinalDead, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(ConstantLive, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(FinalLive, 6) + '\n';
      return str;
    }

    internal static ILoadFactors FromCoaString(List<string> parameters) {
      LoadFactors loadFactors = new LoadFactors();
      loadFactors.ConstantDead = CoaHelper.ConvertToDouble(parameters[2]);
      loadFactors.FinalDead = CoaHelper.ConvertToDouble(parameters[3]);
      loadFactors.ConstantLive = CoaHelper.ConvertToDouble(parameters[4]);
      loadFactors.FinalLive = CoaHelper.ConvertToDouble(parameters[5]);
      return loadFactors;
    }
  }
}
