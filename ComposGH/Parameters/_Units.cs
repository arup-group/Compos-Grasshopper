using System;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using UnitsNet;
using UnitsNet.Units;
using Oasys.Units;

namespace ComposGH
{
  /// <summary>
  /// Class to hold units used in Grasshopper Compos file. 
  /// </summary>
  public static class Units
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

    internal static List<string> FilteredAngleUnits = new List<string>()
        {
            AngleUnit.Radian.ToString(),
            AngleUnit.Degree.ToString()
        };
    #region lengths
    internal static List<string> FilteredLengthUnits = new List<string>()
        {
            LengthUnit.Millimeter.ToString(),
            LengthUnit.Centimeter.ToString(),
            LengthUnit.Meter.ToString(),
            LengthUnit.Inch.ToString(),
            LengthUnit.Foot.ToString()
        };

    public static Length Tolerance
    {
      get { return m_tolerance; }
      set { m_tolerance = value; }
    }
    private static Length m_tolerance = new Length(0.0001, LengthUnit.Meter);

    public static int SignificantDigits
    {
      get { return BitConverter.GetBytes(decimal.GetBits((decimal)m_tolerance.As(LengthUnitGeometry))[3])[2]; ; }
    }

    public static LengthUnit LengthUnitGeometry
    {
      get
      {
        if (m_units == null || useRhinoLengthGeometryUnit)
        {
          m_length_geometry = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
        }
        else
        {
          m_length_geometry = m_units.BaseUnits.Length;
        }
        return m_length_geometry;
      }
      set
      {
        useRhinoLengthGeometryUnit = false;
        m_length_geometry = value;
        // update unit system
        BaseUnits units = new BaseUnits(
            m_length_geometry,
            m_units.BaseUnits.Mass, m_units.BaseUnits.Time, m_units.BaseUnits.Current, m_units.BaseUnits.Temperature, m_units.BaseUnits.Amount, m_units.BaseUnits.LuminousIntensity);
        m_units = new UnitsNet.UnitSystem(units);
      }
    }
    public static void UseRhinoLengthUnitGeometry()
    {
      useRhinoLengthGeometryUnit = false;
      m_length_geometry = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
    }
    private static LengthUnit m_length_geometry;
    internal static bool useRhinoLengthGeometryUnit;

    public static LengthUnit LengthUnitSection
    {
      get
      {
        if (useRhinoLengthSectionUnit)
        {
          m_length_section = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
        }
        return m_length_section;
      }
      set
      {
        useRhinoLengthSectionUnit = false;
        m_length_section = value;
      }
    }
    public static void UseRhinoLengthUnitSection()
    {
      useRhinoLengthSectionUnit = false;
      m_length_section = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
    }
    private static LengthUnit m_length_section;
    internal static bool useRhinoLengthSectionUnit;
    internal static AreaMomentOfInertiaUnit SectionAreaMomentOfInertiaUnit
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
    internal static AreaMomentOfInertiaUnit GetSectionAreaMomentOfInertiaUnit(LengthUnit unit)
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
    internal static AreaUnit SectionAreaUnit
    {
      get
      {
        Length len = new Length(1, LengthUnitSection);
        Area unitArea = len * len;
        return unitArea.Unit;
      }
    }

    public static LengthUnit LengthUnitResult
    {
      get
      {
        if (useRhinoLengthResultUnit)
        {
          m_length_result = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
        }
        return m_length_result;
      }
      set
      {
        useRhinoLengthResultUnit = false;
        m_length_result = value;
      }
    }
    public static void UseRhinoLengthUnitResult()
    {
      useRhinoLengthResultUnit = false;
      m_length_result = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
    }
    private static LengthUnit m_length_result;
    internal static bool useRhinoLengthResultUnit;

    #endregion

