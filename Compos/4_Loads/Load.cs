using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ComposAPI {
  public enum LoadType {
    Point,
    Uniform,
    Linear,
    TriLinear,
    Patch,
    MemberLoad,
    Axial
  }

  public enum LoadDistribution {
    Line,
    Area
  }

  public class Load : ILoad {
    public LoadType Type => m_type;
    internal LoadType m_type;

    #region constructors
    public Load() {
      // empty constructor
    }
    #endregion

    #region coa interop
    internal static IList<ILoad> FromCoaString(string coaString, string name, ComposUnits units) {
      var loads = new List<ILoad>();

      ForceUnit forceUnit = units.Force;
      LengthUnit lengthUnit = units.Length;
      ForcePerLengthUnit forcePerLengthUnit = ComposUnitsHelper.GetForcePerLengthUnit(forceUnit, lengthUnit);
      PressureUnit forcePerAreaUnit = ComposUnitsHelper.GetForcePerAreaUnit(forceUnit, lengthUnit);

      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines) {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END") {
          return loads;
        }

        if (parameters[0] == CoaIdentifier.UnitData) {
          units.FromCoaString(parameters);
        }

        if (parameters[1] != name) {
          continue;
        }

        if (parameters[0] == CoaIdentifier.Load) {
          int i = 2;

          switch (parameters[i++]) {
            case CoaIdentifier.Loads.PointLoad:

              loads.Add(new PointLoad(
                new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit)
                ));
              break;

            case CoaIdentifier.Loads.UniformLoad:
              if (parameters[i++] == CoaIdentifier.Loads.DistributionLinear) {
                loads.Add(new UniformLoad(
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit)
                  ));
              }
              else {
                loads.Add(new UniformLoad(
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit)
                  ));
              }
              break;

            case CoaIdentifier.Loads.LinearLoad:
              if (parameters[i++] == CoaIdentifier.Loads.DistributionLinear) {
                loads.Add(new LinearLoad(
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit)
                  ));
              }
              else {
                loads.Add(new LinearLoad(
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit)
                  ));
              }

              break;

            case CoaIdentifier.Loads.TriLinearLoad:
              if (parameters[i++] == CoaIdentifier.Loads.DistributionLinear) {
                loads.Add(new TriLinearLoad(
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit)
                  ));
              }
              else {
                loads.Add(new TriLinearLoad(
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit)
                  ));
              }

              break;

            case CoaIdentifier.Loads.PatchLoad:
              if (parameters[i++] == CoaIdentifier.Loads.DistributionLinear) {
                loads.Add(new PatchLoad(
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  new ForcePerLength(Convert.ToDouble(parameters[i++], noComma), forcePerLengthUnit),
                  CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit)
                  ));
              }
              else {
                loads.Add(new PatchLoad(
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  new Pressure(Convert.ToDouble(parameters[i++], noComma), forcePerAreaUnit),
                  CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit)
                  ));
              }

              break;

            case CoaIdentifier.Loads.AxialLoad:
              loads.Add(new AxialLoad(
                  new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                  new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                  new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                  new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                  new Length(Convert.ToDouble(parameters[i++], noComma), lengthUnit),
                  new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                  new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                  new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                  new Force(Convert.ToDouble(parameters[i++], noComma), forceUnit),
                  new Length(Convert.ToDouble(parameters[i++], noComma), lengthUnit)
                  ));
              break;

            case CoaIdentifier.Loads.MemberLoad:
              loads.Add(new MemberLoad(parameters[i++],
                (MemberLoad.SupportSide)Enum.Parse(typeof(MemberLoad.SupportSide), parameters[i++]),
                CoaHelper.ConvertToLengthOrRatio(parameters[i++], lengthUnit)));
              break;

            default:
              // continue;
              break;
          }
        }
      }
      return loads;
    }

    public string ToCoaString(string name, ComposUnits units) {
      // | LOAD | name |
      string str = "LOAD" + '\t' + name + '\t';

      Enum unit;

      switch (Type) {
        case LoadType.Point:
          // | LOAD | name | type | ConsDead1 | ConsLive1 | FinalDead1 | FinalLive1 | Dist1(for load types of "Point")
          var pointLoad = (PointLoad)this;
          // | type |
          str += "Point" + '\t';
          unit = units.Force;
          // | ConsDead1 |
          str += CoaHelper.FormatSignificantFigures(pointLoad.Load.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive1 |
          str += CoaHelper.FormatSignificantFigures(pointLoad.Load.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead1 |
          str += CoaHelper.FormatSignificantFigures(pointLoad.Load.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive1 |
          str += CoaHelper.FormatSignificantFigures(pointLoad.Load.FinalLive.ToUnit(unit).Value, 6) + '\t';
          // | Dist1 |
          str += CoaHelper.FormatSignificantFigures(pointLoad.Load.Position, units.Length, 6) + '\t';

          break;

        case LoadType.Uniform:
          // | LOAD | name | type | Distribution |ConsDead1 | ConsLive1 | FinalDead1 | FinalLive1(for load types of "Uniform")
          var uniformLoad = (UniformLoad)this;
          // | type |
          str += "Uniform" + '\t';
          // | Distribution |
          str += (uniformLoad.Distribution == LoadDistribution.Line) ? "Line" + '\t' : "Area" + '\t';
          if (uniformLoad.Distribution == LoadDistribution.Line) {
            unit = ComposUnitsHelper.GetForcePerLengthUnit(units.Force, units.Length);
          }
          else {
            unit = ComposUnitsHelper.GetForcePerAreaUnit(units.Force, units.Length);
          }
          // | ConsDead1 |
          str += CoaHelper.FormatSignificantFigures(uniformLoad.Load.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive1 |
          str += CoaHelper.FormatSignificantFigures(uniformLoad.Load.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead1 |
          str += CoaHelper.FormatSignificantFigures(uniformLoad.Load.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive1 |
          str += CoaHelper.FormatSignificantFigures(uniformLoad.Load.FinalLive.ToUnit(unit).Value, 6) + '\t';

          break;

        case LoadType.Linear:
          // | LOAD | name | type | Distribution |ConsDead1 | ConsLive1 | FinalDead1 | FinalLive1 | ConsDead2 | ConsLive2 | FinalDead2 | FinalLive2 | (for load types of "Linear")
          var linearLoad = (LinearLoad)this;
          // | type |
          str += "Linear" + '\t';
          // | Distribution |
          str += (linearLoad.Distribution == LoadDistribution.Line) ? "Line" + '\t' : "Area" + '\t';
          if (linearLoad.Distribution == LoadDistribution.Line) {
            unit = ComposUnitsHelper.GetForcePerLengthUnit(units.Force, units.Length);
          }
          else {
            unit = ComposUnitsHelper.GetForcePerAreaUnit(units.Force, units.Length);
          }
          // | ConsDead1 |
          str += CoaHelper.FormatSignificantFigures(linearLoad.LoadW1.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive1 |
          str += CoaHelper.FormatSignificantFigures(linearLoad.LoadW1.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead1 |
          str += CoaHelper.FormatSignificantFigures(linearLoad.LoadW1.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive1 |
          str += CoaHelper.FormatSignificantFigures(linearLoad.LoadW1.FinalLive.ToUnit(unit).Value, 6) + '\t';
          // | ConsDead2 |
          str += CoaHelper.FormatSignificantFigures(linearLoad.LoadW2.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive2 |
          str += CoaHelper.FormatSignificantFigures(linearLoad.LoadW2.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead2 |
          str += CoaHelper.FormatSignificantFigures(linearLoad.LoadW2.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive@ |
          str += CoaHelper.FormatSignificantFigures(linearLoad.LoadW2.FinalLive.ToUnit(unit).Value, 6) + '\t';
          break;

        case LoadType.TriLinear:
          // | LOAD | name | type | Distribution |ConsDead1 | ConsLive1 | FinalDead1 | FinalLive1 | Dist1 | ConsDead2 | ConsLive2 | FinalDead2 | FinalLive2 | Dist2(for load types of "Tri-Linear" & "Patch")
          var triLinearLoad = (TriLinearLoad)this;
          // | type |
          str += "Tri-Linear" + '\t';
          // | Distribution |
          str += (triLinearLoad.Distribution == LoadDistribution.Line) ? "Line" + '\t' : "Area" + '\t';
          if (triLinearLoad.Distribution == LoadDistribution.Line) {
            unit = ComposUnitsHelper.GetForcePerLengthUnit(units.Force, units.Length);
          }
          else {
            unit = ComposUnitsHelper.GetForcePerAreaUnit(units.Force, units.Length);
          }
          // | ConsDead1 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW1.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive1 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW1.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead1 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW1.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive1 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW1.FinalLive.ToUnit(unit).Value, 6) + '\t';
          // | Dist1 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW1.Position, units.Length, 6) + '\t';
          // | ConsDead2 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW2.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive2 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW2.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead2 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW2.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive@ |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW2.FinalLive.ToUnit(unit).Value, 6) + '\t';
          // | Dist2 |
          str += CoaHelper.FormatSignificantFigures(triLinearLoad.LoadW2.Position, units.Length, 6) + '\t';
          break;

        case LoadType.Patch:
          // | LOAD | name | type | Distribution |ConsDead1 | ConsLive1 | FinalDead1 | FinalLive1 | Dist1 | ConsDead2 | ConsLive2 | FinalDead2 | FinalLive2 | Dist2(for load types of "Tri-Linear" & "Patch")
          var patchLoad = (PatchLoad)this;
          // | type |
          str += "Patch" + '\t';
          // | Distribution |
          str += (patchLoad.Distribution == LoadDistribution.Line) ? "Line" + '\t' : "Area" + '\t';
          if (patchLoad.Distribution == LoadDistribution.Line) {
            unit = ComposUnitsHelper.GetForcePerLengthUnit(units.Force, units.Length);
          }
          else {
            unit = ComposUnitsHelper.GetForcePerAreaUnit(units.Force, units.Length);
          }
          // | ConsDead1 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW1.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive1 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW1.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead1 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW1.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive1 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW1.FinalLive.ToUnit(unit).Value, 6) + '\t';
          // | Dist1 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW1.Position, units.Length, 6) + '\t';
          // | ConsDead2 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW2.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive2 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW2.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead2 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW2.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive@ |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW2.FinalLive.ToUnit(unit).Value, 6) + '\t';
          // | Dist2 |
          str += CoaHelper.FormatSignificantFigures(patchLoad.LoadW2.Position, units.Length, 6) + '\t';
          break;

        case LoadType.MemberLoad:
          // | LOAD | name | type | mem-name | support | pos(for load types of "Member load")
          var memberLoad = (MemberLoad)this;
          // | type |
          str += "Member load" + '\t';
          // | mem-name |
          str += memberLoad.MemberName.ToUpper() + '\t';
          // | support |
          str += memberLoad.Support.ToString() + '\t';
          // | pos |
          str += CoaHelper.FormatSignificantFigures(memberLoad.Position, units.Length, 6) + '\t';
          break;

        case LoadType.Axial:
          // | LOAD | name | type | ConsDead1 | ConsLive1 | FinalDead1 | FinalLive1 | DistFromTop1 |ConsDead2 | ConsLive2 | FinalDead2 | FinalLive2 | DistFromTop2(for load type"Axial")
          var axialLoad = (AxialLoad)this;
          // | type |
          str += "Axial" + '\t';
          unit = units.Force;
          // | ConsDead1 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.LoadW1.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive1 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.LoadW1.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead1 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.LoadW1.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive1 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.LoadW1.FinalLive.ToUnit(unit).Value, 6) + '\t';
          // | Dist1 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.Depth1.ToUnit(units.Length).Value, 6) + '\t';
          // | ConsDead2 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.LoadW2.ConstantDead.ToUnit(unit).Value, 6) + '\t';
          // | ConsLive2 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.LoadW2.ConstantLive.ToUnit(unit).Value, 6) + '\t';
          // | FinalDead2 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.LoadW2.FinalDead.ToUnit(unit).Value, 6) + '\t';
          // | FinalLive@ |
          str += CoaHelper.FormatSignificantFigures(axialLoad.LoadW2.FinalLive.ToUnit(unit).Value, 6) + '\t';
          // | Dist2 |
          str += CoaHelper.FormatSignificantFigures(axialLoad.Depth2.ToUnit(units.Length).Value, 6) + '\t';
          break;

        default:
          throw new NotImplementedException();
          // | LOAD | name | type | Distribution |ConsDead1 | ConsLive1 | FinalDead1 | FinalLive1 | Dist1 | Dist2(for load types of "Area UDL")
          // | LOAD | name | type | ConsDead1 | ConsLive1 | FinalDead1 | FinalLive1 |ConsDead2 | ConsLive2 | FinalDead2 | FinalLive2(for load type "Moment")
          //case LoadType.Moment:
          //str += "Moment" + '\t';
      }
      str = str.Remove(str.Length - 1, 1);
      str += '\n';
      return str;
    }
    #endregion

    #region methods
    public override string ToString() {
      // update with better naming
      return Type.ToString() + " Load";
    }
    #endregion
  }
}
