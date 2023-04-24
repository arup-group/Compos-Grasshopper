using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Interface for accessing various (code dependent) specifications for a <see cref="Stud"/>
  /// </summary>
  public interface IStudSpecification {
    bool EC4_Limit { get; set; }
    bool NCCI { get; set; }
    IQuantity NoStudZoneEnd { get; set; }
    IQuantity NoStudZoneStart { get; set; }
    Length ReinforcementPosition { get; set; }
    StudSpecType SpecType { get; set; }
    // Stud Specifications
    bool Welding { get; set; }
  }
}
