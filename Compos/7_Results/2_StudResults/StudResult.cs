using System;
using System.Collections.Generic;
using System.Linq;
using OasysUnits;
using OasysUnits.Units;

namespace ComposAPI
{
  internal enum StudResultOption
  {
    STUD_CONCRTE_FORCE, // Actual stud capacity
    STUD_NUM_LEFT_PROV, // Actual number of studs provided from left end
    STUD_NUM_RIGHT_PROV, // Actual number of studs provided from right end
    STUD_NUM_LEFT_USED, // Used number of studs provided from left end
    STUD_NUM_RIGHT_USED, // Used number of studs provided from right end
    STUD_CONCRTE_FORCE_100, // Required stud capacity for 100% shear interaction
    STUD_CONCRTE_FORCE_REQ, // Required stud capacity for given moment
    STUD_INTERACT_REQ, // Required shear interaction for given moment
    STUD_ONE_CAPACITY, // One shear stud capacity
    STUD_PERCENT_INTERACTION, // Actual shear interaction
    STUD_CAPACITY_LEFT, // Actual shear capacity from left end
    STUD_CAPACITY_RIGHT, // Actual shear capacity from right end
  }

  public class StudResult : SubResult, IStudResult
  {
    public StudResult(Member member, int numIntermediatePos) : base(member, numIntermediatePos)
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList().Max();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Actual shear capacity from left end
    /// </summary>
    public List<Force> StudCapacityStart
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_CAPACITY_LEFT;
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
      }
    }

    /// <summary>
    /// Actual shear capacity from right end
    /// </summary>
    public List<Force> StudCapacityEnd
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_CAPACITY_RIGHT;
        return this.GetResults(resultType).Select(x => (Force)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Ratio)x).ToList();
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
        return this.GetResults(resultType).Select(x => (Ratio)x).ToList();
      }
    }

    /// <summary>
    /// Actual number of studs provided from left end
    /// </summary>
    public List<int> NumberOfStudsStart
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_NUM_LEFT_PROV;
        return this.GetResults(resultType).Select(x => (int)x.As(ScalarUnit.Amount)).ToList();
      }
    }
    /// <summary>
    /// Used number of studs provided from left end
    /// </summary>
    public List<int> NumberOfStudsRequiredStart
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_NUM_LEFT_USED;
        return this.GetResults(resultType).Select(x => (int)x.As(ScalarUnit.Amount)).ToList();
      }
    }

    /// <summary>
    /// Actual number of studs provided from right end
    /// </summary>
    public List<int> NumberOfStudsEnd
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_NUM_RIGHT_PROV;
        return this.GetResults(resultType).Select(x => (int)x.As(ScalarUnit.Amount)).ToList();
      }
    }

    /// <summary>
    /// Used number of studs provided from right end
    /// </summary>
    public List<int> NumberOfStudsRequiredEnd
    {
      get
      {
        StudResultOption resultType = StudResultOption.STUD_NUM_RIGHT_USED;
        return this.GetResults(resultType).Select(x => (int)x.As(ScalarUnit.Amount)).ToList();
      }
    }

    private Dictionary<StudResultOption, List<IQuantity>> ResultsCache = new Dictionary<StudResultOption, List<IQuantity>>();

    private List<IQuantity> GetResults(StudResultOption resultType)
    {
      if(!this.ResultsCache.ContainsKey(resultType))
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
              results.Add(new Scalar(value, ScalarUnit.Amount));
              break;
          }
        }
        this.ResultsCache.Add(resultType, results);
      }
      return this.ResultsCache[resultType];
    }
  }
}
