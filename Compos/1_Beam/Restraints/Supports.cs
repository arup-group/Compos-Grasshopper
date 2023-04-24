using System.Collections.Generic;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public enum IntermediateRestraint {
    None,
    Mid__Span,
    Third_Points,
    Quarter_Points,
    Custom
  }

  /// <summary>
  /// Object with information about support conditions. The Supports object is required for input(s) when creating a <see cref="Restraint"/> object.
  /// </summary>
  public class Supports : ISupports {
    public bool BothFlangesFreeToRotateOnPlanAtEnds { get; set; } = false;
    public IList<IQuantity> CustomIntermediateRestraintPositions { get; set; }
    public IntermediateRestraint IntermediateRestraintPositions {
      get {
        if (CustomIntermediateRestraintPositions != null) {
          return IntermediateRestraint.Custom;
        } else {
          return m_intermediateRestraints;
        }
      }
      set => m_intermediateRestraints = value;
    }
    public bool SecondaryMemberAsIntermediateRestraint { get; set; } = true;
    private IntermediateRestraint m_intermediateRestraints;

    public Supports() {
      IntermediateRestraintPositions = IntermediateRestraint.None;
    }

    public Supports(List<IQuantity> customIntermediateRestraintPositions, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds) {
      CustomIntermediateRestraintPositions = customIntermediateRestraintPositions;
      SecondaryMemberAsIntermediateRestraint = secondaryMemberIntermediateRestraint;
      BothFlangesFreeToRotateOnPlanAtEnds = bothFlangesFreeToRotateOnPlanAtEnds;
      IntermediateRestraintPositions = IntermediateRestraint.Custom;
    }

    public Supports(IntermediateRestraint intermediateRestraintPositions, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds) {
      IntermediateRestraintPositions = intermediateRestraintPositions;
      SecondaryMemberAsIntermediateRestraint = secondaryMemberIntermediateRestraint;
      BothFlangesFreeToRotateOnPlanAtEnds = bothFlangesFreeToRotateOnPlanAtEnds;
    }

    public override string ToString() {
      string sec = SecondaryMemberAsIntermediateRestraint ? ", SMIR" : "";
      string flange = BothFlangesFreeToRotateOnPlanAtEnds ? ", FFRE" : "";
      string res = IntermediateRestraintPositions.ToString().Replace("__", "-").Replace("_", " ");
      if (CustomIntermediateRestraintPositions != null) {
        res = "Custom:{";
        foreach (IQuantity pos in CustomIntermediateRestraintPositions) {
          res += " ";
          if (pos.QuantityInfo.UnitType == typeof(LengthUnit)) {
            var l = (Length)pos;
            res += l.ToString("g2").Replace(" ", string.Empty);
          } else {
            var p = (Ratio)pos;
            res += p.ToString("g2").Replace(" ", string.Empty);
          }
          res += ",";
        }
        res = res.TrimEnd(',');
        res += "}";
      }

      return res + sec + flange;
    }
  }
}
