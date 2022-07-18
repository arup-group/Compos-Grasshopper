using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public enum LoadCombination
  {
    Equation6_10,
    Equation6_10a__6_10b,
    Custom
  }

  public class SafetyFactorsEN : ISafetyFactorsEN
  {
    public IMaterialPartialFactorsEN MaterialFactors { get; set; } = null;
    public ILoadCombinationFactors LoadCombinationFactors { get; set; } = new LoadCombinationFactors();

    public SafetyFactorsEN()
    {
      // empty constructor
    }

    internal static ISafetyFactorsEN FromCoaString(string coaString, string name)
    {
      SafetyFactorsEN safetyFactors = new SafetyFactorsEN();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return safetyFactors;

        if (parameters[1] != name)
          continue;

        switch (parameters[0])
        {
          case (CoaIdentifier.SafetyFactorLoad):
            safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Custom;
            LoadCombinationFactors loadFactors = (LoadCombinationFactors)safetyFactors.LoadCombinationFactors;
            loadFactors.Constantgamma_G = CoaHelper.ConvertToDouble(parameters[2]);
            loadFactors.Constantgamma_Q = CoaHelper.ConvertToDouble(parameters[3]);
            loadFactors.Finalgamma_G = CoaHelper.ConvertToDouble(parameters[4]);
            loadFactors.Finalgamma_Q = CoaHelper.ConvertToDouble(parameters[5]);
            safetyFactors.LoadCombinationFactors = loadFactors;
            break;

          case (CoaIdentifier.EC4LoadCombinationFactors):
            switch (parameters[2])
            {
              case ("EC0_WORST_6_10A_10B"):
                safetyFactors.LoadCombinationFactors = new LoadCombinationFactors(LoadCombination.Equation6_10a__6_10b);
                break;
              case ("USER_DEFINED"):
                safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Custom;
                LoadCombinationFactors combinationFactors = (LoadCombinationFactors)safetyFactors.LoadCombinationFactors;
                combinationFactors.ConstantXi = CoaHelper.ConvertToDouble(parameters[3]);
                combinationFactors.FinalXi = CoaHelper.ConvertToDouble(parameters[4]);
                combinationFactors.ConstantPsi = CoaHelper.ConvertToDouble(parameters[5]);
                combinationFactors.FinalPsi = CoaHelper.ConvertToDouble(parameters[6]);
                safetyFactors.LoadCombinationFactors = combinationFactors;
                break;
              case ("EC0_6_10"):
              default:
                safetyFactors.LoadCombinationFactors.LoadCombination = LoadCombination.Equation6_10;
                break;
            }
            break;

          case (CoaIdentifier.SafetyFactorMaterial):
            safetyFactors.MaterialFactors = MaterialPartialFactorsEN.FromCoaString(parameters);
            break;
        }
      }
      return safetyFactors;
    }

    public string ToCoaString(string name)
    {
      string str = "";
      if (this.MaterialFactors != null)
        str = this.MaterialFactors.ToCoaString(name);

      if (this.LoadCombinationFactors != null)
        str += this.LoadCombinationFactors.ToCoaString(name);

      return str;
    }
  }

  /// <summary>
  /// Class for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used
  /// </summary>
  public class MaterialPartialFactorsEN : IMaterialPartialFactorsEN
  {
    public double gamma_M0 { get; set; } = 1.0;
    public double gamma_M1 { get; set; } = 1.0;
    public double gamma_M2 { get; set; } = 1.25;
    public double gamma_C { get; set; } = 1.5;
    public double gamma_Deck { get; set; } = 1.0;
    public double gamma_vs { get; set; } = 1.25;
    public double gamma_S { get; set; } = 1.15;
    public MaterialPartialFactorsEN() { }

    internal static MaterialPartialFactorsEN FromCoaString(List<string> parameters)
    {
      MaterialPartialFactorsEN materialPartialFactors = new MaterialPartialFactorsEN();
      materialPartialFactors.gamma_M0 = CoaHelper.ConvertToDouble(parameters[2]);
      materialPartialFactors.gamma_M1 = CoaHelper.ConvertToDouble(parameters[3]);
      materialPartialFactors.gamma_M2 = CoaHelper.ConvertToDouble(parameters[4]);
      materialPartialFactors.gamma_C = CoaHelper.ConvertToDouble(parameters[5]);
      materialPartialFactors.gamma_Deck = CoaHelper.ConvertToDouble(parameters[7]);
      materialPartialFactors.gamma_vs = CoaHelper.ConvertToDouble(parameters[8]);
      materialPartialFactors.gamma_S = CoaHelper.ConvertToDouble(parameters[9]);
      return materialPartialFactors;
    }
    public string ToCoaString(string name)
    {
      string str = "SAFETY_FACTOR_MATERIAL" + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_M0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_M1, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_M2, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_C, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(1.25, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_Deck, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_vs, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_S, 6) + '\n';
      return str;
    }
  }

  /// <summary>
  /// Custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public class LoadCombinationFactors : ILoadCombinationFactors
  {
    public double Constantgamma_G { get; set; } = 1.35;
    public double Finalgamma_G { get; set; } = 1.35;
    public double Constantgamma_Q { get; set; } = 1.5;
    public double Finalgamma_Q { get; set; } = 1.5;
    public LoadCombination LoadCombination { get; set; } = LoadCombination.Equation6_10;
    public double ConstantXi { get; set; } = 1.0; //	EC0 reduction factor at construction stage (dead/permenant load)
    public double FinalXi { get; set; } = 1.0; // EC0 reduction factor at final stage (dead/permenant load)
    public double ConstantPsi { get; set; } = 1.0; // factor for combination value of variable action at construction stage
    public double FinalPsi { get; set; } = 1.0; // factor for combination value of variable action at final stage

    public LoadCombinationFactors() { }
    internal LoadCombinationFactors(LoadCombination loadCombination)
    {
      LoadCombination = loadCombination;
      if (loadCombination == LoadCombination.Equation6_10a__6_10b)
      {
        ConstantXi = 0.85;
        FinalXi = 0.85;
        ConstantPsi = 1.0;
        FinalPsi = 0.7;
      }
    }

    #region coainterop

    public string ToCoaString(string name)
    {
      if (this.LoadCombination == LoadCombination.Custom)
      {
        string str = "SAFETY_FACTOR_LOAD" + '\t' + name + '\t';
        str += CoaHelper.FormatSignificantFigures(this.Constantgamma_G, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(this.Constantgamma_Q, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(this.Finalgamma_G, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(this.Finalgamma_Q, 6) + '\n';

        str += "EC4_LOAD_COMB_FACTORS" + '\t' + name + '\t' + "USER_DEFINED" + '\t';
        str += CoaHelper.FormatSignificantFigures(this.ConstantXi, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(this.FinalXi, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(this.ConstantPsi, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(this.FinalPsi, 6) + '\n';
        return str;
      }
      else
      {
        string str = "EC4_LOAD_COMB_FACTORS" + '\t';
        str += name + '\t';
        switch (this.LoadCombination)
        {
          case (LoadCombination.Equation6_10a__6_10b):
            str += "EC0_WORST_6_10A_10B";
            break;
          case (LoadCombination.Equation6_10):
          default:
            str += "EC0_6_10";
            break;
        }
        return str + '\n';
      }
    }
    #endregion
  }
}
