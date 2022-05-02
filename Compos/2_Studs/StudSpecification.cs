using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Object for setting various (code dependent) specifications for a <see cref="ComposGH.Stud.Stud"/>
  /// </summary>
  public class StudSpecification
  {
    // Stud Specifications
    public bool Welding { get; set; }
    public bool NCCI { get; set; }
    public bool EC4_Limit { get; set; }
    public Length NoStudZoneStart { get; set; }
    public Length NoStudZoneEnd { get; set; }
    public Length ReinforcementPosition { get; set; }
    public enum StudSpecType
    {
      EC4,
      BS5950,
      Other
    }
    public StudSpecType SpecType;

    #region constructors
    public StudSpecification()
    {
      // empty constructor
    }
    /// <summary>
    /// for EC4 code
    /// </summary>
    /// <param name="noStudZoneStart"></param>
    /// <param name="noStudZoneEnd"></param>
    /// <param name="reinforcementPosition"></param>
    /// <param name="welding"></param>
    /// <param name="ncci"></param>
    public StudSpecification(Length noStudZoneStart, Length noStudZoneEnd, Length reinforcementPosition, bool welding, bool ncci)
    {
      this.NoStudZoneStart = noStudZoneStart;
      this.NoStudZoneEnd = noStudZoneEnd;
      this.ReinforcementPosition = reinforcementPosition;
      this.Welding = welding;
      this.NCCI = ncci;
      this.SpecType = StudSpecType.EC4;
    }
    /// <summary>
    /// for BS5950 code
    /// </summary>
    /// <param name="useEC4Limit"></param>
    /// <param name="noStudZoneStart"></param>
    /// <param name="noStudZoneEnd"></param>
    /// <param name=""></param>
    public StudSpecification(bool useEC4Limit, Length noStudZoneStart, Length noStudZoneEnd)
    {
      this.NoStudZoneStart = noStudZoneStart;
      this.NoStudZoneEnd = noStudZoneEnd;
      this.EC4_Limit = useEC4Limit;
      this.SpecType = StudSpecType.BS5950;
    }
    /// <summary>
    /// for codes: AS/NZ, HK, 
    /// </summary>
    /// <param name="noStudZoneStart"></param>
    /// <param name="noStudZoneEnd"></param>
    /// <param name="welding"></param>
    public StudSpecification(Length noStudZoneStart, Length noStudZoneEnd, bool welding)
    {
      this.NoStudZoneStart = noStudZoneStart;
      this.NoStudZoneEnd = noStudZoneEnd;
      this.Welding = welding;
      this.SpecType = StudSpecType.Other;
    }

    #endregion

    #region methods

    public StudSpecification Duplicate()
    {
      if (this == null) { return null; }
      StudSpecification dup = (StudSpecification)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string noStudStart = (this.NoStudZoneStart.Value == 0) ? "" : "NSZS:" + this.NoStudZoneStart.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      string noStudEnd = (this.NoStudZoneEnd.Value == 0) ? "" : "NSZE:" + this.NoStudZoneEnd.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      string rebarPos = (this.ReinforcementPosition.Value == 0) ? "" : "RbP:" + this.ReinforcementPosition.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
      string welding = (this.Welding == true) ? "Welded" : "";
      string ncci = (this.NCCI == true) ? "NCCI Limit" : "";
      string ec4 = (this.EC4_Limit == true) ? "EC4 Limit" : "";
      string joined = string.Join(" ", new List<string>() { noStudStart, noStudEnd, rebarPos, welding, ncci, ec4 });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
