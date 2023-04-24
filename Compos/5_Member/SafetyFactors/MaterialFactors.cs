using System.Collections.Generic;
using ComposAPI.Helpers;

namespace ComposAPI {
  /// <summary>
  /// Class for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used
  /// </summary>
  public class MaterialFactors : IMaterialFactors {
    public double ConcreteCompression { get; set; } = 1.5;
    public double ConcreteShear { get; set; } = 1.25;
    public double MetalDecking { get; set; } = 1.0;
    public double Reinforcement { get; set; } = 1.15;
    public double ShearStud { get; set; } = 1.25;
    public double SteelBeam { get; set; } = 1.0;

    public MaterialFactors() { }

    public string ToCoaString(string name) {
      string str = CoaIdentifier.SafetyFactorMaterial + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(SteelBeam, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(1.0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(1.0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(ConcreteCompression, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(ConcreteShear, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(MetalDecking, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(ShearStud, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(Reinforcement, 6) + '\n';
      return str;
    }

    internal static IMaterialFactors FromCoaString(List<string> parameters) {
      var materialPartialFactors = new MaterialFactors {
        SteelBeam = CoaHelper.ConvertToDouble(parameters[2]),
        ConcreteCompression = CoaHelper.ConvertToDouble(parameters[5]),
        ConcreteShear = CoaHelper.ConvertToDouble(parameters[6]),
        MetalDecking = CoaHelper.ConvertToDouble(parameters[7]),
        ShearStud = CoaHelper.ConvertToDouble(parameters[8]),
        Reinforcement = CoaHelper.ConvertToDouble(parameters[9])
      };
      return materialPartialFactors;
    }
  }
}
