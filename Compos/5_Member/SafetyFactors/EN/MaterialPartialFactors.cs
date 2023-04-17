using ComposAPI.Helpers;
using System.Collections.Generic;

namespace ComposAPI {
  /// <summary>
  /// Class for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used
  /// </summary>
  public class MaterialPartialFactors : IMaterialPartialFactors {
    public double gamma_C { get; set; } = 1.5;
    public double gamma_Deck { get; set; } = 1.0;
    public double gamma_M0 { get; set; } = 1.0;
    public double gamma_M1 { get; set; } = 1.0;
    public double gamma_M2 { get; set; } = 1.25;
    public double gamma_S { get; set; } = 1.15;
    public double gamma_vs { get; set; } = 1.25;

    public MaterialPartialFactors() { }

    public string ToCoaString(string name) {
      string str = "SAFETY_FACTOR_MATERIAL" + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(gamma_M0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(gamma_M1, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(gamma_M2, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(gamma_C, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(1.25, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(gamma_Deck, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(gamma_vs, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(gamma_S, 6) + '\n';
      return str;
    }

    internal static MaterialPartialFactors FromCoaString(List<string> parameters) {
      MaterialPartialFactors materialPartialFactors = new MaterialPartialFactors();
      materialPartialFactors.gamma_M0 = CoaHelper.ConvertToDouble(parameters[2]);
      materialPartialFactors.gamma_M1 = CoaHelper.ConvertToDouble(parameters[3]);
      materialPartialFactors.gamma_M2 = CoaHelper.ConvertToDouble(parameters[4]);
      materialPartialFactors.gamma_C = CoaHelper.ConvertToDouble(parameters[5]);
      materialPartialFactors.gamma_Deck = CoaHelper.ConvertToDouble(parameters[7]);
      materialPartialFactors.gamma_vs = CoaHelper.ConvertToDouble(parameters[8]);
      materialPartialFactors.gamma_S = CoaHelper.ConvertToDouble(parameters[9]);
      return materialPartialFactors;
    }
  }
}
