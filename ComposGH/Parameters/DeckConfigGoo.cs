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
  public class DeckConfiguration
  {
    public Angle Angle { get; set; }
    public bool IsDiscontinous { get; set; }
    public bool IsWelded { get; set; }


    #region constructors
    public DeckConfiguration()
    {
      // empty constructor
    }

    public DeckConfiguration(Angle angle, bool isDiscontinous, bool isWelded)
    {
      this.Angle = angle;
      this.IsDiscontinous = isDiscontinous;
      this.IsWelded = isWelded;

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

    public DeckConfiguration Duplicate()
    {
      if (this == null) { return null; }
      DeckConfiguration dup = (DeckConfiguration)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string angle = (this.Angle.Value == 0) ? "" : this.Angle.ToString().Replace(" ", string.Empty);
      string isDiscontinous = (this.IsDiscontinous == true) ? "" : this.IsDiscontinous.ToString().Replace(" ", string.Empty);
      string isWelded = (this.IsWelded == true) ? "" : this.IsWelded.ToString().Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { angle, isDiscontinous, isWelded });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class DeckConfigurationGoo : GH_Goo<DeckConfiguration>
  {
    #region constructors
    public DeckConfigurationGoo()
    {
      this.Value = new DeckConfiguration();
    }
    public DeckConfigurationGoo(DeckConfiguration item)
    {
      if (item == null)
        item = new DeckConfiguration();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public DeckConfigurationGoo DuplicateGoo()
    {
      return new DeckConfigurationGoo(Value == null ? new DeckConfiguration() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Deck Config";
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

      if (typeof(Q).IsAssignableFrom(typeof(DeckConfiguration)))
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
      if (typeof(DeckConfiguration).IsAssignableFrom(source.GetType()))
      {
        Value = (DeckConfiguration)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
