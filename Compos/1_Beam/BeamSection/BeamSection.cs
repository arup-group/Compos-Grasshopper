using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;
using System.Globalization;
using System.IO;

namespace ComposAPI
{
  /// <summary>
  /// A Beam Section object contains information about the profile dimensions, 
  /// start position and if the section is tapered to next section.
  /// </summary>
  public class BeamSection
  {
    // Setting out
    public bool TaperedToNext
    {
      get
      {
        if (this.isCatalogue)
          return false;
        else
          return m_taper;
      }
      set
      {
        if (!this.isCatalogue)
          m_taper = value;
      }
    }
    private bool m_taper;
    public Length StartPosition { get; set; } = Length.Zero;

    // Dimensions
    public Length Depth { get; set; }
    public Length TopFlangeWidth { get; set; }
    public Length BottomFlangeWidth { get; set; }
    public Length TopFlangeThickness { get; set; }
    public Length BottomFlangeThickness { get; set; }
    public Length RootRadius { get; set; } = Length.Zero;
    public Length WebThickness { get; set; }
    public bool isCatalogue;

    public string SectionDescription { get; set; }
    public const string CoaIdentifierPrefix = "BEAM_SECTION_AT_X";

    #region constructors
    public BeamSection()
    {
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
      Length topFlangeThickness, Length bottomFlangeThickness, bool taperToNext = false)
    {
      this.Depth = depth;
      this.TopFlangeWidth = topFlangeWidth;
      this.BottomFlangeWidth = bottomFlangeWidth;
      this.WebThickness = webThickness;
      this.TopFlangeThickness = topFlangeThickness;
      this.BottomFlangeThickness = bottomFlangeThickness;
      this.isCatalogue = false;
      this.TaperedToNext = taperToNext;

      LengthUnit unit = this.Depth.Unit;
      string u = " ";
      try { u = unitString(unit); }
      catch (Exception) { unit = LengthUnit.Millimeter; }

      string dims = string.Join(" ",
        this.Depth.As(unit).ToString(),
        this.TopFlangeWidth.As(unit).ToString(),
        this.BottomFlangeWidth.As(unit).ToString(),
        this.WebThickness.As(unit).ToString(),
        this.TopFlangeThickness.As(unit).ToString(),
        this.BottomFlangeThickness.As(unit).ToString());

      this.SectionDescription = "STD GI" + u + dims.Replace(',', '.');
    }

    /// <summary>
    /// Create symmetric I section
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="flangeWidth"></param>
    /// <param name="webThickness"></param>
    /// <param name="flangeThickness"></param>
    public BeamSection(Length depth, Length flangeWidth, Length webThickness, Length flangeThickness, bool taperToNext = false)
    {
      this.Depth = depth;
      this.TopFlangeWidth = flangeWidth;
      this.BottomFlangeWidth = TopFlangeWidth;
      this.WebThickness = webThickness;
      this.TopFlangeThickness = flangeThickness;
      this.BottomFlangeThickness = TopFlangeThickness;
      this.isCatalogue = false;
      this.m_taper = taperToNext;

      LengthUnit unit = this.Depth.Unit;
      string u = " ";
      try { u = unitString(unit); }
      catch (Exception) { unit = LengthUnit.Millimeter; }

      string dims = string.Join(" ",
        this.Depth.As(unit).ToString(),
        this.TopFlangeWidth.As(unit).ToString(),
        this.WebThickness.As(unit).ToString(),
        this.TopFlangeThickness.As(unit).ToString());

      this.SectionDescription = "STD I" + u + dims.Replace(',', '.');
    }

