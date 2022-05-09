using System;
using System.Collections.Generic;
using System.Globalization;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum DeckingSteelGrade
  {
    S280,
    S350
  }

  public class CatalogueDecking : Decking
  {
    public string Catalogue { get; set; } //	catalogue name of the decking
    public string Profile { get; set; } // decking name
    public DeckingSteelGrade Grade { get; set; } //	decking material grade
    public const string CoaIdentifier = "DECKING_CATALOGUE";

    public CatalogueDecking()
    {
      this.m_type = DeckingType.Catalogue;
    }

    public CatalogueDecking(string catalogue, string profile, DeckingSteelGrade deckingSteelGrade, IDeckingConfiguration deckingConfiguration)
    {
      this.Catalogue = catalogue;
      this.Profile = profile;
      this.Grade = deckingSteelGrade;
      this.DeckingConfiguration = deckingConfiguration;
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

    #region coa interop
    internal CatalogueDecking(List<string> parameters, ComposUnits units)
    {
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      this.Catalogue = parameters[2];
      this.Profile = parameters[3];
      this.Grade = (DeckingSteelGrade)Enum.Parse(typeof(DeckingSteelGrade), parameters[4]);
      DeckingConfiguration deckingConfiguration = new DeckingConfiguration();
      deckingConfiguration.Angle = new Angle(Convert.ToDouble(parameters[5], noComma), units.Angle);

      if (parameters[6] == "DECKING_JOINTED")
        deckingConfiguration.IsDiscontinous = true;
      else
        deckingConfiguration.IsDiscontinous = false;

      if (parameters[7] == "JOINT_WELDED")
        deckingConfiguration.IsWelded = true;
      else
        deckingConfiguration.IsWelded = false;
      this.DeckingConfiguration = deckingConfiguration;
    }

    public override string ToCoaString(string name, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add("DECKING_CATALOGUE");
      parameters.Add(name);
      parameters.Add(this.Catalogue);
      parameters.Add(this.Profile);
      parameters.Add(this.Grade.ToString());
      parameters.Add(this.DeckingConfiguration.Angle.ToUnit(units.Angle).ToString());

      if (this.DeckingConfiguration.IsDiscontinous)
        parameters.Add("DECKING_JOINTED");
      else
        parameters.Add("DECKING_CONTINUED");

      if (this.DeckingConfiguration.IsWelded)
        parameters.Add("JOINT_WELDED");
      else
        parameters.Add("JOINT_NOT_WELD");

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string catalogue = this.Catalogue.ToString();
      string profile = this.Profile.ToString();
      string deckSteelType = this.Type.ToString();
      string deckConfiguration = this.DeckingConfiguration.ToString();
      string joined = string.Join(" ", new List<string>() { catalogue, profile, deckSteelType, deckConfiguration });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
