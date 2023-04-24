using System.Collections.Generic;
using ComposAPI.Helpers;

namespace ComposAPI {
  /// <summary>
  /// Class for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used
  /// </summary>
  public class MaterialPartialFactors : IMaterialPartialFactors {
    public double Gamma_C { get; set; } = 1.5;
    public double Gamma_Deck { get; set; } = 1.0;
    public double Gamma_M0 { get; set; } = 1.0;
    public double Gamma_M1 { get; set; } = 1.0;
    public double Gamma_M2 { get; set; } = 1.25;
    public double Gamma_S { get; set; } = 1.15;
    public double Gamma_vs { get; set; } = 1.25;

    public MaterialPartialFactors() { }

    public string ToCoaString(string name) {
      string str = "SAFETY_FACTOR_MATERIAL" + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(Gamma_M0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(Gamma_M1, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(Gamma_M2, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(Gamma_C, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(1.25, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(Gamma_Deck, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(Gamma_vs, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(Gamma_S, 6) + '\n';
      return str;
    }

    internal static MaterialPartialFactors FromCoaString(List<string> parameters) {
      var materialPartialFactors = new MaterialPartialFactors {
        Gamma_M0 = CoaHelper.ConvertToDouble(parameters[2]),
        Gamma_M1 = CoaHelper.ConvertToDouble(parameters[3]),
        Gamma_M2 = CoaHelper.ConvertToDouble(parameters[4]),
        Gamma_C = CoaHelper.ConvertToDouble(parameters[5]),
        Gamma_Deck = CoaHelper.ConvertToDouble(parameters[7]),
        Gamma_vs = CoaHelper.ConvertToDouble(parameters[8]),
        Gamma_S = CoaHelper.ConvertToDouble(parameters[9])
      };
      return materialPartialFactors;
    }
  }
}
