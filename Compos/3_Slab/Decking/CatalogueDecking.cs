using System.Collections.Generic;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class CatalogueDecking : Decking
  {
    public enum DeckingSteelGrade
    {
      S280,
      S350
    }

    public string Catalogue { get; set; } //	catalogue name of the decking
    public string Profile { get; set; } // decking name
    public DeckingSteelGrade Grade { get; set; } //	decking material grade
    public CatalogueDecking()
    {
      this.m_type = DeckingType.Catalogue;
    }
    public const string CoaIdentifier = "DECKING_CATALOGUE";

    public CatalogueDecking(string catalogue, string profile, DeckingSteelGrade deckSteelType, DeckingConfiguration deckConfiguration)
    {
      this.Catalogue = catalogue;
      this.Profile = profile;
      this.Grade = deckSteelType;
      this.DeckingConfiguration = deckConfiguration;
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
    internal CatalogueDecking(List<string> parameters)
    {
      this.Strength = new Pressure(Convert.ToDouble(parameters[3]), pressureUnit);
      this.DeckingConfiguration = new DeckingConfiguration();
      this.DeckingConfiguration.Angle = new Angle(Convert.ToDouble(parameters[4]), angleUnit);
      this.b1 = new Length(Convert.ToDouble(parameters[5]), lengthUnit);
      this.b2 = new Length(Convert.ToDouble(parameters[6]), lengthUnit);
      this.b3 = new Length(Convert.ToDouble(parameters[7]), lengthUnit);
      this.Depth = new Length(Convert.ToDouble(parameters[8]), lengthUnit);
      this.Thickness = new Length(Convert.ToDouble(parameters[9]), lengthUnit);
      this.b4 = new Length(Convert.ToDouble(parameters[10]), lengthUnit);
      this.b5 = new Length(Convert.ToDouble(parameters[11]), lengthUnit);

      if (parameters[12] == "DECKING_JOINTED")
        this.DeckingConfiguration.IsDiscontinous = true;
      else
        this.DeckingConfiguration.IsDiscontinous = false;

      if (parameters[12] == "JOINT_WELDED")
        this.DeckingConfiguration.IsWelded = true;
      else
        this.DeckingConfiguration.IsWelded = false;
    }

    internal override string ToCoaString(string name, AngleUnit angleUnit, LengthUnit lengthUnit, PressureUnit pressureUnit)
    {
      List<string> parameters = new List<string>();
      parameters.Add("DECKING_CATALOGUE");
      parameters.Add(name);
      parameters.Add(this.Catalogue);
      parameters.Add(this.Profile);
      parameters.Add(this.Grade.ToString());
      parameters.Add(this.DeckingConfiguration.Angle.ToUnit(angleUnit).ToString());
      
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

    public override Decking Duplicate()
    {
      if (this == null) { return null; }
      CatalogueDecking dup = (CatalogueDecking)this.MemberwiseClone();
      dup.DeckingConfiguration = this.DeckingConfiguration.Duplicate();
      return dup;
    }

    public override string ToString()
    {
      string catalogue = this.Catalogue.ToString();
      string profile = this.Profile.ToString();
      string deckSteelType = this.Type.ToString();
      string deckConfiguration = this.DeckingConfiguration.ToString();
      string joined = string.Join(" ", new List<string>() { catalogue, profile, deckSteelType, deckConfiguration });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
  }
}
