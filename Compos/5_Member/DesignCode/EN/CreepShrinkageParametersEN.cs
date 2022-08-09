using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class CreepShrinkageParametersEN : ICreepShrinkageParameters
  {
    /// <summary>
    /// Creep multiplier used for calculating E ratio for long term and shrinkage (see clause 5.4.2.2 of EN 1994-1-1:2004) 
    /// </summary>
    public double CreepCoefficient { get; set; }
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
    public Ratio RelativeHumidity { get; set; } = new Ratio(50, RatioUnit.Percent);

    public CreepShrinkageParametersEN() { }
  }
}
