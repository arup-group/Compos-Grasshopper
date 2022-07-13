﻿using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Interface for accessing various (code dependent) specifications for a <see cref="Stud"/>
  /// </summary>
  public interface IStudSpecification
  {
    // Stud Specifications
    bool Welding { get; set; }
    bool NCCI { get; set; }
    bool EC4_Limit { get; set; }
    Length NoStudZoneStart { get; set; }
    Length NoStudZoneEnd { get; set; }
    Length ReinforcementPosition { get; set; }
    StudSpecType SpecType { get; set; }
  }
}
