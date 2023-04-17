using System.Collections.Generic;

namespace ComposAPI {
  /// <summary>
  /// Stud interface containing <see cref="IStudDimensions"/>, <see cref="IStudSpecification"/>, and spacing/layout settings (custom or automatic)
  /// </summary>
  public interface IStud {
    bool CheckStudSpacing { get; }
    // Stud Spacing
    IList<IStudGroupSpacing> CustomSpacing { get; }
    IStudDimensions Dimensions { get; }
    double Interaction { get; }
    double MinSavingMultipleZones { get; }
    IStudSpecification Specification { get; }
    StudSpacingType StudSpacingType { get; }

    string ToCoaString(string name, ComposUnits units, Code designCode);
  }
}
