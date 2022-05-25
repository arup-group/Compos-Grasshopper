using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public class SafetyFactors : ISafetyFactors
  {
    public IMaterialPartialFactors MaterialFactors { get; set; } = null;
    public ILoadFactors LoadFactors { get; set; } = null;

    public SafetyFactors()
    {
      // default initialiser
    }
  }

  /// <summary>
  /// Class for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used
  /// </summary>
  public class MaterialPartialFactors : IMaterialPartialFactors
  {
    public double SteelBeam { get; set; } = 1.0;
    public double ConcreteCompression { get; set; } = 1.5;
    public double ConcreteShear { get; set; } = 1.25;
    public double MetalDecking { get; set; } = 1.0;
    public double ShearStud { get; set; } = 1.25;
    public double Reinforcement { get; set; } = 1.15;

    public MaterialPartialFactors() { }

    #region coainterop
    internal static IMaterialPartialFactors FromCoaString(List<string> parameters)
    {
      MaterialPartialFactors materialPartialFactors = new MaterialPartialFactors();
      materialPartialFactors.SteelBeam = CoaHelper.ConvertToDouble(parameters[2]);
      materialPartialFactors.ConcreteCompression = CoaHelper.ConvertToDouble(parameters[5]);
      materialPartialFactors.ConcreteShear = CoaHelper.ConvertToDouble(parameters[6]);
      materialPartialFactors.MetalDecking = CoaHelper.ConvertToDouble(parameters[7]);
      materialPartialFactors.ShearStud = CoaHelper.ConvertToDouble(parameters[8]);
      materialPartialFactors.Reinforcement = CoaHelper.ConvertToDouble(parameters[9]);
      return materialPartialFactors;
    }

    public string ToCoaString(string name)
    {
      string str = CoaIdentifier.SafetyFactorMaterial + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(this.SteelBeam, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(1.0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(1.0, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.ConcreteCompression, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.ConcreteShear, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.MetalDecking, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.ShearStud, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Reinforcement, 6) + '\n';
      return str;
    }
    #endregion
  }

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
