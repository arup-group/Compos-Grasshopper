using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Object for setting custom spacing/layout for a <see cref="ComposGH.Stud.Stud"/>
  /// </summary>
  public class StudGroupSpacing
  {
    public Length DistanceFromStart { get; set; }
    public int NumberOfRows { get; set; } = 2;
    public int NumberOfLines { get; set; } = 1;
    public Length Spacing { get; set; }

    // Stud spacing
    public enum StudSpacingType
    {
      Automatic,
      Partial_Interaction,
      Min_Num_of_Studs,
      Custom
    }
    #region constructors
    public StudGroupSpacing()
    {
      // empty constructor
    }

    public StudGroupSpacing(Length distanceFromStart, int numberOfRows, int numberOfLines, Length spacing)
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

    #region methods
    public override string ToString()
    {
      string start = (this.DistanceFromStart.Value == 0) ? "" : "From:" + this.DistanceFromStart.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      string rows = NumberOfRows + "R";
      string lines = NumberOfLines + "L";
      string spacing = "@" + this.Spacing.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { start, rows, lines, spacing });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
