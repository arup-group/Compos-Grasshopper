using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using Grasshopper.Documentation;
using Rhino.Collections;
using UnitsNet;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
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
      MidSpan,
      ThirdPts,
      QuarterPts,
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

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
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
      string sec = (this.SecondaryMemberIntermediateRestraint) ? ", Sec. mem. interm. res." : "";
      string flange = (this.BothFlangesFreeToRotateOnPlanAtEnds) ? ", Flngs. free rot. ends" : "";
      if (this.IntermediateRestraintPositions == IntermediateRestraint.Custom & CustomIntermediateRestraintPositions != null)
      {
        string res = "Restraint Pos: Start";
        foreach (Length pos in this.CustomIntermediateRestraintPositions)
          res += ", " + pos.ToUnit(Units.LengthUnitGeometry).ToString("f0").Replace(" ", string.Empty);
        res += ", End";
        string joined = string.Join(" ", new List<string>() { res, sec, flange });
        return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
      }
      else
      {
        string res = "Restraint Pos: Start";
        if (this.IntermediateRestraintPositions != IntermediateRestraint.None)
          res += ", " + this.IntermediateRestraintPositions.ToString();
        res += ", End";
        string joined = string.Join("", new List<string>() { res, sec, flange });
        return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
      }
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class SupportsGoo : GH_Goo<Supports>
  {
    #region constructors
    public SupportsGoo()
    {
      this.Value = new Supports();
    }
    public SupportsGoo(Supports item)
    {
      if (item == null)
        item = new Supports();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public SupportsGoo DuplicateGoo()
    {
      return new SupportsGoo(Value == null ? new Supports() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Support";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }
    public override string ToString()
    {
      if (Value == null)
        return "Null";
      else
        return "Compos " + TypeName + " {" + Value.ToString() + "}"; ;
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(Supports)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into our custom class.

      if (source == null) { return false; }

      //Cast from GsaMaterial
      if (typeof(Supports).IsAssignableFrom(source.GetType()))
      {
        Value = (Supports)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
