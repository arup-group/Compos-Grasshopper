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

  public class ComposCustomDeck
  {
    private DeckConfiguration dconf;

    public Length DistanceB1 { get; set; }
    public Length DistanceB2 { get; set; }
    public Length DistanceB3 { get; set; }
    public Length DistanceB4 { get; set; }
    public Length DistanceB5 { get; set; }
    public Length Depth { get; set; }
    public Length Thickness { get; set; }
    public Pressure Strength { get; set; }
    public DeckConfiguration DeckConfiguration { get; set; }

    #region constructors
    public ComposCustomDeck()
    {
      // empty constructor
    }

    public ComposCustomDeck(Length distanceB1, Length distanceB2, Length distanceB3, Length distanceB4, Length distanceB5, Length depth, Length thickness, Pressure stress)
    {
      this.DistanceB1 = distanceB1;
      this.DistanceB2 = distanceB2;
      this.DistanceB3 = distanceB3;
      this.DistanceB4 = distanceB4;
      this.DistanceB5 = distanceB5;
      this.Depth = depth;
      this.Thickness = thickness;
      this.Strength = stress;

    }

    public ComposCustomDeck(Length distanceB1, Length distanceB2, Length distanceB3, Length distanceB4, Length distanceB5, Length depth, Length thickness, Pressure stress, DeckConfiguration dconf) : this(distanceB1, distanceB2, distanceB3, distanceB4, distanceB5, depth, thickness, stress)
    {
      this.dconf = dconf;
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

    public ComposCustomDeck Duplicate()
    {
      if (this == null) { return null; }
      ComposCustomDeck dup = (ComposCustomDeck)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string distanceB1 = (this.DistanceB1.Value == 0) ? "" : "b1:" + this.DistanceB1.ToString().Replace(" ", string.Empty);
      string distanceB2 = (this.DistanceB2.Value == 0) ? "" : "b2:" + this.DistanceB2.ToString().Replace(" ", string.Empty);
      string distanceB3 = (this.DistanceB3.Value == 0) ? "" : "b3:" + this.DistanceB3.ToString().Replace(" ", string.Empty);
      string distanceB4 = (this.DistanceB4.Value == 0) ? "" : "b4:" + this.DistanceB4.ToString().Replace(" ", string.Empty);
      string distanceB5 = (this.DistanceB5.Value == 0) ? "" : "b5:" + this.DistanceB5.ToString().Replace(" ", string.Empty);
      string depth = (this.Depth.Value == 0) ? "" : "d:" + this.Depth.ToString().Replace(" ", string.Empty);
      string thickness = (this.Thickness.Value == 0) ? "" : "th:" + this.Thickness.ToString().Replace(" ", string.Empty);
      string stress = (this.Strength.Value == 0) ? "" : "stress:" + this.Strength.ToString().Replace(" ", string.Empty);

      string joined = string.Join(" ", new List<string>() { distanceB1, distanceB2, distanceB3, distanceB4, distanceB5, depth, thickness, stress });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }

  public class ComposStandardDeck
  {
    public string Catalogue { get; set; }
    public string Profile { get; set; }
    public DeckSteelType SteelType { get; set; }
    public DeckConfiguration DeckConfiguration { get; set; }

    public enum DeckSteelType
    {
      S280,
      S350
    }
    #region constructors
    public ComposStandardDeck()
    {
      // empty constructor
    }

    public ComposStandardDeck(string catalogue, string profile, DeckSteelType deckSteelType, DeckConfiguration deckConfiguration)
    {
      this.Catalogue = catalogue;
      this.Profile = profile;
      this.SteelType = deckSteelType;
      this.DeckConfiguration = deckConfiguration;

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

    public ComposStandardDeck Duplicate()
    {
      if (this == null) { return null; }
      ComposStandardDeck dup = (ComposStandardDeck)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string catalogue = this.Catalogue.ToString();
      string profile = this.Profile.ToString();
      string deckSteelType = this.SteelType.ToString();
      string deckConfiguration = this.DeckConfiguration.ToString();

      string joined = string.Join(" ", new List<string>() { catalogue, profile, deckSteelType, deckConfiguration });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }

  public class ComposDeck
  {
    private string profile;
    private string catalogue;
    private DeckConfiguration dconf;
    private ComposStandardDeck.DeckSteelType steelType;

    public ComposCustomDeck CustomDeck { get; set; }
    public ComposStandardDeck StandardDeck { get; set; }
    internal enum DeckType
    {
      CustomDeck,
      StandardDeck
    }
    internal DeckType Type { get; set; }

    #region constructors
    public ComposDeck()
    {
      // empty constructor
    }
    public ComposDeck(ComposCustomDeck customDeck)
    {
      this.CustomDeck = customDeck;
      this.Type = DeckType.CustomDeck;
    }

    public ComposDeck(Length distB1, Length distB2, Length distB3, Length distB4, Length distB5, Length depth, Length thickness, Pressure stress, DeckConfiguration dconf)
    {
      this.CustomDeck = new ComposCustomDeck(distB1, distB2, distB3, distB4, distB5, depth, thickness, stress, dconf);
    }

    public ComposDeck(ComposStandardDeck standardDeck)
    {
      this.StandardDeck = standardDeck;
      this.Type = DeckType.StandardDeck;
    }
    public enum DeckSteelType
    {
      S280,
      S350
    }

    public ComposDeck(string catalogue, string profile, DeckSteelType steelType,DeckConfiguration dconf)
    {
      this.StandardDeck = new ComposStandardDeck(catalogue, profile, steelType, dconf);
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

    #region coa interop
    internal ComposDeck(string coaString)
    {
      // to do - implement from coa string method
    }

    public ComposDeck(string coaString, string profile, DeckType deckSteelType, DeckConfiguration dconf) : this(coaString)
    {
      this.profile = profile;
      this.steelType = deckSteelType;
      this.dconf = dconf;
    }

    public ComposDeck(string coaString, string profile, ComposStandardDeck.DeckSteelType steelType, DeckConfiguration dconf) : this(coaString)
    {
      this.profile = profile;
      this.steelType = steelType;
      this.dconf = dconf;
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }
    #endregion

    #region methods

    public ComposDeck Duplicate()
    {
      if (this == null) { return null; }
      ComposDeck dup = (ComposDeck)this.MemberwiseClone();
      if (this.Type == DeckType.CustomDeck)
        dup.CustomDeck = this.CustomDeck.Duplicate();
      if (this.Type == DeckType.StandardDeck)
        dup.StandardDeck = this.StandardDeck.Duplicate();
      return dup;
    }

    public override string ToString()
    {
      switch (Type)
      {
        case DeckType.CustomDeck: return this.CustomDeck.ToString();
        case DeckType.StandardDeck: return this.StandardDeck.ToString();
        default: return base.ToString();
      }
    }

    #endregion
  }

  public class ComposDeckGoo : GH_Goo<ComposDeck>
  {
    #region constructors
    public ComposDeckGoo()
    {
      this.Value = new ComposDeck();
    }
    public ComposDeckGoo(ComposDeck item)
    {
      if (item == null)
        item = new ComposDeck();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public ComposDeckGoo DuplicateGoo()
    {
      return new ComposDeckGoo(Value == null ? new ComposDeck() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Deck Dimension";
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

      if (typeof(Q).IsAssignableFrom(typeof(ComposDeck)))
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
      if (typeof(ComposDeck).IsAssignableFrom(source.GetType()))
      {
        Value = (ComposDeck)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
