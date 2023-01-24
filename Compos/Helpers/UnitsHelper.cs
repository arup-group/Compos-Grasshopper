using System;
using System.Collections.Generic;
using System.Linq;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI.Helpers
{
  /// <summary>
  /// Get or Set global units in this static class
  /// </summary>
  public static class ComposUnitsHelper
  {
    public enum ComposUnits
    {
      Length_Geometry,
      Length_Section,
      Length_Results,
      Force,
      Stress,
      Mass,
    }

    #region lengths
    public static List<string> FilteredLengthUnits = new List<string>()
        {
            LengthUnit.Millimeter.ToString(),
            LengthUnit.Centimeter.ToString(),
            LengthUnit.Meter.ToString(),
            LengthUnit.Inch.ToString(),
            LengthUnit.Foot.ToString()
        };

    public static LengthUnit LengthUnitGeometry { get; set; } = LengthUnit.Meter;

    public static LengthUnit LengthUnitSection { get; set; } = LengthUnit.Meter;

    public static AreaMomentOfInertiaUnit SectionAreaMomentOfInertiaUnit
    {
      get
      {
        switch (LengthUnitSection)
        {
          case LengthUnit.Millimeter:
            return AreaMomentOfInertiaUnit.MillimeterToTheFourth;
          case LengthUnit.Centimeter:
            return AreaMomentOfInertiaUnit.CentimeterToTheFourth;
          case LengthUnit.Meter:
            return AreaMomentOfInertiaUnit.MeterToTheFourth;
          case LengthUnit.Foot:
            return AreaMomentOfInertiaUnit.FootToTheFourth;
          case LengthUnit.Inch:
            return AreaMomentOfInertiaUnit.InchToTheFourth;
          default:
            return AreaMomentOfInertiaUnit.Undefined;
        }
      }
    }
    public static AreaMomentOfInertiaUnit AreaMomentOfInertiaUnit
    {
      get { return GetSectionAreaMomentOfInertiaUnit(LengthUnitSection); }
    }
    public static AreaMomentOfInertiaUnit GetSectionAreaMomentOfInertiaUnit(LengthUnit unit)
    {
      switch (unit)
      {
        case LengthUnit.Millimeter:
          return AreaMomentOfInertiaUnit.MillimeterToTheFourth;
        case LengthUnit.Centimeter:
          return AreaMomentOfInertiaUnit.CentimeterToTheFourth;
        case LengthUnit.Meter:
          return AreaMomentOfInertiaUnit.MeterToTheFourth;
        case LengthUnit.Foot:
          return AreaMomentOfInertiaUnit.FootToTheFourth;
        case LengthUnit.Inch:
          return AreaMomentOfInertiaUnit.InchToTheFourth;
        default:
          return AreaMomentOfInertiaUnit.Undefined;
      }
    }
    public static AreaUnit SectionAreaUnit
    {
      get
      {
        switch (LengthUnitSection)
        {
          case LengthUnit.Millimeter:
            return AreaUnit.SquareMillimeter;
          case LengthUnit.Centimeter:
            return AreaUnit.SquareCentimeter;
          case LengthUnit.Meter:
            return AreaUnit.SquareMeter;
          case LengthUnit.Foot:
            return AreaUnit.SquareFoot;
          case LengthUnit.Inch:
            return AreaUnit.SquareInch;
          default:
            return AreaUnit.Undefined;
        }
      }
    }

    public static LengthUnit LengthUnitResult { get; set; } = LengthUnit.Meter;

    #endregion

    #region force
    public static ForceUnit ForceUnit { get; set; } = ForceUnit.Newton;

    public static List<string> FilteredForceUnits = new List<string>()
        {
            ForceUnit.Newton.ToString(),
            ForceUnit.Kilonewton.ToString(),
            ForceUnit.Meganewton.ToString(),
            ForceUnit.PoundForce.ToString(),
            ForceUnit.KilopoundForce.ToString(),
            ForceUnit.TonneForce.ToString()
        };

    public static ForcePerLengthUnit ForcePerLengthUnit
    {
      get
      {
        switch (LengthUnitGeometry)
        {
          case LengthUnit.Millimeter:
            switch (ForceUnit)
            {
              case ForceUnit.Newton:
                return ForcePerLengthUnit.NewtonPerMillimeter;
              case ForceUnit.Kilonewton:
                return ForcePerLengthUnit.KilonewtonPerMillimeter;
              case ForceUnit.Meganewton:
                return ForcePerLengthUnit.MeganewtonPerMillimeter;
              case ForceUnit.TonneForce:
                return ForcePerLengthUnit.TonneForcePerMillimeter;
              default:
                return ForcePerLengthUnit.Undefined;
            }
          case LengthUnit.Centimeter:
            switch (ForceUnit)
            {
              case ForceUnit.Newton:
                return ForcePerLengthUnit.NewtonPerCentimeter;
              case ForceUnit.Kilonewton:
                return ForcePerLengthUnit.KilonewtonPerCentimeter;
              case ForceUnit.Meganewton:
                return ForcePerLengthUnit.MeganewtonPerCentimeter;
              case ForceUnit.TonneForce:
                return ForcePerLengthUnit.TonneForcePerCentimeter;
              default:
                return ForcePerLengthUnit.Undefined;
            }
          case LengthUnit.Meter:
            switch (ForceUnit)
            {
              case ForceUnit.Newton:
                return ForcePerLengthUnit.NewtonPerMeter;
              case ForceUnit.Kilonewton:
                return ForcePerLengthUnit.KilonewtonPerMeter;
              case ForceUnit.Meganewton:
                return ForcePerLengthUnit.MeganewtonPerMeter;
              case ForceUnit.TonneForce:
                return ForcePerLengthUnit.TonneForcePerMeter;
              default:
                return ForcePerLengthUnit.Undefined;
            }
          case LengthUnit.Foot:
            switch (ForceUnit)
            {
              case ForceUnit.PoundForce:
                return ForcePerLengthUnit.PoundForcePerFoot;
              case ForceUnit.KilopoundForce:
                return ForcePerLengthUnit.KilopoundForcePerFoot;
              default:
                return ForcePerLengthUnit.Undefined;
            }
          case LengthUnit.Inch:
            switch (ForceUnit)
            {
              case ForceUnit.PoundForce:
                return ForcePerLengthUnit.PoundForcePerInch;
              case ForceUnit.KilopoundForce:
                return ForcePerLengthUnit.KilopoundForcePerInch;
              default:
                return ForcePerLengthUnit.Undefined;
            }
          default:
            return ForcePerLengthUnit.Undefined;
        }
      }
    }
    public static ForcePerLengthUnit GetForcePerLengthUnit(ForceUnit forceUnit, LengthUnit lengthUnit)
    {
      switch (lengthUnit)
      {
        case LengthUnit.Millimeter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return ForcePerLengthUnit.NewtonPerMillimeter;
            case ForceUnit.Kilonewton:
              return ForcePerLengthUnit.KilonewtonPerMillimeter;
            case ForceUnit.Meganewton:
              return ForcePerLengthUnit.MeganewtonPerMillimeter;
            case ForceUnit.TonneForce:
              return ForcePerLengthUnit.TonneForcePerMillimeter;
            default:
              return ForcePerLengthUnit.Undefined;
          }
        case LengthUnit.Centimeter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return ForcePerLengthUnit.NewtonPerCentimeter;
            case ForceUnit.Kilonewton:
              return ForcePerLengthUnit.KilonewtonPerCentimeter;
            case ForceUnit.Meganewton:
              return ForcePerLengthUnit.MeganewtonPerCentimeter;
            case ForceUnit.TonneForce:
              return ForcePerLengthUnit.TonneForcePerCentimeter;
            default:
              return ForcePerLengthUnit.Undefined;
          }
        case LengthUnit.Meter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return ForcePerLengthUnit.NewtonPerMeter;
            case ForceUnit.Kilonewton:
              return ForcePerLengthUnit.KilonewtonPerMeter;
            case ForceUnit.Meganewton:
              return ForcePerLengthUnit.MeganewtonPerMeter;
            case ForceUnit.TonneForce:
              return ForcePerLengthUnit.TonneForcePerMeter;
            default:
              return ForcePerLengthUnit.Undefined;
          }
        case LengthUnit.Foot:
          switch (forceUnit)
          {
            case ForceUnit.PoundForce:
              return ForcePerLengthUnit.PoundForcePerFoot;
            case ForceUnit.KilopoundForce:
              return ForcePerLengthUnit.KilopoundForcePerFoot;
            default:
              return ForcePerLengthUnit.Undefined;
          }
        case LengthUnit.Inch:
          switch (forceUnit)
          {
            case ForceUnit.PoundForce:
              return ForcePerLengthUnit.PoundForcePerInch;
            case ForceUnit.KilopoundForce:
              return ForcePerLengthUnit.KilopoundForcePerInch;
            default:
              return ForcePerLengthUnit.Undefined;
          }
        default:
          return ForcePerLengthUnit.Undefined;
      }
    }

    public static List<string> FilteredForcePerLengthUnits = new List<string>()
      {
        ForcePerLengthUnit.NewtonPerMillimeter.ToString(),
        ForcePerLengthUnit.NewtonPerCentimeter.ToString(),
        ForcePerLengthUnit.NewtonPerMeter.ToString(),

        ForcePerLengthUnit.KilonewtonPerMillimeter.ToString(),
        ForcePerLengthUnit.KilonewtonPerCentimeter.ToString(),
        ForcePerLengthUnit.KilonewtonPerMeter.ToString(),

        ForcePerLengthUnit.MeganewtonPerMeter.ToString(),

        ForcePerLengthUnit.TonneForcePerCentimeter.ToString(),
        ForcePerLengthUnit.TonneForcePerMeter.ToString(),
        ForcePerLengthUnit.TonneForcePerMillimeter.ToString(),

        ForcePerLengthUnit.PoundForcePerInch.ToString(),
        ForcePerLengthUnit.PoundForcePerFoot.ToString(),
        ForcePerLengthUnit.PoundForcePerYard.ToString(),

        ForcePerLengthUnit.KilopoundForcePerInch.ToString(),
        ForcePerLengthUnit.KilopoundForcePerFoot.ToString()
      };
    #endregion

    #region moment
    public static MomentUnit MomentUnit { get; set; } = MomentUnit.KilonewtonMeter;

    public static List<string> FilteredMomentUnits = Enum.GetNames(typeof(MomentUnit)).ToList();

    public static MomentUnit GetMomentUnit(ForceUnit forceUnit, LengthUnit lengthUnit)
    {
      switch (lengthUnit)
      {
        case LengthUnit.Millimeter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return MomentUnit.NewtonMillimeter;
            case ForceUnit.Kilonewton:
              return MomentUnit.KilonewtonMillimeter;
            case ForceUnit.Meganewton:
              return MomentUnit.MeganewtonMillimeter;
            case ForceUnit.TonneForce:
              return MomentUnit.TonneForceMillimeter;
            default:
              return MomentUnit.Undefined;
          }
        case LengthUnit.Centimeter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return MomentUnit.NewtonCentimeter;
            case ForceUnit.Kilonewton:
              return MomentUnit.KilonewtonCentimeter;
            case ForceUnit.Meganewton:
              return MomentUnit.MeganewtonCentimeter;
            case ForceUnit.TonneForce:
              return MomentUnit.TonneForceCentimeter;
            default:
              return MomentUnit.Undefined;
          }
        case LengthUnit.Meter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return MomentUnit.NewtonMeter;
            case ForceUnit.Kilonewton:
              return MomentUnit.KilonewtonMeter;
            case ForceUnit.Meganewton:
              return MomentUnit.MeganewtonMeter;
            case ForceUnit.TonneForce:
              return MomentUnit.TonneForceMeter;
            default:
              return MomentUnit.Undefined;
          }
        case LengthUnit.Foot:
          switch (forceUnit)
          {
            case ForceUnit.PoundForce:
              return MomentUnit.PoundForceFoot;
            case ForceUnit.KilopoundForce:
              return MomentUnit.KilopoundForceFoot;
            default:
              return MomentUnit.Undefined;
          }
        case LengthUnit.Inch:
          switch (forceUnit)
          {
            case ForceUnit.PoundForce:
              return MomentUnit.PoundForceInch;
            case ForceUnit.KilopoundForce:
              return MomentUnit.KilopoundForceInch;
            default:
              return MomentUnit.Undefined;
          }
        default:
          return MomentUnit.Undefined;
      }
    }
    #endregion

    #region stress
    public static PressureUnit StressUnit
    {
      get { return m_stress; }
      set { m_stress = value; }
    }
    private static PressureUnit m_stress = PressureUnit.NewtonPerSquareMeter;
    public static List<string> FilteredStressUnits = new List<string>()
      {
        PressureUnit.Pascal.ToString(),
        PressureUnit.Kilopascal.ToString(),
        PressureUnit.Megapascal.ToString(),
        PressureUnit.Gigapascal.ToString(),
        PressureUnit.NewtonPerSquareMillimeter.ToString(),
        PressureUnit.NewtonPerSquareMeter.ToString(),
        PressureUnit.PoundForcePerSquareInch.ToString(),
        PressureUnit.PoundForcePerSquareFoot.ToString(),
        PressureUnit.KilopoundForcePerSquareInch.ToString(),
      };


    public static List<string> FilteredForcePerAreaUnits = new List<string>()
      {
        PressureUnit.NewtonPerSquareMillimeter.ToString(),
        PressureUnit.NewtonPerSquareCentimeter.ToString(),
        PressureUnit.NewtonPerSquareMeter.ToString(),
        PressureUnit.KilonewtonPerSquareCentimeter.ToString(),
        PressureUnit.KilonewtonPerSquareMillimeter.ToString(),
        PressureUnit.KilonewtonPerSquareMeter.ToString(),
        PressureUnit.PoundForcePerSquareInch.ToString(),
        PressureUnit.PoundForcePerSquareFoot.ToString(),
        PressureUnit.KilopoundForcePerSquareInch.ToString(),
      };

    public static PressureUnit ForcePerAreaUnit
    {
      get
      {
        switch (LengthUnitGeometry)
        {
          case LengthUnit.Millimeter:
            switch (ForceUnit)
            {
              case ForceUnit.Newton:
                return PressureUnit.NewtonPerSquareMillimeter;
              case ForceUnit.Kilonewton:
                return PressureUnit.KilonewtonPerSquareMillimeter;
              case ForceUnit.TonneForce:
                return PressureUnit.TonneForcePerSquareMillimeter;
              default:
                return PressureUnit.Undefined;
            }
          case LengthUnit.Centimeter:
            switch (ForceUnit)
            {
              case ForceUnit.Newton:
                return PressureUnit.NewtonPerSquareCentimeter;
              case ForceUnit.Kilonewton:
                return PressureUnit.KilonewtonPerSquareCentimeter;
              case ForceUnit.TonneForce:
                return PressureUnit.TonneForcePerSquareCentimeter;
              default:
                return PressureUnit.Undefined;
            }
          case LengthUnit.Meter:
            switch (ForceUnit)
            {
              case ForceUnit.Newton:
                return PressureUnit.NewtonPerSquareMeter;
              case ForceUnit.Kilonewton:
                return PressureUnit.KilonewtonPerSquareMeter;
              case ForceUnit.Meganewton:
                return PressureUnit.MeganewtonPerSquareMeter;
              case ForceUnit.TonneForce:
                return PressureUnit.TonneForcePerSquareMeter;
              default:
                return PressureUnit.Undefined;
            }
          case LengthUnit.Foot:
            switch (ForceUnit)
            {
              case ForceUnit.PoundForce:
                return PressureUnit.PoundForcePerSquareFoot;
              case ForceUnit.KilopoundForce:
                return PressureUnit.KilopoundForcePerSquareFoot;
              default:
                return PressureUnit.Undefined;
            }
          case LengthUnit.Inch:
            switch (ForceUnit)
            {
              case ForceUnit.PoundForce:
                return PressureUnit.PoundForcePerSquareInch;
              case ForceUnit.KilopoundForce:
                return PressureUnit.KilopoundForcePerSquareInch;
              default:
                return PressureUnit.Undefined;
            }
          default:
            return PressureUnit.Undefined;
        }
      }
    }

    public static PressureUnit GetForcePerAreaUnit(ForceUnit forceUnit, LengthUnit lengthUnit)
    {
      switch (lengthUnit)
      {
        case LengthUnit.Millimeter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return PressureUnit.NewtonPerSquareMillimeter;
            case ForceUnit.Kilonewton:
              return PressureUnit.KilonewtonPerSquareMillimeter;
            case ForceUnit.TonneForce:
              return PressureUnit.TonneForcePerSquareMillimeter;
            default:
              return PressureUnit.Undefined;
          }
        case LengthUnit.Centimeter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return PressureUnit.NewtonPerSquareCentimeter;
            case ForceUnit.Kilonewton:
              return PressureUnit.KilonewtonPerSquareCentimeter;
            case ForceUnit.TonneForce:
              return PressureUnit.TonneForcePerSquareCentimeter;
            default:
              return PressureUnit.Undefined;
          }
        case LengthUnit.Meter:
          switch (forceUnit)
          {
            case ForceUnit.Newton:
              return PressureUnit.NewtonPerSquareMeter;
            case ForceUnit.Kilonewton:
              return PressureUnit.KilonewtonPerSquareMeter;
            case ForceUnit.Meganewton:
              return PressureUnit.MeganewtonPerSquareMeter;
            case ForceUnit.TonneForce:
              return PressureUnit.TonneForcePerSquareMeter;
            default:
              return PressureUnit.Undefined;
          }
        case LengthUnit.Foot:
          switch (forceUnit)
          {
            case ForceUnit.PoundForce:
              return PressureUnit.PoundForcePerSquareFoot;
            case ForceUnit.KilopoundForce:
              return PressureUnit.KilopoundForcePerSquareFoot;
            default:
              return PressureUnit.Undefined;
          }
        case LengthUnit.Inch:
          switch (forceUnit)
          {
            case ForceUnit.PoundForce:
              return PressureUnit.PoundForcePerSquareInch;
            case ForceUnit.KilopoundForce:
              return PressureUnit.KilopoundForcePerSquareInch;
            default:
              return PressureUnit.Undefined;
          }
        default:
          return PressureUnit.Undefined;
      }
    }
    #endregion

    #region mass
    public static MassUnit MassUnit
    {
      get { return m_mass; }
      set { m_mass = value; }
    }
    private static MassUnit m_mass = MassUnit.Kilogram;
    public static List<string> FilteredMassUnits = new List<string>()
        {
            MassUnit.Gram.ToString(),
            MassUnit.Kilogram.ToString(),
            MassUnit.Tonne.ToString(),
            MassUnit.Kilotonne.ToString(),
            MassUnit.Pound.ToString(),
            MassUnit.Kilopound.ToString(),
            MassUnit.LongTon.ToString(),
            MassUnit.Slug.ToString()
        };
    #endregion

    #region density
    public static DensityUnit DensityUnit
    {
      get
      {
        Mass mass = Mass.From(1, MassUnit);
        Length len = Length.From(1, LengthUnitGeometry);
        Volume vol = len * len * len;

        Density density = mass / vol;
        return density.Unit;
      }
    }
    public static DensityUnit GetDensityUnit(MassUnit massUnit, LengthUnit lengthUnit)
    {
      switch (lengthUnit)
      {
        case LengthUnit.Millimeter:
          switch (massUnit)
          {
            case MassUnit.Gram:
              return DensityUnit.GramPerCubicMillimeter;
            case MassUnit.Kilogram:
              return DensityUnit.KilogramPerCubicMillimeter;
            case MassUnit.Tonne:
              return DensityUnit.TonnePerCubicMillimeter;
            default:
              return DensityUnit.Undefined;
          }
        case LengthUnit.Centimeter:
          switch (massUnit)
          {
            case MassUnit.Gram:
              return DensityUnit.GramPerCubicCentimeter;
            case MassUnit.Kilogram:
              return DensityUnit.KilogramPerCubicCentimeter;
            case MassUnit.Tonne:
              return DensityUnit.TonnePerCubicCentimeter;
            default:
              return DensityUnit.Undefined;
          }
        case LengthUnit.Meter:
          switch (massUnit)
          {
            case MassUnit.Gram:
              return DensityUnit.GramPerCubicMeter;
            case MassUnit.Kilogram:
              return DensityUnit.KilogramPerCubicMeter;
            case MassUnit.Tonne:
              return DensityUnit.TonnePerCubicMeter;
            default:
              return DensityUnit.Undefined;
          }
        case LengthUnit.Foot:
          switch (massUnit)
          {
            case MassUnit.Pound:
              return DensityUnit.PoundPerCubicFoot;
            case MassUnit.Kilopound:
              return DensityUnit.KilopoundPerCubicFoot;
            default:
              return DensityUnit.Undefined;
          }
        case LengthUnit.Inch:
          switch (massUnit)
          {
            case MassUnit.Pound:
              return DensityUnit.PoundPerCubicInch;
            case MassUnit.Kilopound:
              return DensityUnit.KilopoundPerCubicInch;
            default:
              return DensityUnit.Undefined;
          }
        default:
          return DensityUnit.Undefined;
      }
    }
    public static List<string> FilteredDensityUnits = new List<string>()
        {
            DensityUnit.GramPerCubicMillimeter.ToString(),
            DensityUnit.GramPerCubicCentimeter.ToString(),
            DensityUnit.GramPerCubicMeter.ToString(),
            DensityUnit.KilogramPerCubicMillimeter.ToString(),
            DensityUnit.KilogramPerCubicCentimeter.ToString(),
            DensityUnit.KilogramPerCubicMeter.ToString(),
            DensityUnit.TonnePerCubicMillimeter.ToString(),
            DensityUnit.TonnePerCubicCentimeter.ToString(),
            DensityUnit.TonnePerCubicMeter.ToString(),
            DensityUnit.PoundPerCubicFoot.ToString(),
            DensityUnit.PoundPerCubicInch.ToString(),
            DensityUnit.KilopoundPerCubicFoot.ToString(),
            DensityUnit.KilopoundPerCubicInch.ToString(),
        };
    #endregion

    #region strain
    public static StrainUnit StrainUnit { get; set; } = StrainUnit.MilliStrain;

    public static List<string> FilteredStrainUnits = new List<string>()
        {
            StrainUnit.Ratio.ToString(),
            StrainUnit.Percent.ToString(),
            StrainUnit.MilliStrain.ToString(),
            StrainUnit.MicroStrain.ToString()
        };
    #endregion
  }
}
