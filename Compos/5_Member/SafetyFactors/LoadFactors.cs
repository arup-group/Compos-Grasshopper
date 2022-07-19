using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public class LoadFactors : ILoadFactors
  {
    public double ConstantDead { get; set; } = 1.4;
    public double FinalDead { get; set; } = 1.6;
    public double ConstantLive { get; set; } = 1.4;
    public double FinalLive { get; set; } = 1.6;

    public LoadFactors() { }

    #region coainterop
    internal static ILoadFactors FromCoaString(List<string> parameters)
    {
      LoadFactors loadFactors = new LoadFactors();
      loadFactors.ConstantDead = CoaHelper.ConvertToDouble(parameters[2]);
      loadFactors.FinalDead = CoaHelper.ConvertToDouble(parameters[3]);
      loadFactors.ConstantLive = CoaHelper.ConvertToDouble(parameters[4]);
      loadFactors.FinalLive = CoaHelper.ConvertToDouble(parameters[5]);
      return loadFactors;
    }

    public string ToCoaString(string name)
    {
      string str = CoaIdentifier.SafetyFactorLoad + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(this.ConstantDead, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.FinalDead, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.ConstantLive, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.FinalLive, 6) + '\n';
      return str;
    }
    #endregion
  }
}
