using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.Member
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

  /// <summary>
  /// Use this class to create a DesignCode. Use inheriting <see cref="EN1994"/> or <see cref="ASNZS2327"/> specifically for those codes respectively.
  /// </summary>
  public class DesignCode
  {
    public Code Code { get; set; }
    public DesignOptions DesignOptions { get; set; } = new DesignOptions();
    public SafetyFactors SafetyFactors { get; set; } = new SafetyFactors();
    public DesignCode() { }
    public DesignCode(Code designcode)
    {
      this.Code = designcode;
      if (designcode == Code.EN1994_1_1_2004)
        throw new Exception("Must use the EN1994 class to create a EN 1994-1-1:2004 DesignCode");
      if (designcode == Code.AS_NZS2327_2017)
        throw new Exception("Must use the ASNZS2327 class to create a AS/NZS2327:2017 DesignCode");
    }
    public virtual DesignCode Duplicate()
    {
      if (this == null) { return null; }
      DesignCode dup = (DesignCode)this.MemberwiseClone();
      dup.DesignOptions = this.DesignOptions.Duplicate();
      dup.SafetyFactors = this.SafetyFactors.Duplicate();
      return dup;
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
          return new DesignCode(Code.BS5950_3_1_1990_Superseeded);
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

    
  }

  /// <summary>
  /// <see cref="DesignCode"/> inherit class specific to EN 1994-1-1:2004
  /// </summary>
  public class EN1994 : DesignCode
  {
    public NationalAnnex NationalAnnex { get; set; } = NationalAnnex.Generic;
    public EN1994Options CodeOptions { get; set; } = new EN1994Options();
    public new EC4SafetyFactors SafetyFactors { get; set; } = new EC4SafetyFactors();
    public EN1994()
    {
      this.Code = Code.EN1994_1_1_2004;
    }
    public override DesignCode Duplicate()
    {
      if (this == null) { return null; }
      EN1994 dup = (EN1994)this.MemberwiseClone();
      dup.DesignOptions = this.DesignOptions.Duplicate();
      dup.CodeOptions = this.CodeOptions.Duplicate();
      dup.SafetyFactors = this.SafetyFactors.Duplicate();
      return dup;
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
      this.Code = Code.EN1994_1_1_2004;
    }
    public override DesignCode Duplicate()
    {
      if (this == null) { return null; }
      ASNZS2327 dup = (ASNZS2327)this.MemberwiseClone();
      dup.DesignOptions = this.DesignOptions.Duplicate();
      dup.CodeOptions = this.CodeOptions.Duplicate();
      dup.SafetyFactors = this.SafetyFactors.Duplicate();
      return dup;
    }
  }
}
