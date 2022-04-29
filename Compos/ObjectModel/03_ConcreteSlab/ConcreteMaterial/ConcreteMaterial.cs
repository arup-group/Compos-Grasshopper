using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;
using Oasys.Units;

namespace ComposAPI.ConcreteSlab
{
  /// <summary>
  /// A Concrete Material object contains information about the material
  /// such as strength, grade, weight type, Young's modulus ratio, etc.
  /// /// </summary>
  public class ConcreteMaterial
  {
    public enum ConcreteGrade
    {
      C20,
      C25,
      C30,
      C32,
      C35,
      C40,
      C45,
      C50,
      C55,
      C60,
      C70,
      C80,
      C90,
      C100
    }

    // EN1994-1-1:2004
    public enum ConcreteGradeEN
    {
      C20_25,
      C25_30,
      C30_37,
      C32_40,
      C35_45,
      C40_50,
      C45_55,
      C50_60,
      C55_67,
      C60_75,
      LC20_22,
      LC25_28,
      LC30_33,
      LC35_38,
      LC40_44,
      LC45_50,
      LC50_55,
      LC55_60,
      LC60_66
    }

    public enum WeightType
    {
      Normal,
      Light
    }

    public enum DensityClass
    {
      None = 0,
      DC801_1000 = 1000,
      DC1001_1200 = 1200,
      DC1201_1400 = 1400,
      DC1401_1600 = 1600,
      DC1601_1800 = 1800,
      DC1801_2000 = 2000
    }

    public string Grade { get; set; }
    public WeightType Type { get; set; } = WeightType.Normal;
    public DensityClass Class { get; set; } = DensityClass.None;
    public Density DryDensity { get; set; }
    public bool UserDensity { get; set; } = false;
    public ERatio ERatio { get; set; }
    public double ImposedLoadPercentage { get; set; }
    public Strain ShrinkageStrain { get; set; }
    public bool UserStrain { get; set; } = false;

    #region constructors
    public ConcreteMaterial()
    {
      // empty constructor
    }

    /// <summary>
    /// "British" constructor
    /// </summary>
    /// <param name="grade"></param>
    /// <param name="type"></param>
    /// <param name="dryDensity"></param>
    /// <param name="userDensity"></param>
    /// <param name="eRatio"></param>
    /// <param name="imposedLoadPercentage"></param>
    public ConcreteMaterial(ConcreteGrade grade, WeightType type, Density dryDensity, bool userDensity, ERatio eRatio, double imposedLoadPercentage)
    {
      this.Grade = grade.ToString();
      this.Type = type;
      this.DryDensity = dryDensity;
      this.UserDensity = userDensity;
      this.ERatio = eRatio;
      this.ImposedLoadPercentage = imposedLoadPercentage;
      this.ShrinkageStrain = new Strain(-0.000325, StrainUnit.MilliStrain);
    }

    /// <summary>
    /// "European" constructor
    /// </summary>
    /// <param name="grade"></param>
    /// <param name="densityClass"></param>
    /// <param name="dryDensity"></param>
    /// <param name="userDensity"></param>
    /// <param name="eRatio"></param>
    /// <param name="imposedLoadPercentage"></param>
    /// <param name="shrinkageStrain"></param>
    /// <param name="userStrain"></param>
    public ConcreteMaterial(ConcreteGradeEN grade, DensityClass densityClass, Density dryDensity, bool userDensity, ERatio eRatio, double imposedLoadPercentage, Strain shrinkageStrain, bool userStrain)
    {
      this.Grade = grade.ToString();
      if (this.Grade.StartsWith("L"))
        this.Type = WeightType.Light;
      this.Class = densityClass;
      this.DryDensity = dryDensity;
      this.UserDensity = userDensity;
      this.ERatio = eRatio;
      this.ImposedLoadPercentage = imposedLoadPercentage;
      this.ShrinkageStrain = shrinkageStrain;
      this.UserStrain = userStrain;
    }

    /// <summary>
    /// "HKSUOS" constructor
    /// </summary>
    /// <param name="grade"></param>
    /// <param name="dryDensity"></param>
    /// <param name="userDensity"></param>
    /// <param name="eRatio"></param>
    /// <param name="imposedLoadPercentage"></param>
    public ConcreteMaterial(ConcreteGrade grade, Density dryDensity, bool userDensity, ERatio eRatio, double imposedLoadPercentage)
    {
      this.Grade = grade.ToString();
      this.DryDensity = dryDensity;
      this.UserDensity = userDensity;
      this.ERatio = eRatio;
      this.ImposedLoadPercentage = imposedLoadPercentage;
      this.ShrinkageStrain = Strain.Zero;
    }

    /// <summary>
    /// "AS/NZ" constructor
    /// </summary>
    /// <param name="grade"></param>
    /// <param name="dryDensity"></param>
    /// <param name="userDensity"></param>
    /// <param name="eRatio"></param>
    /// <param name="imposedLoadPercentage"></param>
    public ConcreteMaterial(ConcreteGrade grade, Density dryDensity, bool userDensity, ERatio eRatio, double imposedLoadPercentage, Strain shrinkageStrain, bool userStrain)
    {
      this.Grade = grade.ToString();
      this.DryDensity = dryDensity;
      this.UserDensity = userDensity;
      this.ERatio = eRatio;
      this.ImposedLoadPercentage = imposedLoadPercentage;
      this.ShrinkageStrain = shrinkageStrain;
      this.UserStrain = userStrain;
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
    internal ConcreteMaterial(string coaString)
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
    public ConcreteMaterial Duplicate()
    {
      if (this == null) { return null; }
      ConcreteMaterial dup = (ConcreteMaterial)this.MemberwiseClone();
      dup.ERatio = this.ERatio.Duplicate();
      return dup;
    }

    public override string ToString()
    {
      string str = this.Grade.ToString().Replace("_", "/");
      str += " " + this.Type + ", DD: " + this.DryDensity;
      return str;
    }
    #endregion
  }
}
