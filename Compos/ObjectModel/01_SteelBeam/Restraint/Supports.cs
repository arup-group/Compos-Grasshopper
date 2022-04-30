using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI.SteelBeam
{
  /// <summary>
  /// Object with information about support conditions. The Supports object is required for input(s) when creating a <see cref="Restraint"/> object.
  /// </summary>
  public class Supports
  {
    public bool SecondaryMemberIntermediateRestraint { get; set; }
    public bool BothFlangesFreeToRotateOnPlanAtEnds { get; set; }
    public List<Length> CustomIntermediateRestraintPositions 
    { 
      get { return this.m_custompositions; }
      set 
      {
        this.IntermediateRestraintPositions = IntermediateRestraint.Custom;
        this.m_custompositions = value;
      }
    }
    private List<Length> m_custompositions = null;
    public enum IntermediateRestraint
    {
      None,
      Mid__Span,
      Third_Points,
      Quarter_Points,
      Custom
    }
    public IntermediateRestraint IntermediateRestraintPositions { get; set; }

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

    public Supports Duplicate()
    {
      if (this == null) { return null; }
      Supports dup = (Supports)this.MemberwiseClone();
      return dup;
    }
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
