using ComposAPI.Helpers;
using OasysUnits;

namespace ComposAPI
{
  /// <summary>
  /// Define Stiffener Plate information used for a <see cref="WebOpening"/>.
  /// </summary>
  public class WebOpeningStiffeners : IWebOpeningStiffeners
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
      DistanceFrom = distance;
      TopStiffenerWidth = topWidth;
      TopStiffenerThickness = topTHK;
      BottomStiffenerWidth = bottomWidth;
      BottomStiffenerThickness = bottomTHK;
      isBothSides = bothSides;
      isNotch = false;
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
      isBothSides = bothSides;
      DistanceFrom = distance;
      TopStiffenerWidth = topWidth;
      TopStiffenerThickness = topTHK;
      isNotch = true;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string start = (DistanceFrom.Value == 0) ? "" : "d:" + DistanceFrom.ToUnit(ComposUnitsHelper.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string top = (TopStiffenerWidth.Value == 0) ? "" : "Top:" + TopStiffenerWidth.As(ComposUnitsHelper.LengthUnitSection).ToString("f0").Replace(" ", string.Empty)
          + "x" + TopStiffenerThickness.ToUnit(ComposUnitsHelper.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string bottom = (BottomStiffenerWidth.Value == 0) ? "" : "Bottom:" + BottomStiffenerWidth.As(ComposUnitsHelper.LengthUnitSection).ToString("f0").Replace(" ", string.Empty)
          + "x" + BottomStiffenerThickness.ToUnit(ComposUnitsHelper.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);

      return string.Join(", ", start, top, bottom).Trim(' ').TrimEnd(',');
    }
    #endregion
  }
}
