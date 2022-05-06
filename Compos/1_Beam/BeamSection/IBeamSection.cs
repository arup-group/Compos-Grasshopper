using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// A Beam Section interfaces provides information about the profile dimensions, 
  /// start position and if the section is tapered to next section.
  /// </summary>
  public interface IBeamSection
  {
    // Setting out
    bool TaperedToNext { get; }
    Length StartPosition { get; }
    // Dimensions
    Length Depth { get; }
    Length TopFlangeWidth { get; }
    Length BottomFlangeWidth { get; }
    Length TopFlangeThickness { get; }
    Length BottomFlangeThickness { get; }
    Length RootRadius { get; }
    Length WebThickness { get; }
    bool isCatalogue { get; }
    string SectionDescription { get; }
  }
}
