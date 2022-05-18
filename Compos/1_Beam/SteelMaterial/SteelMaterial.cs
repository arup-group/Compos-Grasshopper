using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum StandardSteelGrade
  {
    S235,
    S275,
    S355,
    S450,
    S460
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
    public bool isCustom { get; set; } 
    public bool ReductionFactorMpl { get; set; } //	Apply Reduction factor to the plastic moment capacity for S420 (EN) and S460 (EN) GRADES
    public StandardSteelGrade Grade { get; set; } // standard material grade
    public WeldMaterialGrade WeldGrade { get; set; } // welding material grade

    private void SetValuesFromStandard(StandardSteelGrade grade)
    {
      this.E = new Pressure(205, PressureUnit.Gigapascal);
      this.Density = new Density(7850, DensityUnit.KilogramPerCubicMeter);
      this.Grade = grade;
      this.isCustom = false;

      switch (grade)
      {
        case StandardSteelGrade.S235:
          this.fy = new Pressure(235, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardSteelGrade.S275:
          this.fy = new Pressure(275, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardSteelGrade.S355:
          this.fy = new Pressure(355, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_42;
          break;

        case StandardSteelGrade.S450:
          this.fy = new Pressure(450, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        case StandardSteelGrade.S460:
          this.fy = new Pressure(460, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        default:
          this.isCustom = true;
          break;
      }
    }

    #region constructors
    public SteelMaterial()
    {
      // empty constructor
    }

    public SteelMaterial(Pressure fy, Pressure E, Density Density, WeldMaterialGrade weldGrade, bool isCustom, bool reductionFacorMpl)
    {
      this.fy = fy;
      this.E = E;
      this.Density = Density;
      this.isCustom = isCustom;
      this.WeldGrade = weldGrade;
      this.ReductionFactorMpl = reductionFacorMpl;
    }

    public SteelMaterial(StandardSteelGrade steelType)
    {
      SetValuesFromStandard(steelType);
    }
    #endregion

    #region coa interop
    internal static ISteelMaterial FromCoaString(List<string> parameters, ComposUnits units)
    {
      SteelMaterial material = new SteelMaterial();

      switch (parameters[0])
      {
        case (CoaIdentifier.BeamSteelMaterialStandard):
          material.isCustom = false;
          material.Grade = (StandardSteelGrade)Enum.Parse(typeof(StandardSteelGrade), parameters[2]);
          break;

        case (CoaIdentifier.BeamSteelMaterialUser):
          material.isCustom = true;
          material.fy = new Pressure(Convert.ToDouble(parameters[2]), units.Stress);
          material.E = new Pressure(Convert.ToDouble(parameters[3]), units.Stress);
          material.Density = new Density(Convert.ToDouble(parameters[4]), units.Density);
          if (parameters[5] == "TRUE")
            material.ReductionFactorMpl = true;
          else
            material.ReductionFactorMpl = false;
          break;

        case (CoaIdentifier.BeamWeldingMaterial):
          material.WeldGrade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), parameters[2].Replace(' ', '_'));
          break;

        default:
          throw new Exception("Unable to convert " + parameters + " to Compos Steel Material.");
      }
      return material;
    }

    public string ToCoaString(string name, Code code, ComposUnits units)
    {
      List<string> steelParameters = new List<string>();
      if (this.isCustom)
      {
        steelParameters.Add("BEAM_STEEL_MATERIAL_USER");
        steelParameters.Add(name);
        steelParameters.Add(CoaHelper.FormatSignificantFigures(this.fy.ToUnit(units.Stress).Value, 6));
        steelParameters.Add(CoaHelper.FormatSignificantFigures(this.E.ToUnit(units.Stress).Value, 6));
        steelParameters.Add(CoaHelper.FormatSignificantFigures(this.Density.ToUnit(units.Density).Value, 6));

        // this seems not to be working!

        //if (this.ReductionFactorMpl)
        //  steelParameters.Add("TRUE");
        //else
        //  steelParameters.Add("FALSE");
      }
      else
      {
        steelParameters.Add("BEAM_STEEL_MATERIAL_STD");
        steelParameters.Add(name);
        if (code == Code.EN1994_1_1_2004)
          steelParameters.Add(this.Grade.ToString() + " (EN)");
        else
          steelParameters.Add(this.Grade.ToString());
      }
      string coaString = CoaHelper.CreateString(steelParameters);

      List<string> weldingParameters = new List<string>();
      weldingParameters.Add("BEAM_WELDING_MATERIAL");
      weldingParameters.Add(name);
      weldingParameters.Add(this.WeldGrade.ToString().Replace('_', ' '));

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

      if (isCustom == false)
      {
        return this.Grade.ToString();
      }
      else
      {
        isCust = "Cust.";
        f = fy.ToUnit(Units.StressUnit).ToString("f0");
        e = E.ToUnit(Units.StressUnit).ToString("f0");
        ro = Density.ToUnit(Units.DensityUnit).ToString("f0");

        return (isCust.Replace(" ", string.Empty) + ", " + f.Replace(" ", string.Empty) + ", " + e.Replace(" ", string.Empty) + ", " + ro.Replace(" ", string.Empty));
      }
    }
    #endregion
  }
}
