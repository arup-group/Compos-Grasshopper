using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.ConcreteSlab
{
  public class TransverseReinforcement
  {
    public Length DistanceFromStart { get; set; }
    public Length DistanceFromEnd { get; set; }
    public Length Diameter { get; set; }
    public Length Spacing { get; set; }
    public Length Cover { get; set; }
    public ReinforcementMaterial Material { get; set; }

    // Rebar spacing
    public enum RebarSpacingType
    {
      Automatic,
      Custom
    }
    #region constructors
    public TransverseReinforcement()
    {
      // empty constructor
    }

    public TransverseReinforcement(ReinforcementMaterial material, Length distanceFromStart, Length distanceFromEnd, Length diameter, Length spacing, Length cover)
    {
      this.Material = material;
      this.DistanceFromStart = distanceFromStart;
      this.DistanceFromEnd = distanceFromEnd;
      this.Diameter = diameter;
      this.Spacing = spacing;
      this.Cover = cover;
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

    public TransverseReinforcement Duplicate()
    {
      if (this == null) { return null; }
      TransverseReinforcement dup = (TransverseReinforcement)this.MemberwiseClone();
      dup.Material = this.Material.Duplicate();
      return dup;
    }
    public override string ToString()
    {
      string start = (this.DistanceFromStart.Value == 0) ? "" : this.DistanceFromStart.ToUnit(Helpers.Units.FileUnits.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty) + "<-";
      string end = (this.DistanceFromEnd.Value == 0) ? "" : "->" + this.DistanceFromEnd.ToUnit(Helpers.Units.FileUnits.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty);
      string startend = start + end;
      startend = startend.Replace("--", "-").Replace(",", string.Empty);
      string mat = this.Material.ToString();
      string dia = "Ø" + this.Diameter.ToUnit(Helpers.Units.FileUnits.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string spacing = "/" + this.Spacing.ToUnit(Helpers.Units.FileUnits.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string cov = ", c:" + this.Cover.ToUnit(Helpers.Units.FileUnits.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
      string diaspacingcov = dia + spacing + cov;

      string joined = string.Join(" ", new List<string>() { startend, mat, diaspacingcov });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
