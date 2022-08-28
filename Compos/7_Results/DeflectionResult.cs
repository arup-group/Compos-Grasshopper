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
  public class DeflectionResult : ResultsBase, IDeflectionResult
  {
    internal Dictionary<InternalForceOption, List<Length>> ResultsCache = new Dictionary<InternalForceOption, List<Length>>();
    public DeflectionResult(Member member) : base(member)
    {
    }


    /// <summary>
    /// Deflection due to Construction dead loads
    /// </summary>
    public List<Length> ConstructionDeadLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType];
      }
    }


    /// <summary>
    /// Deflection due to additional dead loads
    /// </summary>
    public List<Length> AdditionalDeadLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType];
      }
    }


    /// <summary>
    /// Deflection due to Final stage live loads
    /// </summary>
    public List<Length> LiveLoad
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType];
      }
    }


    /// <summary>
    /// Deflection due to shrinkage of concrete
    /// </summary>
    public List<Length> Shrinkage
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType];
      }
    }


    /// <summary>
    /// Deflection due to post Construction loads
    /// </summary>
    public List<Length> PostConstruction
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType];
      }
    }


    /// <summary>
    /// Total Deflection
    /// </summary>
    public List<Length> Total
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType];
      }
    }


    /// <summary>
    /// Mode shape
    /// </summary>
    public List<Length> ModeShape
    {
      get
      {
        InternalForceOption resultType = InternalForceOption.ULTI_AXIAL_CONS;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType];
      }
    }


    private void GetResults(InternalForceOption resultType)
    {
      List<Length> results = new List<Length>();
      for (short pos = 0; pos < this.NumIntermediatePos; pos++)
      {
        float value = this.Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));
        results.Add(new Length(value, LengthUnit.Meter));
      }
      ResultsCache.Add(resultType, results);
    }
  }
}
