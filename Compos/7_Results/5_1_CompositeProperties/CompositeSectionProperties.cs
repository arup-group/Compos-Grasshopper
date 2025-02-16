﻿using System;
using System.Collections.Generic;
using System.Linq;
using ComposAPI.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class CompositeSectionProperties : SubResult, ICompositeSectionProperties {
    /// <summary>
    /// Effective Area of composite section under combined loading
    /// </summary>
    public List<Area> AreaEffective {
      get {
        BeamResultOption resultType = BeamResultOption.AREA_EFFECT;
        return GetResults(resultType).Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Area of composite section for long term loading
    /// </summary>
    public List<Area> AreaLongTerm {
      get {
        BeamResultOption resultType = BeamResultOption.AREA_LONG_TERM;
        return GetResults(resultType).Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Area of composite section for short term loading
    /// </summary>
    public List<Area> AreaShortTerm {
      get {
        BeamResultOption resultType = BeamResultOption.AREA_SHORT_TERM;
        return GetResults(resultType).Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Area of composite section for shrinkage
    /// </summary>
    public List<Area> AreaShrinkage {
      get {
        BeamResultOption resultType = BeamResultOption.AREA_SHRINK;
        return GetResults(resultType).Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Area of composite section for vibration
    /// </summary>
    public List<Area> AreaVibration {
      get {
        BeamResultOption resultType = BeamResultOption.AREA_VIBRATION;
        return GetResults(resultType).Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Area of steel beam alone
    /// </summary>
    public List<Area> BeamArea {
      get {
        BeamResultOption resultType = BeamResultOption.AREA_STEEL_BEAM;
        return GetResults(resultType).Select(x => (Area)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of steel beam alone
    /// </summary>
    public List<AreaMomentOfInertia> BeamMomentOfInertia {
      get {
        BeamResultOption resultType = BeamResultOption.I_STEEL_BEAM;
        return GetResults(resultType).Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of steel beam alone, measured from the bottom of steel beam
    /// </summary>
    public List<Length> BeamNeutralAxisPosition {
      get {
        BeamResultOption resultType = BeamResultOption.X_STEEL_BEAM;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Effective slab width on left side
    /// </summary>
    public List<Length> EffectiveSlabWidthLeft {
      get {
        ResultOption resultType = ResultOption.SLAB_WIDTH_L_EFFECT;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Effective slab width on right side
    /// </summary>
    public List<Length> EffectiveSlabWidthRight {
      get {
        ResultOption resultType = ResultOption.SLAB_WIDTH_R_EFFECT;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Welding thickness at bottom - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web.
    /// </summary>
    public List<Length> GirderWeldThicknessBottom {
      get {
        BeamResultOption resultType = BeamResultOption.GIRDER_WELD_THICK_B;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Welding thickness at top - the throat thickness of welding, this thickness is calculated based on the equal shear strength of the welding and the steel beam web.
    /// </summary>
    public List<Length> GirderWeldThicknessTop {
      get {
        BeamResultOption resultType = BeamResultOption.GIRDER_WELD_THICK_T;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Effective Moment of Inertia of composite section under combined loading
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaEffective {
      get {
        BeamResultOption resultType = BeamResultOption.I_EFFECTIVE;
        return GetResults(resultType).Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of composite section for long term loading
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaLongTerm {
      get {
        BeamResultOption resultType = BeamResultOption.I_LONG_TERM;
        return GetResults(resultType).Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of composite section for short term loading
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaShortTerm {
      get {
        BeamResultOption resultType = BeamResultOption.I_SHORT_TERM;
        return GetResults(resultType).Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of composite section for shrinkage
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaShrinkage {
      get {
        BeamResultOption resultType = BeamResultOption.I_SHRINK;
        return GetResults(resultType).Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Moment of Inertia of composite section for vibration
    /// </summary>
    public List<AreaMomentOfInertia> MomentOfInertiaVibration {
      get {
        BeamResultOption resultType = BeamResultOption.I_VIBRATION;
        return GetResults(resultType).Select(x => (AreaMomentOfInertia)x).ToList();
      }
    }

    /// <summary>
    /// Natural frequency for composite section
    /// </summary>
    public Frequency NaturalFrequency {
      get {
        if (ComposUnitsHelper.IsEqual(m_frequency,Frequency.Zero)) {
          m_frequency = new Frequency(Member.UtilisationFactor(UtilisationFactorOption.NaturalFrequency), FrequencyUnit.Hertz);
        }
        return m_frequency;
      }
    }

    /// <summary>
    /// Effective Neutral axis position of composite section under combined loading, measured from the bottom of steel beam
    /// </summary>
    public List<Length> NeutralAxisPositionEffective {
      get {
        BeamResultOption resultType = BeamResultOption.X_EFFECTIVE;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of composite section for long term loading, measured from the bottom of steel beam
    /// </summary>
    public List<Length> NeutralAxisPositionLongTerm {
      get {
        BeamResultOption resultType = BeamResultOption.X_LONG_TERM;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of composite section for short term loading, measured from the bottom of steel beam
    /// </summary>
    public List<Length> NeutralAxisPositionShortTerm {
      get {
        BeamResultOption resultType = BeamResultOption.X_SHORT_TERM;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of composite section for shrinkage, measured from the bottom of steel beam
    /// </summary>
    public List<Length> NeutralAxisPositionShrinkage {
      get {
        BeamResultOption resultType = BeamResultOption.X_SHRINK;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis position of composite section for vibration, measured from the bottom of steel beam
    /// </summary>
    public List<Length> NeutralAxisPositionVibration {
      get {
        BeamResultOption resultType = BeamResultOption.X_VIBRATION;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    private Frequency m_frequency = Frequency.Zero;

    private Dictionary<Enum, List<IQuantity>> ResultsCache = new Dictionary<Enum, List<IQuantity>>();

    public CompositeSectionProperties(Member member, int numIntermediatePos) : base(member, numIntermediatePos) {
    }

    private List<IQuantity> GetResults(BeamResultOption resultType) {
      if (!ResultsCache.ContainsKey(resultType)) {
        var results = new List<IQuantity>();
        for (short pos = 0; pos < NumIntermediatePos; pos++) {
          float value = Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

          switch (resultType) {
            case BeamResultOption.GIRDER_WELD_THICK_T:
            case BeamResultOption.GIRDER_WELD_THICK_B:
            case BeamResultOption.X_STEEL_BEAM:
            case BeamResultOption.X_LONG_TERM:
            case BeamResultOption.X_SHORT_TERM:
            case BeamResultOption.X_SHRINK:
            case BeamResultOption.X_EFFECTIVE:
            case BeamResultOption.X_VIBRATION:
              results.Add(new Length(value, LengthUnit.Meter));
              break;

            case BeamResultOption.I_STEEL_BEAM:
            case BeamResultOption.I_LONG_TERM:
            case BeamResultOption.I_SHORT_TERM:
            case BeamResultOption.I_SHRINK:
            case BeamResultOption.I_EFFECTIVE:
            case BeamResultOption.I_VIBRATION:
              results.Add(new AreaMomentOfInertia(value, AreaMomentOfInertiaUnit.MeterToTheFourth));
              break;

            case BeamResultOption.AREA_STEEL_BEAM:
            case BeamResultOption.AREA_LONG_TERM:
            case BeamResultOption.AREA_SHORT_TERM:
            case BeamResultOption.AREA_SHRINK:
            case BeamResultOption.AREA_EFFECT:
            case BeamResultOption.AREA_VIBRATION:
              results.Add(new Area(value, AreaUnit.SquareMeter));
              break;
          }
        }
        ResultsCache.Add(resultType, results);
      }
      return ResultsCache[resultType];
    }

    private List<IQuantity> GetResults(ResultOption resultType) {
      if (!ResultsCache.ContainsKey(resultType)) {
        var results = new List<IQuantity>();
        for (short pos = 0; pos < NumIntermediatePos; pos++) {
          float value = Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

          results.Add(new Length(value, LengthUnit.Meter));
        }
        ResultsCache.Add(resultType, results);
      }
      return ResultsCache[resultType];
    }
  }

  internal enum BeamResultOption {
    GIRDER_WELD_THICK_T, // Welding thickness at top
    GIRDER_WELD_THICK_B, // Welding thickness at bottom
    I_STEEL_BEAM, // moment of Inertia of steel beam
    X_STEEL_BEAM, // Neutral axis depth of steel beam
    AREA_STEEL_BEAM, // Area of steel beam
    I_LONG_TERM, // moment of Inertia of beam for long term loading
    X_LONG_TERM, // Neutral axis depth of beam for long term loading
    AREA_LONG_TERM, // Area of beam for long term loading
    I_SHORT_TERM, // moment of Inertia of beam for short term loading
    X_SHORT_TERM, // Neutral axis depth of beam for short term loading
    AREA_SHORT_TERM, // Area of beam for short term loading
    I_SHRINK, // moment of Inertia of beam for shrinkage
    X_SHRINK, // Neutral axis depth of beam for shrinkage
    AREA_SHRINK, // Area of beam for shrinkage
    I_EFFECTIVE, // Effective moment of Inertia of beam
    X_EFFECTIVE, // Neutral axis depth of beam under combined loading
    AREA_EFFECT, // Effective Area of beam
    I_VIBRATION, // moment of Inertia of beam for vibration
    X_VIBRATION, // Neutral axis depth of beam for vibration
    AREA_VIBRATION, // Area of beam for vibration
  }
}
