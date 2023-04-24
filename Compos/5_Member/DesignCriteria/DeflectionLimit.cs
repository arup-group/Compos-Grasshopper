using System.Collections.Generic;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class DeflectionLimit : IDeflectionLimit {
    public Length AbsoluteDeflection { get; set; } = Length.Zero;
    public Ratio SpanOverDeflectionRatio { get; set; } = Ratio.Zero;

    public DeflectionLimit() { }

    public DeflectionLimit(double absoluteDeflection, LengthUnit lengthUnit) {
      AbsoluteDeflection = new Length(absoluteDeflection, lengthUnit);
    }

    public DeflectionLimit(double spanDeflectionRatio) {
      SpanOverDeflectionRatio = new Ratio(spanDeflectionRatio, RatioUnit.DecimalFraction);
    }

    public string ToCoaString(string name, DeflectionLimitLoadType type, ComposUnits units) {
      string coaString = "";

      if (AbsoluteDeflection != Length.Zero) {
        var parameters = new List<string> {
          CoaIdentifier.DesignCriteria.DeflectionLimit,
          name,
          GetLoadTypeCoaString(type),
          "ABSOLUTE",
          CoaHelper.FormatSignificantFigures(AbsoluteDeflection.ToUnit(units.Displacement).Value, 6)
        };

        coaString += CoaHelper.CreateString(parameters);
      }

      if (SpanOverDeflectionRatio != Ratio.Zero) {
        var parameters = new List<string> {
          CoaIdentifier.DesignCriteria.DeflectionLimit,
          name,
          GetLoadTypeCoaString(type),
          "SPAN/DEF_RATIO",
          CoaHelper.FormatSignificantFigures(SpanOverDeflectionRatio.DecimalFractions, 6)
        };

        coaString += CoaHelper.CreateString(parameters);
      }

      return coaString;
    }

    public override string ToString() {
      string str = "";
      if (AbsoluteDeflection != Length.Zero) {
        str += "δ:" + AbsoluteDeflection.ToUnit(ComposUnitsHelper.LengthUnitResult).ToString("f0").Replace(" ", string.Empty) + ", ";
      }

      if (SpanOverDeflectionRatio != Ratio.Zero) {
        str += "δ:1/" + SpanOverDeflectionRatio.DecimalFractions.ToString("f0").Replace(" ", string.Empty);
      }
      return str.TrimEnd(' ').TrimEnd(',');
    }

    internal static IDeflectionLimit FromCoaString(string coaString, string name, DeflectionLimitLoadType type, ComposUnits units) {
      var defLim = new DeflectionLimit();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines) {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END") {
          return defLim;
        }

        if (parameters[0] == CoaIdentifier.UnitData) {
          units.FromCoaString(parameters);
        }

        if (parameters[1] != name) {
          continue;
        }

        if (parameters[2] != GetLoadTypeCoaString(type)) {
          continue;
        }

        if (parameters[3] == "SPAN/DEF_RATIO") {
          defLim.SpanOverDeflectionRatio = new Ratio(CoaHelper.ConvertToDouble(parameters[4]), RatioUnit.DecimalFraction);
        } else if (parameters[3] == "ABSOLUTE") {
          defLim.AbsoluteDeflection = new Length(CoaHelper.ConvertToDouble(parameters[4]), units.Displacement);
        }
      }
      return defLim;
    }

    internal static DeflectionLimitLoadType GetLoadType(string coaString) {
      switch (coaString) {
        case "CONSTRUCTION_DEAD_LOAD":
          return DeflectionLimitLoadType.ConstructionDeadLoad;

        case "ADDITIONAL_DEAD_LOAD":
          return DeflectionLimitLoadType.AdditionalDeadLoad;

        case "FINAL_LIVE_LOAD":
          return DeflectionLimitLoadType.FinalLiveLoad;

        case "TOTAL":
          return DeflectionLimitLoadType.Total;

        case "POST_CONSTRUCTION":
          return DeflectionLimitLoadType.PostConstruction;
      }
      return DeflectionLimitLoadType.Total;
    }

    internal static string GetLoadTypeCoaString(DeflectionLimitLoadType type) {
      switch (type) {
        case DeflectionLimitLoadType.ConstructionDeadLoad:
          return "CONSTRUCTION_DEAD_LOAD";

        case DeflectionLimitLoadType.AdditionalDeadLoad:
          return "ADDITIONAL_DEAD_LOAD";

        case DeflectionLimitLoadType.FinalLiveLoad:
          return "FINAL_LIVE_LOAD";

        case DeflectionLimitLoadType.Total:
          return "TOTAL";

        case DeflectionLimitLoadType.PostConstruction:
          return "POST_CONSTRUCTION";
      }
      return null;
    }
  }

  public enum DeflectionLimitLoadType {
    ConstructionDeadLoad,
    AdditionalDeadLoad,
    FinalLiveLoad,
    Total,
    PostConstruction
  }
}
