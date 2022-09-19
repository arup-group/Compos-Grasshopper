using System;
using System.Collections.Generic;
using System.Linq;
using OasysUnitsNet;
using OasysUnitsNet.Units;

namespace ComposAPI
{
  public class SlabStressResult : SubResult, ISlabStressResult
  {
    public SlabStressResult(Member member, int numIntermediatePos) : base(member, numIntermediatePos)
    {
    }

    #region concrete stress
    /// <summary>
    /// Maximum stress in concrete slab due to additional dead loads
    /// </summary>
    public List<Pressure> ConcreteStressAdditionalDeadLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_ADDI_CONC_STRESS;
        return this.GetResults(resultType).Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in concrete slab due to Final stage live dead loads
    /// </summary>
    public List<Pressure> ConcreteStressFinalLiveLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_LIVE_CONC_STRESS;
        return this.GetResults(resultType).Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in concrete slab due to shrinkage
    /// </summary>
    public List<Pressure> ConcreteStressFinalShrinkage
    {
      get
      {
        StressOption resultType = StressOption.STRESS_SHRINK_CONC_STRESS;
        return this.GetResults(resultType).Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in concrete slab in Final stage
    /// </summary>
    public List<Pressure> ConcreteStressFinal
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_TOTL_CONC_STRESS;
        return this.GetResults(resultType).Select(x => (Pressure)x).ToList();
      }
    }
    #endregion
    #region concrete strain
    /// <summary>
    /// Maximum strain in concrete slab due to additional dead loads
    /// </summary>
    public List<Strain> ConcreteStrainAdditionalDeadLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_ADDI_CONC_STRAIN;
        return this.GetResults(resultType).Select(x => (Strain)x).ToList();
      }
    }

    /// <summary>
    /// Maximum strain in concrete slab due to Final stage live dead loads
    /// </summary>
    public List<Strain> ConcreteStrainFinalLiveLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_LIVE_CONC_STRAIN;
        return this.GetResults(resultType).Select(x => (Strain)x).ToList();
      }
    }

    /// <summary>
    /// Maximum strain in concrete slab due to shrinkage
    /// </summary>
    public List<Strain> ConcreteStrainFinalShrinkage
    {
      get
      {
        StressOption resultType = StressOption.STRESS_SHRINK_CONC_STRAIN;
        return this.GetResults(resultType).Select(x => (Strain)x).ToList();
      }
    }

    /// <summary>
    /// Maximum strain in concrete slab in Final stage
    /// </summary>
    public List<Strain> ConcreteStrainFinal
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_TOTL_CONC_STRAIN;
        return this.GetResults(resultType).Select(x => (Strain)x).ToList();
      }
    }
    #endregion

    private Dictionary<StressOption, List<IQuantity>> ResultsCache = new Dictionary<StressOption, List<IQuantity>>();
    private List<IQuantity> GetResults(StressOption resultType)
    {
      if (!this.ResultsCache.ContainsKey(resultType))
      {
        List<IQuantity> results = new List<IQuantity>();
        for (short pos = 0; pos < this.NumIntermediatePos; pos++)
        {
          float value = this.Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

          switch (resultType)
          {
            case StressOption.STRESS_ADDI_CONC_STRESS:
            case StressOption.STRESS_FINA_LIVE_CONC_STRESS:
            case StressOption.STRESS_SHRINK_CONC_STRESS:
            case StressOption.STRESS_FINA_TOTL_CONC_STRESS:
              results.Add(new Pressure(value, PressureUnit.Pascal));
              break;
            case StressOption.STRESS_ADDI_CONC_STRAIN:
            case StressOption.STRESS_FINA_LIVE_CONC_STRAIN:
            case StressOption.STRESS_SHRINK_CONC_STRAIN:
            case StressOption.STRESS_FINA_TOTL_CONC_STRAIN:
              results.Add(new Strain(value, StrainUnit.Ratio));
              break;
          }
        }
        this.ResultsCache.Add(resultType, results);
      }
      return this.ResultsCache[resultType];
    }
  }
}