    #region force
    public static ForceUnit ForceUnit
    {
      get { return m_force; }
      set { m_force = value; }
    }
    private static ForceUnit m_force = ForceUnit.Kilonewton;
    internal static List<string> FilteredForceUnits = new List<string>()
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
        Force force = Force.From(1, ForceUnit);
        Length length = Length.From(1, LengthUnitGeometry);
        ForcePerLength kNperM = force / length;
        return kNperM.Unit;
      }
    }

    internal static List<string> FilteredForcePerLengthUnits = new List<string>()
        {
            ForcePerLengthUnit.NewtonPerMillimeter.ToString(),
            ForcePerLengthUnit.NewtonPerCentimeter.ToString(),
            ForcePerLengthUnit.NewtonPerMeter.ToString(),

            ForcePerLengthUnit.KilonewtonPerMillimeter.ToString(),
            ForcePerLengthUnit.KilonewtonPerCentimeter.ToString(),
            ForcePerLengthUnit.KilonewtonPerMeter.ToString(),

            ForcePerLengthUnit.TonneForcePerCentimeter.ToString(),
            ForcePerLengthUnit.TonneForcePerMeter.ToString(),
            ForcePerLengthUnit.TonneForcePerMillimeter.ToString(),

            ForcePerLengthUnit.MeganewtonPerMeter.ToString(),

            ForcePerLengthUnit.PoundForcePerInch.ToString(),
            ForcePerLengthUnit.PoundForcePerFoot.ToString(),
            ForcePerLengthUnit.PoundForcePerYard.ToString(),

            ForcePerLengthUnit.KilopoundForcePerInch.ToString(),
            ForcePerLengthUnit.KilopoundForcePerFoot.ToString()
        };
    #endregion

    #region moment
    public static MomentUnit MomentUnit
    {
      get { return m_moment; }
      set { m_moment = value; }
    }
    private static MomentUnit m_moment = MomentUnit.KilonewtonMeter;
    internal static List<string> FilteredMomentUnits = Enum.GetNames(typeof(MomentUnit)).ToList();
    #endregion

    #region stress
    public static PressureUnit StressUnit
    {
      get { return m_stress; }
      set { m_stress = value; }
    }
    private static PressureUnit m_stress = PressureUnit.Megapascal;
    internal static List<string> FilteredStressUnits = new List<string>()
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
    internal static List<string> FilteredForcePerAreaUnits = new List<string>()
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
    #endregion

    #region mass
    public static MassUnit MassUnit
    {
      get { return m_mass; }
      set { m_mass = value; }
    }
    private static MassUnit m_mass = MassUnit.Tonne;
    internal static List<string> FilteredMassUnits = new List<string>()
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
    internal static List<string> FilteredDensityUnits = new List<string>()
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
    public static StrainUnit StrainUnit
    {
      get { return m_strain; }
      set { m_strain = value; }
    }
    private static StrainUnit m_strain = StrainUnit.MilliStrain;
    internal static List<string> FilteredStrainUnits = new List<string>()
        {
            StrainUnit.Ratio.ToString(),
            StrainUnit.Percent.ToString(),
            StrainUnit.MilliStrain.ToString(),
            StrainUnit.MicroStrain.ToString()
        };
    #endregion

    #region unit system
    public static UnitsNet.UnitSystem UnitSystem
    {
      get { return m_units; }
      set { m_units = value; }
    }
    private static UnitsNet.UnitSystem m_units;
    #endregion

    #region methods
    internal static void SetupUnits()
    {
      bool settingsExist = ReadSettings();
      if (!settingsExist)
      {
        // get rhino document length unit
        m_length_geometry = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
        m_length_section = LengthUnit.Centimeter;
        m_length_result = LengthUnit.Millimeter;

        SaveSettings();
      }
      // get SI units
      UnitsNet.UnitSystem si = UnitsNet.UnitSystem.SI;

      BaseUnits units = new BaseUnits(
          m_length_geometry,
          si.BaseUnits.Mass, si.BaseUnits.Time, si.BaseUnits.Current, si.BaseUnits.Temperature, si.BaseUnits.Amount, si.BaseUnits.LuminousIntensity);
      m_units = new UnitsNet.UnitSystem(units);

    }
    internal static void SaveSettings()
    {
      Grasshopper.Instances.Settings.SetValue("ComposLengthUnitGeometry", LengthUnitGeometry.ToString());
      Grasshopper.Instances.Settings.SetValue("ComposUseRhinoLengthGeometryUnit", useRhinoLengthGeometryUnit);

      Grasshopper.Instances.Settings.SetValue("ComposLengthUnitSection", LengthUnitSection.ToString());
      Grasshopper.Instances.Settings.SetValue("ComposUseRhinoLengthSectionUnit", useRhinoLengthSectionUnit);

      Grasshopper.Instances.Settings.SetValue("ComposLengthUnitResult", LengthUnitResult.ToString());
      Grasshopper.Instances.Settings.SetValue("ComposUseRhinoLengthResultUnit", useRhinoLengthResultUnit);

      Grasshopper.Instances.Settings.SetValue("ComposTolerance", Tolerance.As(LengthUnitGeometry));

      Grasshopper.Instances.Settings.SetValue("ComposForceUnit", ForceUnit.ToString());
      Grasshopper.Instances.Settings.SetValue("ComposMomentUnit", MomentUnit.ToString());
      Grasshopper.Instances.Settings.SetValue("ComposStressUnit", StressUnit.ToString());

      Grasshopper.Instances.Settings.SetValue("ComposMassUnit", MassUnit.ToString());

      Grasshopper.Instances.Settings.WritePersistentSettings();
    }
    internal static bool ReadSettings()
    {
      if (!Grasshopper.Instances.Settings.ConstainsEntry("ComposLengthUnit"))
        return false;

      string lengthGeometry = Grasshopper.Instances.Settings.GetValue("ComposLengthUnitGeometry", string.Empty);
      useRhinoLengthGeometryUnit = Grasshopper.Instances.Settings.GetValue("ComposUseRhinoLengthGeometryUnit", false);

      string lengthSection = Grasshopper.Instances.Settings.GetValue("ComposLengthUnitSection", string.Empty);
      useRhinoLengthSectionUnit = Grasshopper.Instances.Settings.GetValue("ComposUseRhinoLengthSectionUnit", false);

      string lengthResult = Grasshopper.Instances.Settings.GetValue("ComposLengthUnitResult", string.Empty);
      useRhinoLengthResultUnit = Grasshopper.Instances.Settings.GetValue("ComposUseRhinoLengthResultUnit", false);

      double tolerance = Grasshopper.Instances.Settings.GetValue("ComposTolerance", double.NaN);

      string force = Grasshopper.Instances.Settings.GetValue("ComposForceUnit", string.Empty);
      string moment = Grasshopper.Instances.Settings.GetValue("ComposMomentUnit", string.Empty);
      string stress = Grasshopper.Instances.Settings.GetValue("ComposStressUnit", string.Empty);

      string mass = Grasshopper.Instances.Settings.GetValue("ComposMassUnit", string.Empty);


      if (useRhinoLengthGeometryUnit)
      {
        m_length_geometry = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
      }
      else
      {
        m_length_geometry = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthGeometry);
      }

      if (useRhinoLengthSectionUnit)
      {
        m_length_section = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
      }
      else
      {
        m_length_section = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthSection);
      }

      if (useRhinoLengthResultUnit)
      {
        m_length_result = GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
      }
      else
      {
        m_length_result = (LengthUnit)Enum.Parse(typeof(LengthUnit), lengthResult);
      }

      m_tolerance = Length.From(tolerance, m_length_geometry);

      m_force = (ForceUnit)Enum.Parse(typeof(ForceUnit), force);
      m_moment = (MomentUnit)Enum.Parse(typeof(MomentUnit), moment);
      m_stress = (PressureUnit)Enum.Parse(typeof(PressureUnit), stress);
      m_mass = (MassUnit)Enum.Parse(typeof(MassUnit), mass);

      return true;
    }
    internal static LengthUnit GetRhinoLengthUnit()
    {
      return GetRhinoLengthUnit(RhinoDoc.ActiveDoc.ModelUnitSystem);
    }
    internal static LengthUnit GetRhinoLengthUnit(Rhino.UnitSystem rhinoUnits)
    {
      List<int> id = new List<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 });
      List<string> name = new List<string>(new string[] {
                "None",
                "Microns",
                "mm",
                "cm",
                "m",
                "km",
                "Microinches",
                "Mils",
                "in",
                "ft",
                "Miles",
                " ",
                "Angstroms",
                "Nanometers",
                "Decimeters",
                "Dekameters",
                "Hectometers",
                "Megameters",
                "Gigameters",
                "Yards" });
      List<LengthUnit> unit = new List<LengthUnit>(new LengthUnit[] {
                LengthUnit.Undefined,
                LengthUnit.Micrometer,
                LengthUnit.Millimeter,
                LengthUnit.Centimeter,
                LengthUnit.Meter,
                LengthUnit.Kilometer,
                LengthUnit.Microinch,
                LengthUnit.Mil,
                LengthUnit.Inch,
                LengthUnit.Foot,
                LengthUnit.Mile,
                LengthUnit.Undefined,
                LengthUnit.Undefined,
                LengthUnit.Nanometer,
                LengthUnit.Decimeter,
                LengthUnit.Undefined,
                LengthUnit.Hectometer,
                LengthUnit.Undefined,
                LengthUnit.Undefined,
                LengthUnit.Yard });
      for (int i = 0; i < id.Count; i++)
        if (rhinoUnits.GetHashCode() == id[i])
          return unit[i];
      return LengthUnit.Undefined;
    }
    #endregion
  }
}
