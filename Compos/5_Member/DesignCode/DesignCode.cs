using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public enum Code
  {
    BS5950_3_1_1990_Superseded = 0,
    BS5950_3_1_1990_A1_2010 = 1,
    EN1994_1_1_2004 = 2,
    HKSUOS_2005 = 3,
    HKSUOS_2011 = 4,
    AS_NZS2327_2017 = 5
  }
  public enum NationalAnnex
  {
    Generic,
    United_Kingdom
  }

  /// <summary>
  /// Use this class to create a DesignCode. Use inheriting <see cref="EN1994"/> or <see cref="ASNZS2327"/> specifically for those codes respectively.
  /// </summary>
  public class DesignCode : IDesignCode
  {
    public Code Code { get; set; }
    public IDesignOptions DesignOptions { get; set; } = new DesignOptions();
    public ISafetyFactors SafetyFactors { get; set; } = new SafetyFactors();
    public DesignCode() { }
    public DesignCode(Code designcode)
    {
      this.Code = designcode;
      if (designcode == Code.EN1994_1_1_2004)
        throw new Exception("Must use the EN1994 class to create a EN 1994-1-1:2004 DesignCode");
      if (designcode == Code.AS_NZS2327_2017)
        throw new Exception("Must use the ASNZS2327 class to create a AS/NZS2327:2017 DesignCode");
    }

    public override string ToString()
    {
      return Coa();
    }

    #region coa interop
    internal DesignCode FromCoa(string coaString)
    {
      switch (coaString)
      {
        case "BS5950-3.1:1990 (superseded)":
          return new DesignCode(Code.BS5950_3_1_1990_Superseded);
        case "BS5950-3.1:1990+A1:2010":
          return new DesignCode(Code.BS5950_3_1_1990_A1_2010);
        case "EN1994-1-1:2004":
          return new EN1994();
        case "HKSUOS:2005":
          return new DesignCode(Code.HKSUOS_2005);
        case "HKSUOS:2011":
          return new DesignCode(Code.HKSUOS_2011);
        case "AS/NZS2327:2017":
          return new ASNZS2327();
        default:
          return null;
      }
    }

    internal string Coa()
    {
      switch (this.Code)
      {
        case Code.BS5950_3_1_1990_Superseded:
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
  }

  /// <summary>
  /// <see cref="DesignCode"/> inherit class specific to EN 1994-1-1:2004
  /// </summary>
  public class EN1994 : DesignCode
  {
    public NationalAnnex NationalAnnex { get; set; } = NationalAnnex.Generic;
    public EC4Options CodeOptions { get; set; } = new EC4Options();
    public new EC4SafetyFactors SafetyFactors { get; set; } = new EC4SafetyFactors();
    public EN1994()
    {
      this.Code = Code.EN1994_1_1_2004;
    }
  }

  /// <summary>
  /// <see cref="DesignCode"/> inherit class specific to AS/NZS2327:2017
  /// </summary>
  public class ASNZS2327 : DesignCode
  {
    public CodeOptions CodeOptions { get; set; } = new CodeOptions();
    public ASNZS2327()
    {
      this.Code = Code.AS_NZS2327_2017;
    }
  }
}
