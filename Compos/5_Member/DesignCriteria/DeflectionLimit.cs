using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum DeflectionLimitLoadType
  {
    CONSTRUCTION_DEAD_LOAD,
    ADDITIONAL_DEAD_LOAD,
    FINAL_LIVE_LOAD,
    TOTAL,
    POST_CONSTRUCTION
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

        if (parameters[2] != type.ToString())
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
        parameters.Add(type.ToString());
        parameters.Add("ABSOLUTE");
        parameters.Add(CoaHelper.FormatSignificantFigures(this.AbsoluteDeflection.ToUnit(units.Displacement).Value, 6));

        coaString += CoaHelper.CreateString(parameters);
      }

      if (this.SpanOverDeflectionRatio != Ratio.Zero)
      {
        List<string> parameters = new List<string>();
        parameters.Add(CoaIdentifier.DesignCriteria.DeflectionLimit);
        parameters.Add(name);
        parameters.Add(type.ToString());
        parameters.Add("SPAN/DEF_RATIO");
        parameters.Add(CoaHelper.FormatSignificantFigures(this.AbsoluteDeflection.ToUnit(units.Displacement).Value, 6));

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
        str += "δ:" + this.AbsoluteDeflection.ToUnit(Units.LengthUnitResult).ToString("f0").Replace(" ", string.Empty);

      if (this.SpanOverDeflectionRatio != Ratio.Zero)
      {
        str += "δ:1/" + this.SpanOverDeflectionRatio.DecimalFractions.ToString("f0").Replace(" ", string.Empty);
      }
      return str;
    }
    #endregion
  }
}
