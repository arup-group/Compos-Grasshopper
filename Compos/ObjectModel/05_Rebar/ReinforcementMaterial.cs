using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.ConcreteSlab
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class ReinforcementMaterial
  {
    public Pressure Fu { get; set; }

    public enum StandardGrade
    {
      BS_250R,
      BS_460T,
      BS_500X,
      BS_1770,
      EN_500A,
      EN_500B,
      EN_500C,
      HK_250,
      HK_460,
      AS_R250N,
      AS_D500L,
      AS_D500N,
      AS_D500E
    }

    private void SetGradeFromStandard(StandardGrade standardGrade)
    {
      switch (standardGrade)
      {
        case StandardGrade.BS_250R:
        case StandardGrade.HK_250:
        case StandardGrade.AS_R250N:
          this.Fu = new Pressure(250, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case StandardGrade.BS_460T:
        case StandardGrade.HK_460:
          this.Fu = new Pressure(460, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case StandardGrade.BS_500X:
        case StandardGrade.AS_D500L:
        case StandardGrade.AS_D500N:
        case StandardGrade.AS_D500E:
        case StandardGrade.EN_500A:
        case StandardGrade.EN_500B:
        case StandardGrade.EN_500C:
          this.Fu = new Pressure(500, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case StandardGrade.BS_1770:
          this.Fu = new Pressure(1770, UnitsNet.Units.PressureUnit.Megapascal);
          break;
      }
    }

    #region constructors
    public ReinforcementMaterial()
    {
      // empty constructor
    }
    public ReinforcementMaterial(Pressure fu)
    {
      this.Fu = fu;
    }

    public ReinforcementMaterial(StandardGrade standardGrade)
    {
      SetGradeFromStandard(standardGrade);
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

    #region methods

    public ReinforcementMaterial Duplicate()
    {
      if (this == null) { return null; }
      ReinforcementMaterial dup = (ReinforcementMaterial)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {

      string f = Fu.ToUnit(Helpers.Units.FileUnits.StressUnit).ToString("f0");
      return f.Replace(" ", string.Empty);
    }
    #endregion
  }
}
