using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  public enum IntermediateRestraint
  {
    None,
    Mid__Span,
    Third_Points,
    Quarter_Points,
    Custom
  }

  /// <summary>
  /// Object with information about support conditions. The Supports object is required for input(s) when creating a <see cref="Restraint"/> object.
  /// </summary>
  public class Supports : ISupports
  {
    public bool SecondaryMemberIntermediateRestraint { get; set; }
    public bool BothFlangesFreeToRotateOnPlanAtEnds { get; set; }
    public IList<Length> CustomIntermediateRestraintPositions { get; set; }
    public IntermediateRestraint IntermediateRestraintPositions
    {
      get 
      {
        if (CustomIntermediateRestraintPositions != null)
          return IntermediateRestraint.Custom;
        else
          return m_intermediateRestraints; 
      }
      set
      {
        this.m_intermediateRestraints = value;
      }
    }
    private IntermediateRestraint m_intermediateRestraints;

    #region constructors
    public Supports()
    {
      // empty constructor
    }
    public Supports(List<Length> customIntermediateRestraintPositions, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds)
    {
      this.CustomIntermediateRestraintPositions = customIntermediateRestraintPositions;
      this.SecondaryMemberIntermediateRestraint = secondaryMemberIntermediateRestraint;
      this.BothFlangesFreeToRotateOnPlanAtEnds = bothFlangesFreeToRotateOnPlanAtEnds;
      this.IntermediateRestraintPositions = IntermediateRestraint.Custom;
    }
    public Supports(IntermediateRestraint intermediateRestraintPositions, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds)
    {
      this.IntermediateRestraintPositions = intermediateRestraintPositions;
      this.SecondaryMemberIntermediateRestraint = secondaryMemberIntermediateRestraint;
      this.BothFlangesFreeToRotateOnPlanAtEnds = bothFlangesFreeToRotateOnPlanAtEnds;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string sec = (this.SecondaryMemberIntermediateRestraint) ? ", SMIR" : "";
      string flange = (this.BothFlangesFreeToRotateOnPlanAtEnds) ? ", FFRE" : "";
      string res = this.IntermediateRestraintPositions.ToString().Replace("__", "-").Replace("_", " ");
      return res + sec + flange;
    }
    #endregion
  }
}
