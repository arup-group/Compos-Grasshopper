using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{

  /// <summary>
  /// Restraint object that contains two <see cref="Supports"/> objects for 'Construction Stage Support' and 'Final Stage Support', and if top flange is laterally restrained in at construction stage.
  /// </summary>
  public class Restraint : IRestraint
  {
    public ISupports ConstructionStageSupports { get; set; }
    public ISupports FinalStageSupports { get; set; }
    public bool TopFlangeRestrained { get; set; }
    internal bool finalSupportsSet;

    #region constructors
    public Restraint()
    {
      // empty constructor
    }
    public Restraint(bool topFlangeRestrained, ISupports constructionStageSupports, ISupports finalStageSupports)
    {
      this.TopFlangeRestrained = topFlangeRestrained;
      this.ConstructionStageSupports = constructionStageSupports;
      this.FinalStageSupports = finalStageSupports;
      this.finalSupportsSet = true;
    }
    public Restraint(bool topFlangeRestrained, ISupports constructionStageSupports)
    {
      this.TopFlangeRestrained = topFlangeRestrained;
      this.ConstructionStageSupports = constructionStageSupports;
      this.FinalStageSupports = new Supports(IntermediateRestraint.None, true, true);
      this.finalSupportsSet = false;
    }
    #endregion

    #region coa interop
    internal Restraint FromCoaString(List<string> parameters, ComposUnits units)
    {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      switch (parameters[0])
      {
        case CoaIdentifier.RetraintPoint:
          //RESTRAINT_POINT	MEMBER-1	STANDARD	3
          if (parameters[2] == "STANDARD")
          {
            IntermediateRestraint intermediateRestraint = new IntermediateRestraint();
            switch (parameters[3])
            {
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
            if (this.ConstructionStageSupports == null)
              this.ConstructionStageSupports = new Supports();
            this.ConstructionStageSupports = new Supports(intermediateRestraint,
                this.ConstructionStageSupports.SecondaryMemberIntermediateRestraint, this.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
          }
          else if (parameters[2] == "USER_DEFINED")
          {
            //RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
            //RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 2 5.00000 F1L TP MINV
            //RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 3 6.00000 F1L TP MINV
            //RESTRAINT_POINT	MEMBER-1	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
            //RESTRAINT_POINT	MEMBER-1	USER_DEFINED	3	2	50.0000%	F1L TP MINV
            //RESTRAINT_POINT	MEMBER-1	USER_DEFINED	3	3	100.000%	F12LW TR MAJV MINV
            List<IQuantity> positions = new List<IQuantity>();
            if (this.ConstructionStageSupports == null)
              this.ConstructionStageSupports = new Supports();
            if (this.ConstructionStageSupports.CustomIntermediateRestraintPositions != null & this.ConstructionStageSupports.CustomIntermediateRestraintPositions.Count != 0)
              positions = this.ConstructionStageSupports.CustomIntermediateRestraintPositions.ToList();
            if (parameters[5].EndsWith("%"))
              positions.Add(new Ratio(Convert.ToDouble(parameters[5].Replace("%", string.Empty), noComma), RatioUnit.Percent));
            else
              positions.Add(new Length(Convert.ToDouble(parameters[5], noComma), units.Length));

            this.ConstructionStageSupports = new Supports(positions,
                this.ConstructionStageSupports.SecondaryMemberIntermediateRestraint, this.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
          }
          break;

        case CoaIdentifier.RestraintTopFlange:
          if (parameters[2] == "TOP_FLANGE_FIXED")
            this.TopFlangeRestrained = true;
          else if (parameters[2] == "TOP_FLANGE_FREE")
            this.TopFlangeRestrained = false;
          break;

        case CoaIdentifier.Restraint2ndBeam:
          if (this.ConstructionStageSupports == null)
            this.ConstructionStageSupports = new Supports();
          Supports construction1 = this.ConstructionStageSupports as Supports;
          if (parameters[2] == "SEC_BEAM_AS_REST")
            construction1.SecondaryMemberIntermediateRestraint = true;
          else if (parameters[2] == "2ND_BEAM_NOT_AS_REST")
            construction1.SecondaryMemberIntermediateRestraint = false;
          this.ConstructionStageSupports = construction1;
          break;

        case CoaIdentifier.EndFlangeFreeRotate:
          if (this.ConstructionStageSupports == null)
            this.ConstructionStageSupports = new Supports();
          Supports construction2 = this.ConstructionStageSupports as Supports;
          if (parameters[2] == "FREE_TO_ROTATE")
            construction2.BothFlangesFreeToRotateOnPlanAtEnds = true;
          else if (parameters[2] == "NOT_FREE_TO_ROTATE")
            construction2.BothFlangesFreeToRotateOnPlanAtEnds = false;
          this.ConstructionStageSupports = construction2;
          break;

        case CoaIdentifier.FinalRestraintPoint:
          //FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0
          if (parameters[2] == "STANDARD")
          {
            IntermediateRestraint intermediateRestraint = new IntermediateRestraint();
            switch (parameters[3])
            {
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
            if (this.FinalStageSupports == null)
              this.FinalStageSupports = new Supports();
            this.FinalStageSupports = new Supports(intermediateRestraint,
                this.FinalStageSupports.SecondaryMemberIntermediateRestraint, this.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
          }
          else if (parameters[2] == "USER_DEFINED")
          {
            //FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
            //FINAL_RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 2 5.00000 F1L TP MINV
            //FINAL_RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 3 6.00000 F1L TP MINV
            List<IQuantity> positions = new List<IQuantity>();
            if (this.FinalStageSupports == null)
              this.FinalStageSupports = new Supports();
            if (this.FinalStageSupports.CustomIntermediateRestraintPositions != null & this.FinalStageSupports.CustomIntermediateRestraintPositions.Count != 0)
              positions = this.FinalStageSupports.CustomIntermediateRestraintPositions.ToList();
            if (parameters[5].EndsWith("%"))
              positions.Add(new Ratio(Convert.ToDouble(parameters[5].Replace("%", string.Empty), noComma), RatioUnit.Percent));
            else
              positions.Add(new Length(Convert.ToDouble(parameters[5], noComma), units.Length));

            this.FinalStageSupports = new Supports(positions,
                this.FinalStageSupports.SecondaryMemberIntermediateRestraint, this.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds);
          }
          break;

        case CoaIdentifier.FinalRestraintNoStud:
          if (parameters[2] == "NOSTUD_ZONE_LATERAL_FIXED")
            this.finalSupportsSet = false;
          else if (parameters[2] == "NOSTUD_ZONE_LATERAL_FREE")
            this.finalSupportsSet = true;
          break;

        case CoaIdentifier.FinalRestraint2ndBeam:
          if (this.FinalStageSupports == null)
            this.FinalStageSupports = new Supports();
          Supports final1 = this.FinalStageSupports as Supports;
          if (parameters[2] == "SEC_BEAM_AS_REST")
            final1.SecondaryMemberIntermediateRestraint = true;
          else if (parameters[2] == "2ND_BEAM_NOT_AS_REST")
            final1.SecondaryMemberIntermediateRestraint = false;
          this.FinalStageSupports = final1;
          break;

        case CoaIdentifier.FinalEndFlangeFreeRotate:
          if (this.FinalStageSupports == null)
            this.FinalStageSupports = new Supports();
          Supports final2 = this.FinalStageSupports as Supports;
          if (parameters[2] == "FREE_TO_ROTATE")
            final2.BothFlangesFreeToRotateOnPlanAtEnds = true;
          else if (parameters[2] == "NOT_FREE_TO_ROTATE")
            final2.BothFlangesFreeToRotateOnPlanAtEnds = false;
          this.FinalStageSupports = final2;
          break;
      }
      return this;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      string str = "";
      // Construction stage support
      if (this.TopFlangeRestrained)
      {
        //RESTRAINT_POINT	MEMBER-1	STANDARD	0
        List<string> parameters = new List<string>();
        parameters.Add("RESTRAINT_POINT");
        parameters.Add(name);
        parameters.Add("STANDARD");
        parameters.Add("0");
        str += CoaHelper.CreateString(parameters);
        //RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FIXED
        str += "RESTRAINT_TOP_FALNGE" + '\t' + name + '\t' + "TOP_FLANGE_FIXED" + '\n';
        //RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST
        str += "RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "SEC_BEAM_AS_REST" + '\n';
        //END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE
        str += "END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "FREE_TO_ROTATE" + '\n';
      }
      else
      {
        if (this.ConstructionStageSupports.IntermediateRestraintPositions != IntermediateRestraint.Custom)
        {
          //RESTRAINT_POINT	MEMBER-1	STANDARD	3
          List<string> parameters = new List<string>();
          parameters.Add("RESTRAINT_POINT");
          parameters.Add(name);

          switch (this.ConstructionStageSupports.IntermediateRestraintPositions)
          {

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
        }
        else
        {
          //RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
          //RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 2 5.00000 F1L TP MINV
          //RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 3 6.00000 F1L TP MINV
          int count = this.ConstructionStageSupports.CustomIntermediateRestraintPositions.Count;
          for (int i = 0; i < count; i++)
          {
            List<string> parameters = new List<string>();
            parameters.Add("RESTRAINT_POINT");
            parameters.Add(name);
            parameters.Add("USER_DEFINED");
            parameters.Add(count.ToString());
            parameters.Add((i + 1).ToString());
            parameters.Add(CoaHelper.FormatSignificantFigures(this.ConstructionStageSupports.CustomIntermediateRestraintPositions[i], units.Length, 6));
            str += CoaHelper.CreateString(parameters);
          }
        }

        //RESTRAINT_TOP_FALNGE	MEMBER-1	TOP_FLANGE_FIXED
        str += "RESTRAINT_TOP_FALNGE" + '\t' + name + '\t' + "TOP_FLANGE_FREE" + '\n';

        //RESTRAINT_2ND_BEAM	MEMBER-1	2ND_BEAM_NOT_AS_REST
        //RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST
        if (this.ConstructionStageSupports.SecondaryMemberIntermediateRestraint)
          str += "RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "SEC_BEAM_AS_REST" + '\n';
        else
          str += "RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "2ND_BEAM_NOT_AS_REST" + '\n';

        //END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE
        //END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE
        if (this.ConstructionStageSupports.BothFlangesFreeToRotateOnPlanAtEnds)
          str += "END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "FREE_TO_ROTATE" + '\n';
        else
          str += "END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "NOT_FREE_TO_ROTATE" + '\n';
      }

      // Final stage support
      if (!this.finalSupportsSet)
      {
        //FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	0
        List<string> parameters = new List<string>();
        parameters.Add("FINAL_RESTRAINT_POINT");
        parameters.Add(name);
        parameters.Add("STANDARD");
        parameters.Add("0");
        str += CoaHelper.CreateString(parameters);
        //FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FIXED
        str += "FINAL_RESTRAINT_NOSTUD" + '\t' + name + '\t' + "NOSTUD_ZONE_LATERAL_FIXED" + '\n';
        //FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST
        str += "FINAL_RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "SEC_BEAM_AS_REST" + '\n';
        //FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE
        str += "FINAL_END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "FREE_TO_ROTATE" + '\n';
      }
      else
      {
        if (this.FinalStageSupports.IntermediateRestraintPositions != IntermediateRestraint.Custom)
        {
          //FINAL_RESTRAINT_POINT	MEMBER-1	STANDARD	3
          List<string> parameters = new List<string>();
          parameters.Add("FINAL_RESTRAINT_POINT");
          parameters.Add(name);

          switch (this.FinalStageSupports.IntermediateRestraintPositions)
          {

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
        }
        else
        {
          //FINAL_RESTRAINT_POINT	MEMBER-2	USER_DEFINED	3	1	0.000000	F12LW TR MAJV MINV
          //FINAL_RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 2 5.00000 F1L TP MINV
          //FINAL_RESTRAINT_POINT MEMBER-2 USER_DEFINED 3 3 6.00000 F1L TP MINV
          int count = this.FinalStageSupports.CustomIntermediateRestraintPositions.Count;
          for (int i = 0; i < count; i++)
          {
            List<string> parameters = new List<string>();
            parameters.Add("FINAL_RESTRAINT_POINT");
            parameters.Add(name);
            parameters.Add("USER_DEFINED");
            parameters.Add(count.ToString());
            parameters.Add((i + 1).ToString());
            parameters.Add(CoaHelper.FormatSignificantFigures(this.FinalStageSupports.CustomIntermediateRestraintPositions[i], units.Length, 6));
            str += CoaHelper.CreateString(parameters);
          }
        }

        //FINAL_RESTRAINT_NOSTUD	MEMBER-1	NOSTUD_ZONE_LATERAL_FREE
        str += "FINAL_RESTRAINT_NOSTUD" + '\t' + name + '\t' + "NOSTUD_ZONE_LATERAL_FREE" + '\n';

        //FINAL_RESTRAINT_2ND_BEAM	MEMBER-1	SEC_BEAM_AS_REST
        if (this.FinalStageSupports.SecondaryMemberIntermediateRestraint)
          str += "FINAL_RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "SEC_BEAM_AS_REST" + '\n';
        else
          str += "FINAL_RESTRAINT_2ND_BEAM" + '\t' + name + '\t' + "2ND_BEAM_NOT_AS_REST" + '\n';

        //FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	FREE_TO_ROTATE
        //FINAL_END_FLANGE_FREE_ROTATE	MEMBER-1	NOT_FREE_TO_ROTATE
        if (this.FinalStageSupports.BothFlangesFreeToRotateOnPlanAtEnds)
          str += "FINAL_END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "FREE_TO_ROTATE" + '\n';
        else
          str += "FINAL_END_FLANGE_FREE_ROTATE" + '\t' + name + '\t' + "NOT_FREE_TO_ROTATE" + '\n';
      }

      return str;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string top = (TopFlangeRestrained) ? "TFLR, " : "";
      string con = "Constr.: " + this.ConstructionStageSupports.ToString();
      string fin = ", Final: None";
      if (this.FinalStageSupports != null)
        fin = ", Final: " + this.FinalStageSupports.ToString();
      return top + con + fin;
    }

    #endregion
  }
}
