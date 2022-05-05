using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Slab dimensions such as Depth, Width, Effective Width, starting position and if the section is tapered to next section.
  /// </summary>
  public interface ISlabDimension
  {
    Length StartPosition { get; }
    // Dimensions
    Length OverallDepth { get; }
    Length AvailableWidthLeft { get; }
    Length AvailableWidtRight { get; }
    bool CustomEffectiveWidth { get; }
    Length EffectiveWidthLeft { get; }
    Length EffectiveWidthRight { get; }
    // Settings
    bool TaperedToNext { get; }
  }
}
