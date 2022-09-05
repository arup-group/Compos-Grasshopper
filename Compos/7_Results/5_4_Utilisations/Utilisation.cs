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
  internal enum UtilisationFactorOption
  {
    FinalMoment,
    FinalShear,
    ConstructionMoment,
    ConstructionShear,
    ConstructionBuckling,
    ConstructionDeflection,
    FinalDeflection,
    TransverseShear,
    WebOpening,
    NaturalFrequency
  }

  public class Utilisation : ResultsBase, IUtilisation
  {
    internal Dictionary<UtilisationFactorOption, Ratio> ResultsCache = new Dictionary<UtilisationFactorOption, Ratio>();
    public Utilisation(Member member) : base(member, 0)
    {
    }

    /// <summary>
    /// Final moment utilisation factor
    /// </summary>
    public Ratio Moment
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.FinalMoment;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    /// <summary>
    /// Final shear utilisation factor
    /// </summary>
    public Ratio Shear
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.FinalShear;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    /// <summary>
    /// Final deflect utilisation factor
    /// </summary>
    public Ratio Deflection
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.FinalDeflection;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    /// <summary>
    /// Construction stage moment utilisation factor
    /// </summary>
    public Ratio MomentConstruction
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.ConstructionMoment;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    /// <summary>
    /// Construction stage shear utilisation factor
    /// </summary>
    public Ratio ShearConstruction
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.ConstructionShear;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    /// <summary>
    /// Construction stage deflect utilisation factor
    /// </summary>
    public Ratio DeflectionConstruction
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.ConstructionDeflection;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    /// <summary>
    /// Construction stage buckling utilisation factor
    /// </summary>
    public Ratio BucklingConstruction
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.ConstructionBuckling;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    /// <summary>
    /// Transverse shear utilisation factor
    /// </summary>
    public Ratio TransverseShear
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.TransverseShear;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    /// <summary>
    /// Web opening utilisation factor
    /// </summary>
    public Ratio WebOpening
    {
      get
      {
        UtilisationFactorOption resultType = UtilisationFactorOption.WebOpening;
        if (!this.ResultsCache.ContainsKey(resultType))
          this.GetResults(resultType);
        return this.ResultsCache[resultType];
      }
    }

    

    private void GetResults(UtilisationFactorOption resultType)
    {
      float value = this.Member.UtilisationFactor(resultType);
      Ratio utilisation = new Ratio(value, RatioUnit.DecimalFraction);
      this.ResultsCache.Add(resultType, utilisation);
    }
  }
}
