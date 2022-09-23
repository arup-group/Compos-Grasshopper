using System.Collections.Generic;
using OasysUnits;
using OasysUnits.Units;

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

  public class Utilisation : SubResult, IUtilisation
  {
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
        return this.GetResults(resultType);
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
        return this.GetResults(resultType);
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
        return this.GetResults(resultType);
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
        return this.GetResults(resultType);
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
        return this.GetResults(resultType);
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
        return this.GetResults(resultType);
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
        return this.GetResults(resultType);
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
        return this.GetResults(resultType);
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
        return this.GetResults(resultType);
      }
    }

    private Dictionary<UtilisationFactorOption, Ratio> ResultsCache = new Dictionary<UtilisationFactorOption, Ratio>();

    private Ratio GetResults(UtilisationFactorOption resultType)
    {
      if (!this.ResultsCache.ContainsKey(resultType))
      {
        float value = this.Member.UtilisationFactor(resultType);
        Ratio utilisation = new Ratio(value, RatioUnit.DecimalFraction);
        this.ResultsCache.Add(resultType, utilisation);
      }
      return this.ResultsCache[resultType];
    }
  }
}
