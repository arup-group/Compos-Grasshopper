using System;
using System.Collections.Generic;
using System.Globalization;
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
      LightWeight
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
    public Ratio ImposedLoadPercentage { get; set; } // percentage of live load acting long term (as dead load)
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
    public ConcreteMaterial(ConcreteGrade grade, WeightType type, Density dryDensity, bool userDensity, IERatio eRatio, Ratio imposedLoadPercentage)
    {
      this.Grade = grade.ToString();
      this.Type = type;
      this.DryDensity = dryDensity;
      this.UserDensity = userDensity;
      this.ERatio = eRatio;
      this.ImposedLoadPercentage = imposedLoadPercentage;
      this.ShrinkageStrain = new Strain(-0.325, StrainUnit.MilliStrain);
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
    public ConcreteMaterial(ConcreteGradeEN grade, DensityClass densityClass, Density dryDensity, bool userDensity, IERatio eRatio, Ratio imposedLoadPercentage, Strain shrinkageStrain, bool userStrain)
    {
      this.Grade = grade.ToString();
      if (this.Grade.StartsWith("L"))
        this.Type = WeightType.LightWeight;
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
    public ConcreteMaterial(ConcreteGrade grade, Density dryDensity, bool userDensity, IERatio eRatio, Ratio imposedLoadPercentage)
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
    public ConcreteMaterial(ConcreteGrade grade, Density dryDensity, bool userDensity, IERatio eRatio, Ratio imposedLoadPercentage, Strain shrinkageStrain, bool userStrain)
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
    internal static IConcreteMaterial FromCoaString(List<string> parameters, ComposUnits units)
    {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      ConcreteMaterial material = new ConcreteMaterial();
      if (parameters[2].Length < 4)
      {
        // BS5950 GRADES
        material.Grade = Enum.Parse(typeof(ConcreteGrade), parameters[2]).ToString();
      }
      else
      {
        // EC4 GRADES
        material.Grade = Enum.Parse(typeof(ConcreteGradeEN), parameters[2].Replace("/", "_")).ToString();
      }
      if (parameters[3] == "NORMAL")
        material.Type = WeightType.Normal;
      else
        material.Type = WeightType.LightWeight;

      int index;
      if (parameters[4] == "USER_DENSITY")
      {
        material.UserDensity = true;
        index = 6;
      }
      else
      {
        material.UserDensity = false;
        if (parameters[6] != "NOT_APPLY")
          material.Class = (DensityClass)Enum.Parse(typeof(DensityClass), "DC" + parameters[6]);
        index = 7;
      }

      material.DryDensity = CoaHelper.ConvertToDensity(parameters[5], units.Density);

      material.ImposedLoadPercentage = new Ratio(Convert.ToDouble(parameters[index], noComma), RatioUnit.DecimalFraction);

      index++;
      if (parameters[index] == "CODE_E_RATIO")
      {
        material.ERatio = new ERatio();
        index++;
      }
      else
      {
        material.ERatio = new ERatio(CoaHelper.ConvertToDouble(parameters[index + 1]), CoaHelper.ConvertToDouble(parameters[index + 2]), CoaHelper.ConvertToDouble(parameters[index + 3]), CoaHelper.ConvertToDouble(parameters[index + 4]));
        index += 5;
      }

      if (parameters[index] == "USER_STRAIN")
      {
        material.UserStrain = true;
        material.ShrinkageStrain = CoaHelper.ConvertToStrain(parameters[index + 1], StrainUnit.Ratio);
      }

      return material;
    }

    /// <summary>
    /// SLAB_CONCRETE_MATERIAL | name | grade | type | density_type | density | density-class | percent | E_ratio_type | E_ratio_short | E_ratio_long | E_ratio_shrink | shrink-type | shrink-strain
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// 
    public string ToCoaString(string name, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.SlabConcreteMaterial);
      parameters.Add(name);
      parameters.Add(this.Grade.Replace("_", "/"));
      if (this.Type == WeightType.Normal)
        parameters.Add("NORMAL");
      else
        parameters.Add("LIGHT");
      if (this.UserDensity)
      {
        parameters.Add("USER_DENSITY");
        parameters.Add(String.Format(CoaHelper.NoComma, "{0:0.00}", this.DryDensity.ToUnit(units.Density).Value));
      }
      else
      {
        parameters.Add("CODE_DENSITY");
        parameters.Add(String.Format(CoaHelper.NoComma, "{0:0.00}", this.DryDensity.ToUnit(units.Density).Value));
        parameters.Add(this.Class.ToString().Replace("DC", ""));
      }
      parameters.Add(String.Format(CoaHelper.NoComma, "{0:0.000000}", this.ImposedLoadPercentage.DecimalFractions));
      if (this.ERatio.UserDefined)
      {
        parameters.Add("USER_E_RATIO");
        parameters.Add(String.Format(CoaHelper.NoComma, "{0:0.00000}", this.ERatio.ShortTerm));
        parameters.Add(String.Format(CoaHelper.NoComma, "{0:0.00000}", this.ERatio.LongTerm));
        parameters.Add(String.Format(CoaHelper.NoComma, "{0:0.00000}", this.ERatio.Vibration));
        parameters.Add(String.Format(CoaHelper.NoComma, "{0:0.000000}", this.ERatio.Shrinkage));
      }
      else
        parameters.Add("CODE_E_RATIO");
      if (this.UserStrain)
      {
        parameters.Add("USER_STRAIN");
        parameters.Add(String.Format(CoaHelper.NoComma, "{0:0.000000000}", this.ShrinkageStrain.Ratio));
      }
      else
        parameters.Add("CODE_STRAIN");

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string str;
      if (this.Grade == null)
        str = "(Grade not set)";
      else
        str = this.Grade.ToString().Replace("_", "/");
      str += " " + this.Type + ", D: " + this.DryDensity.As(Units.DensityUnit).ToString().Replace(" ", string.Empty);
      return str;
    }
    #endregion
  }
}
