using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public enum CementClass
  {
    S,
    N,
    R
  }

  public class CodeOptions : ICodeOptions
  {
    public bool ConsiderShrinkageDeflection { get; set; } = false;
    public virtual ICreepShrinkageParameters LongTerm { get; set; } = new CreepShrinkageParameters() { CreepCoefficient = 2.0 };
    public virtual ICreepShrinkageParameters ShortTerm { get; set; } = new CreepShrinkageParameters() { CreepCoefficient = 2.0 };
    /// <summary>
    /// Deafult constructor with AS/NZ values and members
    /// </summary>
    public CodeOptions()
    {
      // default initialiser
    }
  }

  public class EC4Options : CodeOptions
  {
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
  }
}
