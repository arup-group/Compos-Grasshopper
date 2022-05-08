using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  /// <summary>
  /// Stud class containing <see cref="ComposAPI.StudDimensions"/>, <see cref="ComposAPI.StudSpecification"/>, and spacing/layout settings (custom or automatic)
  /// </summary>
  public class Stud : IStud
  {
    public IStudDimensions StudDimensions { get; set; }
    public IStudSpecification StudSpecification { get; set; }
    // Stud Spacing
    public List<IStudGroupSpacing> CustomSpacing { get; set; } = null;
    public double Interaction { get; set; }
    public double MinSavingMultipleZones { get; set; }
    public bool CheckStudSpacing { get; set; }

    public StudSpacingType StudSpacingType
    {
      get { return this.m_spacingType; }
      set
      {
        this.m_spacingType = value;
        switch (value)
        {
          case StudSpacingType.Custom:
            this.Interaction = double.NaN;
            this.MinSavingMultipleZones = double.NaN;
            break;
          case StudSpacingType.Automatic:
          case StudSpacingType.Min_Num_of_Studs:
            this.Interaction = double.NaN;
            if (this.MinSavingMultipleZones == double.NaN)
              this.MinSavingMultipleZones = 0.20;
            break;
          case StudSpacingType.Partial_Interaction:
            if (this.Interaction == double.NaN)
              this.Interaction = 0.85;
            if (this.MinSavingMultipleZones == double.NaN)
              this.MinSavingMultipleZones = 0.20;
            break;
        }
      }
    }

    private StudSpacingType m_spacingType;

    #region constructors
    public Stud()
    {
      // empty constructor
    }

    /// <summary>
    /// Create Stud Dimensions with Custom spacing type
    /// </summary>
    /// <param name="stud"></param>
    /// <param name="spec"></param>
    /// <param name="spacings"></param>
    /// <param name="checkSpacing"></param>
    public Stud(IStudDimensions stud, IStudSpecification spec, List<IStudGroupSpacing> spacings, bool checkSpacing)
    {
      this.StudDimensions = stud;
      this.StudSpecification = spec;
      this.CustomSpacing = spacings;
      this.CheckStudSpacing = checkSpacing;
      this.StudSpacingType = StudSpacingType.Custom;
      this.Interaction = double.NaN;
      this.MinSavingMultipleZones = double.NaN;
    }

    /// <summary>
    /// Create Stud Dimensions with Automatic or Minimum number of Studs types
    /// </summary>
    /// <param name="stud"></param>
    /// <param name="spec"></param>
    /// <param name="minSaving"></param>
    /// <param name="type"></param>
    /// <exception cref="ArgumentException"></exception>
    public Stud(IStudDimensions stud, IStudSpecification spec, double minSaving, StudSpacingType type)
    {
      this.StudDimensions = stud;
      this.StudSpecification = spec;
      this.StudSpacingType = type;
      this.MinSavingMultipleZones = minSaving;
      switch (type)
      {
        case StudSpacingType.Min_Num_of_Studs:
        case StudSpacingType.Automatic:
          break;

        default:
          throw new ArgumentException("Stud spacing type must be either Automatic or Minimum Number of Studs");
      }
      this.Interaction = double.NaN;
    }

    /// <summary>
    /// Create Stud Dimensions with Partial interaction type
    /// </summary>
    /// <param name="stud"></param>
    /// <param name="spec"></param>
    /// <param name="minSaving"></param>
    /// <param name="interaction"></param>
    public Stud(IStudDimensions stud, IStudSpecification spec, double minSaving, double interaction)
    {
      this.StudDimensions = stud;
      this.StudSpecification = spec;
      this.StudSpacingType = StudSpacingType.Partial_Interaction;
      this.MinSavingMultipleZones = minSaving;
      this.Interaction = interaction;
    }
    #endregion

    #region coa interop
    internal Stud FromCoaString(List<string> parameters, ForceUnit forceUnit, LengthUnit lengthUnit)
    {
      

    }

    public string ToCoaString(string name, ForceUnit forceUnit, PressureUnit stressUnit, LengthUnit lengthGeometryUnit, LengthUnit lengthSectionUnit, Code designCode)
    {
      //STUD_DEFINITION	MEMBER-1	STANDARD	19mm/100mm	WELDED_YES
      //STUD_DEFINITION	MEMBER-2	USER_DEFINED	19.0000	100.000	95000.0	REDUCED_YES	WELDED_YES
      //STUD_DEFINITION	MEMBER-3	USER_DEFINED	12.0000	345.000	75982.5	REDUCED_NO	WELDED_YES
      string str = "STUD_DEFINITION" + '\t' + name + '\t';
      string studSize = GetStandardSize(this.StudDimensions);
      if (studSize == "Custom")
      {
        studSize = "USER_DEFINED" + '\t';
        studSize += CoaHelper.FormatSignificantFigures(this.StudDimensions.Diameter.ToUnit(lengthSectionUnit).Value, 6) + '\t';
        studSize += CoaHelper.FormatSignificantFigures(this.StudDimensions.Height.ToUnit(lengthSectionUnit).Value, 6) + '\t';
        studSize += CoaHelper.FormatSignificantFigures(this.StudDimensions.CharacterStrength.ToUnit(forceUnit).Value, 6) + '\t';
        studSize += "REDUCED_NO" + '\t';
      }
      else
        studSize = "STANDARD" + '\t' + studSize + '\t';
      str += studSize + ((this.StudSpecification.Welding) ? "WELDED_YES" : "WELDED_NO") + '\n';

      
      if (this.StudSpacingType == StudSpacingType.Custom)
      {
        for (int i = 0; i < this.CustomSpacing.Count; i++)
        {
          str += "STUD_LAYOUT" + '\t' + name + '\t';
          str += CoaIdentifier.StudGroupSpacings.StudLayoutCustom + '\t';
          str += this.CustomSpacing.Count.ToString() + '\t' + (i + 1).ToString() + '\t';
          str += CoaHelper.FormatSignificantFigures(this.CustomSpacing[i].DistanceFromStart.ToUnit(lengthGeometryUnit).Value, 6) + '\t';
          str += this.CustomSpacing[i].NumberOfRows + '\t' + this.CustomSpacing[i].NumberOfLines + '\t';
          str += this.CustomSpacing[i].NumberOfRows + '\t' + this.CustomSpacing[i].NumberOfLines + '\t';
          // these next two values are documented as row-spacing:	spacing of the rows and line-spacing: spacing of the lines but cannot be set anywhere in Compos?? - first is 76mm except for ASNZ code where it is 57mm, second is always 95mm
          double rowSpacing = (designCode == Code.AS_NZS2327_2017) ? 0.057 : 0.076;
          str += CoaHelper.FormatSignificantFigures(new Length(rowSpacing, LengthUnit.Meter).ToUnit(lengthGeometryUnit).Value, 6) + '\t';
          str += CoaHelper.FormatSignificantFigures(new Length(0.095, LengthUnit.Meter).ToUnit(lengthGeometryUnit).Value, 6) + '\t';
          str += CoaHelper.FormatSignificantFigures(this.CustomSpacing[i].Spacing.ToUnit(lengthGeometryUnit).Value, 6) + '\t';
          str += this.CustomSpacing[i].
        }
      }
      else
      {
        str += "STUD_LAYOUT" + '\t' + name + '\t';
        switch (this.StudSpacingType)
        {
          case StudSpacingType.Automatic:
            str += CoaIdentifier.StudGroupSpacings.StudLayoutAutomatic + '\t';
            str += CoaHelper.FormatSignificantFigures(this.MinSavingMultipleZones, 6) + '\t';
            break;
          
          case StudSpacingType.Partial_Interaction:
            str += CoaIdentifier.StudGroupSpacings.StudLayoutPartial_Interaction + '\t';
            str += CoaHelper.FormatSignificantFigures(this.MinSavingMultipleZones, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(this.Interaction, 6) + '\t';
            break;
          
          case StudSpacingType.Min_Num_of_Studs:
            str += CoaIdentifier.StudGroupSpacings.StudLayoutMin_Num_of_Studs + '\t';
            str += CoaHelper.FormatSignificantFigures(this.MinSavingMultipleZones, 6) + '\t';
            break;
          
          default:
            throw new Exception("Unknown stud spacing type");
        }
      }
      


      str = str.Remove(str.Length - 1, 1);
      str += '\n';
      return str;
    }

    internal string GetStandardSize(IStudDimensions dimensions)
    {
      double dia = dimensions.Diameter.As(LengthUnit.Millimeter);
      if (!(Math.Abs(dia % 1) <= (Double.Epsilon * 100)))
        return "Custom";
      int d = (int)dia;

      double height = dimensions.Height.As(LengthUnit.Millimeter);
      if (!(Math.Abs(height % 1) <= (Double.Epsilon * 100)))
        return "Custom";
      int h = (int)height;

      switch (d)
      {
        case 13:
          switch (h)
          {
            //D13mmH65mm,
            case 65:
              return d + "mm/" + h + "mm";
            default:
              return "Custom";
          }
        case 16:
          switch (h)
          {
            //D16mmH70mm,
            //D16mmH75mm,
            case 70:
            case 75:
              return d + "mm/" + h + "mm";
            default:
              return "Custom";
          }
        case 19:
          switch (h)
          {
            //D19mmH75mm,
            //D19mmH95mm,
            //D19mmH100mm,
            //D19mmH125mm,
            case 75:
            case 95:
            case 100:
            case 125:
              return d + "mm/" + h + "mm";
            default:
              return "Custom";
          }
        case 22:
          switch (h)
          {
            //D22mmH95mm,
            //D22mmH100mm,
            case 95:
            case 100:
              return d + "mm/" + h + "mm";
            default:
              return "Custom";
          }
        case 25:
          switch (h)
          {
            //D25mmH95mm,
            //D25mmH100mm,
            case 95:
            case 100:
              return d + "mm/" + h + "mm";
            default:
              return "Custom";
          }
        default:
          return "Custom";
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string size = this.StudDimensions.Diameter.As(Units.LengthUnitSection).ToString("f0") + "/" + this.StudDimensions.Height.ToUnit(Units.LengthUnitSection).ToString("f0");
      return size.Replace(" ", string.Empty);
    }

    #endregion
  }
}
