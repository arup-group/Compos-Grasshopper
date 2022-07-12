using System.Collections.Generic;
using UnitsNet.Units;

namespace ComposAPI
{
  /// <summary>
  /// Stud interface containing <see cref="IStudDimensions"/>, <see cref="IStudSpecification"/>, and spacing/layout settings (custom or automatic)
  /// </summary>
  public interface IStud
  {
    IStudDimensions Dimensions { get; }
    IStudSpecification Specification { get; }
    // Stud Spacing
    IList<IStudGroupSpacing> CustomSpacing { get; }
    double Interaction { get; }
    double MinSavingMultipleZones { get; }
    bool CheckStudSpacing { get; }
    StudSpacingType StudSpacingType { get; }

    string ToCoaString(string name, ComposUnits units, Code designCode);
  }
}
