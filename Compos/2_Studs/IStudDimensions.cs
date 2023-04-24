using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Interface for accessing dimensions and strength for a <see cref="IStud"/>
  /// </summary>
  public interface IStudDimensions {
    Force CharacterStrength { get; set; }
    Length Diameter { get; set; }
    Pressure Fu { get; set; }
    Length Height { get; set; }
    bool IsStandard { get; set; }
    bool IsStandardENGrade { get; set; }

    void SetGradeFromStandard(StandardStudGrade standardGrade);

    void SetSizeFromStandard(StandardStudSize size);
  }
}
