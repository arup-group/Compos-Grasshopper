using Grasshopper.Kernel.Types;
using ComposAPI;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class DeckingConfigurationGoo : GH_Goo<IDeckingConfiguration>
  {
    #region constructors
    public DeckingConfigurationGoo()
    {
      this.Value = new DeckingConfiguration();
    }
    public DeckingConfigurationGoo(IDeckingConfiguration item)
    {
      if (item == null)
        item = new DeckingConfiguration();
      this.Value = item; //.Duplicate() as DeckingConfiguration;
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public DeckingConfigurationGoo DuplicateGoo()
    {
      return new DeckingConfigurationGoo(Value == null ? new DeckingConfiguration() : Value);// .Duplicate() as DeckingConfiguration);
    }
    #endregion

    #region properties
     public override bool IsValid => (this.Value == null) ? false : true;
    public override string TypeName => "Deck Config.";
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

      if (typeof(Q).IsAssignableFrom(typeof(DeckingConfiguration)))
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
      if (typeof(DeckingConfiguration).IsAssignableFrom(source.GetType()))
      {
        Value = (DeckingConfiguration)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}