using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;

namespace ComposAPI.ConcreteSlab
{
  /// <summary>
  /// Slab dimensions such as Depth, Width, Effective Width, starting position and if the section is tapered to next section.
  /// </summary>
  public class ConcreteSlabDimension
  {
    public Length StartPosition { get; set; } = Length.Zero;

    // Dimensions
    public Length OverallDepth { get; set; }
    public Length AvailableWidthLeft { get; set; }
    public Length AvailableWidtRight { get; set; }
    public bool OverrideEffectiveWidth { get; set; }
    public Length EffectiveWidthLeft { get; set; }
    public Length EffectiveWidthRight { get; set; }

    public bool TaperedToNext { get; set; }

    #region constructors
    public ConcreteSlabDimension()
    {
      // empty constructor
    }

    public ConcreteSlabDimension(Length startPosition, Length overallDepth, Length availableWidthLeft, Length availableWidthRight, bool overrideEffectiveWidth, 
      Length effectiveWidthLeft, Length effectiveWidthRight, bool taperedToNext = false)
    {
      this.StartPosition = startPosition;
      this.OverallDepth = overallDepth;
      this.AvailableWidthLeft = availableWidthLeft;
      this.AvailableWidtRight = availableWidthRight;
      this.OverrideEffectiveWidth = overrideEffectiveWidth;
      this.EffectiveWidthLeft = effectiveWidthLeft;
      this.EffectiveWidthRight = effectiveWidthRight;
      this.TaperedToNext = taperedToNext;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region methods

    public ConcreteSlabDimension Duplicate()
    {
      if (this == null) { return null; }
      ConcreteSlabDimension dup = (ConcreteSlabDimension)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string start = "";
      if (this.StartPosition != Length.Zero)
        start = ", Px:" + this.StartPosition.ToString("f0").Replace(" ", string.Empty);
      string tapered = "";
      if (this.TaperedToNext)
        tapered = ", Tapered";

      //return (this.SectionDescription == null) ? "Null profile" : this.SectionDescription + start + tapered;
      return "";
    }
    #endregion
  }
}
