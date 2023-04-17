using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public enum StandardStudGrade {
    SD1_EN13918,
    SD2_EN13918,
    SD3_EN13918,
  }

  public enum StandardStudSize {
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

  /// <summary>
  /// Object for setting dimensions and strength for a <see cref="ComposGH.Stud.Stud"/>
  /// </summary>
  public class StudDimensions : IStudDimensions {
    public Force CharacterStrength { get; set; }
    public Length Diameter { get; set; } // diameter of stud
    public Pressure Fu { get; set; }
    public Length Height { get; set; } // welded height of stud
                                       // characteristic strength of stud
    public bool IsStandard { get; set; }
    public bool IsStandardENGrade { get; set; }

    public StudDimensions() {
      // empty constructor
    }

    /// <summary>
    /// Create Custom size with strength from Stress
    /// </summary>
    /// <param name="diameter"></param>
    /// <param name="height"></param>
    /// <param name="fu"></param>
    public StudDimensions(Length diameter, Length height, Pressure fu) {
      Diameter = diameter;
      Height = height;
      Fu = fu;
    }

    /// <summary>
    /// Create Custom size with strength from Force
    /// </summary>
    /// <param name="diameter"></param>
    /// <param name="height"></param>
    /// <param name="strength"></param>
    public StudDimensions(Length diameter, Length height, Force strength) {
      Diameter = diameter;
      Height = height;
      CharacterStrength = strength;
    }

    /// <summary>
    /// Create Standard size with standard strength depending on code applied
    /// </summary>
    /// <param name="size"></param>
    /// <param name="strength"></param>
    public StudDimensions(StandardStudSize size) {
      SetSizeFromStandard(size);
    }

    /// <summary>
    /// Create Standard size with custom strength from Stress
    /// </summary>
    /// <param name="size"></param>
    /// <param name="strength"></param>
    public StudDimensions(StandardStudSize size, Force strength) {
      SetSizeFromStandard(size);
      CharacterStrength = strength;
    }

    /// <summary>
    /// Create Standard size with a custom strength from Stress
    /// </summary>
    /// <param name="size"></param>
    /// <param name="fu"></param>
    public StudDimensions(StandardStudSize size, Pressure fu) {
      SetSizeFromStandard(size);
      Fu = fu;
    }

    /// <summary>
    /// Create Standard size with Standard Grade
    /// </summary>
    /// <param name="size"></param>
    /// <param name="standardGrade"></param>
    public StudDimensions(StandardStudSize size, StandardStudGrade standardGrade) {
      SetSizeFromStandard(size);
      SetGradeFromStandard(standardGrade);
      IsStandardENGrade = true;
    }

    /// <summary>
    /// Create Custom size with Standard Grade
    /// </summary>
    /// <param name="diameter"></param>
    /// <param name="height"></param>
    /// <param name="standardGrade"></param>
    public StudDimensions(Length diameter, Length height, StandardStudGrade standardGrade) {
      Diameter = diameter;
      Height = height;
      SetGradeFromStandard(standardGrade);
    }

    public void SetGradeFromStandard(StandardStudGrade standardGrade) {
      IsStandardENGrade = true;
      switch (standardGrade) {
        case StandardStudGrade.SD1_EN13918:
          Fu = new Pressure(400, PressureUnit.NewtonPerSquareMillimeter);
          break;

        case StandardStudGrade.SD2_EN13918:
          Fu = new Pressure(450, PressureUnit.NewtonPerSquareMillimeter);
          break;

        case StandardStudGrade.SD3_EN13918:
          Fu = new Pressure(500, PressureUnit.NewtonPerSquareMillimeter);
          break;
      }
    }

    public void SetSizeFromStandard(StandardStudSize size) {
      IsStandard = true;
      switch (size) {
        case StandardStudSize.D13mmH65mm:
          Diameter = new Length(13, LengthUnit.Millimeter);
          Height = new Length(65, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D16mmH75mm:
          Diameter = new Length(16, LengthUnit.Millimeter);
          Height = new Length(75, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D19mmH75mm:
          Diameter = new Length(19, LengthUnit.Millimeter);
          Height = new Length(75, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D19mmH100mm:
          Diameter = new Length(19, LengthUnit.Millimeter);
          Height = new Length(100, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D19mmH125mm:
          Diameter = new Length(19, LengthUnit.Millimeter);
          Height = new Length(125, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D22mmH100mm:
          Diameter = new Length(22, LengthUnit.Millimeter);
          Height = new Length(100, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D25mmH100mm:
          Diameter = new Length(25, LengthUnit.Millimeter);
          Height = new Length(100, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D16mmH70mm:
          Diameter = new Length(16, LengthUnit.Millimeter);
          Height = new Length(70, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D19mmH95mm:
          Diameter = new Length(19, LengthUnit.Millimeter);
          Height = new Length(95, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D22mmH95mm:
          Diameter = new Length(22, LengthUnit.Millimeter);
          Height = new Length(95, LengthUnit.Millimeter);
          break;

        case StandardStudSize.D25mmH95mm:
          Diameter = new Length(25, LengthUnit.Millimeter);
          Height = new Length(95, LengthUnit.Millimeter);
          break;
      }
    }

    public override string ToString() {
      string dia = Diameter.As(Height.Unit).ToString("g5");
      string h = Height.ToString("g5");
      string f = (Fu.Value == 0) ? CharacterStrength.ToString("g5") : Fu.ToString("g5");

      return "Ø" + dia.Replace(" ", string.Empty) + "/" + h.Replace(" ", string.Empty) + (IsStandard ? "" : ", f:" + f.Replace(" ", string.Empty));
    }
  }
}
