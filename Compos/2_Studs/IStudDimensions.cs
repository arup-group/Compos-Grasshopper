using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Interface for accessing dimensions and strength for a <see cref="IStud"/>
  /// </summary>
  public interface IStudDimensions
  {
    Length Diameter { get; set; }
    Length Height { get; set; }
    Force CharacterStrength { get; set; }
    Pressure Fu { get; set; }
    bool IsStandard { get; set; }
    bool IsStandardENGrade { get; set; }

    void SetGradeFromStandard(StandardStudGrade standardGrade);
    void SetSizeFromStandard(StandardStudSize size);
  }
}
