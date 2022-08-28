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
  public class StudResult : ResultsBase, IStudResult
  {
    internal Dictionary<StudResultOption, List<IQuantity>> ResultsCache = new Dictionary<StudResultOption, List<IQuantity>>();
    public StudResult(Member member) : base(member)
    {
    }

    /// <summary>
    /// Actual stud capacity, as [number of studs] x [single stud capacity]
    /// </summary>
    public List<Force> StudCapacity
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_CONCRTE_FORCE;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Capacity of a single stud
    /// </summary>
    public Force SingleStudCapacity
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_ONE_CAPACITY;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList().Max();
      }
    }

    /// <summary>
    /// Required stud capacity for given moment
    /// </summary>
    public List<Force> StudCapacityRequired
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_CONCRTE_FORCE_REQ;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Required stud capacity for 100% shear interaction
    /// </summary>
    public List<Force> StudCapacityRequiredForFullShearInteraction
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_CONCRTE_FORCE_100;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Actual shear capacity from left end
    /// </summary>
    public List<Force> StudCapacityLeft
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_CAPACITY_LEFT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Actual shear capacity from right end
    /// </summary>
    public List<Force> StudCapacityRight
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_CAPACITY_RIGHT;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Actual shear interaction
    /// </summary>
    public List<Ratio> ShearInteraction
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_PERCENT_INTERACTION;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Ratio)x).ToList();
      }
    }

    /// <summary>
    /// Required shear interaction for given moment
    /// </summary>
    public List<Ratio> ShearInteractionRequired
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_INTERACT_REQ;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (Ratio)x).ToList();
      }
    }

    /// <summary>
    /// Actual number of studs provided from left end
    /// </summary>
    public List<int> NumberOfStudsLeft
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_NUM_LEFT_PROV;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (int)x.As(ScalarUnit.Amount)).ToList();
      }
    }
    /// <summary>
    /// Used number of studs provided from left end
    /// </summary>
    public List<int> NumberOfStudsRequiredLeft
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_NUM_LEFT_USED;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (int)x.As(ScalarUnit.Amount)).ToList();
      }
    }

    /// <summary>
    /// Actual number of studs provided from right end
    /// </summary>
    public List<int> NumberOfStudsRight
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_NUM_RIGHT_PROV;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (int)x.As(ScalarUnit.Amount)).ToList();
      }
    }

    /// <summary>
    /// Used number of studs provided from right end
    /// </summary>
    public List<int> NumberOfStudsRequiredRight
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_NUM_RIGHT_USED;
        if (!ResultsCache.ContainsKey(resultType))
          GetResults(resultType);
        return ResultsCache[resultType].Select(x => (int)x.As(ScalarUnit.Amount)).ToList();
      }
    }

    private void GetResults(StudResultOption resultType)
    {
      List<IQuantity> results = new List<IQuantity>();
      for (short pos = 0; pos < this.NumIntermediatePos; pos++)
      {
        float value = this.Member.GetResult(resultType.ToString(), Convert.ToInt16(pos));

        switch (resultType)
        {
          case StudResultOption.STUD_CONCRTE_FORCE:
          case StudResultOption.STUD_ONE_CAPACITY:
          case StudResultOption.STUD_CONCRTE_FORCE_REQ:
          case StudResultOption.STUD_CONCRTE_FORCE_100:
          case StudResultOption.STUD_CAPACITY_LEFT:
          case StudResultOption.STUD_CAPACITY_RIGHT:
            results.Add(new Force(value, ForceUnit.Newton));
            break;
          case StudResultOption.STUD_PERCENT_INTERACTION:
          case StudResultOption.STUD_INTERACT_REQ:
            results.Add(new Ratio(value, RatioUnit.DecimalFraction));
            break;
          case StudResultOption.STUD_NUM_LEFT_PROV:
          case StudResultOption.STUD_NUM_LEFT_USED:
          case StudResultOption.STUD_NUM_RIGHT_PROV:
          case StudResultOption.STUD_NUM_RIGHT_USED:
            results.Add(new UnitsNet.Scalar(value, ScalarUnit.Amount));
            break;
        }
      }
      ResultsCache.Add(resultType, results);
    }
  }
}
