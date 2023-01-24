using ComposAPI.Helpers;
using System.Collections.Generic;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  public enum DeflectionLimitLoadType
  {
    ConstructionDeadLoad,
    AdditionalDeadLoad,
    FinalLiveLoad,
    Total,
    PostConstruction
  }
  public class DeflectionLimit : IDeflectionLimit
  {
    public Length AbsoluteDeflection { get; set; } = Length.Zero;
    public Ratio SpanOverDeflectionRatio { get; set; } = Ratio.Zero;

    public DeflectionLimit() { }

    public DeflectionLimit(double absoluteDeflection, LengthUnit lengthUnit)
    {
      this.AbsoluteDeflection = new Length(absoluteDeflection, lengthUnit);
    }
    public DeflectionLimit(double spanDeflectionRatio)
    {
      this.SpanOverDeflectionRatio = new Ratio(spanDeflectionRatio, RatioUnit.DecimalFraction);
    }
    
    #region coa interop
    internal static string GetLoadTypeCoaString(DeflectionLimitLoadType type)
    {
      switch (type)
      {
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
    internal static DeflectionLimitLoadType GetLoadType(string coaString)
    {
      switch (coaString)
      {
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
    internal static IDeflectionLimit FromCoaString(string coaString, string name, DeflectionLimitLoadType type, ComposUnits units)
    {
      DeflectionLimit defLim = new DeflectionLimit();

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return defLim;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        if (parameters[2] != GetLoadTypeCoaString(type))
          continue;

        if (parameters[3] == "SPAN/DEF_RATIO")
          defLim.SpanOverDeflectionRatio = new Ratio(CoaHelper.ConvertToDouble(parameters[4]), RatioUnit.DecimalFraction);
        else if (parameters[3] == "ABSOLUTE")
          defLim.AbsoluteDeflection = new Length(CoaHelper.ConvertToDouble(parameters[4]), units.Displacement);
      }
      return defLim;
    }

    public string ToCoaString(string name, DeflectionLimitLoadType type, ComposUnits units)
    {
      string coaString = "";

      if (this.AbsoluteDeflection != Length.Zero)
      {
        List<string> parameters = new List<string>();
        parameters.Add(CoaIdentifier.DesignCriteria.DeflectionLimit);
        parameters.Add(name);
        parameters.Add(GetLoadTypeCoaString(type));
        parameters.Add("ABSOLUTE");
        parameters.Add(CoaHelper.FormatSignificantFigures(this.AbsoluteDeflection.ToUnit(units.Displacement).Value, 6));

        coaString += CoaHelper.CreateString(parameters);
      }

      if (this.SpanOverDeflectionRatio != Ratio.Zero)
      {
        List<string> parameters = new List<string>();
        parameters.Add(CoaIdentifier.DesignCriteria.DeflectionLimit);
        parameters.Add(name);
        parameters.Add(GetLoadTypeCoaString(type));
        parameters.Add("SPAN/DEF_RATIO");
        parameters.Add(CoaHelper.FormatSignificantFigures(this.SpanOverDeflectionRatio.DecimalFractions, 6));

        coaString += CoaHelper.CreateString(parameters);
      }

      return coaString;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string str = "";
      if (this.AbsoluteDeflection != Length.Zero)
        str += "δ:" + this.AbsoluteDeflection.ToUnit(ComposUnitsHelper.LengthUnitResult).ToString("f0").Replace(" ", string.Empty) + ", ";

      if (this.SpanOverDeflectionRatio != Ratio.Zero)
      {
        str += "δ:1/" + this.SpanOverDeflectionRatio.DecimalFractions.ToString("f0").Replace(" ", string.Empty);
      }
      return str.TrimEnd(' ').TrimEnd(',');
    }
    #endregion
  }
}
