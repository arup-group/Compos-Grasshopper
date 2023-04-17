using OasysUnits;
using OasysUnits.Units;
using System.Collections.Generic;

namespace ComposAPI {
  public class Utilisation : SubResult, IUtilisation {
    /// <summary>
    /// Construction stage buckling utilisation factor
    /// </summary>
    public Ratio BucklingConstruction {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.ConstructionBuckling;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Final deflect utilisation factor
    /// </summary>
    public Ratio Deflection {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.FinalDeflection;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Construction stage deflect utilisation factor
    /// </summary>
    public Ratio DeflectionConstruction {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.ConstructionDeflection;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Final moment utilisation factor
    /// </summary>
    public Ratio Moment {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.FinalMoment;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Construction stage moment utilisation factor
    /// </summary>
    public Ratio MomentConstruction {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.ConstructionMoment;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Final shear utilisation factor
    /// </summary>
    public Ratio Shear {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.FinalShear;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Construction stage shear utilisation factor
    /// </summary>
    public Ratio ShearConstruction {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.ConstructionShear;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Transverse shear utilisation factor
    /// </summary>
    public Ratio TransverseShear {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.TransverseShear;
        return GetResults(resultType);
      }
    }

    /// <summary>
    /// Web opening utilisation factor
    /// </summary>
    public Ratio WebOpening {
      get {
        UtilisationFactorOption resultType = UtilisationFactorOption.WebOpening;
        return GetResults(resultType);
      }
    }

    private Dictionary<UtilisationFactorOption, Ratio> ResultsCache = new Dictionary<UtilisationFactorOption, Ratio>();

    public Utilisation(Member member) : base(member, 0) {
    }

    private Ratio GetResults(UtilisationFactorOption resultType) {
      if (!ResultsCache.ContainsKey(resultType)) {
        float value = Member.UtilisationFactor(resultType);
        Ratio utilisation = new Ratio(value, RatioUnit.DecimalFraction);
        ResultsCache.Add(resultType, utilisation);
      }
      return ResultsCache[resultType];
    }
  }

  internal enum UtilisationFactorOption {
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
}
