using ComposAPI.Helpers;
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

  public class CodeOptionsASNZ : ICodeOptions
  {
    public bool ConsiderShrinkageDeflection { get; set; } = false;
    public ICreepShrinkageParameters LongTerm { get; set; } = new CreepShrinkageParametersASNZ() { CreepCoefficient = 2.0 };
    public ICreepShrinkageParameters ShortTerm { get; set; } = new CreepShrinkageParametersASNZ() { CreepCoefficient = 2.0 };
    /// <summary>
    /// Deafult constructor with AS/NZ values and members
    /// </summary>
    public CodeOptionsASNZ()
    {
      // default initialiser
    }
  }
}
