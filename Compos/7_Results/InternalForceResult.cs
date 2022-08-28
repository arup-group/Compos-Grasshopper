using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Units;
using UnitsNet;
using UnitsNet.Units;

namespace ComposAPI
{
  public class InternalForceResult : ResultsBase, IInternalForceResult
  {
    internal Dictionary<InternalForceOption, List<IQuantity>> ResultsCache = new Dictionary<InternalForceOption, List<IQuantity>>();
    public InternalForceResult(Member member) : base(member)
    {
    }

    #region moment
    /// <summary>
    /// Construction stage ultimate moment
    /// </summary>
    public List<Moment> MomentULSConstruction
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_MOM_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Final stage ultimate moment
    /// </summary>
    public List<Moment> MomentULS
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_MOM_FINA;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Construction stage working dead load moment
    /// </summary>
    public List<Moment> MomentConstructionDeadLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_MOM_CONS_DEAD;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Construction stage working live load moment
    /// </summary>
    public List<Moment> MomentConstructionLiveLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_MOM_CONS_LIVE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Final stage working additional dead load moment
    /// </summary>
    public List<Moment> MomentFinalAdditionalDeadLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_MOM_FINA_ADDI;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Final stage working live load moment
    /// </summary>
    public List<Moment> MomentFinalLiveLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_MOM_FINA_LIVE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }

    /// <summary>
    /// Final stage working shrinkage moment
    /// </summary>
    public List<Moment> MomentFinalShrinkage
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_MOM_FINA_SHRI;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Moment)x).ToList();
      }
    }
    #endregion
    #region shear
    /// <summary>
    /// Construction stage ultimate shear
    /// </summary>
    public List<Force> ShearULSConstructionStage
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_SHE_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Final stage ultimate shear
    /// </summary>
    public List<Force> ShearULS
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_SHE_FINA;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Construction stage working dead load moment
    /// </summary>
    public List<Force> ShearConstructionDeadLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_SHE_CONS_DEAD;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Construction stage working live load moment
    /// </summary>
    public List<Force> ShearConstructionLiveLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_SHE_CONS_LIVE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Final stage working additional dead load moment
    /// </summary>
    public List<Force> ShearFinalAdditionalDeadLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_SHE_FINA_ADDI;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Final stage working live load moment
    /// </summary>
    public List<Force> ShearFinalLiveLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_SHE_FINA_LIVE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }
    #endregion
    #region force
    /// <summary>
    /// Construction stage ultimate shear
    /// </summary>
    public List<Force> AxialULSConstructionStage
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Final stage ultimate shear
    /// </summary>
    public List<Force> AxialULS
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_FINA;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Construction stage working dead load moment
    /// </summary>
    public List<Force> AxialConstructionDeadLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_AXIAL_CONS_DEAD;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Construction stage working live load moment
    /// </summary>
    public List<Force> AxialConstructionLiveLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_AXIAL_CONS_LIVE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Final stage working additional dead load moment
    /// </summary>
    public List<Force> AxialFinalAdditionalDeadLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_AXIAL_FINA_ADDI;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Final stage working live load moment
    /// </summary>
    public List<Force> AxialFinalLiveLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.WORK_AXIAL_FINA_LIVE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }
    #endregion
    private void GetResults(InternalForceOption resultType)
    {
      List<IQuantity> results = new List<IQuantity>();
      for (short pos = 0; pos < this.NumIntermediatePos; pos++)
      {
        float value = this.Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

        switch (resultType)
        {
          case InternalForceOption.ULTI_MOM_CONS:
          case InternalForceOption.ULTI_MOM_FINA:
          case InternalForceOption.WORK_MOM_CONS_DEAD:
          case InternalForceOption.WORK_MOM_CONS_LIVE:
          case InternalForceOption.WORK_MOM_FINA_ADDI:
          case InternalForceOption.WORK_MOM_FINA_LIVE:
          case InternalForceOption.WORK_MOM_FINA_SHRI:
            results.Add(new Moment(value, MomentUnit.NewtonMeter));
            break;
          case InternalForceOption.ULTI_SHE_CONS:
          case InternalForceOption.ULTI_SHE_FINA:
          case InternalForceOption.WORK_SHE_CONS_DEAD:
          case InternalForceOption.WORK_SHE_CONS_LIVE:
          case InternalForceOption.WORK_SHE_FINA_ADDI:
          case InternalForceOption.WORK_SHE_FINA_LIVE:
          case InternalForceOption.ULTI_AXIAL_CONS:
          case InternalForceOption.ULTI_AXIAL_FINA:
          case InternalForceOption.WORK_AXIAL_CONS_DEAD:
          case InternalForceOption.WORK_AXIAL_CONS_LIVE:
          case InternalForceOption.WORK_AXIAL_FINA_ADDI:
          case InternalForceOption.WORK_AXIAL_FINA_LIVE:
            results.Add(new Force(value, ForceUnit.Newton));
            break;
        }
      }
      ResultsCache.Add(resultType, results);
    }
  }
}
