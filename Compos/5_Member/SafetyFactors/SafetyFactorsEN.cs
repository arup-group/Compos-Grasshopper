using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
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

  public class SafetyFactorsEN : IEC4SafetyFactors
  {
    public IEC4MaterialPartialFactors MaterialFactors { get; set; } = null;
    public ILoadFactors LoadFactors { get; set; }
    public ILoadCombinationFactors LoadCombinationFactors { get; set; } = new LoadCombinationFactors();

    public SafetyFactorsEN()
    {
      // empty constructor
    }

    public string ToCoaString(string name)
    {
      string str = "";
      if (this.MaterialFactors != null)
        str = this.MaterialFactors.ToCoaString(name);

      if (this.LoadCombinationFactors.LoadCombination != LoadCombination.Custom)
      {
        switch (this.LoadCombinationFactors.LoadCombination)
        {
          case LoadCombination.Equation6_10:
            str += "EC4_LOAD_COMB_FACTORS" + '\t' + name + '\t' + "EC0_6_10" + '\t';
            str += CoaHelper.FormatSignificantFigures(1.35, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(1.35, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(1.5, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(1.5, 6) + '\n';
            break;

          case LoadCombination.Equation6_10a__6_10b:
            str += "EC4_LOAD_COMB_FACTORS" + '\t' + name + '\t' + "EC0_6_WORST_10A_10B" + '\t';
            str += CoaHelper.FormatSignificantFigures(0.85, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(0.85, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(1.0, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(0.7, 6) + '\n';
            break;
          case LoadCombination.Custom:
            if (this.LoadCombinationFactors == null)
              this.LoadCombinationFactors = new LoadCombinationFactors();
            str += this.LoadCombinationFactors.ToCoaString(name);
            break;
        }
      }
      return str;
    }
  }

  /// <summary>
  /// Class for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used
  /// </summary>
  public class EC4MaterialPartialFactors : IEC4MaterialPartialFactors
  {
    public double gamma_M0 { get; set; } = 1.0;
    public double gamma_M1 { get; set; } = 1.0;
    public double gamma_M2 { get; set; } = 1.25;
    public double gamma_C { get; set; } = 1.5;
    public double gamma_Deck { get; set; } = 1.0;
    public double gamma_vs { get; set; } = 1.25;
    public double gamma_S { get; set; } = 1.15;
    public EC4MaterialPartialFactors() { }

    public string ToCoaString(string name)
    {
      string str = "SAFETY_FACTOR_MATERIAL" + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_M0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_M1, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_M2, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_C, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.gamma_C, 6) + '\t';
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
    //public double Constantgamma_G { get; set; } = 1.35;
    //public double Finalgamma_G { get; set; } = 1.35;
    //public double Constantgamma_Q { get; set; } = 1.5;
    //public double Finalgamma_Q { get; set; } = 1.5;
    public LoadCombination LoadCombination { get; set; } = LoadCombination.Equation6_10;
    public double ConstantXi { get; set; } = 1.0; //	EC0 reduction factor at construction stage (dead/permenant load)
    public double FinalXi { get; set; } = 1.0; // EC0 reduction factor at final stage (dead/permenant load)
    public double ConstantPsi { get; set; } = 1.0; // factor for combination value of variable action at construction stage
    public double FinalPsi { get; set; } = 1.0; // factor for combination value of variable action at final stage

    public LoadCombinationFactors() { }

    #region coainterop
    internal static ILoadCombinationFactors FromCoaString(List<string> parameters)
    {
      LoadCombinationFactors combinationFactors = new LoadCombinationFactors();
      switch(parameters[2])
      {
        case ("EC0_WORST_6_10A_10B"):
          combinationFactors.LoadCombination = LoadCombination.Equation6_10a__6_10b;
          break;
        case ("USER_DEFINED"):
          combinationFactors.LoadCombination = LoadCombination.Custom;
          combinationFactors.ConstantXi = CoaHelper.ConvertToDouble(parameters[3]);
          combinationFactors.FinalXi = CoaHelper.ConvertToDouble(parameters[4]);
          combinationFactors.ConstantPsi = CoaHelper.ConvertToDouble(parameters[5]);
          combinationFactors.FinalPsi = CoaHelper.ConvertToDouble(parameters[6]);
          break;
        case ("EC0_6_10"):
        default:
          combinationFactors.LoadCombination = LoadCombination.Equation6_10;
          break;
      }
      return combinationFactors;
    }

    public string ToCoaString(string name)
    {
      //string str = "SAFETY_FACTOR_LOAD" + '\t' + name + '\t';
      //str += CoaHelper.FormatSignificantFigures(this.Constantgamma_G, 6) + '\t';
      //str += CoaHelper.FormatSignificantFigures(this.Finalgamma_G, 6) + '\t';
      //str += CoaHelper.FormatSignificantFigures(this.Constantgamma_Q, 6) + '\t';
      //str += CoaHelper.FormatSignificantFigures(this.Finalgamma_Q, 6) + '\n';
      string str = "EC4_LOAD_COMB_FACTORS" + '\t';
      str += name + '\t';
      switch (this.LoadCombination)
      {
        case (LoadCombination.Equation6_10a__6_10b):
          str += "EC0_WORST_6_10A_10B";
          break;
        case (LoadCombination.Custom):
          str += "USER_DEFINED";
          str += CoaHelper.FormatSignificantFigures(this.ConstantXi, 6) + '\t';
          str += CoaHelper.FormatSignificantFigures(this.FinalXi, 6) + '\t';
          str += CoaHelper.FormatSignificantFigures(this.ConstantPsi, 6) + '\t';
          str += CoaHelper.FormatSignificantFigures(this.FinalPsi, 6) + '\n';
          break;
        case (LoadCombination.Equation6_10):
        default:
          str += "EC0_6_10";
          break;
      }
      return str;
    }
    #endregion
  }
}
