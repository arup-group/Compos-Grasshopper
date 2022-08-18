using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ComposAPI.Helpers;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public enum StandardStudSize
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

  public enum StandardStudGrade
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
    public Length Diameter { get; set; } // diameter of stud
    public Length Height { get; set; } // welded height of stud
    public Pressure Fu
    {
      get { return this.m_fu; }
      set
      {
        this.m_fu = value;
        this.m_strength = Force.Zero;
      }
    }
    public Force CharacterStrength // characteristic strength of stud
    {
      get { return this.m_strength; }
      set
      {
        this.m_strength = value;
        this.m_fu = Pressure.Zero;
        this.IsStandard = false;
      }
    }
    public bool IsStandard { get; set; }
    private Force m_strength { get; set; }
    private Pressure m_fu { get; set; }

    public void SetSizeFromStandard(StandardStudSize size)
    {
      this.IsStandard = true;
      switch (size)
      {
        case StandardStudSize.D13mmH65mm:
          this.Diameter = new Length(13, LengthUnit.Millimeter);
          this.Height = new Length(65, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D16mmH75mm:
          this.Diameter = new Length(16, LengthUnit.Millimeter);
          this.Height = new Length(75, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D19mmH75mm:
          this.Diameter = new Length(19, LengthUnit.Millimeter);
          this.Height = new Length(75, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D19mmH100mm:
          this.Diameter = new Length(19, LengthUnit.Millimeter);
          this.Height = new Length(100, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D19mmH125mm:
          this.Diameter = new Length(19, LengthUnit.Millimeter);
          this.Height = new Length(125, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D22mmH100mm:
          this.Diameter = new Length(22, LengthUnit.Millimeter);
          this.Height = new Length(100, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D25mmH100mm:
          this.Diameter = new Length(25, LengthUnit.Millimeter);
          this.Height = new Length(100, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D16mmH70mm:
          this.Diameter = new Length(16, LengthUnit.Millimeter);
          this.Height = new Length(70, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D19mmH95mm:
          this.Diameter = new Length(19, LengthUnit.Millimeter);
          this.Height = new Length(95, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D22mmH95mm:
          this.Diameter = new Length(22, LengthUnit.Millimeter);
          this.Height = new Length(95, LengthUnit.Millimeter);
          break;
        case StandardStudSize.D25mmH95mm:
          this.Diameter = new Length(25, LengthUnit.Millimeter);
          this.Height = new Length(95, LengthUnit.Millimeter);
          break;
      }
    }

    public void SetGradeFromStandard(StandardStudGrade standardGrade)
    {
      switch (standardGrade)
      {
        case StandardStudGrade.SD1_EN13918:
          this.Fu = new Pressure(400, PressureUnit.NewtonPerSquareMillimeter);
          break;
        case StandardStudGrade.SD2_EN13918:
          this.Fu = new Pressure(450, PressureUnit.NewtonPerSquareMillimeter);
          break;
        case StandardStudGrade.SD3_EN13918:
          this.Fu = new Pressure(500, PressureUnit.NewtonPerSquareMillimeter);
          break;
      }
      this.IsStandard = true;
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
    /// Create Standard size with standard strength depending on code applied
    /// </summary>
    /// <param name="size"></param>
    /// <param name="strength"></param>
    public StudDimensions(StandardStudSize size)
    {
      this.SetSizeFromStandard(size);
    }

    /// <summary>
    /// Create Standard size with custom strength from Stress
    /// </summary>
    /// <param name="size"></param>
    /// <param name="strength"></param>
    public StudDimensions(StandardStudSize size, Force strength)
    {
      this.SetSizeFromStandard(size);
      this.CharacterStrength = strength;
    }

    /// <summary>
    /// Create Standard size with a custom strength from Stress
    /// </summary>
    /// <param name="size"></param>
    /// <param name="fu"></param>
    public StudDimensions(StandardStudSize size, Pressure fu)
    {
      this.SetSizeFromStandard(size);
      this.Fu = fu;
    }

    /// <summary>
    /// Create Standard size with Standard Grade
    /// </summary>
    /// <param name="size"></param>
    /// <param name="standardGrade"></param>
    public StudDimensions(StandardStudSize size, StandardStudGrade standardGrade)
    {
      this.SetSizeFromStandard(size);
      this.SetGradeFromStandard(standardGrade);
      this.IsStandard = true;
    }

    /// <summary>
    /// Create Custom size with Standard Grade
    /// </summary>
    /// <param name="diameter"></param>
    /// <param name="height"></param>
    /// <param name="standardGrade"></param>
    public StudDimensions(Length diameter, Length height, StandardStudGrade standardGrade)
    {
      this.Diameter = diameter;
      this.Height = height;
      this.SetGradeFromStandard(standardGrade);
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string dia = this.Diameter.As(this.Height.Unit).ToString("g5");
      string h = this.Height.ToString("g5");
      string f = (this.Fu.Value == 0) ? this.CharacterStrength.ToString("g5") : Fu.ToString("g5");
      
      return "Ø" + dia.Replace(" ", string.Empty) + "/" + h.Replace(" ", string.Empty) + ((this.IsStandard) ? "" : ", f:" + f.Replace(" ", string.Empty));
    }
    #endregion
  }
}
