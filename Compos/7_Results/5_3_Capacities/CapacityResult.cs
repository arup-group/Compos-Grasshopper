using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComposAPI {
  public class CapacityResult : SubResult, ICapacityResult {
    /// <summary>
    /// Assumed plastic Sagging moment capacity
    /// </summary>
    public List<Moment> AssumedBeamPlasticMoment {
      get {
        CapacityOption resultType = CapacityOption.CAPA_MOM_BEAM_PLAS_SAG;
        return GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Assumed plastic Hogging moment capacity
    /// </summary>
    public List<Moment> AssumedBeamPlasticMomentHogging {
      get {
        CapacityOption resultType = CapacityOption.CAPA_MOM_BEAM_PLAS_HOG;
        return GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Assumed 100% shear interaction Sagging moment capacity
    /// </summary>
    public List<Moment> AssumedMomentFullShearInteraction {
      get {
        CapacityOption resultType = CapacityOption.CAPA_MOM_100_INTER_SAG;
        return GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Assumed 100% shear interaction Sagging moment capacity
    /// </summary>
    public List<Moment> AssumedMomentFullShearInteractionHogging {
      get {
        CapacityOption resultType = CapacityOption.CAPA_MOM_100_INTER_HOG;
        return GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Assumed 100% shear interaction Sagging moment
    /// </summary>
    public List<Length> AssumedNeutralAxisFullShearInteraction {
      get {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_100_INTER_SAG;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Assumed 100% shear interaction Sagging moment
    /// </summary>
    public List<Length> AssumedNeutralAxisFullShearInteractionHogging {
      get {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_100_INTER_HOG;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Assumed plastic Sagging moment
    /// </summary>
    public List<Length> AssumedPlasticNeutralAxis {
      get {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_BEAM_PLAS_SAG;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Assumed plastic Hogging moment
    /// </summary>
    public List<Length> AssumedPlasticNeutralAxisHogging {
      get {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_BEAM_PLAS_HOG;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Sagging moment capacity in Final stage
    /// </summary>
    public List<Moment> Moment {
      get {
        CapacityOption resultType = CapacityOption.CAPA_MOM_ULTI_FINA_SAG;
        return GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Sagging moment capacity in Construction stage
    /// </summary>
    public List<Moment> MomentConstruction {
      get {
        CapacityOption resultType = CapacityOption.CAPA_MOM_ULTI_CONS_SAG;
        return GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Hogging moment capacity in Construction stage
    /// </summary>
    public List<Moment> MomentHoggingConstruction {
      get {
        CapacityOption resultType = CapacityOption.CAPA_MOM_ULTI_CONS_HOG;
        return GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Hogging moment capacity in Final stage
    /// </summary>
    public List<Moment> MomentHoggingFinal {
      get {
        CapacityOption resultType = CapacityOption.CAPA_MOM_ULTI_FINA_HOG;
        return GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Sagging moment in Final stage
    /// </summary>
    public List<Length> NeutralAxis {
      get {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_ULTI_FINA_SAG;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Sagging moment in Construction stage
    /// </summary>
    public List<Length> NeutralAxisConstruction {
      get {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_ULTI_CONS_SAG;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Hogging moment in Construction stage
    /// </summary>
    public List<Length> NeutralAxisHoggingConstruction {
      get {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_ULTI_CONS_HOG;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Hogging moment in Final stage
    /// </summary>
    public List<Length> NeutralAxisHoggingFinal {
      get {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_ULTI_FINA_HOG;
        return GetResults(resultType).Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Shear capacity
    /// </summary>
    public List<Force> Shear {
      get {
        CapacityOption resultType = CapacityOption.CAPA_SHE_SHEAR;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Shear capacity with web buckling
    /// </summary>
    public List<Force> ShearBuckling {
      get {
        CapacityOption resultType = CapacityOption.CAPA_SHE_BUCLE;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Used shear capacity
    /// </summary>
    public List<Force> ShearRequired {
      get {
        CapacityOption resultType = CapacityOption.CAPA_SHE_PV;
        return GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    private Dictionary<CapacityOption, List<IQuantity>> ResultsCache = new Dictionary<CapacityOption, List<IQuantity>>();

    public CapacityResult(Member member, int numIntermediatePos) : base(member, numIntermediatePos) {
    }

    private List<IQuantity> GetResults(CapacityOption resultType) {
      if (!ResultsCache.ContainsKey(resultType)) {
        List<IQuantity> results = new List<IQuantity>();
        for (short pos = 0; pos < NumIntermediatePos; pos++) {
          float value = Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

          switch (resultType) {
            case CapacityOption.CAPA_MOM_ULTI_CONS_HOG:
            case CapacityOption.CAPA_MOM_ULTI_FINA_HOG:
            case CapacityOption.CAPA_MOM_ULTI_CONS_SAG:
            case CapacityOption.CAPA_MOM_ULTI_FINA_SAG:
            case CapacityOption.CAPA_MOM_BEAM_PLAS_HOG:
            case CapacityOption.CAPA_MOM_100_INTER_HOG:
            case CapacityOption.CAPA_MOM_BEAM_PLAS_SAG:
            case CapacityOption.CAPA_MOM_100_INTER_SAG:
              results.Add(new Moment(value, MomentUnit.NewtonMeter));
              break;

            case CapacityOption.NEUTRAL_X_ULTI_CONS_HOG:
            case CapacityOption.NEUTRAL_X_ULTI_FINA_HOG:
            case CapacityOption.NEUTRAL_X_ULTI_CONS_SAG:
            case CapacityOption.NEUTRAL_X_ULTI_FINA_SAG:
            case CapacityOption.NEUTRAL_X_BEAM_PLAS_HOG:
            case CapacityOption.NEUTRAL_X_100_INTER_HOG:
            case CapacityOption.NEUTRAL_X_BEAM_PLAS_SAG:
            case CapacityOption.NEUTRAL_X_100_INTER_SAG:
              results.Add(new Length(value, LengthUnit.Meter));
              break;

            case CapacityOption.CAPA_SHE_SHEAR:
            case CapacityOption.CAPA_SHE_BUCLE:
            case CapacityOption.CAPA_SHE_PV:
              results.Add(new Force(value, ForceUnit.Newton));
              break;
          }
        }
        ResultsCache.Add(resultType, results);
      }
      return ResultsCache[resultType];
    }
  }

  internal enum CapacityOption {
    CAPA_MOM_ULTI_CONS_HOG, // Hogging moment capacity in Construction stage
    NEUTRAL_X_ULTI_CONS_HOG, // Neutral axis depth under Hogging moment in Construction stage
    CAPA_MOM_ULTI_FINA_HOG, // Hogging moment capacity in Final stage
    NEUTRAL_X_ULTI_FINA_HOG, // Neutral axis depth under Hogging moment in Final stage
    CAPA_MOM_ULTI_CONS_SAG, // Sagging moment capacity in Construction stage
    NEUTRAL_X_ULTI_CONS_SAG, // Neutral axis depth under Sagging moment in Construction stage
    CAPA_MOM_ULTI_FINA_SAG, // Sagging moment capacity in Final stage
    NEUTRAL_X_ULTI_FINA_SAG, // Neutral axis depth under Sagging moment in Final stage
    CAPA_SHE_SHEAR, // shear capacity
    CAPA_SHE_BUCLE, // shear capacity with web buckling
    CAPA_SHE_PV, // Used shear capacity
    CAPA_MOM_BEAM_PLAS_HOG, // Assumed plastic Hogging moment capacity in Construction stage
    NEUTRAL_X_BEAM_PLAS_HOG, // Neutral axis depth under Assumed plastic Hogging moment in Construction stage
    CAPA_MOM_100_INTER_HOG, // Assumed 100% shear interaction hogging moment capacity in final stage
    NEUTRAL_X_100_INTER_HOG, // Neutral axis depth under assumed 100% interaction hogging moment in final stage
    CAPA_MOM_BEAM_PLAS_SAG, // Assumed plastic Sagging moment capacity in Construction stage
    NEUTRAL_X_BEAM_PLAS_SAG, // Neutral axis depth under Assumed plastic Sagging moment in Construction stage
    CAPA_MOM_100_INTER_SAG, // Assumed 100% shear interaction sagging moment capacity in final stage
    NEUTRAL_X_100_INTER_SAG, // Neutral axis depth under assumed 100% interaction sagging moment in final stage
  }
}
