using System;
using System.Collections.Generic;
using System.Globalization;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public enum RebarGrade {
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

  public class ReinforcementMaterial : IReinforcementMaterial {
    public Pressure Fy { get; set; }
    public RebarGrade Grade { get; set; }
    public bool UserDefined { get; set; } = false;
    // characteristic strength of user defined reinforcement

    public ReinforcementMaterial() {
      // empty constructor
    }

    public ReinforcementMaterial(Pressure fy) {
      UserDefined = true;
      Fy = fy;
    }

    public ReinforcementMaterial(RebarGrade grade) {
      Grade = grade;
      UserDefined = false;
      SetFy();
    }

    public string ToCoaString(string name) {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      var parameters = new List<string> {
        CoaIdentifier.RebarMaterial,
        name
      };
      if (UserDefined) {
        parameters.Add("USER_DEFINED");
        parameters.Add(CoaHelper.FormatSignificantFigures(Fy.ToUnit(PressureUnit.NewtonPerSquareMeter).Value, 6));
      } else {
        parameters.Add("STANDARD");
        parameters.Add(Grade.ToString().Remove(0, 3));
      }

      return CoaHelper.CreateString(parameters);
    }

    public override string ToString() {
      if (UserDefined) {
        string str = Fy.ToUnit(ComposUnitsHelper.StressUnit).ToString("f0");
        return str.Replace(" ", string.Empty);
      } else {
        return Grade.ToString().Remove(0, 3);
      }
    }

    internal static IReinforcementMaterial FromCoaString(List<string> parameters, Code code) {
      var material = new ReinforcementMaterial();

      if (parameters[2] == "USER_DEFINED") {
        material.UserDefined = true;
        material.Fy = CoaHelper.ConvertToStress(parameters[3], PressureUnit.NewtonPerSquareMeter);
      } else {
        material.UserDefined = false;
        string gradePrefix;
        switch (code) {
          case Code.AS_NZS2327_2017:
            gradePrefix = "AS_";
            break;

          case Code.BS5950_3_1_1990_A1_2010:
          case Code.BS5950_3_1_1990_Superseded:
            gradePrefix = "BS_";
            break;

          case Code.EN1994_1_1_2004:
            gradePrefix = "EN_";
            break;

          case Code.HKSUOS_2005:
          case Code.HKSUOS_2011:
            gradePrefix = "HK_";
            break;

          default:
            throw new Exception("Reinforcement material not implemented for Code " + code.ToString());
        }
        material.Grade = (RebarGrade)Enum.Parse(typeof(RebarGrade), gradePrefix + parameters[3]);
        material.SetFy();
      }
      return material;
    }

    internal void SetFy() {
      switch (Grade) {
        case RebarGrade.BS_250R:
        case RebarGrade.HK_250:
        case RebarGrade.AS_R250N:
          Fy = new Pressure(250, PressureUnit.Megapascal);
          break;

        case RebarGrade.BS_460T:
        case RebarGrade.HK_460:
          Fy = new Pressure(460, PressureUnit.Megapascal);
          break;

        case RebarGrade.BS_500X:
        case RebarGrade.AS_D500L:
        case RebarGrade.AS_D500N:
        case RebarGrade.AS_D500E:
        case RebarGrade.EN_500A:
        case RebarGrade.EN_500B:
        case RebarGrade.EN_500C:
          Fy = new Pressure(500, PressureUnit.Megapascal);
          break;

        case RebarGrade.BS_1770:
          Fy = new Pressure(1770, PressureUnit.Megapascal);
          break;
      }
    }
  }
}
