using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class FrequencyLimits : IFrequencyLimits
  {
    public Frequency MinimumRequired { get; set; } = Frequency.Zero;
    public Ratio DeadLoadIncl { get; set; } = new Ratio(1, RatioUnit.DecimalFraction);
    public Ratio LiveLoadIncl { get; set; } = new Ratio(0.1, RatioUnit.DecimalFraction);

    public FrequencyLimits() { }
    
    #region coa interop
    internal static IFrequencyLimits FromCoaString(List<string> parameters)
    {
      if (parameters[2] == "IGNORE_NATURAL_FREQUENCY") return null;
      
      FrequencyLimits freqLim = new FrequencyLimits();
      int i = 3;
      freqLim.MinimumRequired = new Frequency(CoaHelper.ConvertToDouble(parameters[i++]), FrequencyUnit.Hertz);
      freqLim.DeadLoadIncl = new Ratio(CoaHelper.ConvertToDouble(parameters[i++]), RatioUnit.DecimalFraction);
      freqLim.LiveLoadIncl = new Ratio(CoaHelper.ConvertToDouble(parameters[i++]), RatioUnit.DecimalFraction);

      return freqLim;
    }

    public string ToCoaString(string name)
    {

      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignCriteria.Frequency);
      parameters.Add(name);
      parameters.Add("CHECK_NATURAL_FREQUENCY");
      parameters.Add(CoaHelper.FormatSignificantFigures(this.MinimumRequired.Hertz, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.DeadLoadIncl.DecimalFractions, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(this.LiveLoadIncl.DecimalFractions, 6));

      string coaString = CoaHelper.CreateString(parameters);

      return coaString;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string str = "Min:" + this.MinimumRequired.ToUnit(FrequencyUnit.Hertz).ToString("f0").Replace(" ", string.Empty);

      str += ", " + this.DeadLoadIncl.ToUnit(RatioUnit.Percent).ToString("f0").Replace(" ", string.Empty) + " DL";
      str += ", " + this.LiveLoadIncl.ToUnit(RatioUnit.Percent).ToString("f0").Replace(" ", string.Empty) + " LL";
      return str;
    }
    #endregion
  }
}
