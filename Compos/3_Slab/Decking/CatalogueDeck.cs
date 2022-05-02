using System.Collections.Generic;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class CatalogueDeck : Decking
  {
    public string Catalogue { get; set; }
    public string Profile { get; set; }
    public DeckingSteelGrade Grade { get; set; }
    public enum DeckingSteelGrade
    {
      S280,
      S350
    }
    public CatalogueDeck()
    {
      this.m_type = DeckingType.Catalogue;
    }
    public CatalogueDeck(string catalogue, string profile, DeckingSteelGrade deckSteelType, DeckingConfiguration deckConfiguration)
    {
      this.Catalogue = catalogue;
      this.Profile = profile;
      this.Grade = deckSteelType;
      this.DeckConfiguration = deckConfiguration;
      this.m_type = DeckingType.Catalogue;

      List<double> sqlValues = Helpers.CatalogueValues.GetCatalogueDeckingValues(catalogue, profile);
      LengthUnit unit = LengthUnit.Meter;
      this.Depth = new Length(sqlValues[0], unit);
      this.b1 = new Length(sqlValues[1], unit);
      this.b2 = new Length(sqlValues[2], unit);
      this.b3 = new Length(sqlValues[3], unit);
      this.b4 = new Length(sqlValues[4], unit);
      this.b5 = new Length(sqlValues[5], unit);
      this.Thickness = new Length(sqlValues[6], unit);
    }
    public override Decking Duplicate()
    {
      if (this == null) { return null; }
      CatalogueDeck dup = (CatalogueDeck)this.MemberwiseClone();
      dup.DeckConfiguration = this.DeckConfiguration.Duplicate();
      return dup;
    }
    public override string ToString()
    {
      string catalogue = this.Catalogue.ToString();
      string profile = this.Profile.ToString();
      string deckSteelType = this.Type.ToString();
      string deckConfiguration = this.DeckConfiguration.ToString();
      string joined = string.Join(" ", new List<string>() { catalogue, profile, deckSteelType, deckConfiguration });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
  }
}