    public BeamSection(string profile)
    {
      profile = profile.Replace(',', '.');

      if (profile.StartsWith("STD I"))
      {
        // example: STD I 200 190.5 8.5 12.7
        // example: STD I(cm) 20. 19. 8.5 1.27

        string[] parts = profile.Split(' ');

        LengthUnit unit = LengthUnit.Millimeter; // default unit for sections is mm

        string[] type = parts[1].Split('(', ')');
        if (type.Length > 1)
        {
          var parser = UnitParser.Default;
          unit = parser.Parse<LengthUnit>(type[1]);
        }
        try
        {
          this.Depth = new Length(double.Parse(parts[2], CultureInfo.InvariantCulture), unit);
          this.TopFlangeWidth = new Length(double.Parse(parts[3], CultureInfo.InvariantCulture), unit);
          this.BottomFlangeWidth = TopFlangeWidth;
          this.WebThickness = new Length(double.Parse(parts[4], CultureInfo.InvariantCulture), unit);
          this.TopFlangeThickness = new Length(double.Parse(parts[5], CultureInfo.InvariantCulture), unit);
          this.BottomFlangeThickness = TopFlangeThickness;
          this.SectionDescription = profile;
          this.isCatalogue = false;

        }
        catch (Exception)
        {
          throw new Exception("Unrecognisable elements in profile string.");
        }
      }
      else if (profile.StartsWith("STD GI"))
      {
        // example: STD GI 400. 300. 250. 12. 25. 20.
        // example: STD GI(cm) 15. 15. 12. 3. 1. 2.

        string[] parts = profile.Split(' ');

        LengthUnit unit = LengthUnit.Millimeter; // default unit for sections is mm

        string[] type = parts[1].Split('(', ')');
        if (type.Length > 1)
        {
          var parser = UnitParser.Default;
          unit = parser.Parse<LengthUnit>(type[1]);
        }
        try
        {
          this.Depth = new Length(double.Parse(parts[2], CultureInfo.InvariantCulture), unit);
          this.TopFlangeWidth = new Length(double.Parse(parts[3], CultureInfo.InvariantCulture), unit);
          this.BottomFlangeWidth = new Length(double.Parse(parts[4], CultureInfo.InvariantCulture), unit);
          this.WebThickness = new Length(double.Parse(parts[5], CultureInfo.InvariantCulture), unit);
          this.TopFlangeThickness = new Length(double.Parse(parts[6], CultureInfo.InvariantCulture), unit);
          this.BottomFlangeThickness = new Length(double.Parse(parts[7], CultureInfo.InvariantCulture), unit);
          this.SectionDescription = profile;
          this.isCatalogue = false;
        }
        catch (Exception)
        {
          throw new Exception("Unrecognisable elements in profile string.");
        }
      }
      else if (profile.StartsWith("CAT"))
      {
        string prof = profile.Split(' ').Last();
        List<double> sqlValues = Helpers.CatalogueValues.GetCatalogueProfileValues(prof);

        LengthUnit unit = LengthUnit.Meter;
        this.Depth = new Length(sqlValues[0], unit);
        this.TopFlangeWidth = new Length(sqlValues[1], unit);
        this.BottomFlangeWidth = new Length(sqlValues[1], unit);
        this.WebThickness = new Length(sqlValues[2], unit);
        this.TopFlangeThickness = new Length(sqlValues[3], unit);
        this.BottomFlangeThickness = new Length(sqlValues[3], unit);
        this.RootRadius = new Length(sqlValues[4], unit);
        this.SectionDescription = profile;
        this.isCatalogue = true;
        this.m_taper = false;
      }
      else
        throw new ArgumentException("Unrecognisable profile type. String must start with 'STD I', 'STD GI' or 'CAT'.");
    }

    private string unitString(LengthUnit unit)
    {
      switch (unit)
      {
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
    #endregion

    #region methods
    public BeamSection Duplicate()
    {
      if (this == null) { return null; }
      BeamSection dup = (BeamSection)this.MemberwiseClone();
      return dup;
    }
    public override string ToString()
    {
      string start = "";
      if (this.StartPosition != Length.Zero)
        start = ", Px:" + this.StartPosition.ToUnit(Units.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty);
      string tapered = "";
      if (this.TaperedToNext)
        tapered = ", Tapered";

      return (this.SectionDescription == null) ? "Null profile" : this.SectionDescription + start + tapered;
    }
    #endregion
  }
}
