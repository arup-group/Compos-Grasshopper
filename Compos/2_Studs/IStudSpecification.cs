using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Interface for accessing various (code dependent) specifications for a <see cref="Stud"/>
  /// </summary>
  public interface IStudSpecification
  {
    // Stud Specifications
     bool Welding { get;  }
     bool NCCI { get;  }
     bool EC4_Limit { get;  }
     Length NoStudZoneStart { get;  }
     Length NoStudZoneEnd { get;  }
     Length ReinforcementPosition { get;  }
     StudSpecType SpecType { get;  }
  }
}
