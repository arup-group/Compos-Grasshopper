using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  /// <summary>
  /// Steel Material for a <see cref="Beam"/>. Contains information about strength, density and Young's Modulus, as well as grade.
  /// </summary>
  public class SteelMaterial
  {
    public enum SteelMaterialGrade
    {
      S235,
      S275,
      S355,
      S450,
      S460
    }

    public enum WeldMaterialGrade
    {
      //None,
      Grade_35,
      Grade_42,
      Grade_50
    }

    public Pressure fy { get; set; } //	characteristic strength
    public Pressure E { get; set; } //	Young's modulus
    public Density Density { get; set; } //	material density
    public bool isCustom { get; set; }
    public bool ReductionFactorMpl { get; set; } //	Apply Reduction factor to the plastic moment capacity for S420 (EN) and S460 (EN) GRADES
    public SteelMaterialGrade Grade { get; set; }
    public WeldMaterialGrade WeldGrade { get; set; }

    private void SetValuesFromStandard(SteelMaterialGrade grade)
    {
      this.E = new Pressure(205, UnitsNet.Units.PressureUnit.Gigapascal);
      this.Density = new Density(7850, UnitsNet.Units.DensityUnit.KilogramPerCubicMeter);
      this.Grade = grade;
      this.isCustom = false;

      switch (grade)
      {
        case SteelMaterialGrade.S235:
          this.fy = new Pressure(235, UnitsNet.Units.PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case SteelMaterialGrade.S275:
          this.fy = new Pressure(275, UnitsNet.Units.PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case SteelMaterialGrade.S355:
          this.fy = new Pressure(355, UnitsNet.Units.PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_42;
          break;

        case SteelMaterialGrade.S450:
          this.fy = new Pressure(450, UnitsNet.Units.PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        case SteelMaterialGrade.S460:
          this.fy = new Pressure(460, UnitsNet.Units.PressureUnit.Megapascal);
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

    public SteelMaterial(SteelMaterialGrade steelType)
    {
      SetValuesFromStandard(steelType);
    }
    #endregion

    #region coa interop
    internal SteelMaterial(List<string> parameters, DensityUnit densityUnit, PressureUnit pressureUnit)
    {
      switch (parameters[0])
      {
        case ("BEAM_STEEL_MATERIAL_STD"):
          this.isCustom = false;
          this.Grade = (SteelMaterialGrade)Enum.Parse(typeof(SteelMaterialGrade), parameters[2]);
          break;

        case ("BEAM_STEEL_MATERIAL_USER"):
          this.isCustom = true;
          this.fy = new Pressure(Convert.ToDouble(parameters[2]), pressureUnit);
          this.E = new Pressure(Convert.ToDouble(parameters[3]), pressureUnit);
          this.Density = new Density(Convert.ToDouble(parameters[4]), densityUnit);
          if (parameters[5] == "TRUE")
            this.ReductionFactorMpl = true;
          else
            this.ReductionFactorMpl = false;
          break;

        case ("BEAM_WELDING_MATERIAL"):
          this.WeldGrade = (WeldMaterialGrade)Enum.Parse(typeof(WeldMaterialGrade), parameters[2].Replace(' ', '_'));
          break;

        default:
          throw new Exception("Unable to convert " + parameters + " to Compos Steel Material.");
      }
    }

    internal string ToCoaString(string name, DensityUnit densityUnit, PressureUnit pressureUnit)
    {
      List<string> parameters = new List<string>();
      if (this.isCustom)
      {
        parameters.Add("BEAM_STEEL_MATERIAL_USER");
        parameters.Add(name);
        parameters.Add(this.fy.ToUnit(pressureUnit).ToString());
        parameters.Add(this.E.ToUnit(pressureUnit).ToString());
        parameters.Add(this.Density.ToUnit(densityUnit).ToString());
        if (this.ReductionFactorMpl)
          parameters.Add("TRUE");
        else
          parameters.Add("FALSE");
      }
      else
      {
        parameters.Add("BEAM_STEEL_MATERIAL_STD");
        parameters.Add(name);
        parameters.Add(this.Grade.ToString());  
      }

      parameters.Add("BEAM_WELDING_MATERIAL");
      parameters.Add(name);
      parameters.Add(this.WeldGrade.ToString().Replace('_', ' '));

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public SteelMaterial Duplicate()
    {
      if (this == null) { return null; }
      SteelMaterial dup = (SteelMaterial)this.MemberwiseClone();
      return dup;
    }

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
