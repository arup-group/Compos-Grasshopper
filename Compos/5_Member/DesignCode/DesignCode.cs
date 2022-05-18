using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
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

    #region coa interop
    internal static DesignCode FromCoaString(List<string> parameters)
    {
      DesignCode dc = new DesignCode();
      switch (parameters[2])
      {
        case "BS5950-3.1:1990 (superseded)":
          dc = new DesignCode(Code.BS5950_3_1_1990_Superseded);
          break;
        case "BS5950-3.1:1990+A1:2010":
          dc = new DesignCode(Code.BS5950_3_1_1990_A1_2010);
          break;
        case "EN1994-1-1:2004":
          dc = new EN1994();
          break;
        case "HKSUOS:2005":
          dc = new DesignCode(Code.HKSUOS_2005);
          break;
        case "HKSUOS:2011":
          dc = new DesignCode(Code.HKSUOS_2011);
          break;
        case "AS/NZS2327:2017":
          dc = new ASNZS2327();
          break;
        default:
          dc = null;
          break;
      }
      DesignOptions designOptions = new DesignOptions();
      designOptions.ProppedDuringConstruction = parameters[3] != "UNPROPPED";
      designOptions.InclSteelBeamWeight = parameters[4] != "BEAM_WEIGHT_NO";
      designOptions.InclConcreteSlabWeight = parameters[5] != "SLAB_WEIGHT_NO";
      designOptions.ConsiderShearDeflection = parameters[6] != "SHEAR_DEFORM_NO";
      designOptions.InclThinFlangeSections = parameters[7] != "THIN_SECTION_NO";

      dc.DesignOptions = designOptions;

      if (dc.Code == Code.AS_NZS2327_2017)
      {
        NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
        CodeOptions codeOptions= new CodeOptions();
        CreepShrinkageParameters longterm = new CreepShrinkageParameters() { CreepCoefficient = Convert.ToDouble(parameters[8], noComma) };
        codeOptions.LongTerm = longterm;
        CreepShrinkageParameters shrinkage = new CreepShrinkageParameters() { CreepCoefficient = Convert.ToDouble(parameters[9], noComma) };
        codeOptions.ShortTerm = shrinkage;
        ASNZS2327 aSNZS = (ASNZS2327)dc;
        aSNZS.CodeOptions = codeOptions;
        return aSNZS;
      }
      return dc;
    }

    public string ToCoaString(string name)
    {
      string str = CoaIdentifier.DesignCode + '\t' + name + '\t';
      switch (this.Code)
      {
        case Code.BS5950_3_1_1990_Superseded:
          str += "BS5950-3.1:1990 (superseded)" + '\t';
          break;

        case Code.BS5950_3_1_1990_A1_2010:
          str += "BS5950-3.1:1990+A1:2010" + '\t';
          break;

        case Code.EN1994_1_1_2004:
          str += "EN1994-1-1:2004" + '\t';
          break;

        case Code.HKSUOS_2005:
          str += "HKSUOS:2005" + '\t';
          break;

        case Code.HKSUOS_2011:
          str += "HKSUOS:2011" + '\t';
          break;

        case Code.AS_NZS2327_2017:
          str += "AS/NZS2327:2017" + '\t';
          break;

        default:
          throw new Exception("Code not recognised");
      }
      str += ((this.DesignOptions.ProppedDuringConstruction) ? "PROPPED" : "UNPROPPED") + '\t';
      str += ((this.DesignOptions.InclSteelBeamWeight) ? "BEAM_WEIGHT_YES" : "BEAM_WEIGHT_NO") + '\t';
      str += ((this.DesignOptions.InclConcreteSlabWeight) ? "SLAB_WEIGHT_YES" : "SLAB_WEIGHT_NO") + '\t';
      str += ((this.DesignOptions.ConsiderShearDeflection) ? "SHEAR_DEFORM_YES" : "SHEAR_DEFORM_NO") + '\t';
      str += ((this.DesignOptions.InclThinFlangeSections) ? "THIN_SECTION_YES" : "THIN_SECTION_NO") + '\t';

      if (this.Code == Code.AS_NZS2327_2017)
      {
        ASNZS2327 aSNZS = (ASNZS2327)this;
        str += CoaHelper.FormatSignificantFigures(aSNZS.CodeOptions.LongTerm.CreepCoefficient, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(aSNZS.CodeOptions.ShortTerm.CreepCoefficient, 6) + '\n';
      }
      else
      {
        str += CoaHelper.FormatSignificantFigures(2.0, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(2.0, 6) + '\n';
      }

      return str;
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
