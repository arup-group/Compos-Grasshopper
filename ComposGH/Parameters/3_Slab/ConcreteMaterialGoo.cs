using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Documentation;
using Rhino.Collections;
using UnitsNet;
using UnitsNet.Units;
using Oasys.Units;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
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

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ConcreteMaterialGoo : GH_Goo<ConcreteMaterial>
  {
    #region constructors
    public ConcreteMaterialGoo()
    {
      this.Value = new ConcreteMaterial();
    }
    public ConcreteMaterialGoo(ConcreteMaterial item)
    {
      if (item == null)
        item = new ConcreteMaterial();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ConcreteMaterialGoo DuplicateGoo()
    {
      return new ConcreteMaterialGoo(this.Value == null ? new ConcreteMaterial() : this.Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Concrete Material";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); // todo: beef this up to be more informative
      }
    }

    public override string ToString()
    {
      if (this.Value == null)
        return "Null";
      else
        return "Compos " + this.TypeName + " {" + this.Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this instance of our custom class into some other type Q.           
      if (typeof(Q).IsAssignableFrom(typeof(ConcreteMaterial)))
      {
        if (this.Value == null)
          target = default;
        else
          target = (Q)(object)this.Value;
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data  into our custom class.
      if (source == null) { return false; }

      // Cast from GsaMaterial
      if (typeof(ConcreteMaterial).IsAssignableFrom(source.GetType()))
      {
        this.Value = (ConcreteMaterial)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
