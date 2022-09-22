using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  public enum CementClass
  {
    S,
    N,
    R
  }
  public class CodeOptionsEN : ICodeOptions
  {
    public bool ConsiderShrinkageDeflection { get; set; } = false;
    public CementClass CementType { get; set; } = CementClass.N;
    /// <summary>
    /// This member will only be used if <see cref="ConsiderShrinkageDeflection"/> is true.
    /// Ignore shrinkage deflection if the ratio of length to depth is less than 20 for normal weight concrete.
    /// </summary>
    public bool IgnoreShrinkageDeflectionForLowLengthToDepthRatios { get; set; } = false;
    /// <summary>
    /// Use approximate modular ratios - Approximate E ratios are used in accordance with 5.2.2 (11) of EN 1994-1-1:2004 
    /// </summary>
    public bool ApproxModularRatios { get; set; } = false;
    public ICreepShrinkageParameters LongTerm { get; set; } = new CreepShrinkageParametersEN()
    { ConcreteAgeAtLoad = 28, CreepCoefficient = 1.1, FinalConcreteAgeCreep = 36500 };
    public ICreepShrinkageParameters ShortTerm { get; set; } = new CreepShrinkageParametersEN()
    { ConcreteAgeAtLoad = 1, CreepCoefficient = 0.55, FinalConcreteAgeCreep = 36500 };

    public CodeOptionsEN()
    {
      // default initialiser
    }

    #region coainterop
    internal static CodeOptionsEN FromCoaString(List<string> parameters)
    {
      CodeOptionsEN codeOptionsEN = new CodeOptionsEN();
     
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      int i = 2;
      
      codeOptionsEN.ConsiderShrinkageDeflection = parameters[i++] == "SHRINKAGE_DEFORM_EC4_YES";
      codeOptionsEN.IgnoreShrinkageDeflectionForLowLengthToDepthRatios = parameters[i++] == "IGNORE_SHRINKAGE_DEFORM_YES";
      codeOptionsEN.ApproxModularRatios = parameters[i++] == "APPROXIMATE_E_RATIO_YES";
      i++; // national annex not set here.
      switch (parameters[i++].Last())
      {
        case 'S':
          codeOptionsEN.CementType = CementClass.S;
          break;
        case 'R':
          codeOptionsEN.CementType = CementClass.R;
          break;
        case 'N':
          codeOptionsEN.CementType = CementClass.N;
          break;
      }
      
      CreepShrinkageParametersEN longTerm = new CreepShrinkageParametersEN();
      CreepShrinkageParametersEN shortTerm = new CreepShrinkageParametersEN();

      longTerm.CreepCoefficient = Convert.ToDouble(parameters[i++], noComma);
      shortTerm.CreepCoefficient = Convert.ToDouble(parameters[i++], noComma);
      longTerm.ConcreteAgeAtLoad = (int)Math.Round(Convert.ToDouble(parameters[i++], noComma));
      shortTerm.ConcreteAgeAtLoad = (int)Math.Round(Convert.ToDouble(parameters[i++], noComma));
      longTerm.FinalConcreteAgeCreep = (int)Math.Round(Convert.ToDouble(parameters[i++], noComma));
      shortTerm.FinalConcreteAgeCreep = (int)Math.Round(Convert.ToDouble(parameters[i++], noComma));
      longTerm.RelativeHumidity = new Ratio(Convert.ToDouble(parameters[i++], noComma), RatioUnit.Percent);
      shortTerm.RelativeHumidity = new Ratio(Convert.ToDouble(parameters[i++], noComma), RatioUnit.Percent);

      codeOptionsEN.LongTerm = longTerm;
      codeOptionsEN.ShortTerm = shortTerm;

      return codeOptionsEN;
    }

    public string ToCoaString(string name, Code code, NationalAnnex nationalAnnex)
    {
      List<string> parameters = new List<string>();
      //EC4_DESIGN_OPTION
      // EC4_DESIGN_OPTION | name | Shrink - deform | ignore - Shrink | ApproxERatios | Country - Name | Cement - Type | Creep_Long | Creep_Shrink | T0_Long | T0_Shrink | T_Long | T_Shrink | RH_Long | RH_Shrink
      parameters.Add(CoaIdentifier.EC4DesignOption); // 0
      parameters.Add(name); // 1

      CoaHelper.AddParameter(parameters, "SHRINKAGE_DEFORM_EC4", this.ConsiderShrinkageDeflection); // 2
      CoaHelper.AddParameter(parameters, "IGNORE_SHRINKAGE_DEFORM", this.IgnoreShrinkageDeflectionForLowLengthToDepthRatios); // 3
      CoaHelper.AddParameter(parameters, "APPROXIMATE_E_RATIO", this.ApproxModularRatios); // 4

      if(nationalAnnex == NationalAnnex.United_Kingdom) // 5
        parameters.Add("United Kingdom");
      else
        parameters.Add("Generic");

      switch(this.CementType) // 6
      {
        case CementClass.S:
          parameters.Add("CLASS_S");
          break;
        case CementClass.N:
        default:
          parameters.Add("CLASS_N");
          break;
        case CementClass.R:
          parameters.Add("CLASS_R");
          break;
      }
      CreepShrinkageParametersEN lt = (CreepShrinkageParametersEN)LongTerm; 
      CreepShrinkageParametersEN st = (CreepShrinkageParametersEN)ShortTerm;
      parameters.Add(CoaHelper.FormatSignificantFigures(lt.CreepCoefficient, 6)); // 7
      parameters.Add(CoaHelper.FormatSignificantFigures(st.CreepCoefficient, 6)); // 8
      parameters.Add(CoaHelper.FormatSignificantFigures(lt.ConcreteAgeAtLoad, 6)); // 9
      parameters.Add(CoaHelper.FormatSignificantFigures(st.ConcreteAgeAtLoad, 6)); // 10
      parameters.Add(CoaHelper.FormatSignificantFigures(lt.FinalConcreteAgeCreep, 6)); // 11
      parameters.Add(CoaHelper.FormatSignificantFigures(st.FinalConcreteAgeCreep, 6)); // 12
      parameters.Add(CoaHelper.FormatSignificantFigures(lt.RelativeHumidity.Percent, 6)); // 13 
      parameters.Add(CoaHelper.FormatSignificantFigures(st.RelativeHumidity.Percent, 6)); // 14

      return CoaHelper.CreateString(parameters);
    }
    #endregion
  }
}
