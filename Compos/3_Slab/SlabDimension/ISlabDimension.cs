﻿using OasysUnits;

namespace ComposAPI
{
  /// <summary>
  /// Slab dimensions such as Depth, Width, Effective Width, starting position and if the section is tapered to next section.
  /// </summary>
  public interface ISlabDimension
  {
    IQuantity StartPosition { get; }
    // Dimensions
    Length OverallDepth { get; }
    Length AvailableWidthLeft { get; }
    Length AvailableWidthRight { get; }
    bool UserEffectiveWidth { get; }
    Length EffectiveWidthLeft { get; }
    Length EffectiveWidthRight { get; }
    // Settings
    bool TaperedToNext { get; }
  }
}
