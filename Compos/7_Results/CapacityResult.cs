﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class CapacityResult : ResultsBase, ICapacityResult
  {
    internal Dictionary<CapacityOption, List<IQuantity>> ResultsCache = new Dictionary<CapacityOption, List<IQuantity>>();
    public CapacityResult(Member member) : base(member)
    {
    }

    /// <summary>
    /// Sagging moment capacity in Final stage
    /// </summary>
    public List<Moment> Moment
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_MOM_ULTI_FINA_SAG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Sagging moment in Final stage
    /// </summary>
    public List<Length> NeutralAxis
    {
      get
      {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_ULTI_FINA_SAG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Sagging moment capacity in Construction stage
    /// </summary>
    public List<Moment> MomentConstruction
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_MOM_ULTI_CONS_SAG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Sagging moment in Construction stage
    /// </summary>
    public List<Length> NeutralAxisConstruction
    {
      get
      {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_ULTI_CONS_SAG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Hogging moment capacity in Construction stage
    /// </summary>
    public List<Moment> MomentHoggingConstruction
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_MOM_ULTI_CONS_HOG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Hogging moment in Construction stage
    /// </summary>
    public List<Length> NeutralAxisHoggingConstruction
    {
      get
      {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_ULTI_CONS_HOG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Hogging moment capacity in Final stage
    /// </summary>
    public List<Moment> MomentHoggingFinal
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_MOM_ULTI_FINA_HOG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Hogging moment in Final stage
    /// </summary>
    public List<Length> NeutralAxisHoggingFinal
    {
      get
      {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_ULTI_FINA_HOG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Shear capacity
    /// </summary>
    public List<Force> Shear
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_SHE_SHEAR;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Shear capacity with web buckling
    /// </summary>
    public List<Force> ShearBuckling
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_SHE_BUCLE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Used shear capacity
    /// </summary>
    public List<Force> ShearRequired
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_SHE_PV;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Assumed plastic Sagging moment capacity
    /// </summary>
    public List<Moment> AssumedPlasticMoment
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_MOM_BEAM_PLAS_SAG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Assumed plastic Sagging moment
    /// </summary>
    public List<Length> AssumedPlasticNeutralAxis
    {
      get
      {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_BEAM_PLAS_SAG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Assumed 100% shear interaction Sagging moment capacity
    /// </summary>
    public List<Moment> AssumedMomentFullShearInteraction
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_MOM_100_INTER_SAG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Assumed 100% shear interaction Sagging moment
    /// </summary>
    public List<Length> AssumedNeutralAxisFullShearInteraction
    {
      get
      {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_100_INTER_SAG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Assumed plastic Hogging moment capacity
    /// </summary>
    public List<Moment> AssumedPlasticMomentHogging
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_MOM_BEAM_PLAS_HOG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Assumed plastic Hogging moment
    /// </summary>
    public List<Length> AssumedPlasticNeutralAxisHogging
    {
      get
      {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_BEAM_PLAS_HOG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    /// <summary>
    /// Assumed 100% shear interaction Sagging moment capacity
    /// </summary>
    public List<Moment> AssumedMomentFullShearInteractionHogging
    {
      get
      {
        CapacityOption resultType = CapacityOption.CAPA_MOM_100_INTER_HOG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Neutral axis depth under Assumed 100% shear interaction Sagging moment
    /// </summary>
    public List<Length> AssumedNeutralAxisFullShearInteractionHogging
    {
      get
      {
        CapacityOption resultType = CapacityOption.NEUTRAL_X_100_INTER_HOG;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Length)x).ToList();
      }
    }

    private void GetResults(CapacityOption resultType)
    {
      List<IQuantity> results = new List<IQuantity>();
      for (short pos = 0; pos < this.NumIntermediatePos; pos++)
      {
        float value = this.Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

        switch (resultType)
        {
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
  }
}
