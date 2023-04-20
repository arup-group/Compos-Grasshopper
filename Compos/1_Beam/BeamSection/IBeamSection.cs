using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// A Beam Section interfaces provides information about the profile dimensions,
  /// start position and if the section is tapered to next section.
  /// </summary>
  public interface IBeamSection {
    Length BottomFlangeThickness { get; }
    Length BottomFlangeWidth { get; }
    // Dimensions
    Length Depth { get; }
    bool IsCatalogue { get; }
    Length RootRadius { get; }
    string SectionDescription { get; }
    IQuantity StartPosition { get; }
    // Setting out
    bool TaperedToNext { get; }
    Length TopFlangeThickness { get; }
    Length TopFlangeWidth { get; }
    Length WebThickness { get; }

    string ToCoaString(string name, int num, int index, ComposUnits units);
  }
}
