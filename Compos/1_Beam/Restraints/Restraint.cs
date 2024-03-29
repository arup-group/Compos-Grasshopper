﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  /// <summary>
  /// Restraint object that contains two <see cref="Supports"/> objects for 'Construction Stage Support' and 'Final Stage Support', and if top flange is laterally restrained in at construction stage.
  /// </summary>
  public class Restraint : IRestraint {
    public ISupports ConstructionStageSupports { get; set; }
    public ISupports FinalStageSupports { get; set; }
    public bool TopFlangeRestrained { get; set; } = true;
    internal bool finalSupportsSet;

    public Restraint() {
      // empty constructor
    }

    public Restraint(bool topFlangeRestrained, ISupports constructionStageSupports, ISupports finalStageSupports) {
      TopFlangeRestrained = topFlangeRestrained;
      ConstructionStageSupports = constructionStageSupports;
      FinalStageSupports = finalStageSupports;
      finalSupportsSet = true;
    }

    public Restraint(bool topFlangeRestrained, ISupports constructionStageSupports) {
      TopFlangeRestrained = topFlangeRestrained;
      ConstructionStageSupports = constructionStageSupports;
      FinalStageSupports = new Supports(IntermediateRestraint.None, true, true);
      finalSupportsSet = false;
    }

    public string ToCoaString(string name, ComposUnits units) {
      string str = "";
      // Construction stage support
      if (TopFlangeRestrained) {
        //RESTRAINT_POINT	MEMBER-1	STANDARD	0
        var parameters = new List<string> {
          "RESTRAINT_POINT",
          name,
          "STANDARD",
          "0"
        };
        str += CoaHelper.CreateString(parameters);
        //RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FIXED
        str += "RESTRAINT_TOP_FALNGE" + '\t' + name + '\t' + "TOP_FLANGE_FIXED" + '\n';
        //RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST
        str += "RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "SEC_BEAM_AS_REST" + '\n';
        //END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE
        str += "END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "FREE_TO_ROTATE" + '\n';
      } else {
        if (ConstructionStageSupports.IntermediateRestraintPositions != IntermediateRestraint.Custom) {
          //RESTRAINT_POINT	MEMBER-1	STANDARD	3
          var parameters = new List<string> {
            "RESTRAINT_POINT",
            name
          };

          switch (ConstructionStageSupports.IntermediateRestraintPositions) {
            case IntermediateRestraint.None:
              parameters.Add("STANDARD");
              parameters.Add("0");
              break;

            case IntermediateRestraint.Mid__Span:
              parameters.Add("STANDARD");
              parameters.Add("1");
              break;

            case IntermediateRestraint.Third_Points:
              parameters.Add("STANDARD");
              parameters.Add("2");
              break;

            case IntermediateRestraint.Quarter_Points:
              parameters.Add("STANDARD");
              parameters.Add("3");
              break;

            default:
              throw new Exception("Unknown intermediate restraint type for construction stage support");
          }
          str += CoaHelper.CreateString(parameters);
        } else {
          //RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
          //RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 2 5.00000 F1L TP MINV
          //RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 3 6.00000 F1L TP MINV
          int count = ConstructionStageSupports.CustomIntermediateRestraintPositions.Count;
          for (int i = 0; i < count; i++) {
            var parameters = new List<string> {
              "RESTRAINT_POINT",
              name,
              "USER_DEFINED",
              count.ToString(),
              (i + 1).ToString(),
              CoaHelper.FormatSignificantFigures(ConstructionStageSupports.CustomIntermediateRestraintPositions[i], units.Length, 6)
            };
            str += CoaHelper.CreateString(parameters);
          }
        }

        //RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FIXED
        str += "RESTRAINT_TOP_FALNGE" + '\t' + name + '\t' + "TOP_FLANGE_FREE" + '\n';

        //RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST
        //RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST
        if (ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint) {
          str += "RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "SEC_BEAM_AS_REST" + '\n';
        } else {
          str += "RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "2ND_BEAM_NOT_AS_REST" + '\n';
        }

        //END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE
        //END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE
        if (ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds) {
          str += "END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "FREE_TO_ROTATE" + '\n';
        } else {
          str += "END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "NOT_FREE_TO_ROTATE" + '\n';
        }
      }

      // Final stage support
      if (!finalSupportsSet) {
        //FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0
        var parameters = new List<string> {
          "FINAL_RESTRAINT_POINT",
          name,
          "STANDARD",
          "0"
        };
        str += CoaHelper.CreateString(parameters);
        //FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FIXED
        str += "FINAL_RESTRAINT_NOSTUD" + '\t' + name + '\t' + "NOSTUD_ZONE_LATERAL_FIXED" + '\n';
        //FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST
        str += "FINAL_RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "SEC_BEAM_AS_REST" + '\n';
        //FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE
        str += "FINAL_END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "FREE_TO_ROTATE" + '\n';
      } else {
        if (FinalStageSupports.IntermediateRestraintPositions != IntermediateRestraint.Custom) {
          //FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	3
          var parameters = new List<string> {
            "FINAL_RESTRAINT_POINT",
            name
          };

          switch (FinalStageSupports.IntermediateRestraintPositions) {
            case IntermediateRestraint.None:
              parameters.Add("STANDARD");
              parameters.Add("0");
              break;

            case IntermediateRestraint.Mid__Span:
              parameters.Add("STANDARD");
              parameters.Add("1");
              break;

            case IntermediateRestraint.Third_Points:
              parameters.Add("STANDARD");
              parameters.Add("2");
              break;

            case IntermediateRestraint.Quarter_Points:
              parameters.Add("STANDARD");
              parameters.Add("3");
              break;

            default:
              throw new Exception("Unknown intermediate restriant type for construction stage support");
          }
          str += CoaHelper.CreateString(parameters);
        } else {
          //FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
          //FINAL_RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 2 5.00000 F1L TP MINV
          //FINAL_RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 3 6.00000 F1L TP MINV
          int count = FinalStageSupports.CustomIntermediateRestraintPositions.Count;
          for (int i = 0; i < count; i++) {
            var parameters = new List<string> {
              "FINAL_RESTRAINT_POINT",
              name,
              "USER_DEFINED",
              count.ToString(),
              (i + 1).ToString(),
              CoaHelper.FormatSignificantFigures(FinalStageSupports.CustomIntermediateRestraintPositions[i], units.Length, 6)
            };
            str += CoaHelper.CreateString(parameters);
          }
        }

        //FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE
        str += "FINAL_RESTRAINT_NOSTUD" + '\t' + name + '\t' + "NOSTUD_ZONE_LATERAL_FREE" + '\n';

        //FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST
        if (FinalStageSupports.SecondaryMemberAsIntermediateRestraint) {
          str += "FINAL_RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "SEC_BEAM_AS_REST" + '\n';
        } else {
          str += "FINAL_RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "2ND_BEAM_NOT_AS_REST" + '\n';
        }

        //FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE
        //FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE
        if (FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds) {
          str += "FINAL_END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "FREE_TO_ROTATE" + '\n';
        } else {
          str += "FINAL_END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "NOT_FREE_TO_ROTATE" + '\n';
        }
      }

      return str;
    }

    public override string ToString() {
      if (FinalStageSupports == null && ConstructionStageSupports == null && TopFlangeRestrained) {
        return "Simply supported";
      }
      string top = TopFlangeRestrained ? "TFLR, " : "";

      string con = "Constr.: simply supported";
      if (!TopFlangeRestrained && ConstructionStageSupports != null) {
        con = "Constr.: " + ConstructionStageSupports.ToString();
      }

      string fin = ", Final: simply supported";
      if (FinalStageSupports != null) {
        fin = ", Final: " + FinalStageSupports.ToString();
      }
      return top + con + fin;
    }

    // not static to update the object
    internal void FromCoaString(List<string> parameters, ComposUnits units) {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      switch (parameters[0]) {
        case CoaIdentifier.RetraintPoint:
          //RESTRAINT_POINT	MEMBER-1	STANDARD	3
          if (parameters[2] == "STANDARD") {
            var intermediateRestraint = new IntermediateRestraint();
            switch (parameters[3]) {
              case "0":
                intermediateRestraint = IntermediateRestraint.None;
                break;

              case "1":
                intermediateRestraint = IntermediateRestraint.Mid__Span;
                break;

              case "2":
                intermediateRestraint = IntermediateRestraint.Third_Points;
                break;

              case "3":
                intermediateRestraint = IntermediateRestraint.Quarter_Points;
                break;

              default:
                throw new Exception("Unknown number of intermediate restraints for construction stage support");
            }
            if (ConstructionStageSupports == null) {
              ConstructionStageSupports = new Supports();
            }
            ConstructionStageSupports = new Supports(intermediateRestraint,
                ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint, ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
          } else if (parameters[2] == "USER_DEFINED") {
            //RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
            //RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 2 5.00000 F1L TP MINV
            //RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 3 6.00000 F1L TP MINV
            //RESTRAINT_POINT	MEMBER-1	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
            //RESTRAINT_POINT	MEMBER-1	USER_DEFINED	3	2	50.0000%	F1L TP MINV
            //RESTRAINT_POINT	MEMBER-1	USER_DEFINED	3	3	100.000%	F12LW TR MAJV MINV
            var positions = new List<IQuantity>();
            if (ConstructionStageSupports == null) {
              ConstructionStageSupports = new Supports();
            }
            if (ConstructionStageSupports.CustomIntermediateRestraintPositions != null && ConstructionStageSupports.CustomIntermediateRestraintPositions.Count != 0) {
              positions = ConstructionStageSupports.CustomIntermediateRestraintPositions.ToList();
            }
            if (parameters[5].EndsWith("%")) {
              positions.Add(new Ratio(Convert.ToDouble(parameters[5].Replace("%", string.Empty), noComma), RatioUnit.Percent));
            } else {
              positions.Add(new Length(Convert.ToDouble(parameters[5], noComma), units.Length));
            }

            ConstructionStageSupports = new Supports(positions,
                ConstructionStageSupports.SecondaryMemberAsIntermediateRestraint, ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
          }
          break;

        case CoaIdentifier.RestraintTopFlange:
          if (parameters[2] == "TOP_FLANGE_FIXED") {
            TopFlangeRestrained = true;
          } else if (parameters[2] == "TOP_FLANGE_FREE") {
            TopFlangeRestrained = false;
          }
          break;

        case CoaIdentifier.Restraint2ndBeam:
          if (ConstructionStageSupports == null) {
            ConstructionStageSupports = new Supports();
          }
          var construction1 = ConstructionStageSupports as Supports;
          if (parameters[2] == "SEC_BEAM_AS_REST") {
            construction1.SecondaryMemberAsIntermediateRestraint = true;
          } else if (parameters[2] == "2ND_BEAM_NOT_AS_REST") {
            construction1.SecondaryMemberAsIntermediateRestraint = false;
          }
          ConstructionStageSupports = construction1;
          break;

        case CoaIdentifier.EndFlangeFreeRotate:
          if (ConstructionStageSupports == null) {
            ConstructionStageSupports = new Supports();
          }
          var construction2 = ConstructionStageSupports as Supports;
          if (parameters[2] == "FREE_TO_ROTATE") {
            construction2.BothFlangesFreeToRotateOnPlanAtEnds = true;
          } else if (parameters[2] == "NOT_FREE_TO_ROTATE") {
            construction2.BothFlangesFreeToRotateOnPlanAtEnds = false;
          }
          ConstructionStageSupports = construction2;
          break;

        case CoaIdentifier.FinalRestraintPoint:
          //FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0
          if (parameters[2] == "STANDARD") {
            var intermediateRestraint = new IntermediateRestraint();
            switch (parameters[3]) {
              case "0":
                intermediateRestraint = IntermediateRestraint.None;
                break;

              case "1":
                intermediateRestraint = IntermediateRestraint.Mid__Span;
                break;

              case "2":
                intermediateRestraint = IntermediateRestraint.Third_Points;
                break;

              case "3":
                intermediateRestraint = IntermediateRestraint.Quarter_Points;
                break;

              default:
                throw new Exception("Unknown number of intermediate restraints for construction stage support");
            }
            if (FinalStageSupports == null) {
              FinalStageSupports = new Supports();
            }
            FinalStageSupports = new Supports(intermediateRestraint,
                FinalStageSupports.SecondaryMemberAsIntermediateRestraint, FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
          } else if (parameters[2] == "USER_DEFINED") {
            //FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
            //FINAL_RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 2 5.00000 F1L TP MINV
            //FINAL_RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 3 6.00000 F1L TP MINV
            var positions = new List<IQuantity>();
            if (FinalStageSupports == null) {
              FinalStageSupports = new Supports();
            }
            if (FinalStageSupports.CustomIntermediateRestraintPositions != null & FinalStageSupports.CustomIntermediateRestraintPositions.Count != 0) {
              positions = FinalStageSupports.CustomIntermediateRestraintPositions.ToList();
            }
            if (parameters[5].EndsWith("%")) {
              positions.Add(new Ratio(Convert.ToDouble(parameters[5].Replace("%", string.Empty), noComma), RatioUnit.Percent));
            } else {
              positions.Add(new Length(Convert.ToDouble(parameters[5], noComma), units.Length));
            }

            FinalStageSupports = new Supports(positions,
                FinalStageSupports.SecondaryMemberAsIntermediateRestraint, FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
          }
          break;

        case CoaIdentifier.FinalRestraintNoStud:
          if (parameters[2] == "NOSTUD_ZONE_LATERAL_FIXED") {
            finalSupportsSet = false;
          } else if (parameters[2] == "NOSTUD_ZONE_LATERAL_FREE") {
            finalSupportsSet = true;
          }
          break;

        case CoaIdentifier.FinalRestraint2ndBeam:
          if (FinalStageSupports == null) {
            FinalStageSupports = new Supports();
          }
          var final1 = FinalStageSupports as Supports;
          if (parameters[2] == "SEC_BEAM_AS_REST") {
            final1.SecondaryMemberAsIntermediateRestraint = true;
          } else if (parameters[2] == "2ND_BEAM_NOT_AS_REST") {
            final1.SecondaryMemberAsIntermediateRestraint = false;
          }
          FinalStageSupports = final1;
          break;

        case CoaIdentifier.FinalEndFlangeFreeRotate:
          if (FinalStageSupports == null) {
            FinalStageSupports = new Supports();
          }

          var final2 = FinalStageSupports as Supports;
          if (parameters[2] == "FREE_TO_ROTATE") {
            final2.BothFlangesFreeToRotateOnPlanAtEnds = true;
          } else if (parameters[2] == "NOT_FREE_TO_ROTATE") {
            final2.BothFlangesFreeToRotateOnPlanAtEnds = false;
          }

          FinalStageSupports = final2;
          break;
      }
    }
  }
}
