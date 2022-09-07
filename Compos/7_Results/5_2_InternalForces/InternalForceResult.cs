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
  internal enum InternalForceOption
  {
    ULTI_MOM_CONS, // Construction stage ultimate moment
    ULTI_MOM_FINA, // Final stage ultimate moment
    ULTI_SHE_CONS, // Construction stage ultimate shear
    ULTI_SHE_FINA, // Final stage ultimate shear
    ULTI_AXIAL_CONS, // Construction stage ultimate axial force
    ULTI_AXIAL_FINA, // Final stage ultimate axial force
    WORK_MOM_CONS_DEAD, // Construction stage working dead load moment
    WORK_MOM_CONS_LIVE, // Construction stage working live load moment
    WORK_MOM_FINA_ADDI, // Final stage working additional dead load moment
    WORK_MOM_FINA_LIVE, // Final stage working live load moment
    WORK_MOM_FINA_SHRI, // Final stage working shrinkage moment
    WORK_SHE_CONS_DEAD, // Construction stage working dead load shear
    WORK_SHE_CONS_LIVE, // Construction stage working live load shear
    WORK_SHE_FINA_ADDI, // Final stage working additional dead load shear
    WORK_SHE_FINA_LIVE, // Final stage working live load shear
    WORK_AXIAL_CONS_DEAD, // Construction stage working dead load axial
    WORK_AXIAL_CONS_LIVE, // Construction stage working live load axial
    WORK_AXIAL_FINA_ADDI, // Final stage working additional dead load axial
    WORK_AXIAL_FINA_LIVE, // Final stage working live load axial
  }

  public class InternalForceResult : SubResult, IInternalForceResult
  {
    public InternalForceResult(Member member, int numIntermediatePos) : base(member, numIntermediatePos)
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
        return this.GetResults(resultType).Select(x => (Moment)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Moment)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Moment)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Moment)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Moment)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Moment)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Moment)x).ToList();
      }
    }
    #endregion
    #region shear
    /// <summary>
    /// Construction stage ultimate shear
    /// </summary>
    public List<Force> ShearULSConstruction
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_SHE_CONS;
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }
    #endregion
    #region force
    /// <summary>
    /// Construction stage ultimate shear
    /// </summary>
    public List<Force> AxialULSConstruction
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }
    #endregion

    private Dictionary<InternalForceOption, List<IQuantity>> ResultsCache = new Dictionary<InternalForceOption, List<IQuantity>>();

    private List<IQuantity> GetResults(InternalForceOption resultType)
    {
      if (!this.ResultsCache.ContainsKey(resultType))
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
        this.ResultsCache.Add(resultType, results);
      }
      return this.ResultsCache[resultType];
    }
  }
}
