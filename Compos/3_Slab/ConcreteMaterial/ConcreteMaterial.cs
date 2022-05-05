using System;
using System.Collections.Generic;
using System.Linq;

using ComposAPI.Helpers;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
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

  /// <summary>
  /// A Concrete Material object contains information about the material
  /// such as strength, grade, weight type, Young's modulus ratio, etc.
  /// /// </summary>
  public class ConcreteMaterial : IConcreteMaterial
  {

    public enum WeightType
    {
      Normal,
      Light
    }

    public enum DensityClass
    {
      DC801_1000 = 1000,
      DC1001_1200 = 1200,
      DC1201_1400 = 1400,
      DC1401_1600 = 1600,
      DC1601_1800 = 1800,
      DC1801_2000 = 2000,
      NOT_APPLY = 0
    }

    public string Grade { get; set; } // concrete material grade
    public WeightType Type { get; set; } = WeightType.Normal; // light weight or normal weight concrete
    public DensityClass Class { get; set; } = DensityClass.NOT_APPLY; //	Leight weight material density class
    public Density DryDensity { get; set; } // material density
    public bool UserDensity { get; set; } = false; //	code density or user defined density
    public IERatio ERatio { get; set; } // steel to concrete Young's modulus ratio
    public double ImposedLoadPercentage { get; set; } // percentage of live load acting long term (as dead load)
    public Strain ShrinkageStrain { get; set; } //	Concrete Shrinkage strain
    public bool UserStrain { get; set; } = false; //	code or user defined shrinkage strain



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
    public ConcreteMaterial(ConcreteGrade grade, WeightType type, Density dryDensity, bool userDensity, IERatio eRatio, double imposedLoadPercentage)
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
    public ConcreteMaterial(ConcreteGradeEN grade, DensityClass densityClass, Density dryDensity, bool userDensity, IERatio eRatio, double imposedLoadPercentage, Strain shrinkageStrain, bool userStrain)
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
    public ConcreteMaterial(ConcreteGrade grade, Density dryDensity, bool userDensity, IERatio eRatio, double imposedLoadPercentage)
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
    public ConcreteMaterial(ConcreteGrade grade, Density dryDensity, bool userDensity, IERatio eRatio, double imposedLoadPercentage, Strain shrinkageStrain, bool userStrain)
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

    #region coa interop
    internal ConcreteMaterial(List<string> parameters, DensityUnit densityUnit, StrainUnit strainUnit)
    {
      if (parameters[1].Length < 4)
      {
        // BS5950 GRADES
        this.Grade = Enum.Parse(typeof(ConcreteGrade), parameters[1]).ToString();
      }
      else
      {
        // EC4 GRADES
        this.Grade = Enum.Parse(typeof(ConcreteGradeEN), parameters[1]).ToString();
      }
      this.Type = (WeightType)Enum.Parse(typeof(WeightType), parameters[2]);
      if (parameters[3] == "USER_DENSITY")
        this.UserDensity = true;
      int i = 4;
      if (this.UserDensity)
      {
        this.DryDensity = new Density(Convert.ToDouble(parameters[i]), densityUnit);
        i++;
      }

      // todo: implement!
    }

    /// <summary>
    /// SLAB_CONCRETE_MATERIAL | name | grade | type | density_type | density | density-class | percent | E_ratio_type | E_ratio_short | E_ratio_long | E_ratio_shrink | shrink-type | shrink-strain
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string ToCoaString(string name, DensityUnit densityUnit, StrainUnit strainUnit)
    {
      List<string> parameters = new List<string>() { CoaIdentifier.SlabConcreteMaterial, name, this.Grade.Replace("_", "/"), this.Type.ToString() };
      if (this.UserDensity)
      {
        parameters.Add("USER_DENSITY");
        parameters.Add(this.DryDensity.ToUnit(densityUnit).ToString());
      }
      else
      {
        parameters.Add("CODE_DENSITY");
        parameters.Add(this.Class.ToString());
      }
      parameters.Add(this.ImposedLoadPercentage.ToString());
      if (this.ERatio.UserDefined)
      {
        parameters.Add("USER_E_RATIO");
        parameters.Add(this.ERatio.ShortTerm.ToString());
        parameters.Add(this.ERatio.LongTerm.ToString());
        parameters.Add(this.ERatio.Vibration.ToString());
        parameters.Add(this.ERatio.Shrinkage.ToString());
      }
      else
        parameters.Add("CODE_E_RATIO");
      if (this.UserStrain)
      {
        parameters.Add("USER_STRAIN");
        parameters.Add(this.ShrinkageStrain.ToUnit(strainUnit).ToString());
      }
      else
        parameters.Add("CODE_STRAIN");

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string str = this.Grade.ToString().Replace("_", "/");
      str += " " + this.Type + ", DD: " + this.DryDensity;
      return str;
    }
    #endregion
  }
}
