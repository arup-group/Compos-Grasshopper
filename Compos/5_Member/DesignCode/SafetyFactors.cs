using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public class SafetyFactors
  {
    public MaterialPartialFactors MaterialFactors { get; set; } = null;
    public LoadFactors LoadFactors { get; set; } = null;

    public SafetyFactors()
    {
      // default initialiser
    }
  }

  /// <summary>
  /// Class for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used
  /// </summary>
  public class MaterialPartialFactors
  {
    public double SteelBeam { get; set; } = 1.0;
    public double ConcreteCompression { get; set; } = 1.5;
    public double ConcreteShear { get; set; } = 1.25;
    public double MetalDecking { get; set; } = 1.0;
    public double ShearStud { get; set; } = 1.25;
    public double Reinforcement { get; set; } = 1.15;
    public MaterialPartialFactors() { }
  }

  /// <summary>
  /// Custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public class LoadFactors
  {
    public double ConstantDead { get; set; } = 1.4;
    public double ConstantLive { get; set; } = 1.4;
    public double FinalDead { get; set; } = 1.6;
    public double FinalLive { get; set; } = 1.6;
    public LoadFactors() { }
  }

  public class EC4SafetyFactors : SafetyFactors
  {
    public new EC4MaterialPartialFactors MaterialFactors { get; set; } = null;
    public new LoadCombinationFactors LoadFactors { get; set; } = null;
    public LoadCombination LoadCombination { get; set; } = LoadCombination.Equation6_10;

    public EC4SafetyFactors()
    {
      // default initialiser
    }
  }

  /// <summary>
  /// Class for custom material factors. These data can be omitted, if they are omitted, code specified safety factor will be used
  /// </summary>
  public class EC4MaterialPartialFactors
  {
    public double gamma_M0 { get; set; } = 1.0;
    public double gamma_M1 { get; set; } = 1.0;
    public double gamma_M2 { get; set; } = 1.25;
    public double gamma_C { get; set; } = 1.5;
    public double gamma_Deck { get; set; } = 1.0;
    public double gamma_vs { get; set; } = 1.25;
    public double gamma_S { get; set; } = 1.15;
    public EC4MaterialPartialFactors() { }
  }

  public enum LoadCombination
  {
    Equation6_10,
    Equation6_10a__6_10b,
    Custom
  }

  /// <summary>
  /// Custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public class LoadCombinationFactors
  {
    public double xi { get; set; } = 1.0;
    public double psi_0 { get; set; } = 1.0;
    public double gamma_G { get; set; } = 1.35;
    public double gamma_Q { get; set; } = 1.5;
    public LoadCombinationFactors() { }
  }
}
