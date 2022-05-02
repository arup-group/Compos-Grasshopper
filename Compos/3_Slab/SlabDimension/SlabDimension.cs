﻿using System;
using System.Collections.Generic;
using System.Linq;

using ComposAPI.Helpers;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Slab dimensions such as Depth, Width, Effective Width, starting position and if the section is tapered to next section.
  /// </summary>
  public class SlabDimension
  {
    public Length StartPosition { get; set; } = Length.Zero;

    // Dimensions
    public Length OverallDepth { get; set; }
    public Length AvailableWidthLeft { get; set; }
    public Length AvailableWidtRight { get; set; }
    public bool CustomEffectiveWidth { get; set; }
    public Length EffectiveWidthLeft { get; set; }
    public Length EffectiveWidthRight { get; set; }

    // Settings
    public bool TaperedToNext { get; set; }

    #region constructors
    public SlabDimension()
    {
      // empty constructor
    }

    public SlabDimension(Length startPosition, Length overallDepth, Length availableWidthLeft, Length availableWidthRight, bool taperedToNext = false)
    {
      this.StartPosition = startPosition;
      this.OverallDepth = overallDepth;
      this.AvailableWidthLeft = availableWidthLeft;
      this.AvailableWidtRight = availableWidthRight;
      this.CustomEffectiveWidth = false;
      this.TaperedToNext = taperedToNext;
    }

    public SlabDimension(Length startPosition, Length overallDepth, Length availableWidthLeft, Length availableWidthRight,
      Length effectiveWidthLeft, Length effectiveWidthRight, bool taperedToNext = false)
    {
      this.StartPosition = startPosition;
      this.OverallDepth = overallDepth;
      this.AvailableWidthLeft = availableWidthLeft;
      this.AvailableWidtRight = availableWidthRight;
      this.CustomEffectiveWidth = true;
      this.EffectiveWidthLeft = effectiveWidthLeft;
      this.EffectiveWidthRight = effectiveWidthRight;
      this.TaperedToNext = taperedToNext;
    }
    #endregion

    #region coa interop
    internal SlabDimension(string coaString)
    {
      // to do - implement from coa string method
    }

    /// <summary>
    /// SLAB_DIMENSION | name | num | index | x | depth | width_l | width_r | tapered | override | eff_width_l | eff_width_r
    /// SLAB_DIMENSION | name | num | index | x | depth | width_l | width_r | tapered | override
    /// </summary>
    /// <param name="name"></param>
    /// <param name="num"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    internal string ToCoaString(string name, string num, string index)
    {
      List<string> parameters = new List<string>() { "SLAB_DIMENSION", name, num, index, this.StartPosition.Value.ToString(), this.OverallDepth.Value.ToString(), this.AvailableWidthLeft.Value.ToString(), this.AvailableWidtRight.Value.ToString() };
      CoaHelper.AddParameter(parameters, "TAPERED", this.TaperedToNext);
      CoaHelper.AddParameter(parameters, "EFFECTIVE_WIDTH", this.CustomEffectiveWidth);
      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public SlabDimension Duplicate()
    {
      if (this == null) { return null; }
      SlabDimension dup = (SlabDimension)this.MemberwiseClone();
      return dup;
    }

    public override string ToString()
    {
      string start = "";
      if (this.StartPosition != Length.Zero)
        start = ", Px:" + this.StartPosition.ToUnit(Units.LengthUnitGeometry).ToString("f0.0#").Replace(" ", string.Empty);
      string tapered = "";
      if (this.TaperedToNext)
        tapered = ", Tapered";

      string d = "d:" + this.OverallDepth.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string w = ", w:" + (this.AvailableWidthLeft + this.AvailableWidtRight).ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      if (this.CustomEffectiveWidth)
        w = ", weff:" + (this.EffectiveWidthLeft + this.EffectiveWidthRight).ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      return d + w + start + tapered;
    }
    #endregion
  }
}
