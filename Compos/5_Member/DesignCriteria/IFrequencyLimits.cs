using OasysUnitsNet;

namespace ComposAPI
{
  public interface IFrequencyLimits
  {
    /// <summary>
    /// Minimum frequiency required
    /// </summary>
    Frequency MinimumRequired { get; }
    /// <summary>
    /// % of dead load to be included in the frequiency calculation
    /// </summary>
    Ratio DeadLoadIncl { get; }
    /// <summary>
    /// % of live load to be included in the frequiency calculation
    /// </summary>
    Ratio LiveLoadIncl { get; }
    string ToCoaString(string name);
  }
}
