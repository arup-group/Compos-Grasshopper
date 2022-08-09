using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class BeamSizeLimitsGoo : GH_Goo<IBeamSizeLimits>
  {
    #region constructors
    public BeamSizeLimitsGoo()
    {
      this.Value = new BeamSizeLimits();
    }
    public BeamSizeLimitsGoo(IBeamSizeLimits item)
    {
      if (item == null)
        item = new BeamSizeLimits();
      this.Value = item; //.Duplicate() as SafetyFactors;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public BeamSizeLimitsGoo DuplicateGoo()
    {
      return new BeamSizeLimitsGoo(Value == null ? new BeamSizeLimits() : Value);
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "BeamSizeLimits";
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

      if (typeof(Q).IsAssignableFrom(typeof(BeamSizeLimits)))
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
      if (typeof(BeamSizeLimits).IsAssignableFrom(source.GetType()))
      {
        Value = (BeamSizeLimits)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
