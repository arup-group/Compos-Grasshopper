using ComposAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnitsNet;

namespace ComposAPI
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

  /// <summary>
  /// Web Opening or Notch for a <see cref="Beam"/> object containing information about opening shape and optionally contains a <see cref="WebOpeningStiffeners"/>.
  /// </summary>
  public class WebOpening : IWebOpening
  {

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
    public IWebOpeningStiffeners OpeningStiffeners { get; set; } = null;

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
    public WebOpening(Length width, Length height, Length positionCentroidFromStart, Length positionCentroidFromTop, IWebOpeningStiffeners stiffeners = null)
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
    public WebOpening(Length diameter, Length positionCentroidFromStart, Length positionCentroidFromTop, IWebOpeningStiffeners stiffeners = null)
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
    public WebOpening(Length width, Length height, NotchPosition position, IWebOpeningStiffeners stiffeners = null)
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
    internal WebOpening FromCoaString(List<string> parameters, ComposUnits units)
    {
      //WEB_OPEN_DIMENSION MEMBER-1 RECTANGULAR 400.000 300.000 7.50000 350.000 STIFFENER_NO
      //WEB_OPEN_DIMENSION MEMBER-1 CIRCULAR 400.000 400.000 3.50000 190.000 STIFFENER_NO
      //WEB_OPEN_DIMENSION MEMBER-1 LEFT_NOTCH 400.000 300.000 50.0000 % 50.0000 % STIFFENER_NO
      //WEB_OPEN_DIMENSION MEMBER-1 RIGHT_NOTCH 400.000 300.000 50.0000 % 50.0000 % STIFFENER_YES ONE_SIDE_STIFFENER 50.0000 100.000 10.0000 100.000 10.0000
      //WEB_OPEN_DIMENSION MEMBER-1 RECTANGULAR 400.000 300.000 1.50000 250.000 STIFFENER_YES BOTH_SIDE_STIFFENER 60.0000 100.000 10.0000 50.0000 5.00000
      //WEB_OPEN_DIMENSION MEMBER-1 CIRCULAR 400.000 400.000 9.50000 150.000 STIFFENER_YES BOTH_SIDE_STIFFENER 10.0000 120.000 12.0000 70.0000 7.00000
      NumberFormatInfo noComma = CultureInfo.InvariantCulture.NumberFormat;
      switch (parameters[2])
      {
        case "LEFT_NOTCH":
          this.WebOpeningType = OpeningType.Start_notch;
          this.Width = new Length(Convert.ToDouble(parameters[3], noComma), units.Section);
          this.Height = new Length(Convert.ToDouble(parameters[4], noComma), units.Section);
          break;

        case "RIGHT_NOTCH":
          this.WebOpeningType = OpeningType.End_notch;
          this.Width = new Length(Convert.ToDouble(parameters[3], noComma), units.Section);
          this.Height = new Length(Convert.ToDouble(parameters[4], noComma), units.Section);
          break;

        case "RECTANGULAR":
          this.WebOpeningType = OpeningType.Rectangular;
          this.Width = new Length(Convert.ToDouble(parameters[3], noComma), units.Section);
          this.Height = new Length(Convert.ToDouble(parameters[4], noComma), units.Section);
          this.CentroidPosFromStart = new Length(Convert.ToDouble(parameters[5], noComma), units.Length);
          this.CentroidPosFromTop = new Length(Convert.ToDouble(parameters[6], noComma), units.Section);
          break;

        case "CIRCULAR":
          this.WebOpeningType = OpeningType.Circular;
          this.Diameter = new Length(Convert.ToDouble(parameters[3], noComma), units.Section);
          this.CentroidPosFromStart = new Length(Convert.ToDouble(parameters[5], noComma), units.Length);
          this.CentroidPosFromTop = new Length(Convert.ToDouble(parameters[6], noComma), units.Section);
          break;
      }
      if (parameters[7] == "STIFFENER_YES")
      {
        WebOpeningStiffeners webOpeningStiffeners = new WebOpeningStiffeners();
        
        if (parameters[8] == "ONE_SIDE_STIFFENER")
          webOpeningStiffeners.isBothSides = false;
        else
          webOpeningStiffeners.isBothSides = true;

        webOpeningStiffeners.DistanceFrom = new Length(Convert.ToDouble(parameters[9], noComma), units.Section);
        webOpeningStiffeners.TopStiffenerWidth = new Length(Convert.ToDouble(parameters[10], noComma), units.Section);
        webOpeningStiffeners.TopStiffenerThickness = new Length(Convert.ToDouble(parameters[11], noComma), units.Section);

        if (parameters[2] == "LEFT_NOTCH" | parameters[2] == "RIGHT_NOTCH")
          webOpeningStiffeners.isNotch = true;
        else 
        { 
          webOpeningStiffeners.isNotch = false;
          webOpeningStiffeners.BottomStiffenerWidth = new Length(Convert.ToDouble(parameters[12], noComma), units.Section);
          webOpeningStiffeners.BottomStiffenerThickness = new Length(Convert.ToDouble(parameters[13], noComma), units.Section);
        }
        this.OpeningStiffeners = webOpeningStiffeners;
      }
      return this;
    }

    public string ToCoaString(string name, ComposUnits units)
    {
      //WEB_OPEN_DIMENSION MEMBER-1 RECTANGULAR 400.000 300.000 7.50000 350.000 STIFFENER_NO
      //WEB_OPEN_DIMENSION MEMBER-1 CIRCULAR 400.000 400.000 3.50000 190.000 STIFFENER_NO
      //WEB_OPEN_DIMENSION MEMBER-1 LEFT_NOTCH 400.000 300.000 50.0000 % 50.0000 % STIFFENER_NO
      //WEB_OPEN_DIMENSION MEMBER-1 RIGHT_NOTCH 400.000 300.000 50.0000 % 50.0000 % STIFFENER_YES ONE_SIDE_STIFFENER 50.0000 100.000 10.0000 100.000 10.0000
      //WEB_OPEN_DIMENSION MEMBER-1 RECTANGULAR 400.000 300.000 1.50000 250.000 STIFFENER_YES BOTH_SIDE_STIFFENER 60.0000 100.000 10.0000 50.0000 5.00000
      //WEB_OPEN_DIMENSION MEMBER-1 CIRCULAR 400.000 400.000 9.50000 150.000 STIFFENER_YES BOTH_SIDE_STIFFENER 10.0000 120.000 12.0000 70.0000 7.00000
      List<string> parameters = new List<string>();
      parameters.Add("WEB_OPEN_DIMENSION");
      parameters.Add(name);
      switch (this.WebOpeningType)
      {
        case OpeningType.Start_notch:
          parameters.Add("LEFT_NOTCH");
          parameters.Add(CoaHelper.FormatSignificantFigures(this.Width.ToUnit(units.Section).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.Height.ToUnit(units.Section).Value, 6));
          parameters.Add("50.0000%");
          parameters.Add("50.0000%");
          break;

        case OpeningType.End_notch:
          parameters.Add("RIGHT_NOTCH");
          parameters.Add(CoaHelper.FormatSignificantFigures(this.Width.ToUnit(units.Section).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.Height.ToUnit(units.Section).Value, 6));
          parameters.Add("50.0000%");
          parameters.Add("50.0000%");
          break;

        case OpeningType.Rectangular:
          parameters.Add("RECTANGULAR");
          parameters.Add(CoaHelper.FormatSignificantFigures(this.Width.ToUnit(units.Section).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.Height.ToUnit(units.Section).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.CentroidPosFromStart.ToUnit(units.Length).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.CentroidPosFromTop.ToUnit(units.Section).Value, 6));
          break;

        case OpeningType.Circular:
          parameters.Add("CIRCULAR");
          parameters.Add(CoaHelper.FormatSignificantFigures(this.Diameter.ToUnit(units.Section).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.Diameter.ToUnit(units.Section).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.CentroidPosFromStart.ToUnit(units.Length).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.CentroidPosFromTop.ToUnit(units.Section).Value, 6));
          break;
      }
      if (this.OpeningStiffeners == null)
        parameters.Add("STIFFENER_NO");
      else
      {
        parameters.Add("STIFFENER_YES");

        if (!this.OpeningStiffeners.isBothSides | this.OpeningStiffeners.isNotch)
          parameters.Add("ONE_SIDE_STIFFENER");
        else
          parameters.Add("BOTH_SIDE_STIFFENER");

        parameters.Add(CoaHelper.FormatSignificantFigures(this.OpeningStiffeners.DistanceFrom.ToUnit(units.Section).Value, 6));
        parameters.Add(CoaHelper.FormatSignificantFigures(this.OpeningStiffeners.TopStiffenerWidth.ToUnit(units.Section).Value, 6));
        parameters.Add(CoaHelper.FormatSignificantFigures(this.OpeningStiffeners.TopStiffenerThickness.ToUnit(units.Section).Value, 6));
        if (this.OpeningStiffeners.isNotch)
        {
          parameters.Add(CoaHelper.FormatSignificantFigures(this.OpeningStiffeners.TopStiffenerWidth.ToUnit(units.Section).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.OpeningStiffeners.TopStiffenerThickness.ToUnit(units.Section).Value, 6));
        }
        else
        {
          parameters.Add(CoaHelper.FormatSignificantFigures(this.OpeningStiffeners.BottomStiffenerWidth.ToUnit(units.Section).Value, 6));
          parameters.Add(CoaHelper.FormatSignificantFigures(this.OpeningStiffeners.BottomStiffenerThickness.ToUnit(units.Section).Value, 6));
        }
      }

      return CoaHelper.CreateString(parameters);
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
          typ = ", Pos:(x:" + this.CentroidPosFromStart.ToUnit(Units.LengthUnitGeometry).ToString("f2").Replace(" ", string.Empty) + ", z:" + this.CentroidPosFromTop.ToUnit(Units.LengthUnitSection).ToString("f0").Replace(" ", string.Empty) + ")";
          break;
      }
      if (this.OpeningStiffeners != null)
        typ += " w/Stiff.";

      return size + typ;
    }

    #endregion
  }
}
