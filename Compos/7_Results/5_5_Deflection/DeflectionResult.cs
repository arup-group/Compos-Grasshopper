using System;
using System.Collections.Generic;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI {
  public class DeflectionResult : SubResult, IDeflectionResult {
    /// <summary>
    /// Deflection due to additional dead loads
    /// </summary>
    public List<Length> AdditionalDeadLoad {
      get {
        DeflectionOption resultType = DeflectionOption.DEFL_ADDI_DEAD_LOAD;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Deflection due to Construction dead loads
    /// </summary>
    public List<Length> ConstructionDeadLoad {
      get {
        DeflectionOption resultType = DeflectionOption.DEFL_CONS_DEAD_LOAD;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Deflection due to Final stage live loads
    /// </summary>
    public List<Length> LiveLoad {
      get {
        DeflectionOption resultType = DeflectionOption.DEFL_FINA_LIVE_LOAD;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Deflection due to post Construction loads
    /// </summary>
    public List<Length> PostConstruction {
      get {
        DeflectionOption resultType = DeflectionOption.DEFL_POST_CONS;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Deflection due to shrinkage of concrete
    /// </summary>
    public List<Length> Shrinkage {
      get {
        DeflectionOption resultType = DeflectionOption.DEFL_SHRINK;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Total Deflection
    /// </summary>
    public List<Length> Total {
      get {
        DeflectionOption resultType = DeflectionOption.DEFL_FINA_TOTAL;
        return GetResults(resultType);
      }
    }

    private Dictionary<DeflectionOption, List<Length>> ResultsCache = new Dictionary<DeflectionOption, List<Length>>();

    public DeflectionResult(Member member, int numIntermediatePos) : base(member, numIntermediatePos) {
    }

    // bug in COM, ModalShape giving only 0 values
    ///// <summary>
    ///// Mode shape
    ///// </summary>
    //public List<Length> ModalShape
    //{
    //  get
    //  {
    //    DeflectionOption resultType = DeflectionOption.MODAL_SHAPE;
    //    if (!ResultsCache.ContainsKey(resultType))
    //      GetResults(resultType);
    //    return ResultsCache[resultType];
    //  }
    //}
    private List<Length> GetResults(DeflectionOption resultType) {
      if (!ResultsCache.ContainsKey(resultType)) {
        var results = new List<Length>();
        for (short pos = 0; pos < NumIntermediatePos; pos++) {
          float value = Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));
          results.Add(new Length(value, LengthUnit.Meter));
        }
        ResultsCache.Add(resultType, results);
      }
      return ResultsCache[resultType];
    }
  }

  internal enum DeflectionOption {
    DEFL_CONS_DEAD_LOAD, // Deflection due to Construction dead loads
    DEFL_ADDI_DEAD_LOAD, // Deflection due to additional dead loads
    DEFL_FINA_LIVE_LOAD, // Deflection due to Final stage live loads
    DEFL_SHRINK, // Deflection due to shrinkage of concrete
    DEFL_POST_CONS, // Deflection due to post Construction loads
    DEFL_FINA_TOTAL, // Total Deflection
    MODAL_SHAPE, // Mode shape
  }
}
