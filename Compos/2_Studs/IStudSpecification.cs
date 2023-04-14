using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Interface for accessing various (code dependent) specifications for a <see cref="Stud"/>
  /// </summary>
  public interface IStudSpecification {
    // Stud Specifications
    bool Welding { get; set; }
    bool Ncci { get; set; }
    bool Ec4Limit { get; set; }
    IQuantity NoStudZoneStart { get; set; }
    IQuantity NoStudZoneEnd { get; set; }
    Length ReinforcementPosition { get; set; }
    StudSpecType SpecType { get; set; }
  }
}
