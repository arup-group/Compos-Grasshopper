﻿using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System.Collections.Generic;

namespace ComposAPI {
  public class FrequencyLimits : IFrequencyLimits {
    public Ratio DeadLoadIncl { get; set; } = new Ratio(1, RatioUnit.DecimalFraction);
    public Ratio LiveLoadIncl { get; set; } = new Ratio(0.1, RatioUnit.DecimalFraction);
    public Frequency MinimumRequired { get; set; } = Frequency.Zero;

    public FrequencyLimits() { }

    public FrequencyLimits(double minReqFrequencyHertz, double deadLoadInclPercentage, double liveLoadInclPercentage) {
      MinimumRequired = new Frequency(minReqFrequencyHertz, FrequencyUnit.Hertz);
      DeadLoadIncl = new Ratio(deadLoadInclPercentage, RatioUnit.Percent);
      LiveLoadIncl = new Ratio(liveLoadInclPercentage, RatioUnit.Percent);
    }

    public string ToCoaString(string name) {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.DesignCriteria.Frequency);
      parameters.Add(name);
      parameters.Add("CHECK_NATURAL_FREQUENCY");
      parameters.Add(CoaHelper.FormatSignificantFigures(MinimumRequired.Hertz, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(DeadLoadIncl.DecimalFractions, 6));
      parameters.Add(CoaHelper.FormatSignificantFigures(LiveLoadIncl.DecimalFractions, 6));

      string coaString = CoaHelper.CreateString(parameters);

      return coaString;
    }

    public override string ToString() {
      string str = "Min:" + MinimumRequired.ToUnit(FrequencyUnit.Hertz).ToString("f0").Replace(" ", string.Empty);

      str += ", " + DeadLoadIncl.ToUnit(RatioUnit.Percent).ToString("f0").Replace(" ", string.Empty) + " DL";
      str += ", " + LiveLoadIncl.ToUnit(RatioUnit.Percent).ToString("f0").Replace(" ", string.Empty) + " LL";
      return str;
    }

    internal static IFrequencyLimits FromCoaString(List<string> parameters) {
      if (parameters[2] == "IGNORE_NATURAL_FREQUENCY") return null;

      FrequencyLimits freqLim = new FrequencyLimits();
      int i = 3;
      freqLim.MinimumRequired = new Frequency(CoaHelper.ConvertToDouble(parameters[i++]), FrequencyUnit.Hertz);
      freqLim.DeadLoadIncl = new Ratio(CoaHelper.ConvertToDouble(parameters[i++]), RatioUnit.DecimalFraction);
      freqLim.LiveLoadIncl = new Ratio(CoaHelper.ConvertToDouble(parameters[i++]), RatioUnit.DecimalFraction);

      return freqLim;
    }
  }
}
