using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

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
    { ConcreteAgeAtLoad = 28, CreepCoefficient = 1.1, FinalConcreteAgeCreep = 36500, RelativeHumidity = 0.5 };
    public ICreepShrinkageParameters ShortTerm { get; set; } = new CreepShrinkageParametersEN()
    { ConcreteAgeAtLoad = 1, CreepCoefficient = 0.55, FinalConcreteAgeCreep = 36500, RelativeHumidity = 0.5 };

    public CodeOptionsEN()
    {
      // default initialiser
    }

    #region coainterop
    internal ICodeOptions FromCoaString(List<string> parameters)
    {
      CodeOptionsEN eC4Options = new CodeOptionsEN();

      // todo

      return eC4Options;
    }

    public string ToCoaString(string name, Code code, NationalAnnex nationalAnnex)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.EC4DesignOption);
      parameters.Add(name);

      CoaHelper.AddParameter(parameters, "SHRINKAGE_DEFORM_EC4", this.ConsiderShrinkageDeflection);
      CoaHelper.AddParameter(parameters, "IGNORE_SHRINKAGE_DEFORM", this.IgnoreShrinkageDeflectionForLowLengthToDepthRatios);
      CoaHelper.AddParameter(parameters, "APPROXIMATE_E_RATIO", this.ApproxModularRatios);

      if(nationalAnnex == NationalAnnex.United_Kingdom)
        parameters.Add("United Kingdom");
      else
        parameters.Add("Generic");

      switch(this.CementType)
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
      parameters.Add(CoaHelper.FormatSignificantFigures(lt.CreepCoefficient, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(st.CreepCoefficient, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(lt.ConcreteAgeAtLoad, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(st.ConcreteAgeAtLoad, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(lt.FinalConcreteAgeCreep, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(st.FinalConcreteAgeCreep, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(lt.RelativeHumidity, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(st.RelativeHumidity, 6));

      return CoaHelper.CreateString(parameters);
    }
    #endregion
  }
}
