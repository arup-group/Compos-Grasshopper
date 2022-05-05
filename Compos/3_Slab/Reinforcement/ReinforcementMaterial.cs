using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public enum RebarGrade
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

  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class ReinforcementMaterial : IReinforcementMaterial
  {
    public Pressure Fu { get; set; }

    private void SetGradeFromStandard(RebarGrade StandardGrade2)
    {
      switch (StandardGrade2)
      {
        case RebarGrade.BS_250R:
        case RebarGrade.HK_250:
        case RebarGrade.AS_R250N:
          this.Fu = new Pressure(250, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case RebarGrade.BS_460T:
        case RebarGrade.HK_460:
          this.Fu = new Pressure(460, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case RebarGrade.BS_500X:
        case RebarGrade.AS_D500L:
        case RebarGrade.AS_D500N:
        case RebarGrade.AS_D500E:
        case RebarGrade.EN_500A:
        case RebarGrade.EN_500B:
        case RebarGrade.EN_500C:
          this.Fu = new Pressure(500, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case RebarGrade.BS_1770:
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

    public ReinforcementMaterial(RebarGrade StandardGrade2)
    {
      SetGradeFromStandard(StandardGrade2);
    }
    #endregion

    #region methods
    public override string ToString()
    {

      string f = Fu.ToUnit(Units.StressUnit).ToString("f0");
      return f.Replace(" ", string.Empty);
    }
    #endregion
  }
}
