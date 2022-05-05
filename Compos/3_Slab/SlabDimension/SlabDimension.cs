using System;
using System.Collections.Generic;
using System.Linq;

using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  /// <summary>
  /// Slab dimensions such as Depth, Width, Effective Width, starting position and if the section is tapered to next section.
  /// </summary>
  public class SlabDimension : ISlabDimension, ICoaObject
  {
    public Length StartPosition { get; set; } = Length.Zero;

    // Dimensions
    public Length OverallDepth { get; set; } //	depth of slab
    public Length AvailableWidthLeft { get; set; } // left hand side width of slab 
    public Length AvailableWidthRight { get; set; } //	right hand side width of slab
    public bool UserEffectiveWidth { get; set; } // override effective width
    public Length EffectiveWidthLeft { get; set; } //	left hand side effective width of slab
    public Length EffectiveWidthRight { get; set; } //	right hand side effective width of slab

    // Settings
    public bool TaperedToNext { get; set; } // Tapered to next section flag


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
      this.AvailableWidthRight = availableWidthRight;
      this.UserEffectiveWidth = false;
      this.TaperedToNext = taperedToNext;
    }

    public SlabDimension(Length startPosition, Length overallDepth, Length availableWidthLeft, Length availableWidthRight,
      Length effectiveWidthLeft, Length effectiveWidthRight, bool taperedToNext = false)
    {
      this.StartPosition = startPosition;
      this.OverallDepth = overallDepth;
      this.AvailableWidthLeft = availableWidthLeft;
      this.AvailableWidthRight = availableWidthRight;
      this.UserEffectiveWidth = true;
      this.EffectiveWidthLeft = effectiveWidthLeft;
      this.EffectiveWidthRight = effectiveWidthRight;
      this.TaperedToNext = taperedToNext;
    }
    #endregion

    #region coa interop
    internal SlabDimension(List<string> parameters, LengthUnit unit)
    {
      if (parameters.Count < 10)
      {
        throw new Exception("Unable to convert " + parameters + " to Compos Slab Dimension.");
      }
      this.StartPosition = new Length(Convert.ToDouble(parameters[4]), unit);
      this.OverallDepth = new Length(Convert.ToDouble(parameters[5]), unit);
      this.AvailableWidthLeft = new Length(Convert.ToDouble(parameters[6]), unit);
      this.AvailableWidthRight = new Length(Convert.ToDouble(parameters[7]), unit);

      if (parameters[8] == "TAPERED_YES")
        this.TaperedToNext = true;
      else
        this.TaperedToNext = false;

      if (parameters[8] == "EFFECTIVE_WIDTH_YES")
      {
        this.UserEffectiveWidth = true;
        if (parameters.Count != 12)
        {
          throw new Exception("Unable to convert " + parameters + " to Compos Slab Dimension.");
        }
        this.EffectiveWidthLeft = new Length(Convert.ToDouble(parameters[9]), unit);
        this.EffectiveWidthRight = new Length(Convert.ToDouble(parameters[9]), unit);
      }
    }

    /// <summary>
    /// SLAB_DIMENSION | name | num | index | x | depth | width_l | width_r | tapered | override | eff_width_l | eff_width_r
    /// SLAB_DIMENSION | name | num | index | x | depth | width_l | width_r | tapered | override
    /// </summary>
    /// <param name="name">member name</param>
    /// <param name="num">number of total slab section to be defined</param>
    /// <param name="index">index of current slab section (1 based)</param>
    /// <returns></returns>
    internal string ToCoaString(string name, int num, int index, LengthUnit lengthUnit)
    {
      List<string> parameters = new List<string>() { CoaIdentifier.SlabDimension, name, Convert.ToString(num), Convert.ToString(index), this.StartPosition.ToUnit(lengthUnit).ToString(), this.OverallDepth.ToUnit(lengthUnit).ToString(), this.AvailableWidthLeft.ToUnit(lengthUnit).ToString(), this.AvailableWidthRight.ToUnit(lengthUnit).ToString() };
      CoaHelper.AddParameter(parameters, "TAPERED", this.TaperedToNext);
      CoaHelper.AddParameter(parameters, "EFFECTIVE_WIDTH", this.UserEffectiveWidth);
      if (this.UserEffectiveWidth)
      {
        parameters.Add(this.EffectiveWidthLeft.ToUnit(lengthUnit).ToString());
        parameters.Add(this.EffectiveWidthRight.ToUnit(lengthUnit).ToString());
      }
      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string start = "";
      if (this.StartPosition != Length.Zero)
        start = ", Px:" + this.StartPosition.ToUnit(Units.LengthUnitGeometry).ToString("f0.0#").Replace(" ", string.Empty);
      string tapered = "";
      if (this.TaperedToNext)
        tapered = ", Tapered";

      string d = "d:" + this.OverallDepth.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string w = ", w:" + (this.AvailableWidthLeft + this.AvailableWidthRight).ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      if (this.UserEffectiveWidth)
        w = ", weff:" + (this.EffectiveWidthLeft + this.EffectiveWidthRight).ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      return d + w + start + tapered;
    }
    #endregion
  }
}
