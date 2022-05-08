using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum StudSpecType
  {
    EC4,
    BS5950,
    Other
  }

  /// <summary>
  /// Object for setting various (code dependent) specifications for a <see cref="ComposGH.Stud.Stud"/>
  /// </summary>
  public class StudSpecification : IStudSpecification
  {
    // Stud Specifications
    public bool Welding { get; set; }
    public bool NCCI { get; set; }
    public bool EC4_Limit { get; set; }
    public Length NoStudZoneStart { get; set; }
    public Length NoStudZoneEnd { get; set; }
    public Length ReinforcementPosition { get; set; }
    public StudSpecType SpecType { get; set; }

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
      this.Welding = true;
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

    internal void FromCoaString(List<string> parameters, LengthUnit lengtGeometryUnit)
    {
      //STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000
      //STUD_EC4_APPLY	MEMBER-1	YES
      //STUD_NCCI_LIMIT_APPLY	MEMBER-1	NO
      //STUD_EC4_RFT_POS	MEMBER-1	0.0300000
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      if (parameters[0] == CoaIdentifier.StudSpecifications.StudNoZone)
      {
        this.NoStudZoneStart = new Length(Convert.ToDouble(parameters[2], noComma), lengtGeometryUnit);
        this.NoStudZoneEnd = new Length(Convert.ToDouble(parameters[3], noComma), lengtGeometryUnit);
      }

      if (parameters[0] == CoaIdentifier.StudSpecifications.StudEC4)
      {
        this.EC4_Limit = parameters[2] == "YES";
        this.SpecType = StudSpecType.BS5950;
      }

      if (parameters[0] == CoaIdentifier.StudSpecifications.StudNCCI)
      {
        this.NCCI = parameters[2] == "YES";
        this.SpecType = StudSpecType.EC4;
      }

      if (parameters[0] == CoaIdentifier.StudSpecifications.StudReinfPos)
      {
        this.SpecType = StudSpecType.EC4;
        this.ReinforcementPosition = new Length(Convert.ToDouble(parameters[2], noComma), lengtGeometryUnit);
      }
    }

    #region methods
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
