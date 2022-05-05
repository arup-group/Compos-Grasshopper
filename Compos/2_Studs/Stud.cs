using System;
using System.Collections.Generic;
using System.Linq;

using UnitsNet;

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
    public override string ToString()
    {
      string size = this.StudDimensions.Diameter.As(Units.LengthUnitSection).ToString("f0") + "/" + this.StudDimensions.Height.ToUnit(Units.LengthUnitSection).ToString("f0");
      return size.Replace(" ", string.Empty);
    }

    #endregion
  }
}
