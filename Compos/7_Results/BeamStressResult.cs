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
  public class BeamStressResult : ResultsBase, IBeamStressResult
  {
    internal Dictionary<StressOption, List<Pressure>> ResultsCache = new Dictionary<StressOption, List<Pressure>>();
    public BeamStressResult(Member member) : base(member)
    {
    }

    #region bottom flange
    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to Construction loads
    /// </summary>
    public List<Pressure> BottomFlangeConstruction
    {
      get
      {
        StressOption resultType = StressOption.STRESS_CONS_BEAM_BOT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to additional dead loads
    /// </summary>
    public List<Pressure> BottomFlangeFinalAdditionalDeadLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_ADDI_BEAM_BOT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to Final stage live dead loads
    /// </summary>
    public List<Pressure> BottomFlangeFinalLiveLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_LIVE_BEAM_BOT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange due to shrinkage
    /// </summary>
    public List<Pressure> BottomFlangeFinalShrinkage
    {
      get
      {
        StressOption resultType = StressOption.STRESS_SHRINK_BEAM_BOT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam bottom Flange in Final stage
    /// </summary>
    public List<Pressure> BottomFlangeFinal
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_TOTL_BEAM_BOT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }
    #endregion
    #region web
    /// <summary>
    /// Maximum stress in steel beam Web due to Construction loads
    /// </summary>
    public List<Pressure> WebConstruction
    {
      get
      {
        StressOption resultType = StressOption.STRESS_CONS_BEAM_WEB;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam Web due to additional dead loads
    /// </summary>
    public List<Pressure> WebFinalAdditionalDeadLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_ADDI_BEAM_WEB;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam Web due to Final stage live dead loads
    /// </summary>
    public List<Pressure> WebFinalLiveLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_LIVE_BEAM_WEB;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam Web due to shrinkage
    /// </summary>
    public List<Pressure> WebFinalShrinkage
    {
      get
      {
        StressOption resultType = StressOption.STRESS_SHRINK_BEAM_WEB;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam Web in Final stage
    /// </summary>
    public List<Pressure> WebFinal
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_TOTL_BEAM_WEB;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }
    #endregion
    #region top flange
    /// <summary>
    /// Maximum stress in steel beam top Flange due to Construction loads
    /// </summary>
    public List<Pressure> TopFlangeConstruction
    {
      get
      {
        StressOption resultType = StressOption.STRESS_CONS_BEAM_TOP;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to additional dead loads
    /// </summary>
    public List<Pressure> TopFlangeFinalAdditionalDeadLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_ADDI_BEAM_TOP;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to Final stage live dead loads
    /// </summary>
    public List<Pressure> TopFlangeFinalLiveLoad
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_LIVE_BEAM_TOP;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam top Flange due to shrinkage
    /// </summary>
    public List<Pressure> TopFlangeFinalShrinkage
    {
      get
      {
        StressOption resultType = StressOption.STRESS_SHRINK_BEAM_TOP;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }

    /// <summary>
    /// Maximum stress in steel beam top Flange in Final stage
    /// </summary>
    public List<Pressure> TopFlangeFinal
    {
      get
      {
        StressOption resultType = StressOption.STRESS_FINA_TOTL_BEAM_TOP;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Pressure)x).ToList();
      }
    }
    #endregion

    private void GetResults(StressOption resultType)
    {
      List<Pressure> results = new List<Pressure>();
      for (short pos = 0; pos < this.NumIntermediatePos; pos++)
      {
        float value = this.Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));
        results.Add(new Pressure(value, PressureUnit.Pascal));
      }
      ResultsCache.Add(resultType, results);
    }
  }
}
