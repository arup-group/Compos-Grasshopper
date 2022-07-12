﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  /// <summary>
  /// Stud class containing <see cref="ComposAPI.StudDimensions"/>, <see cref="ComposAPI.StudSpecification"/>, and spacing/layout settings (custom or automatic)
  /// </summary>
  public class Stud : IStud
  {
    public IStudDimensions Dimensions { get; set; }
    public IStudSpecification Specification { get; set; }
    // Stud Spacing
    public IList<IStudGroupSpacing> CustomSpacing { get; set; } = null;
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
      this.Dimensions = stud;
      this.Specification = spec;
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
      this.Dimensions = stud;
      this.Specification = spec;
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
      this.Dimensions = stud;
      this.Specification = spec;
      this.StudSpacingType = StudSpacingType.Partial_Interaction;
      this.MinSavingMultipleZones = minSaving;
      this.Interaction = interaction;
    }
    #endregion

    #region coa interop
    internal static Stud FromCoaString(string coaString, string name, Code code, ComposUnits units)
    {
      Stud stud = new Stud();
      stud.Specification = new StudSpecification();
      stud.Dimensions = new StudDimensions();
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;

      List<string> lines = CoaHelper.SplitAndStripLines(coaString);
      foreach (string line in lines)
      {
        List<string> parameters = CoaHelper.Split(line);

        if (parameters[0] == "END")
          return stud;

        if (parameters[1] != name)
          continue;

        switch (parameters[0])
        {
          case (CoaIdentifier.UnitData):
            units.FromCoaString(parameters);
            break;

          case (CoaIdentifier.StudDimensions.StudDefinition):
            // ### Stud dimensions  / STUD_DEFINITION ###
            //STUD_DEFINITION	MEMBER-1	STANDARD	19mm/100mm	WELDED_YES
            //STUD_DEFINITION	MEMBER-2	USER_DEFINED	19.0000	100.000	95000.0	REDUCED_YES	WELDED_YES
            //STUD_DEFINITION	MEMBER-3	USER_DEFINED	12.0000	345.000	75982.5	REDUCED_NO	WELDED_YES
            if (parameters[2] == CoaIdentifier.StudDimensions.StudDimensionStandard)
            {
              string size = "D" + parameters[3].Replace("/", "H");
              StandardStudSize standardSize = (StandardStudSize)Enum.Parse(typeof(StandardStudSize), size);
              stud.Dimensions.SetSizeFromStandard(standardSize);
              switch (code)
              {
                case Code.BS5950_3_1_1990_Superseded:
                case Code.BS5950_3_1_1990_A1_2010:
                  stud.Dimensions.CharacterStrength = new Force(90, ForceUnit.Kilonewton);
                  break;
                case Code.HKSUOS_2005:
                case Code.HKSUOS_2011:
                  stud.Dimensions.CharacterStrength = new Force(76.3497, ForceUnit.Kilonewton);
                  break;
                case Code.AS_NZS2327_2017:
                  stud.Dimensions.CharacterStrength = new Force(97.9845, ForceUnit.Kilonewton);
                  break;
              }
              stud.Dimensions.IsStandard = true;
            }
            else if (parameters[2] == CoaIdentifier.StudDimensions.StudDimensionCustom)
            {
              stud.Dimensions.Diameter = new Length(Convert.ToDouble(parameters[3], noComma), units.Section);
              stud.Dimensions.Height = new Length(Convert.ToDouble(parameters[4], noComma), units.Section);
              stud.Dimensions.CharacterStrength = new Force(Convert.ToDouble(parameters[5], noComma), units.Force);
            }
            bool isWelded = parameters.Last() == "WELDED_YES";
            stud.Specification.Welding = isWelded;
            break;

          case (CoaIdentifier.StudSpecifications.StudEC4):
            //STUD_EC4_APPLY	MEMBER-1	YES
            stud.Specification.EC4_Limit = parameters[2] == "YES";
            stud.Specification.SpecType = StudSpecType.BS5950;
            break;

          // this needs to be EC4_STUD_GRADE instead of STUD_GRADE (wrong in Compos documentation!)
          case ("EC4_STUD_GRADE"):
            //EC4_STUD_GRADE	MEMBER-1	CODE_GRADE_NO	4.50000e+008
            //EC4_STUD_GRADE	MEMBER-1	CODE_GRADE_YES	SD2_EN13918
            if (parameters[2] == CoaIdentifier.StudDimensions.StudGradeEC4Standard)
            {
              StandardStudGrade standardGrade = (StandardStudGrade)Enum.Parse(typeof(StandardStudGrade), parameters[3]);
              stud.Dimensions.SetGradeFromStandard(standardGrade);
              stud.Dimensions.IsStandard = true;
            }
            else if (parameters[2] == CoaIdentifier.StudDimensions.StudGradeEC4Custom)
            {
              stud.Dimensions.Fu = new Pressure(Convert.ToDouble(parameters[3], noComma), units.Stress);
            }
            break;

          case ("STUD_LAYOUT"):
            //STUD_LAYOUT	MEMBER-1	AUTO_100	0.200000
            //STUD_LAYOUT	MEMBER-1	AUTO_PERCENT	0.200000	0.850000
            //STUD_LAYOUT	MEMBER-1	AUTO_MINIMUM_STUD	0.200000
            //STUD_LAYOUT	MEMBER-1	USER_DEFINED	3	1	0.000000	2	1	0.0760000	0.0950000	0.150000	CHECK_SPACE_NO
            switch (parameters[2])
            {
              case CoaIdentifier.StudGroupSpacings.StudLayoutAutomatic:
                stud.StudSpacingType = StudSpacingType.Automatic;
                stud.MinSavingMultipleZones = Convert.ToDouble(parameters[3], noComma);
                break;

              case CoaIdentifier.StudGroupSpacings.StudLayoutPartial_Interaction:
                stud.StudSpacingType = StudSpacingType.Partial_Interaction;
                stud.MinSavingMultipleZones = Convert.ToDouble(parameters[3], noComma);
                stud.Interaction = Convert.ToDouble(parameters[4], noComma);
                break;

              case CoaIdentifier.StudGroupSpacings.StudLayoutMin_Num_of_Studs:
                stud.StudSpacingType = StudSpacingType.Min_Num_of_Studs;
                stud.MinSavingMultipleZones = Convert.ToDouble(parameters[3], noComma);
                break;

              case CoaIdentifier.StudGroupSpacings.StudLayoutCustom:
                stud.StudSpacingType = StudSpacingType.Custom;
                stud.CheckStudSpacing = parameters.Last() != "CHECK_SPACE_NO";

                StudGroupSpacing custom = StudGroupSpacing.FromCoaString(parameters, units);
                if (stud.CustomSpacing == null)
                  stud.CustomSpacing = new List<IStudGroupSpacing>();
                stud.CustomSpacing.Add(custom);
                break;
            }
            break;

          case (CoaIdentifier.StudSpecifications.StudNCCI):
            //STUD_NCCI_LIMIT_APPLY	MEMBER-1	NO
            stud.Specification.NCCI = parameters[2] == "YES";
            stud.Specification.SpecType = StudSpecType.EC4;
            break;

          case (CoaIdentifier.StudSpecifications.StudNoZone):
            //STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000
            stud.Specification.NoStudZoneStart = new Length(Convert.ToDouble(parameters[2], noComma), units.Length);
            stud.Specification.NoStudZoneEnd = new Length(Convert.ToDouble(parameters[3], noComma), units.Length);
            break;

          case (CoaIdentifier.StudSpecifications.StudReinfPos):
            //STUD_EC4_RFT_POS	MEMBER-1	0.0300000
            stud.Specification.SpecType = StudSpecType.EC4;
            stud.Specification.ReinforcementPosition = new Length(Convert.ToDouble(parameters[2], noComma), units.Length);
            break;

          default:
            // continue;
            break;
        }
      }
      return stud;
    }

    public string ToCoaString(string name, ComposUnits units, Code designCode)
    {
      // ### Stud dimensions  / STUD_DEFINITION ###
      //STUD_DEFINITION	MEMBER-1	STANDARD	19mm/100mm	WELDED_YES
      //STUD_DEFINITION	MEMBER-2	USER_DEFINED	19.0000	100.000	95000.0	REDUCED_YES	WELDED_YES
      //STUD_DEFINITION	MEMBER-3	USER_DEFINED	12.0000	345.000	75982.5	REDUCED_NO	WELDED_YES
      string str = CoaIdentifier.StudDimensions.StudDefinition + '\t' + name + '\t';

      string studSize = this.GetStandardSize(this.Dimensions);
      if (!this.Dimensions.IsStandard)
      {
        studSize = CoaIdentifier.StudDimensions.StudDimensionCustom + '\t';
        studSize += CoaHelper.FormatSignificantFigures(this.Dimensions.Diameter.ToUnit(units.Section).Value, 6) + '\t';
        studSize += CoaHelper.FormatSignificantFigures(this.Dimensions.Height.ToUnit(units.Section).Value, 6) + '\t';
        studSize += CoaHelper.FormatSignificantFigures(this.Dimensions.CharacterStrength.ToUnit(units.Force).Value, 6) + '\t';
        studSize += "REDUCED_NO" + '\t';
      }
      else
        studSize = CoaIdentifier.StudDimensions.StudDimensionStandard + '\t' + studSize + '\t';
      str += studSize + ((this.Specification.Welding) ? "WELDED_YES" : "WELDED_NO") + '\n';

      // ### Stud spacing / STUD_LAYOUT###

      if (this.StudSpacingType == StudSpacingType.Custom)
      {
        //STUD_LAYOUT	MEMBER-1	USER_DEFINED	2	1	0.000000	2	1	0.0570000	0.0950000	0.150000	CHECK_SPACE_NO
        //STUD_LAYOUT MEMBER-1 USER_DEFINED 2 2 8.000000 3 2 0.0570000 0.0950000 0.250000 CHECK_SPACE_NO
        //STUD_LAYOUT	MEMBER-1	USER_DEFINED	1	1	0.000000	2	1	76.0000	95.0000	150.000	CHECK_SPACE_NO
        for (int i = 0; i < this.CustomSpacing.Count; i++)
        {
          str += CoaIdentifier.StudGroupSpacings.StudLayout + '\t' + name + '\t';
          str += CoaIdentifier.StudGroupSpacings.StudLayoutCustom + '\t';
          str += this.CustomSpacing.Count.ToString() + '\t' + (i + 1).ToString() + '\t';
          str += CoaHelper.FormatSignificantFigures(this.CustomSpacing[i].DistanceFromStart.ToUnit(units.Length).Value, 6) + '\t';
          str += this.CustomSpacing[i].NumberOfRows.ToString() + '\t';
          str += this.CustomSpacing[i].NumberOfLines.ToString() + '\t';
          // these next two values are documented as row-spacing:	spacing of the rows and line-spacing: spacing of the lines but cannot be set anywhere in Compos?? - first is 76mm except for ASNZ code where it is 57mm, second is always 95mm
          double rowSpacing = (designCode == Code.AS_NZS2327_2017) ? 0.057 : 0.076;
          str += CoaHelper.FormatSignificantFigures(new Length(rowSpacing, LengthUnit.Meter).ToUnit(units.Length).Value, 6) + '\t';
          str += CoaHelper.FormatSignificantFigures(new Length(0.095, LengthUnit.Meter).ToUnit(units.Length).Value, 6) + '\t';
          str += CoaHelper.FormatSignificantFigures(this.CustomSpacing[i].Spacing.ToUnit(units.Length).Value, 6) + '\t';
          str += (this.CheckStudSpacing) ? CoaIdentifier.StudGroupSpacings.StudLayoutCheckCustom : "CHECK_SPACE_NO";
          str += '\n';
        }
      }
      else
      {
        str += CoaIdentifier.StudGroupSpacings.StudLayout + '\t' + name + '\t';
        switch (this.StudSpacingType)
        {
          case StudSpacingType.Automatic:
            //STUD_LAYOUT	MEMBER-1	AUTO_100	0.200000
            str += CoaIdentifier.StudGroupSpacings.StudLayoutAutomatic + '\t';
            str += CoaHelper.FormatSignificantFigures(this.MinSavingMultipleZones, 6) + '\n';
            break;

          case StudSpacingType.Partial_Interaction:
            //STUD_LAYOUT	MEMBER-1	AUTO_PERCENT	0.200000	0.850000
            str += CoaIdentifier.StudGroupSpacings.StudLayoutPartial_Interaction + '\t';
            str += CoaHelper.FormatSignificantFigures(this.MinSavingMultipleZones, 6) + '\t';
            str += CoaHelper.FormatSignificantFigures(this.Interaction, 6) + '\n';
            break;

          case StudSpacingType.Min_Num_of_Studs:
            //STUD_LAYOUT	MEMBER-1	AUTO_MINIMUM_STUD	0.200000
            str += CoaIdentifier.StudGroupSpacings.StudLayoutMin_Num_of_Studs + '\t';
            str += CoaHelper.FormatSignificantFigures(this.MinSavingMultipleZones, 6) + '\n';
            break;

          default:
            throw new Exception("Unknown stud spacing type");
        }
      }

      // ### No Stud Zone / STUD_NO_STUD_ZONE ###
      // STUD_NO_STUD_ZONE	MEMBER-1	0.000000	0.000000
      str += CoaIdentifier.StudSpecifications.StudNoZone + '\t' + name + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Specification.NoStudZoneStart.ToUnit(units.Length).Value, 6) + '\t';
      str += CoaHelper.FormatSignificantFigures(this.Specification.NoStudZoneEnd.ToUnit(units.Length).Value, 6) + '\n';

      // ### Other code-dependent specs ###
      switch (this.Specification.SpecType)
      {
        case StudSpecType.BS5950:
          //STUD_EC4_APPLY	MEMBER-1	YES
          str += CoaIdentifier.StudSpecifications.StudEC4 + '\t' + name + '\t' + ((this.Specification.EC4_Limit) ? "YES" : "NO") + '\n';
          break;

        case StudSpecType.EC4:
          //STUD_EC4_APPLY	MEMBER-1	YES - this is always set
          str += CoaIdentifier.StudSpecifications.StudEC4 + '\t' + name + '\t' + "YES" + '\n';
          //STUD_NCCI_LIMIT_APPLY MEMBER-1 NO
          str += CoaIdentifier.StudSpecifications.StudNCCI + '\t' + name + '\t' + ((this.Specification.NCCI) ? "YES" : "NO") + '\n';
          //STUD_EC4_RFT_POS	MEMBER-1	0.0300000
          str += CoaIdentifier.StudSpecifications.StudReinfPos + '\t' + name + '\t';
          str += CoaHelper.FormatSignificantFigures(this.Specification.ReinforcementPosition.ToUnit(units.Length).Value, 6) + '\n';
          //EC4_STUD_GRADE	MEMBER-1	CODE_GRADE_YES	SD2_EN13918
          //EC4_STUD_GRADE	MEMBER-1	CODE_GRADE_NO	4.50000e+008
          str += CoaIdentifier.StudDimensions.StudGradeEC4 + '\t' + name + '\t';
          string studGrade = GetStandardGrade(this.Dimensions);
          if (studGrade == "Custom")
          {
            str += CoaIdentifier.StudDimensions.StudGradeEC4Custom + '\t';
            str += CoaHelper.FormatSignificantFigures(this.Dimensions.Fu.ToUnit(units.Stress).Value, 6) + '\n';
          }
          else
          {
            str += CoaIdentifier.StudDimensions.StudGradeEC4Standard + '\t' + studGrade + '\n';
          }
          break;

        case StudSpecType.Other:
          //STUD_EC4_APPLY	MEMBER-1	YES - this is always set
          str += CoaIdentifier.StudSpecifications.StudEC4 + '\t' + name + '\t' + "YES" + '\n';
          break;

        default:
          break;
      }

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

    internal string GetStandardGrade(IStudDimensions dimensions)
    {
      double fu = dimensions.Fu.As(PressureUnit.NewtonPerSquareMillimeter);
      if (!(Math.Abs(fu % 1) <= (Double.Epsilon * 100)))
        return "Custom";
      int f = (int)fu;

      switch (f)
      {
        case 400:
          return "SD1_EN13918";

        case 450:
          return "SD2_EN13918";

        case 500:
          return "SD3_EN13918";

        default:
          return "Custom";
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string size = this.Dimensions.Diameter.As(Units.LengthUnitSection).ToString("f0") + "/" + this.Dimensions.Height.ToUnit(Units.LengthUnitSection).ToString("f0");
      return size.Replace(" ", string.Empty);
    }

    #endregion
  }
}
