using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Define Stiffener Plate information used for a <see cref="WebOpening"/>.
  /// </summary>
  public class WebOpeningStiffeners
  {
    public Length DistanceFrom { get; set; }
    public Length TopStiffenerWidth { get; set; }
    public Length TopStiffenerThickness { get; set; }
    public Length BottomStiffenerWidth { get; set; }
    public Length BottomStiffenerThickness { get; set; }
    public bool isBothSides { get; set; }
    public bool isNotch { get; set; }

    #region constructors
    public WebOpeningStiffeners()
    {
      // empty constructor
    }
    /// <summary>
    /// Create web opening stiffeners
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="topWidth"></param>
    /// <param name="topTHK"></param>
    /// <param name="bottomWidth"></param>
    /// <param name="bottomTHK"></param>
    /// <param name="bothSides"></param>
    public WebOpeningStiffeners(Length distance, Length topWidth, Length topTHK, Length bottomWidth, Length bottomTHK, bool bothSides)
    {
      this.DistanceFrom = distance;
      this.TopStiffenerWidth = topWidth;
      this.TopStiffenerThickness = topTHK;
      this.BottomStiffenerWidth = bottomWidth;
      this.BottomStiffenerThickness = bottomTHK;
      this.isBothSides = bothSides;
      this.isNotch = false;
    }
    /// <summary>
    /// Create notch stiffener
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="topWidth"></param>
    /// <param name="topTHK"></param>
    /// <param name="bothSides"></param>
    public WebOpeningStiffeners(Length distance, Length topWidth, Length topTHK, bool bothSides)
    {
      this.isBothSides = bothSides;
      this.DistanceFrom = distance;
      this.TopStiffenerWidth = topWidth;
      this.TopStiffenerThickness = topTHK;
      this.isNotch = true;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string start = (this.DistanceFrom.Value == 0) ? "" : "d:" + this.DistanceFrom.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string top = (this.TopStiffenerWidth.Value == 0) ? "" : "Top:" + this.TopStiffenerWidth.As(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty)
          + "x" + this.TopStiffenerThickness.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string bottom = (this.BottomStiffenerWidth.Value == 0) ? "" : "Bottom:" + this.BottomStiffenerWidth.As(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty)
          + "x" + this.BottomStiffenerThickness.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);

      return string.Join(", ", start, top, bottom).Trim(' ').TrimEnd(',');
    }
    #endregion
  }
}
