using ComposAPI.Helpers;

namespace ComposAPI {
  public enum LoadCombination {
    Equation6_10,
    Equation6_10a__6_10b,
    Custom
  }  /// <summary>

     /// Custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
     /// </summary>
  public class LoadCombinationFactors : ILoadCombinationFactors {
    public double Constantgamma_G { get; set; } = 1.35;
    public double Constantgamma_Q { get; set; } = 1.5;
    public double ConstantPsi { get; set; } = 1.0;
    public double ConstantXi { get; set; } = 1.0;
    public double Finalgamma_G { get; set; } = 1.35;
    public double Finalgamma_Q { get; set; } = 1.5;
    // factor for combination value of variable action at construction stage
    public double FinalPsi { get; set; } = 1.0;
    //	EC0 reduction factor at construction stage (dead/permenant load)
    public double FinalXi { get; set; } = 1.0;
    public LoadCombination LoadCombination { get; set; } = LoadCombination.Equation6_10;
    // EC0 reduction factor at final stage (dead/permenant load)
    // factor for combination value of variable action at final stage

    public LoadCombinationFactors() { }

    internal LoadCombinationFactors(LoadCombination loadCombination) {
      LoadCombination = loadCombination;
      if (loadCombination == LoadCombination.Equation6_10a__6_10b) {
        ConstantXi = 0.85;
        FinalXi = 0.85;
        ConstantPsi = 1.0;
        FinalPsi = 0.7;
      }
    }

    // from CoaString happens in SafetyFactorsEN class
    public string ToCoaString(string name) {
      if (LoadCombination == LoadCombination.Custom) {
        string str = "SAFETY_FACTOR_LOAD" + '\t' + name + '\t';
        str += CoaHelper.FormatSignificantFigures(Constantgamma_G, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(Finalgamma_G, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(Constantgamma_Q, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(Finalgamma_Q, 6) + '\n';

        str += "EC4_LOAD_COMB_FACTORS" + '\t' + name + '\t' + "USER_DEFINED" + '\t';
        str += CoaHelper.FormatSignificantFigures(ConstantXi, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(FinalXi, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(ConstantPsi, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(FinalPsi, 6) + '\n';
        return str;
      }
      else {
        string str = "EC4_LOAD_COMB_FACTORS" + '\t';
        str += name + '\t';
        switch (LoadCombination) {
          case (LoadCombination.Equation6_10a__6_10b):
            str += "EC0_WORST_6_10A_10B" + '\t';
            str += CoaHelper.FormatSignificantFigures(ConstantXi, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(FinalXi, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(ConstantPsi, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(FinalPsi, 6) + '\n';
            break;

          case (LoadCombination.Equation6_10):
          default:
            str += "EC0_6_10" + '\t';
            str += CoaHelper.FormatSignificantFigures(ConstantXi, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(FinalXi, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(ConstantPsi, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(FinalPsi, 6) + '\n';
            break;
        }
        return str;
      }
    }
  }
}
