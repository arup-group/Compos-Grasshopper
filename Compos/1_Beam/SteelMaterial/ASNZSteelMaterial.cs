using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;

namespace ComposAPI {
  public class ASNZSteelMaterial : SteelMaterial {
    public new StandardASNZSteelMaterialGrade Grade { get; set; } // standard material grade

    public ASNZSteelMaterial() {
      // empty constructor
    }

    public ASNZSteelMaterial(StandardASNZSteelMaterialGrade grade) {
      SetValuesFromStandard(grade);
    }

    public override string ToCoaString(string name, Code code, ComposUnits units) {
      List<string> steelParameters = new List<string>();
      steelParameters.Add("BEAM_STEEL_MATERIAL_STD");
      steelParameters.Add(name);
      steelParameters.Add(ToString());
      string coaString = CoaHelper.CreateString(steelParameters);

      List<string> weldingParameters = new List<string>();
      weldingParameters.Add("BEAM_WELDING_MATERIAL");
      weldingParameters.Add(name);
      weldingParameters.Add(WeldGrade.ToString().Replace('_', ' '));
      coaString += CoaHelper.CreateString(weldingParameters);

      return coaString;
    }

    public override string ToString() {
      switch (Grade) {
        case StandardASNZSteelMaterialGrade.C450_AS1163:
          return "C450(AS1163)";

        case StandardASNZSteelMaterialGrade.C350_AS1163:
          return "C350(AS1163)";

        case StandardASNZSteelMaterialGrade.C250_AS1163:
          return "C250(AS1163)";

        case StandardASNZSteelMaterialGrade.HA400_AS1594:
          return "HA400(AS1594)";

        case StandardASNZSteelMaterialGrade.HA3_AS1594:
          return "HA3(AS1594)";

        case StandardASNZSteelMaterialGrade.HW350_AS1594:
          return "HW350(AS1594)";

        case StandardASNZSteelMaterialGrade.HA350_AS1594:
          return "HA350(AS1594)";

        case StandardASNZSteelMaterialGrade.HA300_1_AS1594:
          return "HA300/1(AS1594)";

        case StandardASNZSteelMaterialGrade.HA_HU300_AS1594:
          return "HA/HU300(AS1594)";

        case StandardASNZSteelMaterialGrade.HA_HU250_AS1594:
          return "HA/HU250(AS1594)";

        case StandardASNZSteelMaterialGrade.HA200_AS1594:
          return "HA200(AS1594)";

        case StandardASNZSteelMaterialGrade.HA4N_AS1594:
          return "HA4N(AS1594)";

        case StandardASNZSteelMaterialGrade.XF400_AS1594:
          return "XF400(AS1594)";

        case StandardASNZSteelMaterialGrade.XF300_AS1594:
          return "XF300(AS1594)";

        case StandardASNZSteelMaterialGrade.Gr450_AS3678:
          return "450(AS3678)";

        case StandardASNZSteelMaterialGrade.Gr400_AS3678:
          return "400(AS3678)";

        case StandardASNZSteelMaterialGrade.Gr350_AS3678:
          return "350(AS3678)";

        case StandardASNZSteelMaterialGrade.WR350_AS3678:
          return "WR350(AS3678)";

        case StandardASNZSteelMaterialGrade.Gr300_AS3678:
          return "300(AS3678)";

        case StandardASNZSteelMaterialGrade.Gr250_AS3678:
          return "250(AS3678)";

        case StandardASNZSteelMaterialGrade.Gr250L15_AS3678:
          return "250L15(AS3678)";

        case StandardASNZSteelMaterialGrade.Gr200_AS3678:
          return "200(AS3678)";

        case StandardASNZSteelMaterialGrade.Gr400_AS3679_1_Flats:
          return "400(AS3679.1 Flats)";

        case StandardASNZSteelMaterialGrade.Gr350_AS3679_1_Flats:
          return "350(AS3679.1 Flats)";

        case StandardASNZSteelMaterialGrade.Gr300_AS3679_1_Flats:
          return "300(AS3679.1 Flats)";

        case StandardASNZSteelMaterialGrade.Gr250_AS3679_1_Flats:
          return "250(AS3679.1 Flats)";

        case StandardASNZSteelMaterialGrade.Gr400_AS3679_1_Hollow:
          return "400(AS3679.1 Hollow)";

        case StandardASNZSteelMaterialGrade.Gr350_AS3679_1_Hollow:
          return "350(AS3679.1 Hollow)";

        case StandardASNZSteelMaterialGrade.Gr300_AS3679_1_Hollow:
          return "300(AS3679.1 Hollow)";

        case StandardASNZSteelMaterialGrade.Gr250_AS3679_1_Hollow:
          return "250(AS3679.1 Hollow)";

        default:
          return "unknown grade";
      }
    }

