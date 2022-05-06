using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class ReinforcementMaterialGoo : GH_Goo<IReinforcementMaterial>
  {
    #region constructors
    public ReinforcementMaterialGoo()
    {
      this.Value = new ReinforcementMaterial();
    }
    public ReinforcementMaterialGoo(IReinforcementMaterial item)
    {
      if (item == null)
        item = new ReinforcementMaterial();
      this.Value = item; //.Duplicate() as ReinforcementMaterial;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ReinforcementMaterialGoo DuplicateGoo()
    {
      return new ReinforcementMaterialGoo(Value == null ? new ReinforcementMaterial() : Value);// .Duplicate() as ReinforcementMaterial);
    }
    #endregion

    #region proHties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Rebar Material";
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

      if (typeof(Q).IsAssignableFrom(typeof(ReinforcementMaterial)))
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
      if (typeof(ReinforcementMaterial).IsAssignableFrom(source.GetType()))
      {
        Value = (ReinforcementMaterial)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
