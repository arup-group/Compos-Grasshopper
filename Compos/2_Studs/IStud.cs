using System.Collections.Generic;
using UnitsNet.Units;

namespace ComposAPI
{
  /// <summary>
  /// Stud interface containing <see cref="IStudDimensions"/>, <see cref="IStudSpecification"/>, and spacing/layout settings (custom or automatic)
  /// </summary>
  public interface IStud
  {
    IStudDimensions StudDimensions { get; }
    IStudSpecification StudSpecification { get; }
    // Stud Spacing
    List<IStudGroupSpacing> CustomSpacing { get; }
    double Interaction { get; }
    double MinSavingMultipleZones { get; }
    bool CheckStudSpacing { get; }
    StudSpacingType StudSpacingType { get; }

    string ToCoaString(string name, ComposUnits units, Code designCode);
  }
}
