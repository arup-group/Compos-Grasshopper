using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OasysUnitsNet;

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

  /// <summary>
  /// Use this class to create a DesignCode. Use inheriting <see cref="EN1994"/> or <see cref="ASNZS2327"/> specifically for those codes respectively.
  /// </summary>
  public class DesignCode : IDesignCode
  {
    public Code Code { get; set; }
    public IDesignOption DesignOption { get; set; } = new DesignOption();
    public ISafetyFactors SafetyFactors { get; set; } = new SafetyFactors();

    public DesignCode() { }

    public DesignCode(Code designcode)
    {
      if (designcode == Code.EN1994_1_1_2004)
        throw new Exception("Must use the EN1994 class to create a EN 1994-1-1:2004 DesignCode");
      if (designcode == Code.AS_NZS2327_2017)
        throw new Exception("Must use the ASNZS2327 class to create a AS/NZS2327:2017 DesignCode");
      this.Code = designcode;
    }

    #region coa interop
    internal static IDesignCode FromCoaString(string coaString, string name, ComposUnits units)
    {
      DesignCode designCode = new DesignCode();
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return designCode;

        if (parameters[0] == CoaIdentifier.UnitData)
          units.FromCoaString(parameters);

        if (parameters[1] != name)
          continue;

        switch (parameters[0])
        {
          case (CoaIdentifier.DesignOption):
            switch (parameters[2])
            {
              case CoaIdentifier.DesignCode.BS_Superseded:
                designCode = new DesignCode(Code.BS5950_3_1_1990_Superseded);
                break;

              case CoaIdentifier.DesignCode.BS:
                designCode = new DesignCode(Code.BS5950_3_1_1990_A1_2010);
                break;

              case CoaIdentifier.DesignCode.EN:
                designCode = new EN1994();
                EN1994 enCode = (EN1994)designCode;
                enCode.SafetyFactors = SafetyFactorsEN.FromCoaString(coaString, name);
                break;

              case CoaIdentifier.DesignCode.HKSUOS2005:
                designCode = new DesignCode(Code.HKSUOS_2005);
                break;

              case CoaIdentifier.DesignCode.HKSUOS2011:
                designCode = new DesignCode(Code.HKSUOS_2011);
                break;

              case CoaIdentifier.DesignCode.ASNZ:
                designCode = new ASNZS2327();
                // because the coa string for "DESIGN_OPTION" includes two values in the end
                // only for ASNZ code creep multipliers this is included here
                CodeOptionsASNZ codeOptionsASNZ = new CodeOptionsASNZ();
                CreepShrinkageParametersASNZ longterm = new CreepShrinkageParametersASNZ() { CreepCoefficient = Convert.ToDouble(parameters[8], noComma) };
                codeOptionsASNZ.LongTerm = longterm;
                CreepShrinkageParametersASNZ shrinkage = new CreepShrinkageParametersASNZ() { CreepCoefficient = Convert.ToDouble(parameters[9], noComma) };
                codeOptionsASNZ.ShortTerm = shrinkage;
                ASNZS2327 aSNZS = (ASNZS2327)designCode;
                aSNZS.CodeOptions = codeOptionsASNZ;
                break;

              default:
                designCode = null;
                break;
            }
            DesignOption designOption = new DesignOption();
            designOption.ProppedDuringConstruction = parameters[3] != "UNPROPPED";
            designOption.InclSteelBeamWeight = parameters[4] != "BEAM_WEIGHT_NO";
            designOption.InclConcreteSlabWeight = parameters[5] != "SLAB_WEIGHT_NO";
            designOption.ConsiderShearDeflection = parameters[6] != "SHEAR_DEFORM_NO";
            designOption.InclThinFlangeSections = parameters[7] != "THIN_SECTION_NO";

            designCode.DesignOption = designOption;

            break;
          
          case (CoaIdentifier.EC4DesignOption):
            EN1994 en = (EN1994)designCode;
            en.CodeOptions = CodeOptionsEN.FromCoaString(parameters);
            if (parameters[5].ToUpper() == "UNITED KINGDOM")
              en.NationalAnnex = NationalAnnex.United_Kingdom;
            else
              en.NationalAnnex = NationalAnnex.Generic;
            designCode = en;
            break;

          case (CoaIdentifier.SafetyFactorLoad):
            if (designCode.Code == Code.EN1994_1_1_2004) { break; } // safety factor for EN handele in switch case for DesignCode above
            else
            {
              SafetyFactors sf_load = (SafetyFactors)designCode.SafetyFactors;
              sf_load.LoadFactors = (LoadFactors)LoadFactors.FromCoaString(parameters);
              designCode.SafetyFactors = sf_load;
            }
            break;

          case (CoaIdentifier.SafetyFactorMaterial):
            if (designCode.Code == Code.EN1994_1_1_2004) { break; } // safety factor for EN handele in switch case for DesignCode above
            else
            {
              SafetyFactors sf_mat = (SafetyFactors)designCode.SafetyFactors;
              sf_mat.MaterialFactors = MaterialFactors.FromCoaString(parameters);
              designCode.SafetyFactors = sf_mat;
            }
            break;

          default:
            // continue;
            break;
        }
      }
      return designCode;
    }

    public virtual string ToCoaString(string name)
    {
      string str = CoaIdentifier.DesignOption + '\t' + name + '\t';
      switch (this.Code)
      {
        case Code.BS5950_3_1_1990_Superseded:
          str += CoaIdentifier.DesignCode.BS_Superseded + '\t';
          break;

        case Code.BS5950_3_1_1990_A1_2010:
          str += CoaIdentifier.DesignCode.BS + '\t';
          break;

        case Code.EN1994_1_1_2004:
          str += CoaIdentifier.DesignCode.EN + '\t';
          break;

        case Code.HKSUOS_2005:
          str += CoaIdentifier.DesignCode.HKSUOS2005 + '\t';
          break;

        case Code.HKSUOS_2011:
          str += CoaIdentifier.DesignCode.HKSUOS2011 + '\t';
          break;

        case Code.AS_NZS2327_2017:
          str += CoaIdentifier.DesignCode.ASNZ + '\t';
          break;

        default:
          throw new Exception("Code not recognised");
      }
      str += ((this.DesignOption.ProppedDuringConstruction) ? "PROPPED" : "UNPROPPED") + '\t';
      str += ((this.DesignOption.InclSteelBeamWeight) ? "BEAM_WEIGHT_YES" : "BEAM_WEIGHT_NO") + '\t';
      str += ((this.DesignOption.InclConcreteSlabWeight) ? "SLAB_WEIGHT_YES" : "SLAB_WEIGHT_NO") + '\t';
      str += ((this.DesignOption.ConsiderShearDeflection) ? "SHEAR_DEFORM_YES" : "SHEAR_DEFORM_NO") + '\t';
      str += ((this.DesignOption.InclThinFlangeSections) ? "THIN_SECTION_YES" : "THIN_SECTION_NO") + '\t';

      if (this.Code == Code.AS_NZS2327_2017)
      {
        // because the coa string for "DESIGN_OPTION" includes two values in the end
        // only for ASNZ code creep multipliers this is included here
        ASNZS2327 aSNZS = (ASNZS2327)this;
        str += CoaHelper.FormatSignificantFigures(aSNZS.CodeOptions.LongTerm.CreepCoefficient, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(aSNZS.CodeOptions.ShortTerm.CreepCoefficient, 6) + '\n';
      }
      else
      {
        str += CoaHelper.FormatSignificantFigures(2.0, 6) + '\t';
        str += CoaHelper.FormatSignificantFigures(2.0, 6) + '\n';
      }

      str += this.SafetyFactors.ToCoaString(name);

      return str;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      switch (this.Code)
      {
        case Code.BS5950_3_1_1990_Superseded:
          return CoaIdentifier.DesignCode.BS_Superseded;
        case Code.BS5950_3_1_1990_A1_2010:
          return CoaIdentifier.DesignCode.BS;
        case Code.EN1994_1_1_2004:
          return CoaIdentifier.DesignCode.EN;
        case Code.HKSUOS_2005:
          return CoaIdentifier.DesignCode.HKSUOS2005;
        case Code.HKSUOS_2011:
          return CoaIdentifier.DesignCode.HKSUOS2011;
        case Code.AS_NZS2327_2017:
          return CoaIdentifier.DesignCode.ASNZ;
      }
      return "";
    }
    #endregion
  }
}
