using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.Member
{
  public class CodeOptions
  {
    public bool ConsiderShrinkageDeflection { get; set; } = false;
    public virtual CreepShrinkageParameters LongTerm { get; set; } = new CreepShrinkageParameters()
    { CreepCoefficient = 2.0 };
    public virtual CreepShrinkageParameters ShortTerm { get; set; } = new CreepShrinkageParameters()
    { CreepCoefficient = 2.0 };
    /// <summary>
    /// Deafult constructor with AS/NZ values and members
    /// </summary>
    public CodeOptions()
    {
      // default initialiser
    }
    public CodeOptions Duplicate()
    {
      if (this == null) { return null; }
      CodeOptions dup = (CodeOptions)this.MemberwiseClone();
      dup.LongTerm = this.LongTerm.Duplicate();
      return dup;
    }
  }
  public class EC4Options : CodeOptions
  {
    public enum CementClass
    {
      S,
      N,
      R
    }
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
    public new CreepShrinkageEuroCodeParameters LongTerm { get; set; } = new CreepShrinkageEuroCodeParameters() 
    { ConcreteAgeAtLoad = 28, CreepCoefficient = 1.1, FinalConcreteAgeCreep = 36500, RelativeHumidity = 0.5 };
    public new CreepShrinkageEuroCodeParameters ShortTerm { get; set; } = new CreepShrinkageEuroCodeParameters()
    { ConcreteAgeAtLoad = 1, CreepCoefficient = 0.55, FinalConcreteAgeCreep = 36500, RelativeHumidity = 0.5 };
    public EC4Options()
    {
      // default initialiser
    }
    public new EC4Options Duplicate()
    {
      if (this == null) { return null; }
      EC4Options dup = (EC4Options)this.MemberwiseClone();
      dup.LongTerm = this.LongTerm.Duplicate();
      dup.ShortTerm = this.ShortTerm.Duplicate();
      return dup;
    }
  }
  public class CreepShrinkageParameters
  {
    /// <summary>
    /// Creep multiplier used for calculating E ratio for long term and shrinkage (see clause 5.4.2.2 of EN 1994-1-1:2004) 
    /// </summary>
    public double CreepCoefficient { get; set; }
    public CreepShrinkageParameters() { }
    public CreepShrinkageParameters Duplicate()
    {
      if (this == null) { return null; }
      CreepShrinkageParameters dup = (CreepShrinkageParameters)this.MemberwiseClone();
      return dup;
    }
  }
  public class CreepShrinkageEuroCodeParameters : CreepShrinkageParameters
  {
    /// <summary>
    /// Age of concrete in days when load applied, used to calculate the creep coefficient 
    /// </summary>
    public int ConcreteAgeAtLoad { get; set; }
    /// <summary>
    /// Final age of concrete in days, used to calculate the creep coefficient 
    /// </summary>
    public int FinalConcreteAgeCreep { get; set; } = 36500;
    /// <summary>
    /// Relative humidity as fraction (0.5 => 50%), used to calculate the creep coefficient 
    /// </summary>
    public double RelativeHumidity { get; set; } = 0.5;
    public CreepShrinkageEuroCodeParameters() { }
    public new CreepShrinkageEuroCodeParameters Duplicate()
    {
      if (this == null) { return null; }
      CreepShrinkageEuroCodeParameters dup = (CreepShrinkageEuroCodeParameters)this.MemberwiseClone();
      return dup;
    }
  }
}
