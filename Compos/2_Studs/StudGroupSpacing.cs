using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OasysUnitsNet;
using OasysUnitsNet.Units;

namespace ComposAPI
{
  public enum StudSpacingType
  {
    Automatic,
    Partial_Interaction,
    Min_Num_of_Studs,
    Custom
  }

  /// <summary>
  /// Object for setting custom spacing/layout for a <see cref="Stud"/>
  /// </summary>
  public class StudGroupSpacing : IStudGroupSpacing
  {
    public IQuantity DistanceFromStart
    {
      get { return this.m_StartPosition; }
      set
      {
        if (value == null) return;
        if (value.QuantityInfo.UnitType != typeof(LengthUnit)
          & value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("Start Position must be either Length or Ratio");
        else
          this.m_StartPosition = value;
      }
    }
    private IQuantity m_StartPosition = Length.Zero;
    public int NumberOfRows { get; set; } = 2;
    public int NumberOfLines { get; set; } = 1;
    public Length Spacing { get; set; }

    #region constructors
    public StudGroupSpacing()
    {
      // empty constructor
    }

    public StudGroupSpacing(IQuantity distanceFromStart, int numberOfRows, int numberOfLines, Length spacing)
    {
      this.DistanceFromStart = distanceFromStart;
      if (numberOfRows < 1)
        throw new ArgumentException("Number of rows must be bigger or equal to 1");
      this.NumberOfRows = numberOfRows;
      if (numberOfLines < 1)
        throw new ArgumentException("Number of lines must be bigger or equal to 1");
      this.NumberOfLines = numberOfLines;
      this.Spacing = spacing;
    }

    #endregion

    internal static StudGroupSpacing FromCoaString(List<string> parameters, ComposUnits units)
    {
      //STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	1	0.000000	2	1	0.0760000	0.0950000	0.150000	CHECK_SPACE_NO
      //STUD_LAYOUT	MEMBER-1	USER_DEFINED	2	1	0.000000	2	1	0.0570000	0.0950000	0.150000	CHECK_SPACE_NO
      //STUD_LAYOUT MEMBER-1 USER_DEFINED 2 2 8.000000 3 2 0.0570000 0.0950000 0.250000 CHECK_SPACE_NO
      StudGroupSpacing groupSpacing = new StudGroupSpacing();
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      groupSpacing.DistanceFromStart = CoaHelper.ConvertToLengthOrRatio(parameters[5], units.Length);
      groupSpacing.NumberOfRows = Convert.ToInt32(parameters[6], noComma);
      groupSpacing.NumberOfLines = Convert.ToInt32(parameters[7], noComma);
      groupSpacing.Spacing = new Length(Convert.ToDouble(parameters[10], noComma), units.Length);

      return groupSpacing;
    }

    #region methods
    public override string ToString()
    {
      string start = "";
      if (this.DistanceFromStart.QuantityInfo.UnitType == typeof(LengthUnit))
      {
        Length l = (Length)this.DistanceFromStart;
        if (l != Length.Zero)
          start = "From:" + l.ToString("g2").Replace(" ", string.Empty);
      }
      else
      {
        Ratio p = (Ratio)this.DistanceFromStart;
        if (p != Ratio.Zero)
          start = "From:" + p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
      }
      string rows = NumberOfRows + "R";
      string lines = NumberOfLines + "L";
      string spacing = "@" + this.Spacing.ToUnit(UnitsHelper.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { start, rows, lines, spacing });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
