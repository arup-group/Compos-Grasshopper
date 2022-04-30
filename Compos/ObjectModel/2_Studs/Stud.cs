using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;

namespace ComposAPI.Studs
{
  /// <summary>
  /// Stud class containing <see cref="ComposGH.Stud.StudDimensions"/>, <see cref="ComposGH.Stud.StudSpecification"/>, and spacing/layout settings (custom or automatic)
  /// </summary>
  public class Stud
  {
    public StudDimensions StudDimension { get; set; }
    public StudSpecification StudSpecification { get; set; }

    // Stud Spacing
    public List<StudGroupSpacing> CustomSpacing { get; set; } = null;
    public double Interaction { get; set; }
    public double MinSavingMultipleZones { get; set; }
    public bool CheckStudSpacing { get; set; }
    public StudGroupSpacing.StudSpacingType StudSpacingType
    {
      get { return this.m_spacingType; }
      set
      {
        this.m_spacingType = value;
        switch (value)
        {
          case StudGroupSpacing.StudSpacingType.Custom:
            this.Interaction = double.NaN;
            this.MinSavingMultipleZones = double.NaN;
            break;
          case StudGroupSpacing.StudSpacingType.Automatic:
          case StudGroupSpacing.StudSpacingType.Min_Num_of_Studs:
            this.Interaction = double.NaN;
            if (this.MinSavingMultipleZones == double.NaN)
              this.MinSavingMultipleZones = 0.20;
            break;
          case StudGroupSpacing.StudSpacingType.Partial_Interaction:
            if (this.Interaction == double.NaN)
              this.Interaction = 0.85;
            if (this.MinSavingMultipleZones == double.NaN)
              this.MinSavingMultipleZones = 0.20;
            break;
        }
      }
    }
    private StudGroupSpacing.StudSpacingType m_spacingType;

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
    public Stud(StudDimensions stud, StudSpecification spec, List<StudGroupSpacing> spacings, bool checkSpacing)
    {
      this.StudDimension = stud;
      this.StudSpecification = spec;
      this.CustomSpacing = spacings;
      this.CheckStudSpacing = checkSpacing;
      this.StudSpacingType = StudGroupSpacing.StudSpacingType.Custom;
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
    public Stud(StudDimensions stud, StudSpecification spec, double minSaving, StudGroupSpacing.StudSpacingType type)
    {
      this.StudDimension = stud;
      this.StudSpecification = spec;
      this.StudSpacingType = type;
      this.MinSavingMultipleZones = minSaving;
      switch (type)
      {
        case StudGroupSpacing.StudSpacingType.Min_Num_of_Studs:
        case StudGroupSpacing.StudSpacingType.Automatic:
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
    public Stud(StudDimensions stud, StudSpecification spec, double minSaving, double interaction)
    {
      this.StudDimension = stud;
      this.StudSpecification = spec;
      this.StudSpacingType = StudGroupSpacing.StudSpacingType.Partial_Interaction;
      this.MinSavingMultipleZones = minSaving;
      this.Interaction = interaction;
    }

    #endregion

    #region coa interop
    internal Stud(string coaString)
    {
      // to do - implement from coa string method
    }

    internal string ToCoaString()
    {
      // to do - implement to coa string method
      return string.Empty;
    }
    #endregion

    #region methods

    public Stud Duplicate()
    {
      if (this == null) { return null; }
      Stud dup = (Stud)this.MemberwiseClone();
      dup.StudDimension = this.StudDimension.Duplicate();
      dup.StudSpecification = this.StudSpecification.Duplicate();
      if (this.CustomSpacing != null)
        dup.CustomSpacing = this.CustomSpacing.ToList();
      return dup;
    }

    public override string ToString()
    {
      string size = this.StudDimension.Diameter.As(Helpers.Units.FileUnits.LengthUnitSection).ToString("f0") + "/" + this.StudDimension.Height.ToUnit(Helpers.Units.FileUnits.LengthUnitSection).ToString("f0");
      return size.Replace(" ", string.Empty);
    }

    #endregion
  }
}
