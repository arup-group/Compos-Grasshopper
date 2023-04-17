using OasysUnits;

namespace ComposAPI {
  public interface IUtilisation {
    /// <summary>
    /// Construction stage buckling utilisation factor
    /// </summary>
    Ratio BucklingConstruction { get; }
    /// <summary>
    /// Final deflect utilisation factor
    /// </summary>
    Ratio Deflection { get; }
    /// <summary>
    /// Construction stage deflect utilisation factor
    /// </summary>
    Ratio DeflectionConstruction { get; }
    /// <summary>
    /// Final moment utilisation factor
    /// </summary>
    Ratio Moment { get; }

    /// <summary>
    /// Construction stage moment utilisation factor
    /// </summary>
    Ratio MomentConstruction { get; }
    /// <summary>
    /// Final shear utilisation factor
    /// </summary>
    Ratio Shear { get; }
    /// <summary>
    /// Construction stage shear utilisation factor
    /// </summary>
    Ratio ShearConstruction { get; }
    /// <summary>
    /// Transverse shear utilisation factor
    /// </summary>
    Ratio TransverseShear { get; }

    /// <summary>
    /// Web opening utilisation factor
    /// </summary>
    Ratio WebOpening { get; }
  }
}
