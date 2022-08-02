using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class FrequencyLimitsGoo : GH_Goo<IFrequencyLimits>
  {
    #region constructors
    public FrequencyLimitsGoo()
    {
      this.Value = new FrequencyLimits();
    }
    public FrequencyLimitsGoo(IFrequencyLimits item)
    {
      if (item == null)
        item = new FrequencyLimits();
      this.Value = item; //.Duplicate() as SafetyFactors;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public FrequencyLimitsGoo DuplicateGoo()
    {
      return new FrequencyLimitsGoo(Value == null ? new FrequencyLimits() : Value);
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "FrequencyLimits";
    public override string TypeDescription => "Compos " + this.TypeName + " Parameter";
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid) { return string.Empty; }
        return IsValid.ToString(); //Todo: beef this up to be more informative.
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

      if (typeof(Q).IsAssignableFrom(typeof(FrequencyLimits)))
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
      if (typeof(FrequencyLimits).IsAssignableFrom(source.GetType()))
      {
        Value = (FrequencyLimits)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
