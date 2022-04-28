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
using UnitsNet.Units;
using System.Globalization;
using System.IO;

namespace ComposGH.Parameters
{
  /// <summary>
  /// Custom class: this class defines the basic properties and methods for our custom class
  /// </summary>
  public class BeamSection
  {
    // Setting out
    public bool TaperedToNext 
    {
      get 
      {
        if (this.m_isCatalogue)
          return false;
        else
          return m_taper; 
      }
      set
      {
        if (!this.m_isCatalogue)
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
    internal Length RootRadius { get; set; } = Length.Zero;
    public Length WebThickness { get; set; }
    internal bool m_isCatalogue;

    public string SectionDescription { get; set; }

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
      this.m_isCatalogue = false;
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
      this.m_isCatalogue = false;
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
          this.m_isCatalogue = false;
          
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
          this.m_isCatalogue = false;
        }
        catch (Exception)
        {
          throw new Exception("Unrecognisable elements in profile string.");
        }
      }
      else if (profile.StartsWith("CAT"))
      {
        string installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Oasys", "Compos 8.6");
        string path = Path.Combine(installPath, "sectlib.db3");
        string prof = profile.Split(' ').Last();
        List<double> sqlValues = Helpers.SqlReadValues.GetProfileValuesFromSQLite(path, prof);

        LengthUnit unit = LengthUnit.Meter;
        this.Depth = new Length(sqlValues[0], unit);
        this.TopFlangeWidth = new Length(sqlValues[1], unit);
        this.BottomFlangeWidth = new Length(sqlValues[1], unit);
        this.WebThickness = new Length(sqlValues[2], unit);
        this.TopFlangeThickness = new Length(sqlValues[3], unit);
        this.BottomFlangeThickness = new Length(sqlValues[3], unit);
        this.RootRadius = new Length(sqlValues[4], unit);
        this.SectionDescription = profile;
        this.m_isCatalogue = true;
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

  /// <summary>
  /// Goo wrapper class, makes sure our custom class can be used in Grasshopper.
  /// </summary>
  public class BeamSectionGoo : GH_Goo<BeamSection>
  {
    #region constructors
    public BeamSectionGoo()
    {
      this.Value = new BeamSection();
    }
    public BeamSectionGoo(BeamSection item)
    {
      if (item == null)
        item = new BeamSection();
      this.Value = item.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGoo();
    }
    public BeamSectionGoo DuplicateGoo()
    {
      return new BeamSectionGoo(Value == null ? new BeamSection() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid => true;
    public override string TypeName => "Beam Section";
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

      if (typeof(Q).IsAssignableFrom(typeof(BeamSection)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(string)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.SectionDescription;
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

      //Cast from custom type
      if (typeof(BeamSection).IsAssignableFrom(source.GetType()))
      {
        Value = (BeamSection)source;
        return true;
      }

      //Cast from string
      if (GH_Convert.ToString(source, out string mystring, GH_Conversion.Both))
      {
        Value = new BeamSection(mystring);
        return true;
      }
      return false;
    }
    #endregion
  }
}
