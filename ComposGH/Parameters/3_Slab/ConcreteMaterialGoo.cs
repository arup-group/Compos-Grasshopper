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

    public ConcreteGrade Grade { get; set; }

    public WeightType Type { get; set; }

    public Density DryDensity { get; set; }

    public SteelConcreteModularRatio SteelConcreteModularRatio { get; set; }

    public double PercentageOfImposedLoadActingLongTerm { get; set; }

    public Strain ShrinkageStrain { get; set; }

    private void SetPropertiesFrom(ConcreteGrade grade, WeightType type)
    {
      switch (type)
      {
        case WeightType.Light:
          this.DryDensity = new Density(1800, DensityUnit.KilogramPerCubicMeter);
          //this.SteelConcreteModularRatioShortTerm = new Ratio(10, RatioUnit.DecimalFraction);
          //this.SteelConcreteModularRatioLongTerm = new Ratio(25, RatioUnit.DecimalFraction);
          //this.SteelConcreteModularRatioVibration = new Ratio(9.32, RatioUnit.DecimalFraction);
          break;

        case WeightType.Normal:
        default:
          this.DryDensity = new Density(2400, DensityUnit.KilogramPerCubicMeter);
          //this.SteelConcreteModularRatioShortTerm = new Ratio(6, RatioUnit.DecimalFraction);
          //this.SteelConcreteModularRatioLongTerm = new Ratio(18, RatioUnit.DecimalFraction);
          //this.SteelConcreteModularRatioVibration = new Ratio(5.39, RatioUnit.DecimalFraction);
          break;
      }
    }

    #region constructors
    public ConcreteMaterial()
    {
      // empty constructor
    }

    public ConcreteMaterial(Density dryDensitiy)
    {
      this.DryDensity = dryDensitiy;
    }

    public ConcreteMaterial(ConcreteGrade grade, WeightType type, Density dryDensity, SteelConcreteModularRatio steelConcreteModularRatio, double percentageOfImposedLoadActingLongTerm, Strain shrinkageStrain)
    {
      this.Grade = grade;
      this.Type = type;
      this.DryDensity = dryDensity;
      this.SteelConcreteModularRatio = steelConcreteModularRatio;
      this.PercentageOfImposedLoadActingLongTerm = percentageOfImposedLoadActingLongTerm;
      this.ShrinkageStrain = shrinkageStrain;

      //this.SetPropertiesFrom(grade, type);
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
      return dup;
    }

    public override string ToString()
    {
      return Grade.ToString().Replace("_", "/");
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
      return new ConcreteMaterialGoo(Value == null ? new ConcreteMaterial() : Value.Duplicate());
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
      if (Value == null)
        return "Null";
      else
        return "Compos " + TypeName + " {" + Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this instance of our custom class into some other type Q.           
      if (typeof(Q).IsAssignableFrom(typeof(ConcreteMaterial)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
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
        Value = (ConcreteMaterial)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
