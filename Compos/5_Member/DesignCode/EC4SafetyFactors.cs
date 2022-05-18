﻿using ComposAPI.Helpers;
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
  public class EC4SafetyFactors : IEC4SafetyFactors
  {
    public IEC4MaterialPartialFactors MaterialFactors { get; set; } = null;
    public ILoadCombinationFactors LoadCombinationFactors { get; set; } = null;
    public LoadCombination LoadCombination { get; set; } = LoadCombination.Equation6_10;

    public EC4SafetyFactors()
    {
      // default initialiser
    }

    public string ToCoaString(string name)
    {
      string str = "";
      if (this.MaterialFactors != null)
        str = this.MaterialFactors.ToCoaString(name);

      if (this.LoadCombination != LoadCombination.Custom)
      {
        switch (this.LoadCombination)
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
    public double Constantxi { get; set; } = 1.0;
    public double Constantpsi_0 { get; set; } = 1.0;
    public double Constantgamma_G { get; set; } = 1.35;
    public double Constantgamma_Q { get; set; } = 1.5;
    public double Finalxi { get; set; } = 1.0;
    public double Finalpsi_0 { get; set; } = 1.0;
    public double Finalgamma_G { get; set; } = 1.35;
    public double Finalgamma_Q { get; set; } = 1.5;

    public LoadCombinationFactors() { }

    public string ToCoaString(string name)
    {
      string str = "SAFETY_FACTOR_LOAD" + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Constantgamma_G, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Finalgamma_G, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Constantgamma_Q, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Finalgamma_Q, 6) + '\n';
      str += "EC4_LOAD_COMB_FACTORS" + '\t' + name + '\t' + "USER_DEFINED" + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Constantxi, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Finalxi, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Constantpsi_0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Finalpsi_0, 6) + '\n';
      return str;
    }
  }
}
