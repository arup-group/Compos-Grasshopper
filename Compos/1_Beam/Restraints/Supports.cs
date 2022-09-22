using System.Collections.Generic;
using OasysUnits;
using OasysUnits.Units;

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
    public bool SecondaryMemberAsIntermediateRestraint { get; set; } = true;
    public bool BothFlangesFreeToRotateOnPlanAtEnds { get; set; } = false;
    public IList<IQuantity> CustomIntermediateRestraintPositions { get; set; }
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
      this.IntermediateRestraintPositions = IntermediateRestraint.None;
    }
    public Supports(List<IQuantity> customIntermediateRestraintPositions, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds)
    {
      this.CustomIntermediateRestraintPositions = customIntermediateRestraintPositions;
      this.SecondaryMemberAsIntermediateRestraint = secondaryMemberIntermediateRestraint;
      this.BothFlangesFreeToRotateOnPlanAtEnds = bothFlangesFreeToRotateOnPlanAtEnds;
      this.IntermediateRestraintPositions = IntermediateRestraint.Custom;
    }
    public Supports(IntermediateRestraint intermediateRestraintPositions, bool secondaryMemberIntermediateRestraint, bool bothFlangesFreeToRotateOnPlanAtEnds)
    {
      this.IntermediateRestraintPositions = intermediateRestraintPositions;
      this.SecondaryMemberAsIntermediateRestraint = secondaryMemberIntermediateRestraint;
      this.BothFlangesFreeToRotateOnPlanAtEnds = bothFlangesFreeToRotateOnPlanAtEnds;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string sec = (this.SecondaryMemberAsIntermediateRestraint) ? ", SMIR" : "";
      string flange = (this.BothFlangesFreeToRotateOnPlanAtEnds) ? ", FFRE" : "";
      string res = this.IntermediateRestraintPositions.ToString().Replace("__", "-").Replace("_", " ");
      if (CustomIntermediateRestraintPositions != null)
      {
        res = "Custom:{";
        foreach (IQuantity pos in CustomIntermediateRestraintPositions)
        {
          res += " ";
          if (pos.QuantityInfo.UnitType == typeof(LengthUnit))
          {
            Length l = (Length)pos;
            res += l.ToString("g2").Replace(" ", string.Empty);
          }
          else
          {
            Ratio p = (Ratio)pos;
            res += p.ToString("g2").Replace(" ", string.Empty);
          }
          res += ",";
        }
        res = res.TrimEnd(',');
        res += "}";
      }  
      
      return res + sec + flange;
    }
    #endregion
  }
}
