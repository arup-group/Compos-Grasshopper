using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum StandardSize
  {
    D13mmH65mm,
    D16mmH70mm,
    D16mmH75mm,
    D19mmH75mm,
    D19mmH95mm,
    D19mmH100mm,
    D19mmH125mm,
    D22mmH95mm,
    D22mmH100mm,
    D25mmH95mm,
    D25mmH100mm,
  }

  public enum StandardGrade
  {
    SD1_EN13918,
    SD2_EN13918,
    SD3_EN13918,
  }

  /// <summary>
  /// Object for setting dimensions and strength for a <see cref="ComposGH.Stud.Stud"/>
  /// </summary>
  public class StudDimensions : IStudDimensions
  {
    public Length Diameter { get; set; }
    public Length Height { get; set; }
    public Pressure Fu
    {
      get { return this.m_fu; }
      set
      {
        this.m_fu = value;
        this.m_strength = Force.Zero;
      }
    }
    public Force CharacterStrength
    {
      get { return this.m_strength; }
      set
      {
        this.m_strength = value;
        this.m_fu = Pressure.Zero;
      }
    }
    private Force m_strength { get; set; }
    private Pressure m_fu { get; set; }

    private void SetSizeFromStandard(StandardSize size)
    {
      switch (size)
      {
        case StandardSize.D13mmH65mm:
          this.Diameter = new Length(13, LengthUnit.Millimeter);
          this.Height = new Length(65, LengthUnit.Millimeter);
          break;
        case StandardSize.D16mmH75mm:
          this.Diameter = new Length(16, LengthUnit.Millimeter);
          this.Height = new Length(75, LengthUnit.Millimeter);
          break;
        case StandardSize.D19mmH75mm:
          this.Diameter = new Length(19, LengthUnit.Millimeter);
          this.Height = new Length(75, LengthUnit.Millimeter);
          break;
        case StandardSize.D19mmH100mm:
          this.Diameter = new Length(19, LengthUnit.Millimeter);
          this.Height = new Length(100, LengthUnit.Millimeter);
          break;
        case StandardSize.D19mmH125mm:
          this.Diameter = new Length(19, LengthUnit.Millimeter);
          this.Height = new Length(125, LengthUnit.Millimeter);
          break;
        case StandardSize.D22mmH100mm:
          this.Diameter = new Length(22, LengthUnit.Millimeter);
          this.Height = new Length(100, LengthUnit.Millimeter);
          break;
        case StandardSize.D25mmH100mm:
          this.Diameter = new Length(25, LengthUnit.Millimeter);
          this.Height = new Length(100, LengthUnit.Millimeter);
          break;
        case StandardSize.D16mmH70mm:
          this.Diameter = new Length(16, LengthUnit.Millimeter);
          this.Height = new Length(70, LengthUnit.Millimeter);
          break;
        case StandardSize.D19mmH95mm:
          this.Diameter = new Length(19, LengthUnit.Millimeter);
          this.Height = new Length(95, LengthUnit.Millimeter);
          break;
        case StandardSize.D22mmH95mm:
          this.Diameter = new Length(22, LengthUnit.Millimeter);
          this.Height = new Length(95, LengthUnit.Millimeter);
          break;
        case StandardSize.D25mmH95mm:
          this.Diameter = new Length(25, LengthUnit.Millimeter);
          this.Height = new Length(95, LengthUnit.Millimeter);
          break;
      }
    }

    

    private void SetGradeFromStandard(StandardGrade standardGrade)
    {
      switch (standardGrade)
      {
        case StandardGrade.SD1_EN13918:
          this.Fu = new Pressure(400, PressureUnit.NewtonPerSquareMillimeter);
          break;
        case StandardGrade.SD2_EN13918:
          this.Fu = new Pressure(450, PressureUnit.NewtonPerSquareMillimeter);
          break;
        case StandardGrade.SD3_EN13918:
          this.Fu = new Pressure(500, PressureUnit.NewtonPerSquareMillimeter);
          break;
      }
    }


    #region constructors
      public StudDimensions()
    {
      // empty constructor
    }
    /// <summary>
    /// Create Custom size with strength from Stress
    /// </summary>
    /// <param name="diameter"></param>
    /// <param name="height"></param>
    /// <param name="fu"></param>
    public StudDimensions(Length diameter, Length height, Pressure fu)
    {
      this.Diameter = diameter;
      this.Height = height;
      this.Fu = fu;
    }
    /// <summary>
    /// Create Custom size with strength from Force
    /// </summary>
    /// <param name="diameter"></param>
    /// <param name="height"></param>
    /// <param name="strength"></param>
    public StudDimensions(Length diameter, Length height, Force strength)
    {
      this.Diameter = diameter;
      this.Height = height;
      this.CharacterStrength = strength;
    }
    /// <summary>
    /// Create Standard size with strength from Stress
    /// </summary>
    /// <param name="size"></param>
    /// <param name="strength"></param>
    public StudDimensions(StandardSize size, Force strength)
    {
      SetSizeFromStandard(size);
      this.CharacterStrength = strength;
    }

    /// <summary>
    /// Create Standard size with strength from Stress
    /// </summary>
    /// <param name="size"></param>
    /// <param name="fu"></param>
    public StudDimensions(StandardSize size, Pressure fu)
    {
      SetSizeFromStandard(size);
      this.Fu = fu;
    }
    /// <summary>
    /// Create Standard size with Standard Grade
    /// </summary>
    /// <param name="size"></param>
    /// <param name="standardGrade"></param>
    public StudDimensions(StandardSize size, StandardGrade standardGrade)
    {
      SetSizeFromStandard(size);
      SetGradeFromStandard(standardGrade);
    }

    /// <summary>
    /// Create Custom size with Standard Grade
    /// </summary>
    /// <param name="diameter"></param>
    /// <param name="height"></param>
    /// <param name="standardGrade"></param>
    public StudDimensions(Length diameter, Length height, StandardGrade standardGrade)
    {
      this.Diameter = diameter;
      this.Height = height;
      SetGradeFromStandard(standardGrade);
    }


    internal StudDimensions FromCoaString(List<string> parameters, LengthUnit lengthSectionUnit, ForceUnit forceUnit, Code code)
    {
      if(parameters[2] == CoaIdentifier.StudDimensions.StudDimensionStandard)
      {
        string size = "D" + parameters[3].Replace("/", "H");
        StandardSize standardSize = (StandardSize)Enum.Parse(typeof(StandardSize), size);
        SetSizeFromStandard(standardSize);
        switch (code)
        {
          case Code.BS5950_3_1_1990_Superseded:
          case Code.BS5950_3_1_1990_A1_2010:
            this.CharacterStrength = new Force(90, ForceUnit.Kilonewton);
            break;
          case Code.HKSUOS_2005:
          case Code.HKSUOS_2011:
            this.CharacterStrength = new Force(76.3497, ForceUnit.Kilonewton);
            break;
          case Code.AS_NZS2327_2017:
            this.CharacterStrength = new Force(97.9845, ForceUnit.Kilonewton);
            break;
        }
      }

      if (parameters[2] == CoaIdentifier.StudDimensions.StudDimensionCustom)
      {
        NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
        this.Diameter = new Length(Convert.ToDouble(parameters[3], noComma), lengthSectionUnit);
        this.Height = new Length(Convert.ToDouble(parameters[4], noComma), lengthSectionUnit);
        this.CharacterStrength = new Force(Convert.ToDouble(parameters[5], noComma), forceUnit);
      }
      return this;
    }

    #endregion

    #region methods
    public override string ToString()
    {
      string dia = Diameter.As(Units.LengthUnitSection).ToString("f0");
      string h = Height.ToUnit(Units.LengthUnitSection).ToString("f0");
      string f = (Fu.Value == 0) ? CharacterStrength.ToUnit(Units.ForceUnit).ToString("f0") : Fu.ToUnit(Units.StressUnit).ToString("f0");

      return "Ø" + dia.Replace(" ", string.Empty) + "/" + h.Replace(" ", string.Empty) + ", f:" + f.Replace(" ", string.Empty);
    }
    #endregion
  }
}
