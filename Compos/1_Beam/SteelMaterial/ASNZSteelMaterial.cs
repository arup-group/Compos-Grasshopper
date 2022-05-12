using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum ASNZSteelMaterialGrade
  {
    C450_AS1163,
    C350_AS1163,
    C250_AS1163,
    HA400_AS1594,
    HW350_AS1594,
    HA350_AS1594,
    HA300_1_AS1594,
    HA_HU300_AS1594,
    HA_HU250_AS1594,
    HA200_AS1594,
    HA4N_AS1594,
    HA3_AS1594,
    XF400_AS1594,
    XF300_AS1594,
    Gr450_AS3678,
    Gr400_AS3678,
    Gr350_AS3678,
    WR350_AS3678,
    Gr300_AS3678,
    Gr250_AS3678,
    Gr250L15_AS3678,
    Gr200_AS3678,
    Gr400_AS3679_1_Flats,
    Gr350_AS3679_1_Flats,
    Gr300_AS3679_1_Flats,
    Gr250_AS3679_1_Flats,
    Gr400_AS3679_1_Hollow,
    Gr350_AS3679_1_Hollow,
    Gr300_AS3679_1_Hollow,
    Gr250_AS3679_1_Hollow
  }
  public class ASNZSteelMaterial : SteelMaterial
  {
    public new ASNZSteelMaterialGrade Grade { get; set; } // standard material grade

    #region constructors
    public ASNZSteelMaterial()
    {
      // empty constructor
    }
    public ASNZSteelMaterial(ASNZSteelMaterialGrade grade)
    {
      this.Grade = grade;
      SetValuesFromStandard();
    }
    public ASNZSteelMaterial(string grade)
    {
      this.Grade = FromString(grade);
      SetValuesFromStandard();
    }
    private void SetValuesFromStandard()
    {
      this.E = new Pressure(200, PressureUnit.Gigapascal);
      this.Density = new Density(7850, UnitsNet.Units.DensityUnit.KilogramPerCubicMeter);
      switch (this.Grade)
      {
        case ASNZSteelMaterialGrade.C450_AS1163:
          this.fy = new Pressure(450, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.C350_AS1163:
          this.fy = new Pressure(350, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.C250_AS1163:
          this.fy = new Pressure(250, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_42;
          break;
        case ASNZSteelMaterialGrade.HA400_AS1594:
          this.fy = new Pressure(380, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_50;
          break;
        case ASNZSteelMaterialGrade.HW350_AS1594:
          this.fy = new Pressure(340, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_50;
          break;
        case ASNZSteelMaterialGrade.HA350_AS1594:
          this.fy = new Pressure(350, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.HA300_1_AS1594:
          this.fy = new Pressure(300, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.HA_HU300_AS1594:
          this.fy = new Pressure(300, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_42;
          break;
        case ASNZSteelMaterialGrade.HA_HU250_AS1594:
          this.fy = new Pressure(250, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_50;
          break;
        case ASNZSteelMaterialGrade.HA200_AS1594:
          this.fy = new Pressure(200, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_50;
          break;
        case ASNZSteelMaterialGrade.HA4N_AS1594:
          this.fy = new Pressure(170, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.HA3_AS1594:
          this.fy = new Pressure(200, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.XF400_AS1594:
          this.fy = new Pressure(380, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.XF300_AS1594:
          this.fy = new Pressure(300, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr450_AS3678:
          this.fy = new Pressure(450, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr400_AS3678:
          this.fy = new Pressure(400, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr350_AS3678:
          this.fy = new Pressure(360, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.WR350_AS3678:
          this.fy = new Pressure(340, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr300_AS3678:
          this.fy = new Pressure(320, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr250_AS3678:
          this.fy = new Pressure(280, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr250L15_AS3678:
          this.fy = new Pressure(280, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr200_AS3678:
          this.fy = new Pressure(200, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr400_AS3679_1_Flats:
          this.fy = new Pressure(400, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr350_AS3679_1_Flats:
          this.fy = new Pressure(360, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr300_AS3679_1_Flats:
          this.fy = new Pressure(320, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr250_AS3679_1_Flats:
          this.fy = new Pressure(260, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr400_AS3679_1_Hollow:
          this.fy = new Pressure(400, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr350_AS3679_1_Hollow:
          this.fy = new Pressure(340, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr300_AS3679_1_Hollow:
          this.fy = new Pressure(300, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        case ASNZSteelMaterialGrade.Gr250_AS3679_1_Hollow:
          this.fy = new Pressure(250, PressureUnit.Megapascal);
          this.WeldGrade = WeldMaterialGrade.Grade_35;
          break;
        default:
          throw new Exception("unknown grade");
      }
    }
    #endregion

    #region coa interop
    public ASNZSteelMaterialGrade FromString(string grade)
    {
      switch (grade)
      {
        case "C450(AS1163)":
          return ASNZSteelMaterialGrade.C450_AS1163;
        case "C350(AS1163)":
          return ASNZSteelMaterialGrade.C350_AS1163;
        case "C250(AS1163)":
          return ASNZSteelMaterialGrade.C250_AS1163;
        case "HA400(AS1594)":
          return ASNZSteelMaterialGrade.HA400_AS1594;
        case "HA3(AS1594)":
          return ASNZSteelMaterialGrade.HA3_AS1594;
        case "HW350(AS1594)":
          return ASNZSteelMaterialGrade.HW350_AS1594;
        case "HA350(AS1594)":
          return ASNZSteelMaterialGrade.HA350_AS1594;
        case "HA300/1(AS1594)":
          return ASNZSteelMaterialGrade.HA300_1_AS1594;
        case "HA/HU300(AS1594)":
          return ASNZSteelMaterialGrade.HA_HU300_AS1594;
        case "HA/HU250(AS1594)":
          return ASNZSteelMaterialGrade.HA_HU250_AS1594;
        case "HA200(AS1594)":
          return ASNZSteelMaterialGrade.HA200_AS1594;
        case "HA4N(AS1594)":
          return ASNZSteelMaterialGrade.HA4N_AS1594;
        case "XF400(AS1594)":
          return ASNZSteelMaterialGrade.XF400_AS1594;
        case "XF300(AS1594)":
          return ASNZSteelMaterialGrade.XF300_AS1594;
        case "450(AS3678)":
          return ASNZSteelMaterialGrade.Gr450_AS3678;
        case "400(AS3678)":
          return ASNZSteelMaterialGrade.Gr400_AS3678;
        case "350(AS3678)":
          return ASNZSteelMaterialGrade.Gr350_AS3678;
        case "WR350(AS3678)":
          return ASNZSteelMaterialGrade.WR350_AS3678;
        case "300(AS3678)":
          return ASNZSteelMaterialGrade.Gr300_AS3678;
        case "250(AS3678)":
          return ASNZSteelMaterialGrade.Gr250_AS3678;
        case "250L15(AS3678)":
          return ASNZSteelMaterialGrade.Gr250L15_AS3678;
        case "200(AS3678)":
          return ASNZSteelMaterialGrade.Gr200_AS3678;
        case "400(AS3679.1 Flats)":
          return ASNZSteelMaterialGrade.Gr400_AS3679_1_Flats;
        case "350(AS3679.1 Flats)":
          return ASNZSteelMaterialGrade.Gr350_AS3679_1_Flats;
        case "300(AS3679.1 Flats)":
          return ASNZSteelMaterialGrade.Gr300_AS3679_1_Flats;
        case "250(AS3679.1 Flats)":
          return ASNZSteelMaterialGrade.Gr250_AS3679_1_Flats;
        case "400(AS3679.1 Hollow)":
          return ASNZSteelMaterialGrade.Gr400_AS3679_1_Hollow;
        case "350(AS3679.1 Hollow)":
          return ASNZSteelMaterialGrade.Gr350_AS3679_1_Hollow;
        case "300(AS3679.1 Hollow)":
          return ASNZSteelMaterialGrade.Gr300_AS3679_1_Hollow;
        case "250(AS3679.1 Hollow)":
          return ASNZSteelMaterialGrade.Gr250_AS3679_1_Hollow;
        default:
          return "unknown grade";
      }
    }
    public new string ToCoaString(string name, Code code, ComposUnits units)
    {
      List<string> steelParameters = new List<string>();
      steelParameters.Add("BEAM_STEEL_MATERIAL_STD");
      steelParameters.Add(name);
      steelParameters.Add(this.ToString());

      return CoaHelper.CreateString(steelParameters); 
    }
    #endregion

    #region methods
    public override string ToString()
    {
      switch (this.Grade)
      {
        case ASNZSteelMaterialGrade.C450_AS1163:
          return "C450(AS1163)";
        case ASNZSteelMaterialGrade.C350_AS1163:
          return "C350(AS1163)";
        case ASNZSteelMaterialGrade.C250_AS1163:
          return "C250(AS1163)";
        case ASNZSteelMaterialGrade.HA400_AS1594:
          return "HA400(AS1594)";
        case ASNZSteelMaterialGrade.HA3_AS1594:
          return "HA3(AS1594)";
        case ASNZSteelMaterialGrade.HW350_AS1594:
          return "HW350(AS1594)";
        case ASNZSteelMaterialGrade.HA350_AS1594:
          return "HA350(AS1594)";
        case ASNZSteelMaterialGrade.HA300_1_AS1594:
          return "HA300/1(AS1594)";
        case ASNZSteelMaterialGrade.HA_HU300_AS1594:
          return "HA/HU300(AS1594)";
        case ASNZSteelMaterialGrade.HA_HU250_AS1594:
          return "HA/HU250(AS1594)";
        case ASNZSteelMaterialGrade.HA200_AS1594:
          return "HA200(AS1594)";
        case ASNZSteelMaterialGrade.HA4N_AS1594:
          return "HA4N(AS1594)";
        case ASNZSteelMaterialGrade.XF400_AS1594:
          return "XF400(AS1594)";
        case ASNZSteelMaterialGrade.XF300_AS1594:
          return "XF300(AS1594)";
        case ASNZSteelMaterialGrade.Gr450_AS3678:
          return "450(AS3678)";
        case ASNZSteelMaterialGrade.Gr400_AS3678:
          return "400(AS3678)";
        case ASNZSteelMaterialGrade.Gr350_AS3678:
          return "350(AS3678)";
        case ASNZSteelMaterialGrade.WR350_AS3678:
          return "WR350(AS3678)";
        case ASNZSteelMaterialGrade.Gr300_AS3678:
          return "300(AS3678)";
        case ASNZSteelMaterialGrade.Gr250_AS3678:
          return "250(AS3678)";
        case ASNZSteelMaterialGrade.Gr250L15_AS3678:
          return "250L15(AS3678)";
        case ASNZSteelMaterialGrade.Gr200_AS3678:
          return "200(AS3678)";
        case ASNZSteelMaterialGrade.Gr400_AS3679_1_Flats:
          return "400(AS3679.1 Flats)";
        case ASNZSteelMaterialGrade.Gr350_AS3679_1_Flats:
          return "350(AS3679.1 Flats)";
        case ASNZSteelMaterialGrade.Gr300_AS3679_1_Flats:
          return "300(AS3679.1 Flats)";
        case ASNZSteelMaterialGrade.Gr250_AS3679_1_Flats:
          return "250(AS3679.1 Flats)";
        case ASNZSteelMaterialGrade.Gr400_AS3679_1_Hollow:
          return "400(AS3679.1 Hollow)";
        case ASNZSteelMaterialGrade.Gr350_AS3679_1_Hollow:
          return "350(AS3679.1 Hollow)";
        case ASNZSteelMaterialGrade.Gr300_AS3679_1_Hollow:
          return "300(AS3679.1 Hollow)";
        case ASNZSteelMaterialGrade.Gr250_AS3679_1_Hollow:
          return "250(AS3679.1 Hollow)";
        default:
          return "unknown grade";
      }
    }
    #endregion
  }
}
