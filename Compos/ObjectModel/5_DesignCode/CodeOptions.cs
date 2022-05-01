using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.DesignCode
{
  
  public class CodeOptions
  {
    public bool ConsiderShrinkageDeflection { get; set; } = false;
    public virtual CreepShrinkageParameters LongTerm { get; set; } = new CreepShrinkageParameters()
    { CreepMultiplier = 2.0 };
    public virtual CreepShrinkageParameters ShortTerm { get; set; } = new CreepShrinkageParameters()
    { CreepMultiplier = 2.0 };

    public CodeOptions()
    {
      // default initialiser
    }
  }
  public class EN1994Options : CodeOptions
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
    { ConcreteAgeAtLoad = 28, CreepMultiplier = 1.1, FinalConcreteAgeCreep = 36500, RelativeHumidity = 0.5 };
    public new CreepShrinkageEuroCodeParameters ShortTerm { get; set; } = new CreepShrinkageEuroCodeParameters()
    { ConcreteAgeAtLoad = 1, CreepMultiplier = 0.55, FinalConcreteAgeCreep = 36500, RelativeHumidity = 0.5 };
    public EN1994Options()
    {
      // default initialiser
    }
  }
  public class CreepShrinkageParameters
  {
    /// <summary>
    /// Creep multiplier used for calculating E ratio for long term and shrinkage (see clause 5.4.2.2 of EN 1994-1-1:2004) 
    /// </summary>
    public double CreepMultiplier { get; set; }
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
  }
}
