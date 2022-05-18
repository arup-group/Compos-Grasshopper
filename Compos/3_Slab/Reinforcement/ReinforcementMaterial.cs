using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;

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

  public class ReinforcementMaterial : IReinforcementMaterial
  {
    public RebarGrade Grade { get; set; }
    public bool UserDefined { get; set; } = false;
    public Pressure Fy { get; set; } // characteristic strength of user defined reinforcement

    #region constructors
    public ReinforcementMaterial()
    {
      // empty constructor
    }

    public ReinforcementMaterial(Pressure fy)
    {
      this.UserDefined = true;
      this.Fy = fy;
    }

    public ReinforcementMaterial(RebarGrade grade)
    {
      this.Grade = grade;
      this.UserDefined = false;
      this.SetFy();
    }
    #endregion

    #region coa interop
    internal static IReinforcementMaterial FromCoaString(List<string> parameters, Code code)
    {
      ReinforcementMaterial material = new ReinforcementMaterial();

      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      if (parameters[2] == "USER_DEFINED")
      {
        material.UserDefined = true;
        material.Fy = new Pressure(Convert.ToDouble(parameters[3], noComma), PressureUnit.NewtonPerSquareMeter);
      }
      else
      {
        material.UserDefined = false;
        string gradePrefix;
        switch (code)
        {
          case (Code.AS_NZS2327_2017):
            gradePrefix = "AS_";
            break;

          case (Code.BS5950_3_1_1990_A1_2010):
          case (Code.BS5950_3_1_1990_Superseded):
            gradePrefix = "BS_";
            break;

          case (Code.EN1994_1_1_2004):
            gradePrefix = "EN_";
            break;

          case (Code.HKSUOS_2005):
          case (Code.HKSUOS_2011):
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

    public string ToCoaString(string name)
    {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      List<string> parameters = new List<string>();
      parameters.Add(CoaIdentifier.RebarMaterial);
      parameters.Add(name);
      if (this.UserDefined)
      {
        parameters.Add("USER_DEFINED");
        parameters.Add(CoaHelper.FormatSignificantFigures(this.Fy.ToUnit(PressureUnit.NewtonPerSquareMeter).Value, 6, true));
      }
      else
      {
        parameters.Add("STANDARD");
        parameters.Add(this.Grade.ToString().Remove(0, 3));
      }

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    internal void SetFy()
    {
      switch (this.Grade)
      {
        case RebarGrade.BS_250R:
        case RebarGrade.HK_250:
        case RebarGrade.AS_R250N:
          this.Fy = new Pressure(250, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case RebarGrade.BS_460T:
        case RebarGrade.HK_460:
          this.Fy = new Pressure(460, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case RebarGrade.BS_500X:
        case RebarGrade.AS_D500L:
        case RebarGrade.AS_D500N:
        case RebarGrade.AS_D500E:
        case RebarGrade.EN_500A:
        case RebarGrade.EN_500B:
        case RebarGrade.EN_500C:
          this.Fy = new Pressure(500, UnitsNet.Units.PressureUnit.Megapascal);
          break;
        case RebarGrade.BS_1770:
          this.Fy = new Pressure(1770, UnitsNet.Units.PressureUnit.Megapascal);
          break;
      }
    }
    public override string ToString()
    {
      if (this.UserDefined)
      {
        string str = Fy.ToUnit(Units.StressUnit).ToString("f0");
        return str.Replace(" ", string.Empty);
      }
      else
      {
        return this.Grade.ToString().Remove(0, 3);
      }
    }
    #endregion
  }
}