    internal static StandardASNZSteelMaterialGrade FromString(string grade) {
      switch (grade) {
        case "C450(AS1163)":
          return StandardASNZSteelMaterialGrade.C450_AS1163;

        case "C350(AS1163)":
          return StandardASNZSteelMaterialGrade.C350_AS1163;

        case "C250(AS1163)":
          return StandardASNZSteelMaterialGrade.C250_AS1163;

        case "HA400(AS1594)":
          return StandardASNZSteelMaterialGrade.HA400_AS1594;

        case "HA3(AS1594)":
          return StandardASNZSteelMaterialGrade.HA3_AS1594;

        case "HW350(AS1594)":
          return StandardASNZSteelMaterialGrade.HW350_AS1594;

        case "HA350(AS1594)":
          return StandardASNZSteelMaterialGrade.HA350_AS1594;

        case "HA300/1(AS1594)":
          return StandardASNZSteelMaterialGrade.HA300_1_AS1594;

        case "HA/HU300(AS1594)":
          return StandardASNZSteelMaterialGrade.HA_HU300_AS1594;

        case "HA/HU250(AS1594)":
          return StandardASNZSteelMaterialGrade.HA_HU250_AS1594;

        case "HA200(AS1594)":
          return StandardASNZSteelMaterialGrade.HA200_AS1594;

        case "HA4N(AS1594)":
          return StandardASNZSteelMaterialGrade.HA4N_AS1594;

        case "XF400(AS1594)":
          return StandardASNZSteelMaterialGrade.XF400_AS1594;

        case "XF300(AS1594)":
          return StandardASNZSteelMaterialGrade.XF300_AS1594;

        case "450(AS3678)":
          return StandardASNZSteelMaterialGrade.Gr450_AS3678;

        case "400(AS3678)":
          return StandardASNZSteelMaterialGrade.Gr400_AS3678;

        case "350(AS3678)":
          return StandardASNZSteelMaterialGrade.Gr350_AS3678;

        case "WR350(AS3678)":
          return StandardASNZSteelMaterialGrade.WR350_AS3678;

        case "300(AS3678)":
          return StandardASNZSteelMaterialGrade.Gr300_AS3678;

        case "250(AS3678)":
          return StandardASNZSteelMaterialGrade.Gr250_AS3678;

        case "250L15(AS3678)":
          return StandardASNZSteelMaterialGrade.Gr250L15_AS3678;

        case "200(AS3678)":
          return StandardASNZSteelMaterialGrade.Gr200_AS3678;

        case "400(AS3679.1 Flats)":
          return StandardASNZSteelMaterialGrade.Gr400_AS3679_1_Flats;

        case "350(AS3679.1 Flats)":
          return StandardASNZSteelMaterialGrade.Gr350_AS3679_1_Flats;

        case "300(AS3679.1 Flats)":
          return StandardASNZSteelMaterialGrade.Gr300_AS3679_1_Flats;

        case "250(AS3679.1 Flats)":
          return StandardASNZSteelMaterialGrade.Gr250_AS3679_1_Flats;

        case "400(AS3679.1 Hollow)":
          return StandardASNZSteelMaterialGrade.Gr400_AS3679_1_Hollow;

        case "350(AS3679.1 Hollow)":
          return StandardASNZSteelMaterialGrade.Gr350_AS3679_1_Hollow;

        case "300(AS3679.1 Hollow)":
          return StandardASNZSteelMaterialGrade.Gr300_AS3679_1_Hollow;

        case "250(AS3679.1 Hollow)":
          return StandardASNZSteelMaterialGrade.Gr250_AS3679_1_Hollow;

        default:
          throw new Exception("unknown grade");
      }
    }

    internal void SetValuesFromStandard(StandardASNZSteelMaterialGrade grade) {
      E = new Pressure(200, PressureUnit.Gigapascal);
      Density = new Density(7850, DensityUnit.KilogramPerCubicMeter);
      Grade = grade;

      switch (grade) {
        case StandardASNZSteelMaterialGrade.C450_AS1163:
          Fy = new Pressure(450, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.C350_AS1163:
          Fy = new Pressure(350, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.C250_AS1163:
          Fy = new Pressure(250, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_42;
          break;

        case StandardASNZSteelMaterialGrade.HA400_AS1594:
          Fy = new Pressure(380, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        case StandardASNZSteelMaterialGrade.HW350_AS1594:
          Fy = new Pressure(340, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        case StandardASNZSteelMaterialGrade.HA350_AS1594:
          Fy = new Pressure(350, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.HA300_1_AS1594:
          Fy = new Pressure(300, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.HA_HU300_AS1594:
          Fy = new Pressure(300, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_42;
          break;

        case StandardASNZSteelMaterialGrade.HA_HU250_AS1594:
          Fy = new Pressure(250, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        case StandardASNZSteelMaterialGrade.HA200_AS1594:
          Fy = new Pressure(200, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_50;
          break;

        case StandardASNZSteelMaterialGrade.HA4N_AS1594:
          Fy = new Pressure(170, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.HA3_AS1594:
          Fy = new Pressure(200, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.XF400_AS1594:
          Fy = new Pressure(380, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.XF300_AS1594:
          Fy = new Pressure(300, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr450_AS3678:
          Fy = new Pressure(450, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr400_AS3678:
          Fy = new Pressure(400, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr350_AS3678:
          Fy = new Pressure(360, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.WR350_AS3678:
          Fy = new Pressure(340, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr300_AS3678:
          Fy = new Pressure(320, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr250_AS3678:
          Fy = new Pressure(280, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr250L15_AS3678:
          Fy = new Pressure(280, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr200_AS3678:
          Fy = new Pressure(200, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr400_AS3679_1_Flats:
          Fy = new Pressure(400, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr350_AS3679_1_Flats:
          Fy = new Pressure(360, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr300_AS3679_1_Flats:
          Fy = new Pressure(320, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr250_AS3679_1_Flats:
          Fy = new Pressure(260, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr400_AS3679_1_Hollow:
          Fy = new Pressure(400, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr350_AS3679_1_Hollow:
          Fy = new Pressure(340, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr300_AS3679_1_Hollow:
          Fy = new Pressure(300, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        case StandardASNZSteelMaterialGrade.Gr250_AS3679_1_Hollow:
          Fy = new Pressure(250, PressureUnit.Megapascal);
          WeldGrade = WeldMaterialGrade.Grade_35;
          break;

        default:
          throw new Exception("unknown grade");
      }
    }
  }

  public enum StandardASNZSteelMaterialGrade {
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
}
