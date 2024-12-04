using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  /// <summary>
  /// A Beam Section object contains information about the profile dimensions,
  /// start position and if the section is tapered to the next section.
  /// </summary>
  public class BeamSection : IBeamSection {
    public Length BottomFlangeThickness { get; set; }
    public Length BottomFlangeWidth { get; set; }
    // Dimensions
    public Length Depth { get; set; }
    public bool IsCatalogue { get; set; }
    public Length RootRadius { get; set; } = Length.Zero;
    public string SectionDescription { get; set; }
    public IQuantity StartPosition {
      get => m_StartPosition;
      set {
        if (value == null) {
          return;
        }
        if (value.QuantityInfo.UnitType != typeof(LengthUnit) & value.QuantityInfo.UnitType != typeof(RatioUnit)) {
          throw new ArgumentException("Start Position must be either Length or Ratio");
        } else {
          m_StartPosition = value;
        }
      }
    }
    // Setting out
    public bool TaperedToNext //	tapered or uniform to the next section
    {
      get {
        if (IsCatalogue) {
          return false;
        } else {
          return m_taper;
        }
      }
      set {
        if (!IsCatalogue) {
          m_taper = value;
        }
      }
    }
    public Length TopFlangeThickness { get; set; }
    public Length TopFlangeWidth { get; set; }
    public Length WebThickness { get; set; }
    private IQuantity m_StartPosition = Length.Zero;
    private bool m_taper;

    public BeamSection() {
      // empty constructor
    }

    /// <summary>
    /// Create assymetric I section
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="topFlangeWidth"></param>
    /// <param name="bottomFlangeWidth"></param>
    /// <param name="webThickness"></param>
    /// <param name="topFlangeThickness"></param>
    /// <param name="bottomFlangeThickness"></param>
    public BeamSection(Length depth, Length topFlangeWidth, Length bottomFlangeWidth, Length webThickness,
      Length topFlangeThickness, Length bottomFlangeThickness, bool taperToNext = false) {
      Depth = depth;
      TopFlangeWidth = topFlangeWidth;
      BottomFlangeWidth = bottomFlangeWidth;
      WebThickness = webThickness;
      TopFlangeThickness = topFlangeThickness;
      BottomFlangeThickness = bottomFlangeThickness;
      IsCatalogue = false;
      TaperedToNext = taperToNext;

      LengthUnit unit = Depth.Unit;
      string u = " ";
      try { u = UnitString(unit); } catch (Exception) { unit = LengthUnit.Millimeter; }

      string dims = string.Join(" ",
        Depth.As(unit).ToString(),
        TopFlangeWidth.As(unit).ToString(),
        BottomFlangeWidth.As(unit).ToString(),
        WebThickness.As(unit).ToString(),
        TopFlangeThickness.As(unit).ToString(),
        BottomFlangeThickness.As(unit).ToString());

      SectionDescription = "STD GI" + u + dims.Replace(',', '.');
    }

    /// <summary>
    /// Create symmetric I section
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="flangeWidth"></param>
    /// <param name="webThickness"></param>
    /// <param name="flangeThickness"></param>
    public BeamSection(Length depth, Length flangeWidth, Length webThickness, Length flangeThickness, bool taperToNext = false) {
      Depth = depth;
      TopFlangeWidth = flangeWidth;
      BottomFlangeWidth = TopFlangeWidth;
      WebThickness = webThickness;
      TopFlangeThickness = flangeThickness;
      BottomFlangeThickness = TopFlangeThickness;
      IsCatalogue = false;
      m_taper = taperToNext;

      LengthUnit unit = Depth.Unit;
      string u = " ";
      try { u = UnitString(unit); } catch (Exception) { unit = LengthUnit.Millimeter; }

      string dims = string.Join(" ",
        Depth.As(unit).ToString(),
        TopFlangeWidth.As(unit).ToString(),
        WebThickness.As(unit).ToString(),
        TopFlangeThickness.As(unit).ToString());

      SectionDescription = "STD I" + u + dims.Replace(',', '.');
    }

    public BeamSection(string profile) {
      SetFromProfileString(profile);
    }

    public string ToCoaString(string name, int num, int index, ComposUnits units) {
      var parameters = new List<string> {
        CoaIdentifier.BeamSectionAtX,
        name,
        Convert.ToString(num),
        Convert.ToString(index)
      };
      if (StartPosition.QuantityInfo.UnitType == typeof(RatioUnit)) {
        // start position in percent
        var p = (Ratio)StartPosition;
        // percentage in coa string for beam section is a negative decimal fraction!
        parameters.Add(CoaHelper.FormatSignificantFigures(p.As(RatioUnit.DecimalFraction) * -1, p.DecimalFractions == 1 ? 5 : 6));
      } else {
        parameters.Add(CoaHelper.FormatSignificantFigures(StartPosition.ToUnit(units.Length).Value, 6));
      }
      parameters.Add(SectionDescription);
      CoaHelper.AddParameter(parameters, "TAPERED", TaperedToNext);

      string coaString = CoaHelper.CreateString(parameters);

      return coaString;
    }

    public override string ToString() {
      string start = "";
      if (StartPosition.QuantityInfo.UnitType == typeof(LengthUnit)) {
        var l = (Length)StartPosition;
        if (!ComposUnitsHelper.IsEqual(l, Length.Zero)) {
          start = ", Px:" + l.ToString("g2").Replace(" ", string.Empty);
        }
      } else {
        var p = (Ratio)StartPosition;
        if (!ComposUnitsHelper.IsEqual(p, Length.Zero)) {
          start = ", Px:" + p.ToUnit(RatioUnit.Percent).ToString("g2").Replace(" ", string.Empty);
        }
      }

      string tapered = "";
      if (TaperedToNext) {
        tapered = ", Tapered";
      }

      string sect = "";
      if (SectionDescription != null) {
        sect = SectionDescription;
        if (IsCatalogue) {
          // remove the catalogue date if exist:
          sect = sect.Split(' ')[0] + " " + sect.Split(' ')[1] + " " + sect.Split(' ')[2];
        }
      }

      return (SectionDescription == null) ? "Null profile" : sect + start + tapered;
    }

    internal static IBeamSection FromCoaString(List<string> parameters, ComposUnits units) {
      var section = new BeamSection();
      //BEAM_SECTION_AT_X	MEMBER-1	3	1	0.000000	STD GI 200 189.2 222.25 8.5 12.7 12.7	TAPERED_YES
      //BEAM_SECTION_AT_X MEMBER-1 3 2 6.00000 STD GI 730 189.2 222.25 8.5 12.7 12.7 TAPERED_YES
      //BEAM_SECTION_AT_X MEMBER-1 3 3 12.0000 STD GI 200 189.2 222.25 8.5 12.7 12.7 TAPERED_YES

      double startPosition = CoaHelper.ConvertToDouble(parameters[4]);
      if (startPosition < 0) {

        // start position in percent
        section.StartPosition = new Ratio(Math.Abs(startPosition), RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
      } else {
        section.StartPosition = CoaHelper.ConvertToLength(parameters[4], units.Length);
      }

      section.SetFromProfileString(parameters[5]);
      // using StartsWith as string is the last parameter and can contain new line character: "TAPERED_YES\n"
      if (parameters[6].StartsWith("TAPERED_YES")) {
        section.TaperedToNext = true;
      } else {
        section.TaperedToNext = false;
      }

      return section;
    }

    private void SetFromProfileString(string profile) {
      profile = profile.Replace(',', '.');

      if (profile.StartsWith("STD I")) {
        // example: STD I 200 190.5 8.5 12.7
        // example: STD I(cm) 20. 19. 8.5 1.27

        string[] parts = profile.Split(' ');

        LengthUnit unit = LengthUnit.Millimeter; // default unit for sections is mm

        string[] type = parts[1].Split('(', ')');
        if (type.Length > 1) {
          UnitParser parser = OasysUnitsSetup.Default.UnitParser;
          unit = parser.Parse<LengthUnit>(type[1]);
        }
        try {
          Depth = new Length(double.Parse(parts[2], CultureInfo.InvariantCulture), unit);
          TopFlangeWidth = new Length(double.Parse(parts[3], CultureInfo.InvariantCulture), unit);
          BottomFlangeWidth = TopFlangeWidth;
          WebThickness = new Length(double.Parse(parts[4], CultureInfo.InvariantCulture), unit);
          TopFlangeThickness = new Length(double.Parse(parts[5], CultureInfo.InvariantCulture), unit);
          BottomFlangeThickness = TopFlangeThickness;
          SectionDescription = profile;
          IsCatalogue = false;
        } catch (Exception) {
          throw new Exception("Unrecognisable elements in profile string.");
        }
      } else if (profile.StartsWith("STD GI")) {
        // example: STD GI 400. 300. 250. 12. 25. 20.
        // example: STD GI(cm) 15. 15. 12. 3. 1. 2.

        string[] parts = profile.Split(' ');

        LengthUnit unit = LengthUnit.Millimeter; // default unit for sections is mm

        string[] type = parts[1].Split('(', ')');
        if (type.Length > 1) {
          UnitParser parser = OasysUnitsSetup.Default.UnitParser;
          unit = parser.Parse<LengthUnit>(type[1]);
        }
        try {
          Depth = new Length(double.Parse(parts[2], CultureInfo.InvariantCulture), unit);
          TopFlangeWidth = new Length(double.Parse(parts[3], CultureInfo.InvariantCulture), unit);
          BottomFlangeWidth = new Length(double.Parse(parts[4], CultureInfo.InvariantCulture), unit);
          WebThickness = new Length(double.Parse(parts[5], CultureInfo.InvariantCulture), unit);
          TopFlangeThickness = new Length(double.Parse(parts[6], CultureInfo.InvariantCulture), unit);
          BottomFlangeThickness = new Length(double.Parse(parts[7], CultureInfo.InvariantCulture), unit);
          SectionDescription = profile;
          IsCatalogue = false;
        } catch (Exception) {
          throw new Exception("Unrecognisable elements in profile string.");
        }
      } else if (profile.StartsWith("CAT")) {
        string prof = profile.Split(' ')[2];

        List<double> sqlValues = SqlReader.Instance.GetCatalogueProfileValues(Path.Combine(ComposIO.InstallPath, "sectlib.db3"), prof);

        LengthUnit unit = LengthUnit.Meter;
        Depth = new Length(sqlValues[0], unit);
        TopFlangeWidth = new Length(sqlValues[1], unit);
        BottomFlangeWidth = new Length(sqlValues[1], unit);
        WebThickness = new Length(sqlValues[2], unit);
        TopFlangeThickness = new Length(sqlValues[3], unit);
        BottomFlangeThickness = new Length(sqlValues[3], unit);
        RootRadius = new Length(sqlValues[4], unit);
        SectionDescription = profile;
        IsCatalogue = true;
        m_taper = false;
      } else {
        throw new ArgumentException("Unrecognisable profile type. String must start with 'STD I', 'STD GI' or 'CAT'.");
      }
    }

    private string UnitString(LengthUnit unit) {
      switch (unit) {
        case LengthUnit.Millimeter:
          return " ";

        case LengthUnit.Centimeter:
          return "(cm) ";

        case LengthUnit.Meter:
          return "(m) ";

        case LengthUnit.Inch:
          return "(in) ";

        case LengthUnit.Foot:
          return "(ft) ";

        default:
          throw new ArgumentException("unrecognised unit - must be mm, cm, m, in or ft.");
      }
    }
  }
}
