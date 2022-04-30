using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.Design
{

  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class DesignCode
  {
    public enum Code
    {
      BS5950_3_1_1990_Superseeded,
      BS5950_3_1_1990_A1_2010,
      EN1994_1_1_2004,
      HKSUOS_2005,
      HKSUOS_2011,
      AS_NZS2327_2017
    }
    public enum NationalAnnex
    {
      Generic,
      United_Kingdom
    }

    public Code Design_Code { get; set; }
    public NationalAnnex National_Annex { get; set; }

    #region constructors
    public DesignCode()
    {
      this.Design_Code = Code.EN1994_1_1_2004;
      this.National_Annex = NationalAnnex.Generic;
    }
    public DesignCode(Code designcode, NationalAnnex nationalAnnex = NationalAnnex.Generic)
    {
      this.Design_Code = designcode;
      this.National_Annex = nationalAnnex;
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

    #region coa interop
    internal DesignCode(string coaString)
    {
      switch (coaString)
      {
        case "BS5950-3.1:1990 (superseded)":
          this.Design_Code = Code.BS5950_3_1_1990_Superseeded;
          break;
        case "BS5950-3.1:1990+A1:2010":
          this.Design_Code = Code.BS5950_3_1_1990_A1_2010;
          break;
        case "EN1994-1-1:2004":
          this.Design_Code = Code.EN1994_1_1_2004;
          break;
        case "HKSUOS:2005":
          this.Design_Code = Code.HKSUOS_2005;
          break;
        case "HKSUOS:2011":
          this.Design_Code = Code.HKSUOS_2011;
          break;
        case "AS/NZS2327:2017":
          this.Design_Code = Code.AS_NZS2327_2017;
          break;
      }
    }

    internal string Coa()
    {
      switch (this.Design_Code)
      {
        case Code.BS5950_3_1_1990_Superseeded:
          return "BS5950-3.1:1990 (superseded)";
        case Code.BS5950_3_1_1990_A1_2010:
          return "BS5950-3.1:1990+A1:2010";
        case Code.EN1994_1_1_2004:
          return "EN1994-1-1:2004";
        case Code.HKSUOS_2005:
          return "HKSUOS:2005";
        case Code.HKSUOS_2011:
          return "HKSUOS:2011";
        case Code.AS_NZS2327_2017:
          return "AS/NZS2327:2017";
      }
      return "";
    }
    #endregion

    #region methods

    public DesignCode Duplicate()
    {
      if (this == null) { return null; }
      DesignCode dup = (DesignCode)this.MemberwiseClone();
      return dup;
    }

    public override string ToString()
    {
      return Coa();
    }

    #endregion
  }
}
