using System;
using System.Collections.Generic;
using System.IO;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  public enum DeckingSteelGrade
  {
    S280,
    S350
  }

  public class CatalogueDecking : Decking, IDecking
  {
    public string Catalogue { get; set; } //	catalogue name of the decking
    public string Profile { get; set; } // decking name
    public DeckingSteelGrade Grade { get; set; } //	decking material grade

    public CatalogueDecking()
    {
      m_type = DeckingType.Catalogue;
    }

    public CatalogueDecking(string catalogue, string profile, DeckingSteelGrade deckingSteelGrade, IDeckingConfiguration deckingConfiguration)
    {
      Catalogue = catalogue;
      Profile = profile;
      Grade = deckingSteelGrade;
      DeckingConfiguration = deckingConfiguration;
      m_type = DeckingType.Catalogue;

      List<double> sqlValues = SqlReader.Instance.GetCatalogueDeckingValues(Path.Combine(ComposIO.InstallPath, "decking.db3"), catalogue, profile);
      LengthUnit unit = LengthUnit.Meter;
      Depth = new Length(sqlValues[0], unit);
      b1 = new Length(sqlValues[1], unit);
      b2 = new Length(sqlValues[2], unit);
      b3 = new Length(sqlValues[3], unit);
      b4 = new Length(sqlValues[4], unit);
      b5 = new Length(sqlValues[5], unit);
      Thickness = new Length(sqlValues[6], unit);
    }

    #region coa interop
    internal static IDecking FromCoaString(List<string> parameters, ComposUnits units)
    {
      CatalogueDecking decking = new CatalogueDecking();

      decking.Catalogue = parameters[2];
      decking.Profile = parameters[3];
      decking.Grade = (DeckingSteelGrade)Enum.Parse(typeof(DeckingSteelGrade), parameters[4]);
      DeckingConfiguration deckingConfiguration = new DeckingConfiguration();
      deckingConfiguration.Angle = CoaHelper.ConvertToAngle(parameters[5], AngleUnit.Degree); // COA string always in degrees

      if (parameters[6] == "DECKING_JOINTED")
        deckingConfiguration.IsDiscontinous = true;
      else
        deckingConfiguration.IsDiscontinous = false;

      if (parameters[7] == "JOINT_WELDED")
        deckingConfiguration.IsWelded = true;
      else
        deckingConfiguration.IsWelded = false;
      decking.DeckingConfiguration = deckingConfiguration;

      return decking;
    }

    public override string ToCoaString(string name, ComposUnits units)
    {
      List<string> parameters = new List<string>();
      parameters.Add("DECKING_CATALOGUE");
      parameters.Add(name);
      parameters.Add(Catalogue);
      parameters.Add(Profile);
      parameters.Add(Grade.ToString());
      parameters.Add(
        CoaHelper.FormatSignificantFigures(
          DeckingConfiguration.Angle.ToUnit(AngleUnit.Degree).Value, 6).ToString());
      // COA string always in degrees

      if (DeckingConfiguration.IsDiscontinous)
        parameters.Add("DECKING_JOINTED");
      else
        // different to documenation and DECKING_USER!
        parameters.Add("DECKING_CONTINUE");

      if (DeckingConfiguration.IsWelded)
        parameters.Add("JOINT_WELDED");
      else
        parameters.Add("JOINT_NOT_WELD");

      return CoaHelper.CreateString(parameters);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string catalogue = Catalogue.ToString();
      string profile = Profile.ToString();
      string deckSteelType = Type.ToString();
      string deckConfiguration = DeckingConfiguration.ToString();
      string joined = string.Join(" ", new List<string>() { catalogue, profile, deckSteelType, deckConfiguration });
      return joined.Replace("  ", " ").TrimEnd(' ').TrimStart(' ');
    }
    #endregion
  }
}
