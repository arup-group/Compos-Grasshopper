﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Object for setting dimensions and strength for a <see cref="ComposGH.Stud.Stud"/>
  /// </summary>
  public class StudDimensions
  {
    public Length Diameter { get; set; }
    public Length Height { get; set; }

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
    private void SetSizeFromStandard(StandardSize size)
    {
      switch (size)
      {
        case StandardSize.D13mmH65mm:
          this.Diameter = new Length(13, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(65, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D16mmH75mm:
          this.Diameter = new Length(16, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(75, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D19mmH75mm:
          this.Diameter = new Length(19, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(75, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D19mmH100mm:
          this.Diameter = new Length(19, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(100, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D19mmH125mm:
          this.Diameter = new Length(19, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(125, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D22mmH100mm:
          this.Diameter = new Length(22, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(100, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D25mmH100mm:
          this.Diameter = new Length(25, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(100, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D16mmH70mm:
          this.Diameter = new Length(16, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(70, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D19mmH95mm:
          this.Diameter = new Length(19, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(95, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D22mmH95mm:
          this.Diameter = new Length(22, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(95, UnitsNet.Units.LengthUnit.Millimeter);
          break;
        case StandardSize.D25mmH95mm:
          this.Diameter = new Length(25, UnitsNet.Units.LengthUnit.Millimeter);
          this.Height = new Length(95, UnitsNet.Units.LengthUnit.Millimeter);
          break;
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
    private Force m_strength;
    public Pressure Fu
    {
      get { return this.m_fu; }
      set
      {
        this.m_fu = value;
        this.m_strength = Force.Zero;
      }
    }
    private Pressure m_fu;

    public enum StandardGrade
    {
      SD1_EN13918,
      SD2_EN13918,
      SD3_EN13918,
    }
    private void SetGradeFromStandard(StandardGrade standardGrade)
    {
      switch (standardGrade)
      {
        case StandardGrade.SD1_EN13918:
          this.Fu = new Pressure(400, UnitsNet.Units.PressureUnit.NewtonPerSquareMillimeter);
          break;
        case StandardGrade.SD2_EN13918:
          this.Fu = new Pressure(450, UnitsNet.Units.PressureUnit.NewtonPerSquareMillimeter);
          break;
        case StandardGrade.SD3_EN13918:
          this.Fu = new Pressure(500, UnitsNet.Units.PressureUnit.NewtonPerSquareMillimeter);
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

    #endregion

    #region methods

    public StudDimensions Duplicate()
    {
      if (this == null) { return null; }
      StudDimensions dup = (StudDimensions)this.MemberwiseClone();
      return dup;
    }
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