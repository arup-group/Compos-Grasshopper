using System;
using System.Collections.Generic;
using System.Linq;
using UnitsNet;

namespace ComposAPI
{
  /// <summary>
  /// Web Opening or Notch for a <see cref="Beam"/> object containing information about opening shape and optionally contains a <see cref="WebOpeningStiffeners"/>.
  /// </summary>
  public class WebOpening
  {
    public enum WebOpeningShape
    {
      Rectangular,
      Circular
    }
    public enum NotchPosition
    {
      Start,
      End
    }
    public enum OpeningType
    {
      Rectangular,
      Circular,
      Start_notch,
      End_notch
    }
    public OpeningType WebOpeningType
    {
      get { return m_webOpeningType; }
      set
      {
        m_webOpeningType = value;
        switch (value)
        {
          case OpeningType.Rectangular:
            this.Diameter = Length.Zero;
            break;
          case OpeningType.Circular:
            this.Width = Length.Zero;
            this.Height = Length.Zero;
            break;
          case OpeningType.Start_notch:
          case OpeningType.End_notch:
            this.Diameter = Length.Zero;
            this.CentroidPosFromStart = Length.Zero;
            this.CentroidPosFromTop = Length.Zero;
            break;
        }
      }
    }
    private OpeningType m_webOpeningType;
    public Length Width { get; set; }
    public Length Height { get; set; }
    public Length Diameter { get; set; }
    public Length CentroidPosFromStart { get; set; }
    public Length CentroidPosFromTop { get; set; }
    public WebOpeningStiffeners OpeningStiffeners { get; set; } = null;

    #region constructors
    public WebOpening()
    {
      // empty constructor
    }
    /// <summary>
    /// Rectangular web opening
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="positionCentroidFromStart"></param>
    /// <param name="positionCentroidFromTop"></param>
    /// <param name="stiffeners"></param>
    public WebOpening(Length width, Length height, Length positionCentroidFromStart, Length positionCentroidFromTop, WebOpeningStiffeners stiffeners = null)
    {
      // static type for this constructor
      this.WebOpeningType = OpeningType.Rectangular;
      // inputs
      this.Width = width;
      this.Height = height;
      this.CentroidPosFromStart = positionCentroidFromStart;
      this.CentroidPosFromTop = positionCentroidFromTop;
      this.OpeningStiffeners = stiffeners;
      // set stiffeners properties
      if (this.OpeningStiffeners != null)
      {
        this.OpeningStiffeners = new WebOpeningStiffeners(
            stiffeners.DistanceFrom, stiffeners.TopStiffenerWidth, stiffeners.TopStiffenerThickness,
            stiffeners.BottomStiffenerWidth, stiffeners.BottomStiffenerThickness, stiffeners.isBothSides);
      }
    }
    /// <summary>
    /// Circular web opening
    /// </summary>
    /// <param name="diameter"></param>
    /// <param name="positionCentroidFromStart"></param>
    /// <param name="positionCentroidFromTop"></param>
    /// <param name="stiffeners"></param>
    public WebOpening(Length diameter, Length positionCentroidFromStart, Length positionCentroidFromTop, WebOpeningStiffeners stiffeners = null)
    {
      // static type for this constructor
      this.WebOpeningType = OpeningType.Circular;
      // inputs
      this.Diameter = diameter;
      this.CentroidPosFromStart = positionCentroidFromStart;
      this.CentroidPosFromTop = positionCentroidFromTop;
      this.OpeningStiffeners = stiffeners;
      // set stiffeners properties
      if (this.OpeningStiffeners != null)
      {
        this.OpeningStiffeners = new WebOpeningStiffeners(
            stiffeners.DistanceFrom, stiffeners.TopStiffenerWidth, stiffeners.TopStiffenerThickness,
            stiffeners.BottomStiffenerWidth, stiffeners.BottomStiffenerThickness, stiffeners.isBothSides);
      }
    }
    /// <summary>
    /// Notch web opening
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="stiffeners"></param> 
    public WebOpening(Length width, Length height, NotchPosition position, WebOpeningStiffeners stiffeners = null)
    {
      // static type for this constructor
      if (position == NotchPosition.Start)
        this.WebOpeningType = OpeningType.Start_notch;
      else
        this.WebOpeningType = OpeningType.End_notch;
      // inputs
      this.Width = width;
      this.Height = height;
      this.OpeningStiffeners = stiffeners;
      // set stiffeners properties
      if (this.OpeningStiffeners != null)
      {
        this.OpeningStiffeners = new WebOpeningStiffeners(
            stiffeners.DistanceFrom, stiffeners.TopStiffenerWidth,
            stiffeners.TopStiffenerThickness, stiffeners.isBothSides);
      }
    }

    #endregion

    #region coa interop
    internal WebOpening(string coaString)
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
      string size = "";
      switch (this.WebOpeningType)
      {
        case OpeningType.Start_notch:
        case OpeningType.End_notch:
        case OpeningType.Rectangular:
          size = this.Width.As(Units.LengthUnitSection).ToString("f0") + "x" + this.Height.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
          break;
        case OpeningType.Circular:
          size = "Ø" + this.Diameter.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty);
          break;
      }

      string typ = "";
      switch (this.WebOpeningType)
      {
        case OpeningType.Start_notch:
          typ = " Start Notch";
          break;
        case OpeningType.End_notch:
          typ = " End Notch";
          break;
        case OpeningType.Rectangular:
        case OpeningType.Circular:
          typ = ", Pos:(x:" + this.CentroidPosFromStart.As(Units.LengthUnitGeometry).ToString("f0") + ", z:" + this.CentroidPosFromTop.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty) + ")";
          break;
      }
      if (this.OpeningStiffeners != null)
        typ += " w/Stiff.";

      return size + typ;
    }

    #endregion
  }
}
