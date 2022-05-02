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

    public SafetyFactors Duplicate()
    {
      if (this == null) { return null; }
      SafetyFactors dup = new SafetyFactors();
      if (this.MaterialFactors != null)
        dup.MaterialFactors = this.MaterialFactors.Duplicate();
      if (this.LoadFactors != null)
        dup.LoadFactors = this.LoadFactors.Duplicate();
      return dup;
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
    public MaterialPartialFactors Duplicate()
    {
      if (this == null) { return null; }
      return (MaterialPartialFactors)this.MemberwiseClone();
    }
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
    public LoadFactors Duplicate()
    {
      if (this == null) { return null; }
      return (LoadFactors)this.MemberwiseClone();
    }
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

    public new EC4SafetyFactors Duplicate()
    {
      if (this == null) { return null; }
      EC4SafetyFactors dup = (EC4SafetyFactors)this.MemberwiseClone();
      if (this.MaterialFactors != null)
        dup.MaterialFactors = this.MaterialFactors.Duplicate();
      if (this.LoadFactors != null)
        dup.LoadFactors = this.LoadFactors.Duplicate();
      return dup;
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
    public EC4MaterialPartialFactors Duplicate()
    {
      if (this == null) { return null; }
      return (EC4MaterialPartialFactors)this.MemberwiseClone();
    }
  }

  public enum LoadCombination
  {
    Equation6_10,
    Equation6_10a_6_10b,
    Custom
  }
  /// <summary>
  /// Custom Load factors. These data can be omitted, if they are omitted, code specified load factor will be used
  /// </summary>
  public class LoadCombinationFactors
  {
    public double xi { get; set; } = 1.0;
    public double psi_0 { get; set; } = 1.0;
    public double psi_G { get; set; } = 1.35;
    public double psi_Q { get; set; } = 1.5;
    public LoadCombinationFactors() { }
    public LoadCombinationFactors Duplicate()
    {
      if (this == null) { return null; }
      return (LoadCombinationFactors)this.MemberwiseClone();
    }
  }
}
