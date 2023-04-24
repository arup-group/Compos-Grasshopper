using OasysUnits;

namespace ComposAPI {
  /// <summary>
  /// Slab dimensions such as Depth, Width, Effective Width, starting position and if the section is tapered to next section.
  /// </summary>
  public interface ISlabDimension {
    Length AvailableWidthLeft { get; }
    Length AvailableWidthRight { get; }
    Length EffectiveWidthLeft { get; }
    Length EffectiveWidthRight { get; }
    // Dimensions 
    Length OverallDepth { get; }
    IQuantity StartPosition { get; }
    // Settings
    bool TaperedToNext { get; }
    bool UserEffectiveWidth { get; }
  }
}
