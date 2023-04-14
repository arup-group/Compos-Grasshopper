using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  public enum StandardSteelGrade
  {
    S235 = 235,
    S275 = 275,
    S355 = 355,
    S450 = 450,
    S460 = 460
  }

  public enum WeldMaterialGrade
  {
    None,
    Grade_35,
    Grade_42,
    Grade_50
  }


  /// <summary>
  /// Steel Material for a <see cref="Beam"/>. Contains information about strength, density and Young's Modulus, as well as grade.
  /// </summary>
  public class SteelMaterial : ISteelMaterial
  {
    public Pressure fy { get; set; } //	characteristic strength
    public Pressure E { get; set; } //	Young's modulus
    public Density Density { get; set; } //	material density
    public bool IsCustom { get; set; }
    public bool ReductionFactorMpl { get; set; } //	Apply Reduction factor to the plastic moment capacity for S420 (EN) and S460 (EN) GRADES
    public StandardSteelGrade Grade { get; set; } // standard material grade
    public WeldMaterialGrade WeldGrade { get; set; } // welding material grade

    #region constructors
    public SteelMaterial()
    {
      // empty constructor
    }

    public SteelMaterial(Pressure fy, Pressure E, Density density, WeldMaterialGrade weldGrade, bool isCustom, bool reductionFacorMpl)
    {
      fy = fy;
      E = E;
      Density = density;
      IsCustom = isCustom;
      WeldGrade = weldGrade;
      ReductionFactorMpl = reductionFacorMpl;
    }

    public SteelMaterial(StandardSteelGrade grade, Code code)
    {
      bool EN = (code == Code.EN1994_1_1_2004);
      SetValuesFromStandard(grade, EN);
    }

    internal void SetValuesFromStandard(StandardSteelGrade grade, bool EN)
    {
      E = new Pressure(EN ? 210 : 205, PressureUnit.Gigapascal);
      Density = new Density(7850, DensityUnit.KilogramPerCubicMeter);
      Grade = grade;
      IsCustom = false;

      switch (grade)
      {
        case StandardSteelGrade.S235:
          fy = new Pressure(235, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardSteelGrade.S275:
          fy = new Pressure(275, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardSteelGrade.S355:
          fy = new Pressure(355, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_42;
          break;

        case StandardSteelGrade.S450:
          fy = new Pressure(450, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        case StandardSteelGrade.S460:
          fy = new Pressure(460, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        default:
          throw new Exception("Unrecognised StandardSteelGrade Enum");
      }
    }
    #endregion

    #region coa interop
    internal static ISteelMaterial FromCoaString(List<string> parameters, ComposUnits units, Code code)
    {
      SteelMaterial material;
      if (code != Code.AS_NZS2327_2017)
        material = new SteelMaterial();
      else
        material = new ASNZSteelMaterial();
      
      switch (parameters[0])
      {
        case (CoaIdentifier.BeamSteelMaterialStandard):
          string grade = parameters[2];

          if (code == Code.AS_NZS2327_2017)
          {
            StandardASNZSteelMaterialGrade standardASNZSteelMaterialGrade = ASNZSteelMaterial.FromString(grade);
            ((ASNZSteelMaterial)material).SetValuesFromStandard(standardASNZSteelMaterialGrade);
          }
          else
          {
            bool EN = grade.EndsWith(" (EN)");
            if (EN)
              grade = grade.Replace(" (EN)", string.Empty);
            StandardSteelGrade standardSteelGrade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), grade);
            material.SetValuesFromStandard(standardSteelGrade, EN);
          }
          break;

        case (CoaIdentifier.BeamSteelMaterialUser):
          material.IsCustom = true;
          material.fy = CoaHelper.ConvertToStress(parameters[2], units.Stress);
          material.E = CoaHelper.ConvertToStress(parameters[3], units.Stress);
          material.Density = CoaHelper.ConvertToDensity(parameters[4], units.Density);
          if (parameters.Count > 5 && parameters[5] == "TRUE")
            material.ReductionFactorMpl = true;
          else
            material.ReductionFactorMpl = false;
          break;

        default:
          throw new Exception("Unable to convert " + parameters + " to Compos Steel Material.");
      }
      return material;
    }
    internal static WeldMaterialGrade WeldGradeFromCoa(List<string> parameters)
    {
      return (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), parameters[2].Replace(' ', '_'));
    }

    public virtual string ToCoaString(string name, Code code, ComposUnits units)
    {
      List<string> steelParameters = new List<string>();
      if (IsCustom)
      {
        steelParameters.Add("BEAM_STEEL_MATERIAL_USER");
        steelParameters.Add(name);
        steelParameters.Add(CoaHelper.FormatSignificantFigures(fy.ToUnit(units.Stress).Value, 6));
        steelParameters.Add(CoaHelper.FormatSignificantFigures(E.ToUnit(units.Stress).Value, 6));
        steelParameters.Add(CoaHelper.FormatSignificantFigures(Density.ToUnit(units.Density).Value, 6));

        if (code == Code.EN1994_1_1_2004)
        {
          if (ReductionFactorMpl)
            steelParameters.Add("TRUE");
          else
            steelParameters.Add("FALSE");
        }
      }
      else
      {
        steelParameters.Add("BEAM_STEEL_MATERIAL_STD");
        steelParameters.Add(name);
        if (code == Code.EN1994_1_1_2004)
          steelParameters.Add(Grade.ToString() + " (EN)");
        else
          steelParameters.Add(Grade.ToString());
      }
      string coaString = CoaHelper.CreateString(steelParameters);

      List<string> weldingParameters = new List<string>();
      weldingParameters.Add("BEAM_WELDING_MATERIAL");
      weldingParameters.Add(name);
      weldingParameters.Add(WeldGrade.ToString().Replace('_', ' '));
      coaString += CoaHelper.CreateString(weldingParameters);

      return coaString;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string isCust = string.Empty;
      string f = string.Empty;
      string e = string.Empty;
      string ro = string.Empty;
      //string wMat = (WeldGrade == WeldMaterialGrade.None) ? "" : WeldGrade.ToString();

      if (IsCustom == false)
      {
        return Grade.ToString();
      }
      else
      {
        isCust = "Cust.";
        f = fy.ToUnit(ComposUnitsHelper.StressUnit).ToString("f0");
        e = E.ToUnit(ComposUnitsHelper.StressUnit).ToString("f0");
        ro = Density.ToUnit(ComposUnitsHelper.DensityUnit).ToString("f0");

        return (isCust.Replace(" ", string.Empty) + ", " + f.Replace(" ", string.Empty) + ", " + e.Replace(" ", string.Empty) + ", " + ro.Replace(" ", string.Empty));
      }
    }
    #endregion
  }
}
