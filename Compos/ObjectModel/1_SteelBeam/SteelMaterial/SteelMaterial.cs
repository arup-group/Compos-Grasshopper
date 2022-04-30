using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.SteelBeam
{
  /// <summary>
  /// Steel Material for a <see cref="Beam"/>. Contains information about strength, density and Young's Modulus, as well as grade.
  /// </summary>
  public class SteelMaterial
  {
    public Pressure fy { get; set; }
    public Pressure E { get; set; }
    public Density Density { get; set; }

    public bool isCustom { get; set; }
    public bool ReductionFactorMpl { get; set; }

    public SteelMaterialGrade Grade { get; set; }
    public enum SteelMaterialGrade
    {
      S235,
      S275,
      S355,
      S450,
      S460
    }
    public WeldMaterialGrade WeldGrade { get; set; }
    public enum WeldMaterialGrade
    {
      None,
      Grade35,
      Grade42,
      Grade50
    }

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
          this.WeldGrade = WeldMaterialGrade.Grade35;
          break;

        case SteelMaterialGrade.S275:
          this.fy = new Pressure(275, UnitsNet.Units.PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade35;
          break;

        case SteelMaterialGrade.S355:
          this.fy = new Pressure(355, UnitsNet.Units.PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade42;
          break;

        case SteelMaterialGrade.S450:
          this.fy = new Pressure(450, UnitsNet.Units.PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade50;
          break;

        case SteelMaterialGrade.S460:
          this.fy = new Pressure(460, UnitsNet.Units.PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade50;
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
    internal SteelMaterial(string coaString)
    {
      // to do - implement from coa string method
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
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
      string wMat = (WeldGrade == WeldMaterialGrade.None) ? "" : WeldGrade.ToString();

      if (isCustom == false)
      {
        return this.Grade.ToString();
      }
      else
      {
        isCust = "Cust.";
        f = fy.ToUnit(Helpers.Units.FileUnits.StressUnit).ToString("f0");
        e = E.ToUnit(Helpers.Units.FileUnits.StressUnit).ToString("f0");
        ro = Density.ToUnit(Helpers.Units.FileUnits.DensityUnit).ToString("f0");

        return (isCust.Replace(" ", string.Empty) + ", " + f.Replace(" ", string.Empty) + ", " + e.Replace(" ", string.Empty) + ", " + ro.Replace(" ", string.Empty));
      }
    }

    #endregion
  }
}
